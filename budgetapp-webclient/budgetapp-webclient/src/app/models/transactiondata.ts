export class Transaction {
  id: number;
  dateStamp: string;
  transType: string;
  description: string;
  amount: number;
}

export class Transactiondata {
  type: string;
  transactions: Transaction[];
}
