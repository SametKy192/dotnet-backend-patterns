using ResultPattern.Api.Common;

namespace ResultPattern.Tests;

public sealed class ResultTests
{
    // ── Result<TValue> ────────────────────────────────────────────────────────

    [Fact]
    public void Result_Success_ShouldBeSuccessful()
    {
        var result = Result<int>.Success(42);

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(42, result.Value);
        Assert.Equal(Error.None, result.Error);
    }

    [Fact]
    public void Result_Failure_ShouldBeFailure()
    {
        var error = new Error("Test.Error", "Something went wrong.");
        var result = Result<int>.Failure(error);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void Result_Failure_AccessingValue_ShouldThrow()
    {
        var result = Result<string>.Failure(Error.NullValue);

        Assert.Throws<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    public void Result_ImplicitConversion_FromValue_ShouldSucceed()
    {
        Result<string> result = "hello";

        Assert.True(result.IsSuccess);
        Assert.Equal("hello", result.Value);
    }

    [Fact]
    public void Result_ImplicitConversion_FromError_ShouldFail()
    {
        Result<string> result = Error.NullValue;

        Assert.True(result.IsFailure);
        Assert.Equal(Error.NullValue, result.Error);
    }

    [Fact]
    public void Result_Match_OnSuccess_ShouldCallOnSuccess()
    {
        var result = Result<int>.Success(10);

        var output = result.Match(
            onSuccess: v => $"Value: {v}",
            onFailure: e => $"Error: {e.Code}");

        Assert.Equal("Value: 10", output);
    }

    [Fact]
    public void Result_Match_OnFailure_ShouldCallOnFailure()
    {
        var error = new Error("Err.Code", "desc");
        var result = Result<int>.Failure(error);

        var output = result.Match(
            onSuccess: v => $"Value: {v}",
            onFailure: e => $"Error: {e.Code}");

        Assert.Equal("Error: Err.Code", output);
    }

    // ── Non-generic Result ────────────────────────────────────────────────────

    [Fact]
    public void NonGenericResult_Ok_ShouldBeSuccessful()
    {
        var result = Result.Ok;

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(Error.None, result.Error);
    }

    [Fact]
    public void NonGenericResult_Failure_ShouldBeFailure()
    {
        var error = new Error("Domain.Error", "desc");
        var result = Result.Failure(error);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void NonGenericResult_Match_OnSuccess_ShouldCallOnSuccess()
    {
        var result = Result.Ok;

        var output = result.Match(
            onSuccess: () => "ok",
            onFailure: e => e.Code);

        Assert.Equal("ok", output);
    }

    [Fact]
    public void NonGenericResult_Match_OnFailure_ShouldCallOnFailure()
    {
        var error = new Error("Fail.Code", "desc");
        var result = Result.Failure(error);

        var output = result.Match(
            onSuccess: () => "ok",
            onFailure: e => e.Code);

        Assert.Equal("Fail.Code", output);
    }
}
