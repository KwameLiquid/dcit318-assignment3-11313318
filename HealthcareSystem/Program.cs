using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthSystem
{
    // Generic repository class
    public class Repository<T>
    {
        private List<T> items = new List<T>();

        public void Add(T item) => items.Add(item);
        public List<T> GetAll() => new List<T>(items);
        public T? GetById(Func<T, bool> predicate) => items.FirstOrDefault(predicate);
        public bool Remove(Func<T, bool> predicate)
        {
            var item = items.FirstOrDefault(predicate);
            if (item != null)
            {
                items.Remove(item);
                return true;
            }
            return false;
        }
    }

    // Patient class
    public class Patient
    {
        public int Id;
        public string Name;
        public int Age;
        public string Gender;

        public Patient(int id, string name, int age, string gender)
        {
            Id = id;
            Name = name;
            Age = age;
            Gender = gender;
        }
    }

    // Prescription class
    public class Prescription
    {
        public int Id;
        public int PatientId;
        public string MedicationName;
        public DateTime DateIssued;

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id;
            PatientId = patientId;
            MedicationName = medicationName;
            DateIssued = dateIssued;
        }
    }

    // Health System App
    public class HealthSystemApp
    {
        private Repository<Patient> _patientRepo = new Repository<Patient>();
        private Repository<Prescription> _prescriptionRepo = new Repository<Prescription>();
        private Dictionary<int, List<Prescription>> _prescriptionMap = new Dictionary<int, List<Prescription>>();

        public void SeedData()
        {
            // Adding patients
            _patientRepo.Add(new Patient(1, "Alice Johnson", 28, "Female"));
            _patientRepo.Add(new Patient(2, "Michael Smith", 40, "Male"));
            _patientRepo.Add(new Patient(3, "Sophia Williams", 35, "Female"));

            // Adding prescriptions
            _prescriptionRepo.Add(new Prescription(101, 1, "Amoxicillin", DateTime.Now.AddDays(-10)));
            _prescriptionRepo.Add(new Prescription(102, 1, "Ibuprofen", DateTime.Now.AddDays(-5)));
            _prescriptionRepo.Add(new Prescription(103, 2, "Paracetamol", DateTime.Now.AddDays(-2)));
            _prescriptionRepo.Add(new Prescription(104, 3, "Cough Syrup", DateTime.Now.AddDays(-7)));
            _prescriptionRepo.Add(new Prescription(105, 3, "Vitamin C", DateTime.Now.AddDays(-1)));
        }

        public void BuildPrescriptionMap()
        {
            _prescriptionMap = _prescriptionRepo
                .GetAll()
                .GroupBy(p => p.PatientId)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        // NEW: Required method from question
        public List<Prescription> GetPrescriptionsByPatientId(int patientId)
        {
            if (_prescriptionMap.ContainsKey(patientId))
            {
                return _prescriptionMap[patientId];
            }
            return new List<Prescription>();
        }

        public void PrintAllPatients()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\nList of Patients:");
            Console.ResetColor();

            foreach (var p in _patientRepo.GetAll())
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"ID: {p.Id} | Name: {p.Name} | Age: {p.Age} | Gender: {p.Gender}");
                Console.ResetColor();
            }
        }

        public void PrintPrescriptionsForPatient(int id)
        {
            var prescriptions = GetPrescriptionsByPatientId(id);

            if (prescriptions.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nPrescriptions for Patient ID {id}:");
                Console.ResetColor();

                foreach (var pres in prescriptions)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"  - Prescription ID: {pres.Id} | Medication: {pres.MedicationName} | Issued: {pres.DateIssued.ToShortDateString()}");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nNo prescriptions found for this patient.");
                Console.ResetColor();
            }
        }

        public void Run()
        {
            SeedData();
            BuildPrescriptionMap();
            PrintAllPatients();

            int selectedId;
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("\nEnter Patient ID to view prescriptions: ");
                Console.ResetColor();

                if (int.TryParse(Console.ReadLine(), out selectedId))
                {
                    var patient = _patientRepo.GetById(p => p.Id == selectedId);
                    if (patient != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine($"\nPatient Details → ID: {patient.Id}, Name: {patient.Name}, Age: {patient.Age}, Gender: {patient.Gender}");
                        Console.ResetColor();

                        PrintPrescriptionsForPatient(selectedId);
                        break;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("No patient found with that ID. Try again.");
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid input. Please enter a number.");
                    Console.ResetColor();
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var app = new HealthSystemApp();
            app.Run();
        }
    }
}
