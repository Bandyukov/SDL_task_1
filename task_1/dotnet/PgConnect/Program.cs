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

try {
    await using var dataSource = NpgsqlDataSource.Create(string.Format(connectionString!, userName, userPassword));

    await using (var cmd = dataSource.CreateCommand("SELECT VERSION();"))
    await using (var reader = await cmd.ExecuteReaderAsync())
    {
        while (await reader.ReadAsync())
        {
            Console.WriteLine(reader.GetString(0));
        }
    }
} catch (Exception e) {
    Console.WriteLine("ERROR: " + e);
}