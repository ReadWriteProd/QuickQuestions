using System;
using System.Collections.Generic;
using System.Globalization;

// --- GIVEN CODE (Mock API Service & Data Models) ---
// (You do not need to modify this part.)

public class ApiTransaction
{
    public int? UserId { get; set; } // Now nullable
    public string Type { get; set; }
    public object Amount { get; set; }
}

public class TransactionService
{
    public List<ApiTransaction> GetTransactionsFromApi()
    {
        Console.WriteLine("Fetching data from buggy API...");
        return new List<ApiTransaction>
        {
            new ApiTransaction { UserId = 101, Type = "credit", Amount = 50.00d },   // 0: Valid
            new ApiTransaction { UserId = 102, Type = "debit",  Amount = 75.20d },   // 1: Valid
            new ApiTransaction { UserId = 101, Type = "debit",  Amount = "120.50" }, // 2: Valid (parsable string)
            new ApiTransaction { UserId = 103, Type = "credit", Amount = 200.00d },  // 3: Valid
            new ApiTransaction { UserId = 101, Type = "debit",  Amount = 30.00d },   // 4: Valid
            new ApiTransaction { UserId = 102, Type = "credit", Amount = null },      // 5: BUG: Null Amount
            new ApiTransaction { UserId = 101, Type = null,    Amount = 20.00d },  // 6: BUG: Null Type
            new ApiTransaction { UserId = 103, Type = "refun",  Amount = 10.00d },    // 7: BUG: Invalid Type
            new ApiTransaction { UserId = 102, Type = "debit",  Amount = -25.00d },   // 8: BUG: Negative Amount
            new ApiTransaction { UserId = null, Type = "credit",Amount = 100.00d }, // 9: BUG: Null UserId
        };
    }
}

// Clean summary data structure
public class UserSummary
{
    public double TotalDebit { get; set; } = 0.0;
    public double TotalCredit { get; set; } = 0.0;
}

// New model for reporting an error
public class ProcessingError
{
    public int OriginalRecordIndex { get; set; }
    public string ErrorMessage { get; set; }
}

// New model for the final result
public class IngestionResult
{
    public Dictionary<int, UserSummary> SummaryReport { get; set; }
    public List<ProcessingError> ErrorReport { get; set; }

    public IngestionResult()
    {
        SummaryReport = new Dictionary<int, UserSummary>();
        ErrorReport = new List<ProcessingError>();
    }
}

// --- SOLUTION TO IMPLEMENT ---

public class TransactionProcessor
{
    public IngestionResult ProcessTransactions(List<ApiTransaction> transactions)
    {
        var result = new IngestionResult();

        for (int i = 0; i < transactions.Count; i++)
        {
            var txn = transactions[i];

            // 1. Validate 'UserId'
            if (txn.UserId == null)
            {
                LogAndContinue(result, i, "Missing required 'UserId'");
                continue;
            }
            // Get the non-nullable ID for the rest of the function
            int userId = txn.UserId.Value;

            // 2. Validate 'Type'
            if (txn.Type == null)
            {
                LogAndContinue(result, i, $"Missing required 'Type' for user {userId}");
                continue;
            }
            
            // 3. Validate 'Amount' (null)
            if (txn.Amount == null)
            {
                LogAndContinue(result, i, $"Missing required 'Amount' for user {userId}");
                continue;
            }

            // 4. Try parsing 'Amount'
            // We use .ToString() to handle both double and string types from the object
            // CultureInfo.InvariantCulture ensures "120.50" parses with a dot, not a comma
            if (!double.TryParse(txn.Amount.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedAmount))
            {
                LogAndContinue(result, i, $"Cannot parse amount: '{txn.Amount}' for user {userId}");
                continue;
            }

            // 5. Validate 'Amount' value (must be positive)
            if (parsedAmount < 0)
            {
                LogAndContinue(result, i, $"Invalid negative amount: {parsedAmount} for user {userId}");
                continue;
            }

            // Ensure the user exists in the summary report
            if (!result.SummaryReport.ContainsKey(userId))
            {
                result.SummaryReport[userId] = new UserSummary();
            }

            // 6. Validate 'Type' value and update summary
            if (txn.Type == "credit")
            {
                result.SummaryReport[userId].TotalCredit += parsedAmount;
            }
            else if (txn.Type == "debit")
            {
                result.SummaryReport[userId].TotalDebit += parsedAmount;
            }
            else
            {
                LogAndContinue(result, i, $"Invalid transaction type: '{txn.Type}' for user {userId}");
                continue;
            }
        }
        
        return result;
    }

    // Helper method to keep the main loop clean
    private void LogAndContinue(IngestionResult result, int index, string message)
    {
        result.ErrorReport.Add(new ProcessingError
        {
            OriginalRecordIndex = index,
            ErrorMessage = message
        });
    }
}

// --- Main execution for testing ---
public class Program
{
    public static void Main(string[] args)
    {
        var service = new TransactionService();
        var processor = new TransactionProcessor();
        
        var rawData = service.GetTransactionsFromApi();
        var report = processor.ProcessTransactions(rawData);

        Console.WriteLine("\n--- Processed Summary ---");
        foreach (var entry in report.SummaryReport)
        {
            Console.WriteLine($"User {entry.Key}: Debit={entry.Value.TotalDebit}, Credit={entry.Value.TotalCredit}");
        }

        Console.WriteLine("\n--- Error Report ---");
        foreach (var error in report.ErrorReport)
        {
            Console.WriteLine($"Index {error.OriginalRecordIndex}: {error.ErrorMessage}");
        }
    }
}
