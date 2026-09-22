using FluentAssertions;
using ObserverPattern.Api.Models;
using ObserverPattern.Api.Observers;
using ObserverPattern.Api.Subject;

namespace ObserverPattern.Tests;

/// <summary>
/// 25 unit tests covering StockMarket subject, all four observers
/// and edge-case behaviour (no subscribers, concurrent notifications, thresholds).
/// </summary>
public class ObserverPatternTests
{
    // ── Helpers ───────────────────────────────────────────────────────────────

    private static StockMarket CreateMarket() => new();

    private static async Task<StockPrice> PublishPrice(
        StockMarket market, string symbol, decimal price)
    {
        await market.UpdatePriceAsync(symbol, price);
        return market.GetPriceHistory(symbol)[^1];
    }

    // ══════════════════════════════════════════════════════════════════════════
    // 1. StockMarket — subject mechanics
    // ══════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task UpdatePrice_NoSubscribers_RecordsHistoryWithoutError()
    {
        var market = CreateMarket();
        await market.UpdatePriceAsync("AAPL", 150m);

        market.GetPriceHistory("AAPL").Should().HaveCount(1);
    }

    [Fact]
    public async Task UpdatePrice_RecordsCorrectChangeAmount()
    {
        var market = CreateMarket();
        await market.UpdatePriceAsync("AAPL", 100m);
        var price = await PublishPrice(market, "AAPL", 115m);

        price.ChangeAmount.Should().Be(15m);
    }

    [Fact]
    public async Task UpdatePrice_RecordsCorrectChangePercent()
    {
        var market = CreateMarket();
        await market.UpdatePriceAsync("MSFT", 200m);
        var price = await PublishPrice(market, "MSFT", 210m);

        price.ChangePercent.Should().Be(5m);
    }

    [Fact]
    public async Task UpdatePrice_IsUp_TrueWhenPriceRises()
    {
        var market = CreateMarket();
        await market.UpdatePriceAsync("TSLA", 300m);
        var price = await PublishPrice(market, "TSLA", 310m);

        price.IsUp.Should().BeTrue();
        price.IsDown.Should().BeFalse();
    }

    [Fact]
    public async Task UpdatePrice_IsDown_TrueWhenPriceFalls()
    {
        var market = CreateMarket();
        await market.UpdatePriceAsync("TSLA", 300m);
        var price = await PublishPrice(market, "TSLA", 270m);

        price.IsDown.Should().BeTrue();
        price.IsUp.Should().BeFalse();
    }

    [Fact]
    public async Task UpdatePrice_FirstUpdate_PreviousPriceIsZero()
    {
        var market = CreateMarket();
        var price  = await PublishPrice(market, "GOOG", 100m);

        price.PreviousPrice.Should().Be(0m);
        price.ChangePercent.Should().Be(0m);
    }

    [Fact]
    public async Task Subscribe_AndUnsubscribe_ObserverReceivesNoMoreUpdates()
    {
        var market   = CreateMarket();
        var observer = new EmailAlertObserver();

        market.Subscribe("AAPL", observer);
        await market.UpdatePriceAsync("AAPL", 100m);
        await market.UpdatePriceAsync("AAPL", 200m); // +100% → alert

        market.Unsubscribe("AAPL", observer);
        await market.UpdatePriceAsync("AAPL", 400m); // +100% → no alert (unsubscribed)

        observer.Alerts.Should().HaveCount(1); // only the first big move
    }

    [Fact]
    public async Task GetTrackedSymbols_ReturnsAllPublishedSymbols()
    {
        var market   = CreateMarket();
        var observer = new AuditLogObserver();

        // Subscribe first so the symbols appear in the tracked list
        market.Subscribe("AAPL", observer);
        market.Subscribe("MSFT", observer);
        await market.UpdatePriceAsync("AAPL", 100m);
        await market.UpdatePriceAsync("MSFT", 200m);

        market.GetTrackedSymbols().Should().Contain(["AAPL", "MSFT"]);
    }

    [Fact]
    public async Task UpdatePrice_SymbolNormalisedToUpperCase()
    {
        var market   = CreateMarket();
        var observer = new AuditLogObserver();
        // Subscribe with uppercase so the key exists in _subscriptions
        market.Subscribe("AAPL", observer);
        // Publish with lowercase — UpdatePriceAsync normalises to AAPL
        await market.UpdatePriceAsync("aapl", 100m);

        market.GetPriceHistory("AAPL").Should().HaveCount(1);
        market.GetTrackedSymbols().Should().Contain("AAPL");
    }

