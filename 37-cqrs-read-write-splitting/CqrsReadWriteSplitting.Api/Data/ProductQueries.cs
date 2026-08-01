using Dapper;
using Microsoft.Data.Sqlite;
using CqrsReadWriteSplitting.Api.Domain.Queries;

namespace CqrsReadWriteSplitting.Api.Data;

public class ProductQueries : IProductQueries
{
    private readonly SqliteConnection _connection;

    public ProductQueries(SqliteConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<ProductReadModel>> GetAllAsync()
    {
        const string sql = "SELECT Id, Name, Price, Stock FROM Products";
        return await _connection.QueryAsync<ProductReadModel>(sql);
    }

    public async Task<ProductReadModel?> GetByIdAsync(int id)
    {
        const string sql = "SELECT Id, Name, Price, Stock FROM Products WHERE Id = @Id";
        return await _connection.QueryFirstOrDefaultAsync<ProductReadModel>(sql, new { Id = id });
    }
}
