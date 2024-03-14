using System.Data;
using Npgsql;

public static class CommandExecutor {

    private static string[] cafeColumns = ["franchise_id", "name", "city", "type", "popularity", "raiting"];
    private static string[] tables = ["Franchises", "Cafe", "Cities", "Types"];


    public static void ShowHelp() {
        Console.WriteLine("Commands:");
        Console.WriteLine(CommandsConfig.SHOW_TABLE + " - show data in selected table");
        Console.WriteLine(CommandsConfig.CREATE_FRANCHISE + " - create a franchise");
        Console.WriteLine(CommandsConfig.MODIFY_CAFE + " - allows to enter command for modifying cafe table");
    }

    public static async Task ShowTable(NpgsqlConnection connection) {
        Console.WriteLine($"Enter table number from 0 to {tables.Length} where");
        for (int i = 0; i < tables.Length; i++) {
            Console.WriteLine($"{i} = {tables[i]}");
        }
        string table = tables[int.Parse(Console.ReadLine())];

        string query = $"SELECT * FROM {table}";
        await using var command = new NpgsqlCommand(query, connection);

        ExecuteAndReadCommand(command);
    }

    public static async Task CreateFranchise(NpgsqlConnection connection) {
        
        Console.WriteLine("Enter franchise name");
        var name = Console.ReadLine();
        Console.WriteLine("Enter franchise code");
        var code = Console.ReadLine();

        await using var command = new NpgsqlCommand("INSERT INTO Franchises (name, code) VALUES (@name, @code)", connection);
        command.Parameters.AddWithValue("name", name);
        command.Parameters.AddWithValue("code", code);
        
        ExecuteCommand(command);
    }

    public static async Task ModifyCafe(NpgsqlConnection connection) {
        while (true) {

            Console.WriteLine("Enter command for cafe. Use /help for info");
            var commandRequest = Console.ReadLine();

            switch (commandRequest) {
                case CommandsConfig.HELP:
                    Console.WriteLine(CommandsConfig.SHOW + " - show cafe for selected franchise");
                    Console.WriteLine(CommandsConfig.SHOW_FILTER + " - show cafe filtered by popularity or raiting");
                    Console.WriteLine(CommandsConfig.CREATE + " - create new cafe");
                    Console.WriteLine(CommandsConfig.UPDATE + " - update cafe data");
                    Console.WriteLine(CommandsConfig.EXIT + " - exit from cafe modifier");
                    break;

                case CommandsConfig.SHOW: {
                    Console.WriteLine("Enter franchise id");
                    var franchiseId = Console.ReadLine();

                    await using var command = new NpgsqlCommand("SELECT id, name, city, type, popularity, raiting FROM Cafe WHERE franchise_id = @franchise_id", connection);
                    command.Parameters.AddWithValue("franchise_id", int.Parse(franchiseId));

                    
                    ExecuteAndReadCommand(command);
                    
                    break;
                }

                case CommandsConfig.SHOW_FILTER: {
                    Console.WriteLine("Enter filter with 0 or 1, where 0 - is popularity, 1 is raiting");
                    string field;
                    int fieldSwithcer = int.Parse(Console.ReadLine() ?? "0");
                    if (fieldSwithcer == 0) {
                        field = "popularity";
                    } else {
                        field = "raiting";
                    }

                    Console.WriteLine("Enter operator = or < or >");
                    var operand = Console.ReadLine();

                    Console.WriteLine("Enter value");
                    var value = Console.ReadLine();

                    await using var command = new NpgsqlCommand($"SELECT id, name, city, type, popularity, raiting FROM Cafe WHERE {field} {operand} @value", connection);

                    if (fieldSwithcer == 0) command.Parameters.AddWithValue("@value", int.Parse(value)); 
                    else command.Parameters.AddWithValue("@value", double.Parse(value, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo)); 

                    
                    ExecuteAndReadCommand(command);

                    break;
                }

                case CommandsConfig.CREATE: {

                    Console.WriteLine("Enter franchise ID");
                    var franchiseId = Console.ReadLine();
                    
                    Console.WriteLine("Enter cafe name");
                    var name = Console.ReadLine();

                    Console.WriteLine("Enter cafe city");
                    var city = Console.ReadLine();

                    Console.WriteLine("Enter cafe type");
                    var type = Console.ReadLine();

                    Console.WriteLine("Enter cafe popularity");
                    var popularity = Console.ReadLine();

                    Console.WriteLine("Enter cafe raiting");
                    var raiting = Console.ReadLine();

                    using var command = new NpgsqlCommand("INSERT INTO Cafe (franchise_id, name, city, type, popularity, raiting) VALUES (@franchise_id, @name, @city, @type, @popularity, @raiting)", connection);
                    command.Parameters.AddWithValue("franchise_id", int.Parse(franchiseId));
                    command.Parameters.AddWithValue("name", name);
                    command.Parameters.AddWithValue("city", city);
                    command.Parameters.AddWithValue("type", type);
                    command.Parameters.AddWithValue("popularity", int.Parse(popularity));
                    command.Parameters.AddWithValue("raiting", double.Parse(raiting, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo));
                   
                    ExecuteCommand(command);

                    break;
                }

                case CommandsConfig.UPDATE: {

                    Console.WriteLine("Enter cafe name");
                    var name = Console.ReadLine();

                    Console.WriteLine($"Enter column with number from 0 to {cafeColumns.Length} where");
                    for (int i = 0; i < cafeColumns.Length; i++) {
                        Console.WriteLine($"{i} = {cafeColumns[i]}");
                    }
                    var columnSwitcher = int.Parse(Console.ReadLine());
                    var column = cafeColumns[columnSwitcher];

                    Console.WriteLine("Enter value");
                    var value = Console.ReadLine();

                    using var command = new NpgsqlCommand($"UPDATE Cafe SET {column} = @value WHERE name = @name", connection);
                    switch (columnSwitcher) {
                        case 0:
                        case 4: {
                            command.Parameters.AddWithValue("value", int.Parse(value));
                            break;
                        }

                        case 5: {
                            command.Parameters.AddWithValue("value", double.Parse(value, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo));
                            break;
                        }

                        default: {
                            command.Parameters.AddWithValue("value", value);
                            break;
                        }
                    }


                    command.Parameters.AddWithValue("name", name);
                    ExecuteCommand(command);
                    
                    break;
                }


                case CommandsConfig.EXIT: {
                    Console.WriteLine("You have exited from Cafe modifier. Enter Command");
                    return;
                }

                default:
                    Console.WriteLine($"Command {commandRequest} does not exist");
                    break;
            }
        }
        
    }

    private static async void ExecuteAndReadCommand(NpgsqlCommand command) {
        try {
            await using var reader = await command.ExecuteReaderAsync();
            ReadData(reader);
        } catch (Exception e) {
            Console.Error.WriteLine(e.StackTrace);
            Console.WriteLine("Try again");
        }
    }
    private static async void ExecuteCommand(NpgsqlCommand command) {
        try {
            await command.ExecuteNonQueryAsync();
        } catch (Exception e) {
            Console.Error.WriteLine(e.StackTrace);
            Console.WriteLine("Try again");
        }
    }

    private static async void ReadData(NpgsqlDataReader reader) {
        while (await reader.ReadAsync()) {
            for (int i = 0; i < reader.FieldCount; i++) {
                Console.Write(reader[i].ToString() + ", ");
            }
            Console.WriteLine("");
        }
    }
}