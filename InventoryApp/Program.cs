using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace InventoryAppNamespace
{
    // Marker interface
    public interface IInventoryEntity
    {
        int Id { get; }
    }

    // Immutable record
    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

    // Generic logger
    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private List<T> _log = new();
        private readonly string _filePath;

        public InventoryLogger(string filePath) => _filePath = filePath;

        public void Add(T item) => _log.Add(item);

        public List<T> GetAll() => new List<T>(_log);

        public void SaveToFile()
        {
            try
            {
                var json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Data saved to '{_filePath}'.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error saving data: {ex.Message}");
                Console.ResetColor();
            }
        }

        public void LoadFromFile()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"File '{_filePath}' not found. No data loaded.");
                    Console.ResetColor();
                    return;
                }

                var json = File.ReadAllText(_filePath);
                _log = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Data loaded from '{_filePath}'.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error loading data: {ex.Message}");
                Console.ResetColor();
            }
        }

        public void Clear() => _log.Clear();
    }

    public class InventoryApp
    {
        private InventoryLogger<InventoryItem> _logger;

        public InventoryApp(string filePath)
        {
            _logger = new InventoryLogger<InventoryItem>(filePath);
        }

        public void AddItemFromUser()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\nEnter new inventory items (type 'done' to finish):");
            Console.ResetColor();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Item ID (or 'done'): ");
                Console.ResetColor();
                string inputId = Console.ReadLine()?.Trim() ?? "";

                if (inputId.ToLower() == "done") break;

                if (!int.TryParse(inputId, out int id))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid ID. Please enter a number.");
                    Console.ResetColor();
                    continue;
                }

                Console.Write("Item Name: ");
                string name = Console.ReadLine()?.Trim() ?? "";

                Console.Write("Quantity: ");
                if (!int.TryParse(Console.ReadLine(), out int qty))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid quantity.");
                    Console.ResetColor();
                    continue;
                }

                _logger.Add(new InventoryItem(id, name, qty, DateTime.Now));

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Item added successfully.");
                Console.ResetColor();
            }
        }

        public void SaveData() => _logger.SaveToFile();

        public void LoadData() => _logger.LoadFromFile();

        public void PrintAllItems()
        {
            var items = _logger.GetAll();
            if (items.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No inventory items found.");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nInventory Items:");
            Console.ResetColor();

            foreach (var item in items)
            {
                Console.WriteLine($"ID: {item.Id} | Name: {item.Name} | Quantity: {item.Quantity} | Date Added: {item.DateAdded}");
            }
        }

        public void ClearMemory()
        {
            _logger.Clear();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Memory cleared — simulating new session.");
            Console.ResetColor();
        }
    }

    class Program
    {
        static void Main()
        {
            string filePath = "inventory_data.json";
            var app = new InventoryApp(filePath);

            // Step 1: User enters items
            app.AddItemFromUser();

            // Step 2: Save to file
            app.SaveData();

            // Step 3: Clear memory (simulate new session)
            app.ClearMemory();

            // Step 4: Load from file
            app.LoadData();

            // Step 5: Display items
            app.PrintAllItems();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
