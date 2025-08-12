using System;
using System.Collections.Generic;

namespace FinanceManagementSystem
{
    public class Transaction
    {
        public int Id { get; }
        public DateTime Date { get; }
        public decimal Amount { get; }
        public string Category { get; }

        public Transaction(int id, DateTime date, decimal amount, string category)
        {
            Id = id;
            Date = date;
            Amount = amount;
            Category = category;
        }

        public override string ToString()
        {
            return $"{Id}: {Category} - {Amount:C} on {Date:d}";
        }
    }

    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[BankTransfer] Processing {transaction.Category} of {transaction.Amount:C} on {transaction.Date:d} (Id: {transaction.Id})");
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[MobileMoney] Processing {transaction.Category} of {transaction.Amount:C}. Quick mobile transfer complete. (Id: {transaction.Id})");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[CryptoWallet] Recording crypto payment for {transaction.Category}: {transaction.Amount:C}. (Id: {transaction.Id})");
        }
    }

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
            Console.WriteLine($"Account {AccountNumber} new balance: {Balance:C}");
        }
    }

        public sealed class SavingsAccount : Account
        {
            public SavingsAccount(string accountNumber, decimal initialBalance)
                : base(accountNumber, initialBalance) { }

            public override void ApplyTransaction(Transaction transaction)
            {
                if (transaction.Amount > Balance)
                {
                    Console.WriteLine("Insufficient funds");
                    return;
                }

                Balance -= transaction.Amount;
                Console.WriteLine($"Transaction applied. Updated balance: {Balance:C}");
            }
        }

 
            public class FinanceApp
            {
                private readonly List<Transaction> _transactions = new List<Transaction>();

                public void Run()
                {
                    var account = new SavingsAccount("SA-1001", 1000m);

                    var t1 = new Transaction(1, DateTime.Now, 120.50m, "Groceries");
                    var t2 = new Transaction(2, DateTime.Now, 300.00m, "Utilities");
                    var t3 = new Transaction(3, DateTime.Now, 900.00m, "Entertainment");

                    ITransactionProcessor p1 = new MobileMoneyProcessor();
                    ITransactionProcessor p2 = new BankTransferProcessor();
                    ITransactionProcessor p3 = new CryptoWalletProcessor();

                    p1.Process(t1);
                    account.ApplyTransaction(t1);
                    _transactions.Add(t1);

                    p2.Process(t2);
                    account.ApplyTransaction(t2);
                    _transactions.Add(t2);

                    p3.Process(t3);
                    account.ApplyTransaction(t3);
                    _transactions.Add(t3);

                    Console.WriteLine("\nAll transactions logged:");
                    foreach (var t in _transactions)
                    {
                        Console.WriteLine(t);
                    }
                }

                public static void Main()
                {
                    var app = new FinanceApp();
                    app.Run();

                    Console.WriteLine("\nPress any key to exit...");
                    Console.ReadKey();
                }

            }
        }
