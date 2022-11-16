using Dapper;
using System.Data;

namespace RespawnDemo.Api.Shared.DataAccess;
public interface IDatabase
{
    Task<IEnumerable<T>> ExecuteFileAsync<T>(string filename, object? parameters = null);
    Task ExecuteFileAsync(string filename, object? parameters);
    Task<int> ExecuteFileAsync(string fileName, IEnumerable<object> parameters);
    Task<(IEnumerable<T1>, IEnumerable<T2>)> ExecuteMultiFileAsync<T1, T2>(string filename, object? parameters = null);
    Task<(IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>)> ExecuteMultiFileAsync<T1, T2, T3>(string filename
        , object? parameters = null);
    Task<(IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>)> ExecuteMultiFileAsync<T1, T2, T3, T4>
        (string filename, object? parameters = null);
    Task<(IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>)>
        ExecuteMultiFileAsync<T1, T2, T3, T4, T5>(string filename, object? parameters = null);
    Task<IEnumerable<T>> ExecuteProcAsync<T>(string procName, object? parameters);
}

public class EmployeeDatabase : IDatabase
{
    private readonly IDbConnection dbConnection;
    private readonly IEmbeddedFileReader fileReader;

    public EmployeeDatabase(IDbConnection dbConnection, IEmbeddedFileReader fileReader)
    {
        this.dbConnection = dbConnection;
        this.fileReader = fileReader;
    }

    public Task ExecuteFileAsync(string filename, object? parameters)
    {
        return ExecuteFileAsync<object>(filename, parameters);
    }

    public async Task<IEnumerable<T>> ExecuteFileAsync<T>(string filename, object? parameters)
    {
        var sql = fileReader.ReadFile(filename);
        var response = await dbConnection.QueryAsync<T>(sql, parameters);
        return response;
    }

    public async Task<int> ExecuteFileAsync(string filename, IEnumerable<object>? parameters)
    {
        var sql = fileReader.ReadFile(filename);
        if (dbConnection.State != ConnectionState.Open)
        {
            dbConnection.Open();
        }
        using var trans = dbConnection.BeginTransaction();
        var response = await dbConnection.ExecuteAsync(sql, parameters, trans);
        trans.Commit();
        return response;
    }

    public async Task<(IEnumerable<T1>, IEnumerable<T2>)> ExecuteMultiFileAsync<T1, T2>(string filename,
        object? parameters = null)
    {
        var sql = fileReader.ReadFile(filename);
        using var multi = await dbConnection.QueryMultipleAsync(sql, parameters);
        var resultOne = await multi.ReadAsync<T1>();
        var resultTwo = await multi.ReadAsync<T2>();

        return (resultOne, resultTwo);
    }

    public async Task<(IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>)>
        ExecuteMultiFileAsync<T1, T2, T3>(string filename, object? parameters = null)
    {
        var sql = fileReader.ReadFile(filename);
        using var multi = await dbConnection.QueryMultipleAsync(sql, parameters);
        var resultOne = await multi.ReadAsync<T1>();
        var resultTwo = await multi.ReadAsync<T2>();
        var resultThree = await multi.ReadAsync<T3>();

        return (resultOne, resultTwo, resultThree);
    }

    public async Task<(IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>)>
        ExecuteMultiFileAsync<T1, T2, T3, T4>(string filename, object? parameters = null)
    {
        var sql = fileReader.ReadFile(filename);
        using var multi = await dbConnection.QueryMultipleAsync(sql, parameters);
        var resultOne = await multi.ReadAsync<T1>();
        var resultTwo = await multi.ReadAsync<T2>();
        var resultThree = await multi.ReadAsync<T3>();
        var resultFour = await multi.ReadAsync<T4>();

        return (resultOne, resultTwo, resultThree, resultFour);
    }

    public async Task<(IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>)>
        ExecuteMultiFileAsync<T1, T2, T3, T4, T5>(string filename, object? parameters = null)
    {
        var sql = fileReader.ReadFile(filename);
        using var multi = await dbConnection.QueryMultipleAsync(sql, parameters);
        var resultOne = await multi.ReadAsync<T1>();
        var resultTwo = await multi.ReadAsync<T2>();
        var resultThree = await multi.ReadAsync<T3>();
        var resultFour = await multi.ReadAsync<T4>();
        var resultFive = await multi.ReadAsync<T5>();

        return (resultOne, resultTwo, resultThree, resultFour, resultFive);
    }

    public Task<IEnumerable<T>> ExecuteProcAsync<T>(string procName, object? parameters = null) => dbConnection
        .QueryAsync<T>(procName, parameters, commandType: CommandType.StoredProcedure);
}