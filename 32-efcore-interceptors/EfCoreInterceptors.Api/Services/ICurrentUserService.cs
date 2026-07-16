namespace EfCoreInterceptors.Api.Services;

public interface ICurrentUserService
{
    string? UserId { get; }
}
