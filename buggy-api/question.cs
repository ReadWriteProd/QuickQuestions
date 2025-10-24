using System;
using System.Collections.Generic;

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
            new ApiTransaction { UserId = 101, Type = "credit", Amount = 50.00d },
            new ApiTransaction { UserId = 102, Type = "debit",  Amount = 75.20d },
            new ApiTransaction { UserId = 101, Type = "debit",  Amount = "120.50" }, // BUG 1: String
            new ApiTransaction { UserId = 103, Type = "credit", Amount = 200.00d },
            new ApiTransaction { UserId = 101, Type = "debit",  Amount = 30.00d },
            new ApiTransaction { UserId = 102, Type = "credit", Amount = null },      // BUG 2: Null Amount
            new ApiTransaction { UserId = 101, Type = null,    Amount = 20.00d },  // BUG 3: Null Type
            new ApiTransaction { UserId = 103, Type = "refun",  Amount = 10.00d },    // BUG 4: Invalid Type
            new ApiTransaction { UserId = 102, Type = "debit",  Amount = -25.00d },   // BUG 5: Negative Amount
            new ApiTransaction { UserId = null, Type = "credit",Amount = 100.00d }, // BUG 6: Null UserId
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

// --- EXAMINEE'S TASK ---

public class TransactionProcessor
{
    public IngestionResult ProcessTransactions(List<ApiTransaction> transactions)
    {
        /*
        Implement this function to process the raw transaction data.

        The function must return an 'IngestionResult' object which contains:
        1.  'SummaryReport': A Dictionary with user_ids (int) as keys
            and 'UserSummary' objects as values.
        2.  'ErrorReport': A List of 'ProcessingError' objects.

        Business Rules for Validation:
        - A transaction is INVALID if:
            1. 'UserId', 'Type', or 'Amount' is null.
            2. 'Type' is not *exactly* "credit" or "debit".
            3. 'Amount' cannot be converted to a positive double.
        - If a transaction is INVALID, it should be skipped in the summary
          and a new 'ProcessingError' should be added to the ErrorReport.
        - If a transaction is VALID, it should be added to the SummaryReport.
        */
        
        var result = new IngestionResult();
        
        // TODO: Implement your logic here.
        // You will need a 'for' loop to get the index.
        

        return result;
    }
}

// --- Main execution for testing ---
// (In a real test, you'd have this in a separate Main method)
/*
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
*/
