using Microsoft.AspNetCore.SignalR;

namespace SignalRScalable.Api.Hubs;

/// <summary>
/// Bildirim hub'ı — istemcilerin WebSocket ile bağlandığı nokta.
/// Grup yönetimi: kullanıcılar gruba girer, sadece grup mesajlarını alır.
/// Redis backplane ile birden fazla sunucu arasında mesajlar senkronize edilir.
/// </summary>
public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// İstemci bağlandığında otomatik çağrılır
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("İstemci bağlandı: {ConnectionId}", Context.ConnectionId);
        await Clients.Caller.SendAsync("Connected", $"Bağlandın: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// İstemci bağlantısı kesildiğinde otomatik çağrılır
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("İstemci ayrıldı: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// İstemciyi bir gruba ekle.
    /// Örnek: kullanıcı kendi userId'si ile gruba girer.
    /// </summary>
    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.Caller.SendAsync("JoinedGroup", $"Gruba katıldın: {groupName}");
        _logger.LogInformation("{ConnectionId} gruba katıldı: {Group}", Context.ConnectionId, groupName);
    }

    /// <summary>
    /// Gruptan çık
    /// </summary>
    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        await Clients.Caller.SendAsync("LeftGroup", $"Gruptan çıktın: {groupName}");
    }

    /// <summary>
    /// Gruba mesaj gönder — sadece o gruptaki istemciler alır
    /// </summary>
    public async Task SendToGroup(string groupName, string message)
    {
        await Clients.Group(groupName).SendAsync("ReceiveMessage", message);
    }
}