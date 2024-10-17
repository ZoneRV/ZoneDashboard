using Dapper;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Serilog;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Retry;
using System.ComponentModel;


namespace DBLibrary.DbAccess
{
    public class SqlDataAccess : ISqlDataAccess
    {
        private readonly IConfiguration _config;

        public SqlDataAccess(IConfiguration config)
        {
            _config = config;
        }

        public async Task<IEnumerable<T>> LoadData<T, U>(string storedProcedure, U parameters, string connectionId = "Default")
        {
            using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));
            
            IEnumerable<T> results = await ExecuteSqlCommand(connection.QueryAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure));

            IEnumerable<T> loadData = results.ToList();
            
            Log.Logger.Debug("{storedProcedure} executed. {results} rows returned.", storedProcedure, loadData.Count());

            return loadData;
        }

        public async Task SaveData<T>(string storedProcedure, T parameters, string connectionId = "Default")
        {
            using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));

            int rowsAffected = await ExecuteSqlCommand(connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure));

            Log.Logger.Debug("{storedProcedure} executed. {rowsAffected} rows affected.", storedProcedure, rowsAffected);
        }

        private async Task<T> ExecuteSqlCommand<T>(Task<T> executeTask)
        {
            IEnumerable<TimeSpan>? delay = Backoff.AwsDecorrelatedJitterBackoff(TimeSpan.FromMilliseconds(50),
                TimeSpan.FromMilliseconds(150), 
                5,
                fastFirst: true);

            AsyncRetryPolicy retryPolicy = Policy
                                           .Handle<SqlException>(SqlServerTransientExceptionDetector.ShouldRetryOn)
                                           .Or<TimeoutException>()
                                           .OrInner<Win32Exception>(SqlServerTransientExceptionDetector.ShouldRetryOn)
                                           .WaitAndRetryAsync(delay, (exception, span, context) =>
                                                                     {
                                                                         Log.Logger.Error(exception, "Sql exception thrown, retrying...");
                                                                     });
            
            
            PolicyResult<T> policyResult = await retryPolicy.ExecuteAndCaptureAsync(
                                                             async () =>
                                                             {
                                                                 await Task.WhenAll(executeTask);
                                                                 return executeTask.Result;
                                                             });

            return policyResult.Result;
        }
    }
}
