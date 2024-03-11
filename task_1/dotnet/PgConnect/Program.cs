// See https://aka.ms/new-console-template for more information
using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Serilog;

const string USERNAME = "USERNAME";
const string USER_PASSWORD = "PASSWORD";
const string PING_INTERVAL = "PING_INTERVAL";
const string LOGS_PATH = "LOGS_PATH";

const string DEFAULT_CONNECTION = "DefaultConnection";

const string SQL_SELECT_VERSION_COMMAND = "SELECT VERSION();";

// region *********************** INIT *********************************

// В СЛУЧАЕ С ДОКЕРОМ БЕРЕТСЯ ИЗ КОНТЕЙНЕРА
// var root = Directory.GetCurrentDirectory();
// var dotenv = Path.Combine(root, "env/settings.env");
// DotEnv.Load(dotenv);

var logger = Path.Combine(Directory.GetCurrentDirectory(), Environment.GetEnvironmentVariable(LOGS_PATH) ?? "some_logger.txt");
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File(logger, rollingInterval: RollingInterval.Day)
    .CreateLogger();

// endregion INIT

// region *********************** CONFIG *********************************

var userName = Environment.GetEnvironmentVariable(USERNAME);
var userPassword = Environment.GetEnvironmentVariable(USER_PASSWORD);

IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var connectionString = config.GetConnectionString(DEFAULT_CONNECTION);

var interval = Environment.GetEnvironmentVariable(PING_INTERVAL);

// endregion CONFIG

// region *********************** CONNECTION *********************************

var attempt = 1;
while(true) {
    try {
        await using var dataSource = NpgsqlDataSource.Create(string.Format(connectionString!, userName, userPassword));
        await using var connection = await dataSource.OpenConnectionAsync();
        if (connection.State == System.Data.ConnectionState.Open) {
            Log.Information("Connection is established. Attempt " + attempt);
        } else {
            Log.Error("Could not establish connection. Attempt " + attempt);
        }

        await using var cmd = new NpgsqlCommand(SQL_SELECT_VERSION_COMMAND, connection);
        await using (var reader = await cmd.ExecuteReaderAsync()) {
            while (await reader.ReadAsync()) {
                var dbVersion = reader.GetString(0);
                
                if (dbVersion != null && dbVersion is string) {
                    Log.Information(dbVersion);
                } else {
                    Log.Information("Database has given strange answer");
                }
            }
        }
    } catch (NpgsqlException e) {
        Log.Error("Error occured when trying to connect to database: " + e.Message);
    } catch (Exception e) {
        Log.Error("Unhandled error: " + e.Message);
    } finally {
        attempt++;
        Log.Information("sleep...");
        Thread.Sleep(TimeSpan.FromSeconds(Convert.ToDouble(interval)));
    }
}

// endregion CONNECTION