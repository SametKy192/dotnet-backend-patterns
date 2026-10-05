using FluentAssertions;
using StatePattern.Api.Models;
using StatePattern.Api.Services;

namespace StatePattern.Tests;

public class StatePatternTests
{
    private static Order NewOrder() => new() { Customer = "Alice", Amount = 100m };
    private static Order Paid()      { var o = NewOrder(); o.Pay(); return o; }
    private static Order Shipped()   { var o = Paid(); o.Ship(); return o; }
    private static Order Delivered() { var o = Shipped(); o.Deliver(); return o; }

    // ── Initial state ─────────────────────────────────────────────────────────

    [Fact] public void NewOrder_IsPending() => NewOrder().Status.Should().Be(OrderStatus.Pending);
    [Fact] public void NewOrder_HasNoHistory() => NewOrder().History.Should().BeEmpty();

    // ── Valid transitions ─────────────────────────────────────────────────────

    [Fact] public void Pending_Pay_BecomesPaid() => Paid().Status.Should().Be(OrderStatus.Paid);
    [Fact] public void Pending_Cancel_BecomesCancelled() { var o = NewOrder(); o.Cancel(); o.Status.Should().Be(OrderStatus.Cancelled); }
    [Fact] public void Paid_Ship_BecomesShipped() => Shipped().Status.Should().Be(OrderStatus.Shipped);
    [Fact] public void Paid_Cancel_BecomesRefunded() { var o = Paid(); o.Cancel(); o.Status.Should().Be(OrderStatus.Refunded); }
    [Fact] public void Paid_Refund_BecomesRefunded() { var o = Paid(); o.Refund(); o.Status.Should().Be(OrderStatus.Refunded); }
    [Fact] public void Shipped_Deliver_BecomesDelivered() => Delivered().Status.Should().Be(OrderStatus.Delivered);
    [Fact] public void Delivered_Refund_BecomesRefunded() { var o = Delivered(); o.Refund(); o.Status.Should().Be(OrderStatus.Refunded); }

    // ── Invalid transitions ───────────────────────────────────────────────────

    [Fact] public void Pending_Ship_Throws() => FluentActions.Invoking(() => NewOrder().Ship()).Should().Throw<InvalidTransitionException>();
    [Fact] public void Pending_Deliver_Throws() => FluentActions.Invoking(() => NewOrder().Deliver()).Should().Throw<InvalidTransitionException>();
    [Fact] public void Pending_Refund_Throws() => FluentActions.Invoking(() => NewOrder().Refund()).Should().Throw<InvalidTransitionException>();
    [Fact] public void Paid_Pay_Throws() => FluentActions.Invoking(() => Paid().Pay()).Should().Throw<InvalidTransitionException>();
    [Fact] public void Paid_Deliver_Throws() => FluentActions.Invoking(() => Paid().Deliver()).Should().Throw<InvalidTransitionException>();
    [Fact] public void Shipped_Cancel_Throws() => FluentActions.Invoking(() => Shipped().Cancel()).Should().Throw<InvalidTransitionException>();
    [Fact] public void Shipped_Refund_Throws() => FluentActions.Invoking(() => Shipped().Refund()).Should().Throw<InvalidTransitionException>();
    [Fact] public void Delivered_Cancel_Throws() => FluentActions.Invoking(() => Delivered().Cancel()).Should().Throw<InvalidTransitionException>();

    [Fact]
    public void Cancelled_IsTerminal()
    {
        var o = NewOrder(); o.Cancel();
        FluentActions.Invoking(() => o.Pay()).Should().Throw<InvalidTransitionException>();
        FluentActions.Invoking(() => o.Refund()).Should().Throw<InvalidTransitionException>();
    }

    [Fact]
    public void Refunded_IsTerminal()
    {
        var o = Paid(); o.Refund();
        FluentActions.Invoking(() => o.Ship()).Should().Throw<InvalidTransitionException>();
        FluentActions.Invoking(() => o.Refund()).Should().Throw<InvalidTransitionException>();
    }

    [Fact]
    public void InvalidTransition_CarriesActionAndCurrentStatus()
    {
        var ex = FluentActions.Invoking(() => NewOrder().Ship()).Should().Throw<InvalidTransitionException>().Which;
        ex.Action.Should().Be("ship");
        ex.Current.Should().Be(OrderStatus.Pending);
    }

    [Fact]
    public void InvalidTransition_DoesNotChangeStateOrHistory()
    {
        var o = NewOrder();
        try { o.Ship(); } catch (InvalidTransitionException) { }
        o.Status.Should().Be(OrderStatus.Pending);
        o.History.Should().BeEmpty();
    }

    // ── History ───────────────────────────────────────────────────────────────

    [Fact]
    public void History_RecordsFullHappyPath()
    {
        var o = Delivered();
        o.History.Select(h => h.To).Should().Equal(OrderStatus.Paid, OrderStatus.Shipped, OrderStatus.Delivered);
        o.History.Select(h => h.Step).Should().Equal(1, 2, 3);
    }

    [Fact]
    public void History_RecordsFromAndAction()
    {
        var o = Paid();
        o.History[0].Action.Should().Be("pay");
        o.History[0].From.Should().Be(OrderStatus.Pending);
    }

    // ── Repository ────────────────────────────────────────────────────────────

    [Fact]
    public void Repository_AddAndFind()
    {
        var repo = new OrderRepository();
        var o = repo.Add(NewOrder());
        repo.Find(o.Id).Should().BeSameAs(o);
        repo.All().Should().ContainSingle();
    }

    [Fact] public void Repository_FindUnknown_ReturnsNull() => new OrderRepository().Find(Guid.NewGuid()).Should().BeNull();

    [Fact]
    public void Orders_AreIndependent()
    {
        var a = NewOrder(); var b = NewOrder();
        a.Pay();
        b.Status.Should().Be(OrderStatus.Pending);
    }
}
