// See https://aka.ms/new-console-template for more information
using System.Data;
using Npgsql;
using Microsoft.Extensions.Configuration;

Console.WriteLine("User is ?");
var userName =  Console.ReadLine();
Console.WriteLine("Password is ?");
var userPassword =  Console.ReadLine();

IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var connectionString = config.GetConnectionString("DefaultConnection");
await using var dataSource = NpgsqlDataSource.Create(string.Format(connectionString!, userName, userPassword));

try {
    await using var connection = await dataSource.OpenConnectionAsync();
    
    if (connection.State == System.Data.ConnectionState.Open) {
        Console.WriteLine("Connection is established");

        Console.WriteLine("Enter command");

        while (true) {

            var command = Console.ReadLine() ?? "";

            switch (command) {
                case CommandsConfig.HELP:
                    CommandExecutor.ShowHelp();
                    break;

                case CommandsConfig.SHOW_TABLE:
                    await CommandExecutor.ShowTable(connection);
                    break;

                case CommandsConfig.CREATE_FRANCHISE:
                    await CommandExecutor.CreateFranchise(connection);
                    break;

                case CommandsConfig.MODIFY_CAFE:
                    await CommandExecutor.ModifyCafe(connection);
                    break;

                default:
                    Console.WriteLine($"Command {command} does not exist");
                    break;
            }
        }  
    } else {
        Console.Error.WriteLine("Could not establish connection. Try again");
    }
} catch (Npgsql.PostgresException e) {
    Console.Error.WriteLine("This user does not exist. Try again");
} catch (Exception e) {
    Console.Error.WriteLine("Unexpected error " + e.StackTrace);
}    