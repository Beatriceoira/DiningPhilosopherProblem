using System;
using System.Collections.Generic;
using System.Threading;

class Philosopher
{
    private readonly int id;
    private readonly object leftFork;
    private readonly object rightFork;
    private readonly int passes;
    private readonly Random random;
    private readonly Action<int, string> updateTable;
    private readonly Barrier barrier;
    private List<string> roundHistory;

    public Philosopher(int id, object leftFork, object rightFork, int passes, Action<int, string> updateTable, Barrier barrier)
    {
        this.id = id;
        this.leftFork = leftFork;
        this.rightFork = rightFork;
        this.passes = passes;
        this.random = new Random();
        this.updateTable = updateTable;
        this.barrier = barrier;
        this.roundHistory = new List<string>();
    }

    public void StartDining()
    {
        for (int round = 0; round < passes; round++)
        {
            Think(round + 1);
            bool ate = Eat(round + 1);
            string actionMessage = ate
                ? $"Round {round + 1}: Philosopher {id} is eating."
                : $"Round {round + 1}: Philosopher {id} could not eat (starved).";
            roundHistory.Add(actionMessage);

            barrier.SignalAndWait();
        }

        updateTable(id, $"Philosopher {id} has finished dining.");
    }

    private void Think(int round)
    {
        string action = $"Round {round}: Philosopher {id} is thinking.";
        roundHistory.Add(action);
        updateTable(id, action);
        Thread.Sleep(random.Next(500, 1000));
    }

    private bool Eat(int round)
    {

        lock (leftFork)
        {
            updateTable(id, $"Philosopher {id} picked up left fork.");

            bool gotRightFork = Monitor.TryEnter(rightFork, TimeSpan.FromMilliseconds(500));
            if (gotRightFork)
            {
                try
                {

                    string action = $"Round {round}: Philosopher {id} is eating.";
                    roundHistory.Add(action);
                    updateTable(id, action);
                    Thread.Sleep(random.Next(500, 1000));
                    return true;
                }
                finally
                {
                    updateTable(id, $"Philosopher {id} put down right fork.");
                    Monitor.Exit(rightFork);
                }
            }
            else
            {
                updateTable(id, $"Round {round}: Philosopher {id} could not pick up right fork.");
                return false;
            }
        }

        updateTable(id, $"Philosopher {id} put down left fork.");
        return false;
    }

    public List<string> GetRoundHistory()
    {
        return roundHistory;
    }
}

class DiningPhilosophers
{
    private static readonly object ConsoleLock = new object();

    static void DisplayTable(int philosopherCount, string[] states)
    {
        Console.Clear();
        Console.WriteLine("Dining Philosophers Table:");

        for (int i = 0; i < philosopherCount; i++)
        {
            Console.Write($"{states[i],-15}");
        }
        Console.WriteLine();

        for (int i = 0; i < philosopherCount; i++)
        {
            Console.Write($"Philosopher {i + 1} {(states[i] == "Eating" ? "(E)" : "(T)"),-15}");
        }
        Console.WriteLine("\n");
    }

    static void Main(string[] args)
    {
        bool playAgain;

        do
        {
            Console.Clear();
            Console.Write("Enter the number of philosophers: ");
            int philosopherCount;

            while (!int.TryParse(Console.ReadLine(), out philosopherCount) || philosopherCount <= 0)
            {
                Console.Write("Please enter a valid number of philosophers: ");
            }

            Console.Write("Enter the number of passes around the table: ");
            int passes;

            while (!int.TryParse(Console.ReadLine(), out passes) || passes <= 0)
            {
                Console.Write("Please enter a valid number of passes: ");
            }

            object[] forks = new object[philosopherCount];
            for (int i = 0; i < philosopherCount; i++)
            {
                forks[i] = new object();
            }

            string[] states = new string[philosopherCount];
            for (int i = 0; i < philosopherCount; i++) states[i] = "Thinking";

            Philosopher[] philosophers = new Philosopher[philosopherCount];
            List<string> roundSummaries = new List<string>();
            Barrier barrier = new Barrier(philosopherCount);

            Action<int, string> updateTable = (int philosopherId, string message) =>
            {
                if (message.Contains("eating"))
                {
                    states[philosopherId - 1] = "Eating";
                }
                else if (message.Contains("thinking"))
                {
                    states[philosopherId - 1] = "Thinking";
                }

                lock (ConsoleLock)
                {
                    DisplayTable(philosopherCount, states);
                    Console.WriteLine(message);
                }
            };

            Thread[] threads = new Thread[philosopherCount];
            for (int i = 0; i < philosopherCount; i++)
            {
                int philosopherIndex = i;
                object leftFork = forks[i];
                object rightFork = forks[(i + 1) % philosopherCount];

                philosophers[i] = new Philosopher(
                    philosopherIndex + 1,
                    leftFork,
                    rightFork,
                    passes,
                    updateTable,
                    barrier);

                threads[i] = new Thread(philosophers[i].StartDining);
                threads[i].Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            Console.Clear();
            Console.WriteLine("\nAll philosophers have finished dining.");
            Console.WriteLine("\n--- Round Summaries ---");

            for (int round = 0; round < passes; round++)
            {
                int eatersCount = 0;

                for (int i = 0; i < philosopherCount; i++)
                {
                    if (philosophers[i].GetRoundHistory().Contains($"Round {round + 1}: Philosopher {i + 1} is eating."))
                        eatersCount++;
                }

                roundSummaries.Add($"Round {round + 1}: {eatersCount} philosophers ate.");
            }

            foreach (string summary in roundSummaries)
            {
                Console.WriteLine(summary);
            }

            while (true)
            {
                Console.Write("Enter the philosopher's number to view their history (or type 'done' to finish): ");
                string input = Console.ReadLine();
                if (input.ToLower() == "done") break;

                if (int.TryParse(input, out int philosopherId) && philosopherId >= 1 && philosopherId <= philosopherCount)
                {
                    foreach (var action in philosophers[philosopherId - 1].GetRoundHistory())
                    {
                        Console.WriteLine(action);
                    }
                }
                else
                {
                    Console.WriteLine("Invalid philosopher number. Please try again.");
                }
            }

            Console.Write("Would you like to play again? (yes/no): ");
            playAgain = Console.ReadLine().Trim().ToLower() == "yes";

        } while (playAgain);

        Console.WriteLine("Thank you for playing!");
    }
}