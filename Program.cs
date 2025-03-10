using System;
using System.Collections.Generic;
using System.Threading;

class BankAccount
{
    private decimal _balance;
    private readonly object _lock = new object();

    public BankAccount(decimal initialBalance)
    {
        _balance = initialBalance;
    }

    public bool Withdraw(decimal amount, string atmName)
    {
        bool lockTaken = false;
        try
        {
            Monitor.Enter(_lock, ref lockTaken);
            if (_balance >= amount)
            {
                _balance -= amount;
                Console.WriteLine($"{atmName}: Выдано {amount:C}. Остаток: {_balance:C}");
                return true;
            }
            else
            {
                Console.WriteLine($"{atmName}: Недостаточно средств. Запрошено {amount:C}, остаток: {_balance:C}");
                return false;
            }
        }
        finally
        {
            if (lockTaken)
                Monitor.Exit(_lock);
        }
    }
}

class ATM
{
    private readonly BankAccount _account;
    private readonly string _atmName;

    public ATM(BankAccount account, string atmName)
    {
        _account = account;
        _atmName = atmName;
    }

    public void Start()
    {
        Random rand = new Random();
        for (int i = 0; i < 5; i++)
        {
            decimal amount = rand.Next(50, 200);
            _account.Withdraw(amount, _atmName);
            Thread.Sleep(rand.Next(500, 1500));
        }
    }
}

class Program
{
    static void Main()
    {
        BankAccount account = new BankAccount(500);
        List<Thread> atms = new List<Thread>();

        for (int i = 1; i <= 3; i++)
        {
            ATM atm = new ATM(account, $"Банкомат {i}");
            Thread thread = new Thread(atm.Start);
            atms.Add(thread);
            thread.Start();
        }

        foreach (var thread in atms)
        {
            thread.Join();
        }

        Console.WriteLine("Все операции завершены.");
    }
}
