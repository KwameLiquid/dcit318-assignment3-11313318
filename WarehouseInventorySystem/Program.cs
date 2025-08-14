using System;
using System.Collections.Generic;

namespace WarehouseInventory
{
    // ========= Custom Exceptions =========
    public class DuplicateItemException : Exception
    {
        public DuplicateItemException(string message) : base(message) { }
    }

    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message) { }
    }

    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException(string message) : base(message) { }
    }

    // ========= Marker Interface =========
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    // ========= Product Types =========
    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; }
        public int WarrantyMonths { get; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Brand = brand;
            WarrantyMonths = warrantyMonths;
        }

        public override string ToString()
            => $"[Electronic] #{Id} {Name} | Brand: {Brand} | Warranty: {WarrantyMonths}m | Qty: {Quantity}";
    }

    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            ExpiryDate = expiryDate;
        }

        public override string ToString()
            => $"[Grocery]   #{Id} {Name} | Expires: {ExpiryDate:yyyy-MM-dd} | Qty: {Quantity}";
    }

    // ========= Generic Inventory Repository =========
    public class InventoryRepository<T> where T : IInventoryItem
    {
        private readonly Dictionary<int, T> _items = new Dictionary<int, T>();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
                throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
            _items[item.Id] = item;
        }

        public T GetItemById(int id)
        {
            if (!_items.TryGetValue(id, out var item))
                throw new ItemNotFoundException($"Item with ID {id} was not found.");
            return item;
        }

        public void RemoveItem(int id)
        {
            if (!_items.Remove(id))
                throw new ItemNotFoundException($"Cannot remove: item with ID {id} does not exist.");
        }

        public List<T> GetAllItems() => new List<T>(_items.Values);

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
                throw new InvalidQuantityException("Quantity cannot be negative.");

            var item = GetItemById(id); // may throw ItemNotFoundException
            item.Quantity = newQuantity;
        }
    }

    // ========= Warehouse Manager =========
    public class WareHouseManager
    {
        private readonly InventoryRepository<ElectronicItem> _electronics = new InventoryRepository<ElectronicItem>();
        private readonly InventoryRepository<GroceryItem> _groceries = new InventoryRepository<GroceryItem>();

        // Small helpers for colorful output
        private static void WriteInfo(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(msg);
            Console.ResetColor();
        }
        private static void WriteOk(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(msg);
            Console.ResetColor();
        }
        private static void WriteWarn(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(msg);
            Console.ResetColor();
        }
        private static void WriteErr(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        public void SeedData()
        {
            // Electronics
            _electronics.AddItem(new ElectronicItem(101, "Smartphone X", 15, "TechPro", 24));
            _electronics.AddItem(new ElectronicItem(102, "Laptop Air", 8, "SkyByte", 12));
            _electronics.AddItem(new ElectronicItem(103, "Noise-Cancel Headset", 25, "SoundFox", 18));

            // Groceries
            _groceries.AddItem(new GroceryItem(201, "Rice 5kg", 40, DateTime.Today.AddMonths(10)));
            _groceries.AddItem(new GroceryItem(202, "Milk 1L", 60, DateTime.Today.AddDays(14)));
            _groceries.AddItem(new GroceryItem(203, "Eggs (30)", 22, DateTime.Today.AddDays(10)));
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            foreach (var item in repo.GetAllItems())
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(item.ToString());
                Console.ResetColor();
            }
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                var item = repo.GetItemById(id); // may throw
                var newQty = item.Quantity + quantity;
                repo.UpdateQuantity(id, newQty); // may throw if negative (shouldn’t be here)
                WriteOk($"Stock updated: '{item.Name}' (ID {id}) → {newQty}");
            }
            catch (Exception ex) when (ex is ItemNotFoundException || ex is InvalidQuantityException)
            {
                WriteErr($"IncreaseStock failed: {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id); // may throw
                WriteWarn($"Item with ID {id} was removed.");
            }
            catch (ItemNotFoundException ex)
            {
                WriteErr($"Remove failed: {ex.Message}");
            }
        }

        public void RunDemo()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("=== Warehouse Inventory Management Demo ===");
            Console.ResetColor();

            WriteInfo("\nSeeding initial data...");
            SeedData();

            WriteInfo("\nAll Grocery Items:");
            PrintAllItems(_groceries);

            WriteInfo("\nAll Electronic Items:");
            PrintAllItems(_electronics);

            // Try scenarios required by the question:
            WriteInfo("\n--- Trying exceptional scenarios ---");

            // 1) Add a duplicate item
            try
            {
                _groceries.AddItem(new GroceryItem(201, "Rice 5kg (Duplicate)", 10, DateTime.Today.AddMonths(9)));
                WriteOk("Duplicate add succeeded (unexpected).");
            }
            catch (DuplicateItemException ex)
            {
                WriteErr($"Duplicate add caught: {ex.Message}");
            }

            // 2) Remove a non-existent item
            RemoveItemById(_electronics, 999); // does not exist

            // 3) Update with invalid quantity
            try
            {
                _electronics.UpdateQuantity(101, -5);
                WriteOk("Invalid quantity update succeeded (unexpected).");
            }
            catch (InvalidQuantityException ex)
            {
                WriteErr($"Invalid quantity caught: {ex.Message}");
            }
            catch (ItemNotFoundException ex)
            {
                WriteErr($"Item not found while updating quantity: {ex.Message}");
            }

            // A normal successful update (to showcase success path)
            WriteInfo("\n--- Normal operations ---");
            IncreaseStock(_groceries, 202, 15);  // Increase Milk by 15
            RemoveItemById(_groceries, 203);     // Remove Eggs

            WriteInfo("\nGrocery Items (after updates):");
            PrintAllItems(_groceries);

            WriteInfo("\nElectronic Items (after updates):");
            PrintAllItems(_electronics);

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("\n=== Demo complete ===");
            Console.ResetColor();
        }
    }

    // ========= Program Entry =========
    public class Program
    {
        public static void Main(string[] args)
        {
            var manager = new WareHouseManager();
            manager.RunDemo();

            Console.Write("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
