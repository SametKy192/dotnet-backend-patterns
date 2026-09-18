using FluentAssertions;
using StrategyPattern.Api.Models;
using StrategyPattern.Api.Services;
using StrategyPattern.Api.Strategies;

namespace StrategyPattern.Tests;

public class ShippingStrategyTests
{
    // ─── Helpers ──────────────────────────────────────────────────────────────

    private static Order DomesticOrder(decimal weightKg = 2m, decimal totalValue = 30m) => new()
    {
        WeightKg    = weightKg,
        TotalValue  = totalValue,
        ItemCount   = 1,
        Destination = "domestic"
    };

    private static Order InternationalOrder(decimal weightKg = 2m, decimal totalValue = 500m) => new()
    {
        WeightKg    = weightKg,
        TotalValue  = totalValue,
        ItemCount   = 1,
        Destination = "international"
    };

    private static ShippingStrategyFactory BuildFactory() =>
        new(new IShippingStrategy[]
        {
            new StandardShippingStrategy(),
            new ExpressShippingStrategy(),
            new OvernightShippingStrategy(),
            new FreeShippingStrategy(),
            new InternationalShippingStrategy()
        });

    // ─── StandardShippingStrategy ─────────────────────────────────────────────

    [Fact]
    public void Standard_CanHandle_Returns_True_For_Domestic()
    {
        new StandardShippingStrategy().CanHandle(DomesticOrder()).Should().BeTrue();
    }

    [Fact]
    public void Standard_CanHandle_Returns_False_For_International()
    {
        new StandardShippingStrategy().CanHandle(InternationalOrder()).Should().BeFalse();
    }

    [Fact]
    public void Standard_Calculate_Returns_Correct_Cost()
    {
        // $5.00 + $1.50 * 2 = $8.00
        var quote = new StandardShippingStrategy().Calculate(DomesticOrder(weightKg: 2m));
        quote.Cost.Should().Be(8.00m);
        quote.IsAvailable.Should().BeTrue();
    }

    [Fact]
    public void Standard_Calculate_Returns_Unavailable_For_International()
    {
        var quote = new StandardShippingStrategy().Calculate(InternationalOrder());
        quote.IsAvailable.Should().BeFalse();
        quote.UnavailableReason.Should().NotBeNullOrEmpty();
    }

    // ─── ExpressShippingStrategy ──────────────────────────────────────────────

    [Fact]
    public void Express_CanHandle_Returns_True_For_Domestic()
    {
        new ExpressShippingStrategy().CanHandle(DomesticOrder()).Should().BeTrue();
    }

    [Fact]
    public void Express_Calculate_Returns_Correct_Cost()
    {
        // $15.00 + $2.50 * 3 = $22.50
        var quote = new ExpressShippingStrategy().Calculate(DomesticOrder(weightKg: 3m));
        quote.Cost.Should().Be(22.50m);
        quote.EstimatedDelivery.Should().Contain("2");
    }

    [Fact]
    public void Express_Returns_Unavailable_For_International()
    {
        var quote = new ExpressShippingStrategy().Calculate(InternationalOrder());
        quote.IsAvailable.Should().BeFalse();
    }

    // ─── OvernightShippingStrategy ────────────────────────────────────────────

    [Fact]
    public void Overnight_Calculate_Returns_Correct_Cost()
    {
        // $35.00 + $5.00 * 1 = $40.00
        var quote = new OvernightShippingStrategy().Calculate(DomesticOrder(weightKg: 1m));
        quote.Cost.Should().Be(40.00m);
        quote.EstimatedDelivery.Should().Contain("Next");
    }

    [Fact]
    public void Overnight_Returns_Unavailable_For_International()
    {
        var quote = new OvernightShippingStrategy().Calculate(InternationalOrder());
        quote.IsAvailable.Should().BeFalse();
    }

    // ─── FreeShippingStrategy ─────────────────────────────────────────────────

    [Fact]
    public void Free_CanHandle_Returns_True_When_Eligible()
    {
        new FreeShippingStrategy().CanHandle(DomesticOrder(totalValue: 75m)).Should().BeTrue();
    }

    [Fact]
    public void Free_CanHandle_Returns_False_When_Below_Threshold()
    {
        new FreeShippingStrategy().CanHandle(DomesticOrder(totalValue: 49.99m)).Should().BeFalse();
    }

    [Fact]
    public void Free_CanHandle_Returns_False_For_International()
    {
        new FreeShippingStrategy().CanHandle(InternationalOrder(totalValue: 500m)).Should().BeFalse();
    }

