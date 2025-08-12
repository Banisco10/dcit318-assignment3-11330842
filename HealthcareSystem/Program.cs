using System;
using System.Collections.Generic;

namespace HealthcareSystem
{
    public class Repository<T>
    {
        private readonly List<T> items = new List<T>();

        public void Add(T item)
        {
            items.Add(item);
        }

        public List<T> GetAll()
        {
            return new List<T>(items);
        }

        public T GetById(Func<T, bool> predicate)
        {
            foreach (var item in items)
            {
                if (predicate(item)) return item;
            }
            return default(T);
        }
        public bool Remove(Func<T, bool> predicate)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (predicate(items[i]))
                {
                    items.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
    }

    public class Patient
    {
        public int Id { get; }
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }

        public Patient(int id, string name, int age, string gender)
        {
            Id = id;
            Name = name;
            Age = age;
            Gender = gender;
        }

        public override string ToString()
        {
            return $"ID: {Id}, Name: {Name}, Age: {Age}, Gender: {Gender}";
        }
    }

    public class Prescription
    {
        public int Id { get; }
        public int PatientId { get; }
        public string MedicationName { get; }
        public DateTime DateIssued { get; }

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id;
            PatientId = patientId;
            MedicationName = medicationName;
            DateIssued = dateIssued;
        }

        public override string ToString()
        {
            return $"Prescription ID: {Id}, Medication: {MedicationName}, Date: {DateIssued:d}";
        }
    }

    public class HealthSystemApp
    {
        private readonly Repository<Patient> _patientRepo = new Repository<Patient>();
        private readonly Repository<Prescription> _prescriptionRepo = new Repository<Prescription>();
        private readonly Dictionary<int, List<Prescription>> _prescriptionMap = new Dictionary<int, List<Prescription>>();

        public void SeedData()
        {

            _patientRepo.Add(new Patient(1, "Alice Smith", 30, "Female"));
            _patientRepo.Add(new Patient(2, "Kwame Nkrumah", 47, "Male"));
            _patientRepo.Add(new Patient(3, "Clara Doe", 22, "Female"));

            _prescriptionRepo.Add(new Prescription(1, 1, "Amoxicillin 500mg", DateTime.Now.AddDays(-10)));
            _prescriptionRepo.Add(new Prescription(2, 1, "Paracetamol 500mg", DateTime.Now.AddDays(-2)));
            _prescriptionRepo.Add(new Prescription(3, 2, "Atorvastatin 20mg", DateTime.Now.AddDays(-30)));
            _prescriptionRepo.Add(new Prescription(4, 3, "Iron Supplement", DateTime.Now.AddDays(-1)));
            _prescriptionRepo.Add(new Prescription(5, 2, "Vitamin D", DateTime.Now.AddDays(-7)));
        }


        public void BuildPrescriptionMap()
        {
            _prescriptionMap.Clear();
            var allPrescriptions = _prescriptionRepo.GetAll();
            foreach (var pres in allPrescriptions)
            {
                List<Prescription> list;
                if (!_prescriptionMap.TryGetValue(pres.PatientId, out list))
                {
                    list = new List<Prescription>();
                    _prescriptionMap[pres.PatientId] = list;
                }
                list.Add(pres);
            }
        }


        public void PrintAllPatients()
        {
            Console.WriteLine("=== All Patients ===");
            var patients = _patientRepo.GetAll();
            foreach (var p in patients)
            {
                Console.WriteLine(p);
            }
        }


        public List<Prescription> GetPrescriptionsByPatientId(int patientId)
        {
            if (_prescriptionMap.TryGetValue(patientId, out var list))
            {
                return new List<Prescription>(list);
            }
            return new List<Prescription>();
        }


        public void PrintPrescriptionsForPatient(int id)
        {
            var patient = _patientRepo.GetById(p => ((Patient)p).Id == id);
            if (patient == null)
            {
                Console.WriteLine($"No patient found with ID {id}");
                return;
            }

            Console.WriteLine($"\nPrescriptions for {patient.Name} (ID: {id}):");
            var prescriptions = GetPrescriptionsByPatientId(id);
            if (prescriptions.Count == 0)
            {
                Console.WriteLine("  No prescriptions found.");
                return;
            }

            foreach (var pr in prescriptions)
            {
                Console.WriteLine("  " + pr);
            }
        }


        public static void Main()
        {
            var app = new HealthSystemApp();
            app.SeedData();
            app.BuildPrescriptionMap();

            app.PrintAllPatients();

            Console.WriteLine("\nEnter a Patient ID to view prescriptions (e.g., 1):");
            string input = Console.ReadLine();
            int patientId;
            if (int.TryParse(input, out patientId))
            {
                app.PrintPrescriptionsForPatient(patientId);
            }
            else
            {
                Console.WriteLine("Invalid patient ID input.");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
