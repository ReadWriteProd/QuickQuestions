from collections import defaultdict
import pprint # Using pretty-print for cleaner output

# --- GIVEN CODE (Mock API Service) ---
# (You do not need to modify this part.)

def get_transactions_from_api() -> list[dict]:
    """
    Simulates fetching data from a buggy external API.
    """
    print("Fetching data from buggy API...")
    return [
        {"user_id": 101, "type": "credit", "amount": 50.00},
        {"user_id": 102, "type": "debit",  "amount": 75.20},
        {"user_id": 101, "type": "debit",  "amount": "120.50"}, # BUG 1: Amount is a string
        {"user_id": 103, "type": "credit", "amount": 200.00},
        {"user_id": 101, "type": "debit",  "amount": 30.00},
        {"user_id": 102, "type": "credit", "amount": None},      # BUG 2: Amount is None
        {"user_id": 101},                                      # BUG 3: Missing 'amount' and 'type'
        {"user_id": 103, "type": "refun",  "amount": 10.00},     # BUG 4: Invalid 'type'
        {"user_id": 102, "type": "debit",  "amount": -25.00},    # BUG 5: Invalid (negative) 'amount'
    ]

# --- EXAMINEE'S TASK ---

def process_transactions(transactions: list[dict]) -> tuple[dict, list[dict]]:
    """
    Implement this function to process the raw transaction data.

    The function must return a tuple containing two items:
    1.  `summary_report`: A dictionary (same as before) with user_ids as keys
        and their 'total_debit' / 'total_credit' as values.
    2.  `error_report`: A list of dictionaries. Each dictionary should
        log an error, capturing the original index of the transaction
        and a clear message about what was wrong.
        e.g., {'index': 6, 'message': 'Missing required key: amount'}

    Business Rules for Validation:
    - A transaction is INVALID if:
        1. It is missing 'user_id', 'type', or 'amount'.
        2. 'type' is not *exactly* "credit" or "debit".
        3. 'amount' cannot be converted to a positive float.
    - If a transaction is INVALID, it should be skipped in the summary
      and an entry should be added to the error_report.
    - If a transaction is VALID, it should be added to the summary_report.
    """
    
    # Using defaultdict for cleaner summary logic
    summary_report = defaultdict(lambda: {"total_debit": 0.0, "total_credit": 0.0})
    error_report = []
    
    # TODO: Implement your logic here.
    # You will need to iterate with an index (e.g., using enumerate)
    
    
    # Convert defaultdict back to a regular dict for the final output
    return dict(summary_report), error_report

# --- Main execution for testing ---
raw_data = get_transactions_from_api()
summary, errors = process_transactions(raw_data)

print("\n--- Processed Summary ---")
pprint.pprint(summary)
print("\n--- Error Report ---")
pprint.pprint(errors)
