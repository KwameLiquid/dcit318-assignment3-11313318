using System;
using System.Collections.Generic;

// ======== RECORD ========
public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

// ======== INTERFACE ========
public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

// ======== PROCESSOR CLASSES ========
public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"[BANK TRANSFER] Processing {transaction.Category} of {transaction.Amount:C} on {transaction.Date:dd-MM-yyyy}");
        Console.ResetColor();
    }
}

public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[MOBILE MONEY] {transaction.Category} payment of {transaction.Amount:C} successfully sent on {transaction.Date:dd-MM-yyyy}");
        Console.ResetColor();
    }
}

public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[CRYPTO WALLET] {transaction.Amount:C} for {transaction.Category} confirmed on blockchain at {transaction.Date:dd-MM-yyyy}");
        Console.ResetColor();
    }
}

// ======== BASE ACCOUNT ========
public class Account
{
    public string AccountNumber { get; }
    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;
        Console.WriteLine($"Transaction applied. New balance: {Balance:C}");
    }
}

// ======== SEALED SAVINGS ACCOUNT ========
public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance)
        : base(accountNumber, initialBalance) { }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] Insufficient funds for {transaction.Category}. Transaction of {transaction.Amount:C} rejected.");
            Console.ResetColor();
        }
        else
        {
            Balance -= transaction.Amount;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"[SUCCESS] {transaction.Category} of {transaction.Amount:C} applied. Updated balance: {Balance:C}");
            Console.ResetColor();
        }
    }
}

// ======== FINANCE APP ========
public class FinanceApp
{
    private List<Transaction> _transactions = new();

    public void Run()
    {
        var account = new SavingsAccount("ACC12345", 1000m);

        // Sample transactions
        var t1 = new Transaction(1, DateTime.Now, 150m, "Groceries");
        var t2 = new Transaction(2, DateTime.Now, 200m, "Utilities");
        var t3 = new Transaction(3, DateTime.Now, 500m, "Entertainment");

        // Processors
        ITransactionProcessor mobileMoney = new MobileMoneyProcessor();
        ITransactionProcessor bankTransfer = new BankTransferProcessor();
        ITransactionProcessor cryptoWallet = new CryptoWalletProcessor();

        // Process and apply
        mobileMoney.Process(t1);
        account.ApplyTransaction(t1);

        bankTransfer.Process(t2);
        account.ApplyTransaction(t2);

        cryptoWallet.Process(t3);
        account.ApplyTransaction(t3);

        // Store transactions
        _transactions.AddRange(new[] { t1, t2, t3 });

        Console.WriteLine("\n=== Transaction Summary ===");
        foreach (var tx in _transactions)
        {
            Console.WriteLine($"{tx.Id}: {tx.Category} - {tx.Amount:C} on {tx.Date:dd-MM-yyyy}");
        }
    }
}

// ======== MAIN PROGRAM ========
class Program
{
    static void Main()
    {
        FinanceApp app = new();
        app.Run();
    }
}
