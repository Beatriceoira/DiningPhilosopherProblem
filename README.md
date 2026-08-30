# Dining Philosophers Problem

A C# console application that demonstrates the classic **Dining Philosophers Problem**, a fundamental concurrency and synchronization problem in computer science.

The simulation models philosophers who alternate between thinking and eating while competing for shared forks. Each philosopher runs on its own thread, making synchronization necessary to prevent unsafe access to shared resources.

Please note this was a simple class activity done in 2nd year of Computer Engineering

## Features

- Configurable number of philosophers
- Configurable number of dining rounds
- Multithreaded philosopher simulation using `System.Threading.Thread`
- Shared fork resources protected with `lock` and `Monitor.TryEnter`
- Barrier synchronization so philosophers progress round-by-round
- Console visualization of philosopher states
- Round-by-round eating summaries
- Individual philosopher activity history
- Option to run the simulation again

## Concepts Demonstrated

This project is designed to demonstrate practical concurrency concepts:

- **Threads** — each philosopher runs independently.
- **Mutual exclusion** — fork objects are synchronized so multiple philosophers cannot use the same fork simultaneously.
- **Locking** — `lock` protects access to a philosopher's left fork.
- **Try-lock behavior** — `Monitor.TryEnter` prevents a philosopher from waiting indefinitely for the right fork.
- **Barriers** — all philosophers synchronize at the end of each round.
- **Race conditions** — shared state such as the console display requires synchronization.
- **Deadlock avoidance** — the simulation uses a timed attempt to acquire the second fork instead of waiting indefinitely.

## How It Works

Each philosopher repeatedly:

1. Thinks for a randomized amount of time.
2. Attempts to acquire the left fork.
3. Attempts to acquire the right fork for up to 500 ms.
4. Eats if both forks are acquired.
5. Releases the forks.
6. Records the action in their history.
7. Waits at the barrier for the other philosophers to finish the round.

After all rounds are complete, the program displays a summary and allows the user to inspect an individual philosopher's history.

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Windows, macOS, or Linux
- A terminal or IDE such as Visual Studio, Visual Studio Code, or JetBrains Rider

## Running the Project

### Clone the repository

```bash
git clone <your-repository-url>
cd DiningPhilosopherProblem
```

### Run with .NET

```bash
dotnet run --project DiningPhilosophersProblem
```

Or open `DiningPhilosophersProblem.sln` in Visual Studio and run the project.

## Example

```text
Enter the number of philosophers: 5
Enter the number of passes around the table: 3

Dining Philosophers Table:
Thinking        Eating         Thinking        Thinking        Eating
...
```

The exact output varies because thinking and eating delays are randomized and multiple threads execute concurrently.

## Project Structure

```text
DiningPhilosopherProblem/
├── DiningPhilosophersProblem.sln
├── DiningPhilosophersProblem/
│   ├── DiningPhilosophersProblem.csproj
│   └── Program.cs
├── .gitignore
└── README.md
```

## Why This Project?

The Dining Philosophers Problem is a classic example of the challenges involved in concurrent programming. It illustrates how independently executing processes can compete for limited shared resources and why synchronization strategies are necessary.

This project provides a practical demonstration of those concepts using native C# threading and synchronization primitives.

## Possible Improvements

- Add a graphical interface
- Add configurable thinking/eating durations
- Track starvation statistics
- Display fork ownership
- Add more explicit deadlock/starvation detection
- Replace manual threads with `Task`-based asynchronous execution
- Separate simulation logic, UI, and models into individual classes
- Add automated unit tests