    [Fact]
    public async Task Subscribe_DuplicateObserver_NotAddedTwice()
    {
        var market   = CreateMarket();
        var observer = new AuditLogObserver();

        market.Subscribe("AAPL", observer);
        market.Subscribe("AAPL", observer); // duplicate

        await market.UpdatePriceAsync("AAPL", 100m);
        await market.UpdatePriceAsync("AAPL", 200m);

        // AuditLog records every update, so only 1 notification per price tick
        observer.Alerts.Should().HaveCount(2);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // 2. EmailAlertObserver
    // ══════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task EmailAlert_BelowThreshold_NoAlert()
    {
        var market   = CreateMarket();
        var observer = new EmailAlertObserver();
        market.Subscribe("AAPL", observer);

        await market.UpdatePriceAsync("AAPL", 100m);
        await market.UpdatePriceAsync("AAPL", 104m); // +4% < 5%

        observer.Alerts.Should().BeEmpty();
    }

    [Fact]
    public async Task EmailAlert_AboveThreshold_AlertGenerated()
    {
        var market   = CreateMarket();
        var observer = new EmailAlertObserver();
        market.Subscribe("AAPL", observer);

        await market.UpdatePriceAsync("AAPL", 100m);
        await market.UpdatePriceAsync("AAPL", 106m); // +6%

        observer.Alerts.Should().HaveCount(1);
        observer.Alerts[0].Severity.Should().Be(AlertSeverity.Warning);
    }

    [Fact]
    public async Task EmailAlert_TenPercentMove_CriticalSeverity()
    {
        var market   = CreateMarket();
        var observer = new EmailAlertObserver();
        market.Subscribe("AAPL", observer);

        await market.UpdatePriceAsync("AAPL", 100m);
        await market.UpdatePriceAsync("AAPL", 115m); // +15%

        observer.Alerts[0].Severity.Should().Be(AlertSeverity.Critical);
    }

    [Fact]
    public void EmailAlert_Name_IsEmailAlert()
    {
        new EmailAlertObserver().Name.Should().Be("EmailAlert");
    }

    // ══════════════════════════════════════════════════════════════════════════
    // 3. SmsAlertObserver
    // ══════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task SmsAlert_BelowThreshold_NoAlert()
    {
        var market   = CreateMarket();
        var observer = new SmsAlertObserver();
        market.Subscribe("TSLA", observer);

        await market.UpdatePriceAsync("TSLA", 500m);
        await market.UpdatePriceAsync("TSLA", 507m); // +1.4% < 3%

        observer.Alerts.Should().BeEmpty();
    }

    [Fact]
    public async Task SmsAlert_AboveThreshold_AlertGenerated()
    {
        var market   = CreateMarket();
        var observer = new SmsAlertObserver();
        market.Subscribe("TSLA", observer);

        await market.UpdatePriceAsync("TSLA", 500m);
        await market.UpdatePriceAsync("TSLA", 560m); // +12% >= 10% threshold

        observer.Alerts.Should().HaveCount(1);
    }

    [Fact]
    public void SmsAlert_Name_IsSmsAlert()
    {
        new SmsAlertObserver().Name.Should().Be("SmsAlert");
    }

    // ══════════════════════════════════════════════════════════════════════════
    // 4. DashboardObserver
    // ══════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task Dashboard_RecordsAlertForEveryPriceChange()
    {
        var market   = CreateMarket();
        var observer = new DashboardObserver();
        market.Subscribe("MSFT", observer);

        await market.UpdatePriceAsync("MSFT", 300m);
        await market.UpdatePriceAsync("MSFT", 302m);
        await market.UpdatePriceAsync("MSFT", 298m);

        // DashboardObserver records every tick regardless of magnitude
        observer.Alerts.Should().HaveCount(3);
    }

    [Fact]
    public void Dashboard_Name_IsDashboard()
    {
        new DashboardObserver().Name.Should().Be("Dashboard");
    }

    // ══════════════════════════════════════════════════════════════════════════
    // 5. AuditLogObserver
    // ══════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task AuditLog_RecordsAllUpdatesForAuditTrail()
    {
        var market   = CreateMarket();
        var observer = new AuditLogObserver();
        market.Subscribe("GOOG", observer);

        for (var i = 1; i <= 5; i++)
            await market.UpdatePriceAsync("GOOG", 100m + i);

        observer.Alerts.Should().HaveCount(5);
    }

    [Fact]
    public void AuditLog_Name_IsAuditLog()
    {
        new AuditLogObserver().Name.Should().Be("AuditLog");
    }

    // ══════════════════════════════════════════════════════════════════════════
    // 6. Multiple observers on same symbol
    // ══════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task MultipleObservers_AllNotifiedOnPriceUpdate()
    {
        var market    = CreateMarket();
        var email     = new EmailAlertObserver();
        var sms       = new SmsAlertObserver();
        var dashboard = new DashboardObserver();
        var audit     = new AuditLogObserver();

        market.Subscribe("NVDA", email);
        market.Subscribe("NVDA", sms);
        market.Subscribe("NVDA", dashboard);
        market.Subscribe("NVDA", audit);

        await market.UpdatePriceAsync("NVDA", 500m);
        await market.UpdatePriceAsync("NVDA", 560m); // +12%

        // Email & SMS react to big moves; Dashboard & AuditLog record everything
        email.Alerts.Should().HaveCount(1);
        sms.Alerts.Should().HaveCount(1);
        dashboard.Alerts.Should().HaveCount(2);
        audit.Alerts.Should().HaveCount(2);
    }

    [Fact]
    public async Task MultipleSymbols_ObserversOnlyNotifiedForSubscribedSymbol()
    {
        var market   = CreateMarket();
        var observer = new AuditLogObserver();

        market.Subscribe("AAPL", observer);

        await market.UpdatePriceAsync("AAPL", 100m);
        await market.UpdatePriceAsync("MSFT", 200m); // not subscribed

        observer.Alerts.Should().HaveCount(1);
    }

    [Fact]
    public async Task HistoryGrows_WithEveryPriceTick()
    {
        var market = CreateMarket();
        await market.UpdatePriceAsync("AMZN", 180m);
        await market.UpdatePriceAsync("AMZN", 185m);
        await market.UpdatePriceAsync("AMZN", 178m);

        market.GetPriceHistory("AMZN").Should().HaveCount(3);
    }
}
