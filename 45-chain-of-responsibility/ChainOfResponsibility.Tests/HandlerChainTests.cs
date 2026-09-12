using ChainOfResponsibility.Api.Handlers;
using ChainOfResponsibility.Api.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace ChainOfResponsibility.Tests;

public class HandlerChainTests
{
    // ─── helpers ──────────────────────────────────────────────────────────────

    private static SupportTicket ValidTicket(
        string apiKey = "key-admin-001",
        TicketCategory category = TicketCategory.General,
        TicketPriority priority = TicketPriority.Low,
        string title = "Need help",
        string description = "Please assist me",
        string requestedBy = "user@example.com") => new()
    {
        Title = title,
        Description = description,
        Priority = priority,
        Category = category,
        RequestedBy = requestedBy,
        ApiKey = apiKey
    };

    private static ITicketHandler BuildFullChain()
    {
        var spam    = new SpamFilterHandler();
        var auth    = new AuthenticationHandler();
        var authz   = new AuthorizationHandler();
        var rate    = new RateLimitHandler();
        var log     = new LoggingHandler(NullLogger<LoggingHandler>.Instance);
        var support = new SupportHandler();

        spam.SetNext(auth).SetNext(authz).SetNext(rate).SetNext(log).SetNext(support);
        return spam;
    }

    // ─── SpamFilterHandler ────────────────────────────────────────────────────

    [Fact]
    public async Task SpamFilter_Rejects_Ticket_With_Spam_Keyword()
    {
        var handler = new SpamFilterHandler();
        var ticket  = ValidTicket(title: "You are a winner click here now");

        var result = await handler.HandleAsync(ticket);

        result.IsRejected.Should().BeTrue();
        result.RejectionReason.Should().Contain("spam");
    }

    [Theory]
    [InlineData("free")]
    [InlineData("winner")]
    [InlineData("prize")]
    [InlineData("click here")]
    [InlineData("buy now")]
    public async Task SpamFilter_Rejects_Each_Spam_Keyword(string keyword)
    {
        var handler = new SpamFilterHandler();
        var ticket  = ValidTicket(description: $"This message contains {keyword} in it");

        var result = await handler.HandleAsync(ticket);

        result.IsRejected.Should().BeTrue();
    }

    [Fact]
    public async Task SpamFilter_Passes_Clean_Ticket_To_Next()
    {
        var handler = new SpamFilterHandler();
        var support = new SupportHandler();
        handler.SetNext(support);

        var ticket = ValidTicket(apiKey: null); // skip auth for this test
        var result = await handler.HandleAsync(ticket);

        result.ProcessingLog.Should().Contain(l => l.Contains("SpamFilter") && l.Contains("Passed"));
    }

    // ─── AuthenticationHandler ────────────────────────────────────────────────

    [Fact]
    public async Task Authentication_Rejects_Missing_ApiKey()
    {
        var handler = new AuthenticationHandler();
        var ticket  = ValidTicket(apiKey: null!);

        var result = await handler.HandleAsync(ticket);

        result.IsRejected.Should().BeTrue();
        result.RejectionReason.Should().Contain("API key");
    }

    [Fact]
    public async Task Authentication_Rejects_Invalid_ApiKey()
    {
        var handler = new AuthenticationHandler();
        var ticket  = ValidTicket(apiKey: "invalid-key");

        var result = await handler.HandleAsync(ticket);

        result.IsRejected.Should().BeTrue();
    }

    [Theory]
    [InlineData("key-admin-001")]
    [InlineData("key-user-002")]
    [InlineData("key-support-003")]
    public async Task Authentication_Passes_Valid_ApiKeys(string key)
    {
        var handler = new AuthenticationHandler();
        var support = new SupportHandler();
        handler.SetNext(support);

        var ticket = ValidTicket(apiKey: key);
        var result = await handler.HandleAsync(ticket);

        result.IsProcessed.Should().BeTrue();
    }

    // ─── AuthorizationHandler ─────────────────────────────────────────────────

    [Fact]
    public async Task Authorization_Rejects_Unauthorized_Category()
    {
        // key-support-003 cannot submit Security tickets
        var handler = new AuthorizationHandler();
        var ticket  = ValidTicket(apiKey: "key-support-003", category: TicketCategory.Security);

        var result = await handler.HandleAsync(ticket);

        result.IsRejected.Should().BeTrue();
        result.RejectionReason.Should().Contain("Security");
    }

