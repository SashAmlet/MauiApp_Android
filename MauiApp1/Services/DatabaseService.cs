using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Services
{
    public class DatabaseService: IDatabaseService
    {
        private readonly string _databasePath;

        public DatabaseService()
        {
            _databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Numbers.db");
            CreateDatabase();
        }

        private void CreateDatabase()
        {
            try
            {
                using (var connection = new SqliteConnection($"Data Source={_databasePath}"))
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Numbers (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Value INTEGER NOT NULL
                )";
                    command.ExecuteNonQuery();
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        public async Task SaveArrayAsync(List<int> numbers)
        {
            using (var connection = new SqliteConnection($"Data Source={_databasePath}"))
            {
                await connection.OpenAsync();

                using (var transaction = await connection.BeginTransactionAsync())
                {
                    // First, we clear the table
                    var deleteCommand = connection.CreateCommand();
                    deleteCommand.CommandText = "DELETE FROM Numbers";
                    await deleteCommand.ExecuteNonQueryAsync();


                    // Then we insert new values
                    foreach (var number in numbers)
                    {
                        var command = connection.CreateCommand();
                        command.CommandText = @"
                        INSERT INTO Numbers (Value)
                        VALUES ($value)";
                        command.Parameters.AddWithValue("$value", number);
                        await command.ExecuteNonQueryAsync();
                    }

                    await transaction.CommitAsync();
                }
            }
        }

        public async Task<List<int>> GetAllNumbersAsync()
        {
            var numbers = new List<int>();

            using (var connection = new SqliteConnection($"Data Source={_databasePath}"))
            {
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT Value FROM Numbers";

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        numbers.Add(reader.GetInt32(0));
                    }
                }
            }

            return numbers;
        }
    }
}
