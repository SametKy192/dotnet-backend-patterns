namespace MultiTenancy.Domain.Entities;

/// <summary>
/// Tenant'a ait entity'lerin implement etmesi gereken interface.
/// TenantId — hangi tenant'a ait olduğunu belirtir.
/// </summary>
public interface ITenantEntity
{
    string TenantId { get; set; }
}