    [Fact]
    public async Task Authorization_Allows_Admin_All_Categories()
    {
        var categories = Enum.GetValues<TicketCategory>();
        foreach (var category in categories)
        {
            var handler = new AuthorizationHandler();
            var support = new SupportHandler();
            handler.SetNext(support);

            var ticket = ValidTicket(apiKey: "key-admin-001", category: category);
            var result = await handler.HandleAsync(ticket);

            result.IsRejected.Should().BeFalse($"admin should be allowed to submit {category} tickets");
        }
    }

    [Fact]
    public async Task Authorization_Rejects_UserKey_For_Security_Category()
    {
        var handler = new AuthorizationHandler();
        var ticket  = ValidTicket(apiKey: "key-user-002", category: TicketCategory.Security);

        var result = await handler.HandleAsync(ticket);

        result.IsRejected.Should().BeTrue();
    }

    // ─── SupportHandler ───────────────────────────────────────────────────────

    [Theory]
    [InlineData(TicketCategory.General,   "General Support Team")]
    [InlineData(TicketCategory.Technical, "Technical Team")]
    [InlineData(TicketCategory.Billing,   "Billing Team")]
    [InlineData(TicketCategory.Security,  "Security Response Team")]
    public async Task Support_Assigns_Correct_Team(TicketCategory category, string expectedTeam)
    {
        var handler = new SupportHandler();
        var ticket  = ValidTicket(category: category);

        var result = await handler.HandleAsync(ticket);

        result.IsProcessed.Should().BeTrue();
        result.AssignedTo.Should().Be(expectedTeam);
    }

    // ─── Full pipeline ────────────────────────────────────────────────────────

    [Fact]
    public async Task FullChain_Processes_Valid_Ticket_Successfully()
    {
        var chain  = BuildFullChain();
        var ticket = ValidTicket();

        var result = await chain.HandleAsync(ticket);

        result.IsProcessed.Should().BeTrue();
        result.IsRejected.Should().BeFalse();
        result.AssignedTo.Should().Be("General Support Team");
    }

    [Fact]
    public async Task FullChain_Returns_Correct_TicketId()
    {
        var chain  = BuildFullChain();
        var ticket = ValidTicket();
        var id     = ticket.Id;

        var result = await chain.HandleAsync(ticket);

        result.TicketId.Should().Be(id);
    }

    [Fact]
    public async Task FullChain_Builds_ProcessingLog()
    {
        var chain  = BuildFullChain();
        var ticket = ValidTicket();

        var result = await chain.HandleAsync(ticket);

        result.ProcessingLog.Should().HaveCountGreaterThan(3);
        result.ProcessingLog.Should().Contain(l => l.Contains("SpamFilter"));
        result.ProcessingLog.Should().Contain(l => l.Contains("Authentication"));
        result.ProcessingLog.Should().Contain(l => l.Contains("Authorization"));
        result.ProcessingLog.Should().Contain(l => l.Contains("RateLimit"));
        result.ProcessingLog.Should().Contain(l => l.Contains("Logging"));
        result.ProcessingLog.Should().Contain(l => l.Contains("Support"));
    }

    [Fact]
    public async Task FullChain_SpamRejection_StopsAt_SpamFilter()
    {
        var chain  = BuildFullChain();
        var ticket = ValidTicket(title: "You are a winner prize free");

        var result = await chain.HandleAsync(ticket);

        result.IsRejected.Should().BeTrue();
        result.ProcessingLog.Should().NotContain(l => l.Contains("Authentication"));
    }

    [Fact]
    public async Task FullChain_InvalidApiKey_StopsAt_Authentication()
    {
        var chain  = BuildFullChain();
        var ticket = ValidTicket(apiKey: "bad-key");

        var result = await chain.HandleAsync(ticket);

        result.IsRejected.Should().BeTrue();
        result.ProcessingLog.Should().NotContain(l => l.Contains("Authorization"));
    }

    // ─── BaseTicketHandler ────────────────────────────────────────────────────

    [Fact]
    public async Task BaseHandler_SetNext_Returns_Next_Handler()
    {
        var spam    = new SpamFilterHandler();
        var support = new SupportHandler();

        var returned = spam.SetNext(support);

        returned.Should().BeSameAs(support);
    }

    [Fact]
    public async Task BaseHandler_NoNext_ReturnsRejected()
    {
        // A handler with no next that passes the ticket should return rejected
        var handler = new LoggingHandler(NullLogger<LoggingHandler>.Instance);
        // No next set — PassToNextAsync will trigger the fallback
        var ticket = ValidTicket();

        var result = await handler.HandleAsync(ticket);

        result.IsRejected.Should().BeTrue();
        result.RejectionReason.Should().Contain("No handler available");
    }
}