    [Fact]
    public void Free_Calculate_Returns_Zero_Cost_When_Eligible()
    {
        var quote = new FreeShippingStrategy().Calculate(DomesticOrder(totalValue: 100m));
        quote.Cost.Should().Be(0m);
        quote.IsAvailable.Should().BeTrue();
    }

    [Fact]
    public void Free_Calculate_Returns_Unavailable_When_Below_Threshold()
    {
        var quote = new FreeShippingStrategy().Calculate(DomesticOrder(totalValue: 20m));
        quote.IsAvailable.Should().BeFalse();
        quote.UnavailableReason.Should().Contain("$50");
    }

    // ─── InternationalShippingStrategy ───────────────────────────────────────

    [Fact]
    public void International_CanHandle_Returns_True_For_International()
    {
        new InternationalShippingStrategy().CanHandle(InternationalOrder()).Should().BeTrue();
    }

    [Fact]
    public void International_CanHandle_Returns_False_For_Domestic()
    {
        new InternationalShippingStrategy().CanHandle(DomesticOrder()).Should().BeFalse();
    }

    [Fact]
    public void International_Calculate_Returns_Correct_Base_Cost()
    {
        // $25.00 + $4.00 * 2 = $33.00 (no customs — value < $1000)
        var quote = new InternationalShippingStrategy().Calculate(InternationalOrder(weightKg: 2m, totalValue: 500m));
        quote.Cost.Should().Be(33.00m);
        quote.IsAvailable.Should().BeTrue();
    }

    [Fact]
    public void International_Calculate_Applies_Customs_Surcharge_For_High_Value()
    {
        // $25 + $4*2 = $33 base; customs = $33 * 0.10 = $3.30 → total $36.30
        var quote = new InternationalShippingStrategy().Calculate(InternationalOrder(weightKg: 2m, totalValue: 1_500m));
        quote.Cost.Should().Be(36.30m);
        quote.Description.Should().Contain("customs");
    }

    // ─── ShippingContext ──────────────────────────────────────────────────────

    [Fact]
    public void Context_Execute_Delegates_To_Active_Strategy()
    {
        var context = new ShippingContext(new StandardShippingStrategy());
        var quote   = context.Execute(DomesticOrder(weightKg: 2m));
        quote.Method.Should().Be(ShippingMethod.Standard);
        quote.Cost.Should().Be(8.00m);
    }

    [Fact]
    public void Context_SetStrategy_Changes_Active_Strategy()
    {
        var context = new ShippingContext(new StandardShippingStrategy());
        context.CurrentMethod.Should().Be(ShippingMethod.Standard);

        context.SetStrategy(new ExpressShippingStrategy());
        context.CurrentMethod.Should().Be(ShippingMethod.Express);

        var quote = context.Execute(DomesticOrder(weightKg: 2m));
        quote.Method.Should().Be(ShippingMethod.Express);
    }

    // ─── ShippingStrategyFactory ──────────────────────────────────────────────

    [Fact]
    public void Factory_GetStrategy_Returns_Correct_Strategy()
    {
        var factory = BuildFactory();
        factory.GetStrategy(ShippingMethod.Overnight).Method.Should().Be(ShippingMethod.Overnight);
    }

    [Fact]
    public void Factory_GetAllQuotes_Returns_Five_Quotes()
    {
        var factory = BuildFactory();
        var quotes  = factory.GetAllQuotes(DomesticOrder());
        quotes.Should().HaveCount(5);
    }

    [Fact]
    public void Factory_GetOptimalQuote_Returns_Free_When_Eligible()
    {
        var factory = BuildFactory();
        var order   = DomesticOrder(totalValue: 200m); // qualifies for free shipping
        var quote   = factory.GetOptimalQuote(order);
        quote.Method.Should().Be(ShippingMethod.Free);
        quote.Cost.Should().Be(0m);
    }

    [Fact]
    public void Factory_GetOptimalQuote_Returns_Standard_When_Free_Not_Eligible()
    {
        var factory = BuildFactory();
        var order   = DomesticOrder(weightKg: 1m, totalValue: 20m); // below free threshold
        var quote   = factory.GetOptimalQuote(order);
        // Standard: $5 + $1.50 = $6.50 < Express $17.50 < Overnight $40
        quote.Method.Should().Be(ShippingMethod.Standard);
    }

    [Fact]
    public void Factory_GetOptimalQuote_Returns_International_For_International_Order()
    {
        var factory = BuildFactory();
        var quote   = factory.GetOptimalQuote(InternationalOrder());
        quote.Method.Should().Be(ShippingMethod.International);
    }
}
