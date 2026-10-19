export class Transactiondetails {
  income: Expenditure;
  expenditure: Expenditure;
  balance: number;
  expSummaryPct: number;
}

export class Expenditure {
  transactions: Transaction[];
  total: number;
}

export class TransactionSummaryEntry {
  name: string;
  pct: number;
}

export class DateFilter {
  startDate: string;
  endDate: string;
}

export class TransactionSummaryData {
  income: Expenditure;
  dateRange: DateFilter;
  summaryData: TransactionSummaryEntry[];
}

export class Transaction {
  id: number;
  dateStamp: string;
  transType: string;
  description: string;
  amount: number;
}
