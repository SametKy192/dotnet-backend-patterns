namespace CqrsReadWriteSplitting.Api.Domain.Queries;

public record ProductReadModel(int Id, string Name, decimal Price, int Stock);

public interface IProductQueries
{
    Task<IEnumerable<ProductReadModel>> GetAllAsync();
    Task<ProductReadModel?> GetByIdAsync(int id);
}
