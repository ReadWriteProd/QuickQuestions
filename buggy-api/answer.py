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
        {"user_id": 101, "type": "credit", "amount": 50.00},      # 0: Valid
        {"user_id": 102, "type": "debit",  "amount": 75.20},      # 1: Valid
        {"user_id": 101, "type": "debit",  "amount": "120.50"}, # 2: Valid (parsable string)
        {"user_id": 103, "type": "credit", "amount": 200.00},    # 3: Valid
        {"user_id": 101, "type": "debit",  "amount": 30.00},      # 4: Valid
        {"user_id": 102, "type": "credit", "amount": None},      # 5: BUG: Amount is None
        {"user_id": 101},                                      # 6: BUG: Missing 'amount' and 'type'
        {"user_id": 103, "type": "refun",  "amount": 10.00},     # 7: BUG: Invalid 'type'
        {"user_id": 102, "type": "debit",  "amount": -25.00},    # 8: BUG: Invalid (negative) 'amount'
    ]

# --- SOLUTION TO IMPLEMENT ---

def process_transactions(transactions: list[dict]) -> tuple[dict, list[dict]]:
    """
    Implement this function to process the raw transaction data.
    """
    
    # Using defaultdict simplifies adding to the summary
    summary_report = defaultdict(lambda: {"total_debit": 0.0, "total_credit": 0.0})
    error_report = []

    for i, txn in enumerate(transactions):
        
        # 1. Validate 'user_id'
        user_id = txn.get('user_id')
        if user_id is None:
            error_report.append({'index': i, 'message': "Missing required key: 'user_id'"})
            continue

        # 2. Validate 'type'
        txn_type = txn.get('type')
        if txn_type is None:
            error_report.append({'index': i, 'message': f"Missing required key: 'type' for user {user_id}"})
            continue

        # 3. Validate 'amount'
        amount_raw = txn.get('amount')
        if amount_raw is None:
            error_report.append({'index': i, 'message': f"Missing required key: 'amount' for user {user_id}"})
            continue

        # 4. Try parsing 'amount'
        try:
            parsed_amount = float(amount_raw)
        except (ValueError, TypeError):
            error_report.append({'index': i, 'message': f"Cannot parse amount: '{amount_raw}' for user {user_id}"})
            continue
            
        # 5. Validate 'amount' value (must be positive)
        if parsed_amount < 0:
            error_report.append({'index': i, 'message': f"Invalid negative amount: {parsed_amount} for user {user_id}"})
            continue

        # 6. Validate 'type' value
        if txn_type == "credit":
            summary_report[user_id]["total_credit"] += parsed_amount
        elif txn_type == "debit":
            summary_report[user_id]["total_debit"] += parsed_amount
        else:
            error_report.append({'index': i, 'message': f"Invalid transaction type: '{txn_type}' for user {user_id}"})
            continue

    # Convert defaultdict back to a regular dict for the final output
    return dict(summary_report), error_report

# --- Main execution for testing ---
raw_data = get_transactions_from_api()
summary, errors = process_transactions(raw_data)

print("\n--- Processed Summary ---")
pprint.pprint(summary)
print("\n--- Error Report ---")
pprint.pprint(errors)
