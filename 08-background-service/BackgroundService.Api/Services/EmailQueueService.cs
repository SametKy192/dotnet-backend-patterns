using System.Collections.Concurrent;

namespace BackgroundService.Api.Services;

/// <summary>
/// In-memory email kuyruğu — thread-safe.
/// Controller buraya ekler, EmailWorker buradan okur.
/// Gerçek projede RabbitMQ veya Azure Service Bus olur.
/// </summary>
public class EmailQueueService
{
    /// <summary>
    /// ConcurrentQueue — aynı anda birden fazla thread güvenle erişebilir
    /// </summary>
    private readonly ConcurrentQueue<string> _queue = new();

    /// <summary>
    /// Kuyruğa email ekle
    /// </summary>
    public void Enqueue(string email) => _queue.Enqueue(email);

    /// <summary>
    /// Kuyruktan email al — yoksa false döner
    /// </summary>
    public bool TryDequeue(out string? email) => _queue.TryDequeue(out email);

    /// <summary>
    /// Kuyruk boş mu
    /// </summary>
    public bool IsEmpty => _queue.IsEmpty;
}