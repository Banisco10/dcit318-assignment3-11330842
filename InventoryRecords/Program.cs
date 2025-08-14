using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;


namespace InventoryRecords
{
    public class InventoryItem : IInventoryEntity
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; }
        public DateTime DateAdded { get; }

        public InventoryItem(int id, string name, int quantity, DateTime dateAdded)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            DateAdded = dateAdded;
        }
    }

    // b. Marker Interface
    public interface IInventoryEntity
    {
        int Id { get; }
    }

    // c. Generic Inventory Logger
    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private List<T> _log = new List<T>();
        private readonly string _filePath;

        public InventoryLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Add(T item)
        {
            _log.Add(item);
        }

        public List<T> GetAll()
        {
            return new List<T>(_log); // Return copy
        }

        public void SaveToFile()
        {
            try
            {
                string jsonData = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
                using (StreamWriter writer = new StreamWriter(_filePath))
                {
                    writer.Write(jsonData);
                }
                Console.WriteLine("Data saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
        }

        public void LoadFromFile()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    Console.WriteLine("No saved file found.");
                    return;
                }

                using (StreamReader reader = new StreamReader(_filePath))
                {
                    string jsonData = reader.ReadToEnd();
                    var items = JsonSerializer.Deserialize<List<T>>(jsonData);
                    if (items != null)
                        _log = items;
                }
                Console.WriteLine("Data loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading from file: {ex.Message}");
            }
        }
    }

    // f. Integration Layer – InventoryApp
    public class InventoryApp
    {
        private InventoryLogger<InventoryItem> _logger;

        public InventoryApp(string filePath)
        {
            _logger = new InventoryLogger<InventoryItem>(filePath);
        }

        public void SeedSampleData()
        {
            _logger.Add(new InventoryItem(1, "Laptop", 10, DateTime.Now));
            _logger.Add(new InventoryItem(2, "Mouse", 50, DateTime.Now));
            _logger.Add(new InventoryItem(3, "Keyboard", 30, DateTime.Now));
            _logger.Add(new InventoryItem(4, "Monitor", 15, DateTime.Now));
            _logger.Add(new InventoryItem(5, "USB Cable", 100, DateTime.Now));
        }

        public void SaveData() => _logger.SaveToFile();
        public void LoadData() => _logger.LoadFromFile();

        public void PrintAllItems()
        {
            foreach (var item in _logger.GetAll())
            {
                Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Date Added: {item.DateAdded}");
            }
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            string filePath = "inventory.json";

            // First session
            InventoryApp app = new InventoryApp(filePath);
            app.SeedSampleData();
            app.SaveData();

            Console.WriteLine("\n--- Simulating new session ---\n");

            // New session
            InventoryApp newAppSession = new InventoryApp(filePath);
            newAppSession.LoadData();
            newAppSession.PrintAllItems();
        }
    }
}
