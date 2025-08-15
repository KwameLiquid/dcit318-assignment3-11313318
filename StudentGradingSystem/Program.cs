using System;
using System.Collections.Generic;
using System.IO;

// Custom exceptions
public class InvalidScoreFormatException : Exception
{
    public InvalidScoreFormatException(string message) : base(message) { }
}

public class MissingFieldException : Exception
{
    public MissingFieldException(string message) : base(message) { }
}

// Student class
public class Student
{
    public int Id;
    public string FullName;
    public int Score;

    public Student(int id, string fullName, int score)
    {
        Id = id;
        FullName = fullName;
        Score = score;
    }

    public string GetGrade()
    {
        if (Score >= 80) return "A";
        if (Score >= 70) return "B";
        if (Score >= 60) return "C";
        if (Score >= 50) return "D";
        return "F";
    }
}

// Processor class
public class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string inputFilePath)
    {
        if (!File.Exists(inputFilePath))
            throw new FileNotFoundException($"Input file not found: {inputFilePath}");

        var students = new List<Student>();

        using (var reader = new StreamReader(inputFilePath))
        {
            string line;
            int lineNumber = 0;
            while ((line = reader.ReadLine()) != null)
            {
                lineNumber++;
                var parts = line.Split(',');

                if (parts.Length != 3)
                    throw new MissingFieldException($"Missing field(s) at line {lineNumber}");

                if (!int.TryParse(parts[0].Trim(), out int id))
                    throw new FormatException($"Invalid student ID format at line {lineNumber}");

                string fullName = parts[1].Trim();

                if (!int.TryParse(parts[2].Trim(), out int score))
                    throw new InvalidScoreFormatException($"Invalid score format at line {lineNumber}");

                students.Add(new Student(id, fullName, score));
            }
        }

        return students;
    }

    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using (var writer = new StreamWriter(outputFilePath))
        {
            foreach (var s in students)
            {
                string reportLine = $"{s.FullName} (ID: {s.Id}): Score = {s.Score}, Grade = {s.GetGrade()}";
                writer.WriteLine(reportLine);
            }
        }
    }
}

// Main Application
class Program
{
    static void Main(string[] args)
    {
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        string inputFile = Path.Combine(baseDir, "students.txt");
        string outputFile = Path.Combine(baseDir, "report.txt");

        var processor = new StudentResultProcessor();

        try
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("📚 Student Grading System - Reading input...");
            Console.ResetColor();

            var students = processor.ReadStudentsFromFile(inputFile);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✅ Successfully read all student records.\n");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("📄 Writing report to file...");
            Console.ResetColor();

            processor.WriteReportToFile(students, outputFile);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✅ Report successfully saved to: {outputFile}");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n--- Student Grades ---");
            foreach (var s in students)
            {
                Console.WriteLine($"{s.FullName} (ID: {s.Id}): Score = {s.Score}, Grade = {s.GetGrade()}");
            }
            Console.ResetColor();
        }
        catch (FileNotFoundException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ {ex.Message}");
            Console.ResetColor();
        }
        catch (InvalidScoreFormatException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ {ex.Message}");
            Console.ResetColor();
        }
        catch (MissingFieldException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ {ex.Message}");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"⚠️ Unexpected error: {ex.Message}");
            Console.ResetColor();
        }

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("\nPress any key to exit...");
        Console.ResetColor();
        Console.ReadKey();
    }
}
