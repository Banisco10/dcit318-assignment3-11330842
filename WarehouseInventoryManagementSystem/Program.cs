using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WarehouseInventoryManagementSystem
{
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

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
        {
            return $"[Electronic] ID: {Id}, Name: {Name}, Brand: {Brand}, Warranty: {WarrantyMonths} months, Qty: {Quantity}";
        }
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
        {
            return $"[Grocery] ID: {Id}, Name: {Name}, Expiry: {ExpiryDate.ToShortDateString()}, Qty: {Quantity}";
        }
    }

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

    public class InventoryRepository<T> where T : IInventoryItem
    {
        private Dictionary<int, T> _items = new Dictionary<int, T>();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
                throw new DuplicateItemException($"Item with ID {item.Id} already exists.");

            _items[item.Id] = item;
        }

        public T GetItemById(int id)
        {
            if (!_items.ContainsKey(id))
                throw new ItemNotFoundException($"Item with ID {id} not found.");

            return _items[id];
        }

        public void RemoveItem(int id)
        {
            if (!_items.ContainsKey(id))
                throw new ItemNotFoundException($"Item with ID {id} not found.");

            _items.Remove(id);
        }

        public List<T> GetAllItems()
        {
            return new List<T>(_items.Values);
        }

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
                throw new InvalidQuantityException("Quantity cannot be negative.");

            if (!_items.ContainsKey(id))
                throw new ItemNotFoundException($"Item with ID {id} not found.");

            _items[id].Quantity = newQuantity;
        }
    }


    public class WareHouseManager
    {
        private InventoryRepository<ElectronicItem> _electronics = new InventoryRepository<ElectronicItem>();
        private InventoryRepository<GroceryItem> _groceries = new InventoryRepository<GroceryItem>();

        public void SeedData()
        {
            _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
            _electronics.AddItem(new ElectronicItem(2, "Smartphone", 25, "Samsung", 12));

            _groceries.AddItem(new GroceryItem(1, "Milk", 50, DateTime.Now.AddDays(7)));
            _groceries.AddItem(new GroceryItem(2, "Rice", 100, DateTime.Now.AddMonths(12)));
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            foreach (var item in repo.GetAllItems())
            {
                Console.WriteLine(item);
            }
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                var item = repo.GetItemById(id);
                repo.UpdateQuantity(id, item.Quantity + quantity);
                Console.WriteLine($"Increased stock for item {item.Name} by {quantity}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error increasing stock: {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine($"Item with ID {id} removed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing item: {ex.Message}");
            }
        }

        public InventoryRepository<ElectronicItem> Electronics => _electronics;
        public InventoryRepository<GroceryItem> Groceries => _groceries;
    }

    class Program
    {
        static void Main(string[] args)
        {
            WareHouseManager manager = new WareHouseManager();

    
            manager.SeedData();

            Console.WriteLine("\nGrocery Items");
            manager.PrintAllItems(manager.Groceries);

            Console.WriteLine("\nElectronic Items");
            manager.PrintAllItems(manager.Electronics);

            Console.WriteLine("\nTesting Exceptions");

            try
            {
                manager.Electronics.AddItem(new ElectronicItem(1, "Tablet", 5, "Apple", 12));
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            manager.RemoveItemById(manager.Groceries, 999);

            
            try
            {
                manager.Groceries.UpdateQuantity(1, -5);
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("\nFinal Grocery Items");
            manager.PrintAllItems(manager.Groceries);

            Console.WriteLine("\n--- Final Electronic Items ---");
            manager.PrintAllItems(manager.Electronics);

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }

}
