using Dapper;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace DBLibrary.DbAccess
{
    public class MSSQLDataAccess : ISqlDataAccess
    {
        private readonly IConfiguration _config;

        public MSSQLDataAccess(IConfiguration config)
        {
            _config = config;
        }

        public async Task<IEnumerable<T>> LoadData<T, U>(string storedProcedure, U parameters, string connectionId = "Default")
        {
            using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));
            
            IEnumerable<T> results = await ISqlDataAccess.ExecuteSqlTaskWithRetry(connection.QueryAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure));

            IEnumerable<T> loadData = results.ToList();
            
            Log.Logger.Debug("{storedProcedure} executed. {results} rows returned.", storedProcedure, loadData.Count());
            
            return loadData;
        }

        public async Task SaveData<T>(string storedProcedure, T parameters, string connectionId = "Default")
        {
            using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));

            int rowsAffected = await ISqlDataAccess.ExecuteSqlTaskWithRetry(connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure));

            Log.Logger.Debug("{storedProcedure} executed. {rowsAffected} rows affected.", storedProcedure, rowsAffected);
        }

        public async Task<IEnumerable<T>> ExecuteQuery<T>(string query, T parameters, string connectionId)
        {
            using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));

            IEnumerable<T> results = await ISqlDataAccess.ExecuteSqlTaskWithRetry(connection.QueryAsync<T>(query, commandType: CommandType.Text));

            IEnumerable<T> loadData = results.ToList();
            
            Log.Logger.Debug("{storedProcedure} executed. {results} rows returned.", query, loadData.Count());
            
            return loadData;
        }
    }
}
