using BudgetMgrServiceLib.Models;
using BudgetMgrServiceLib.Processors;

namespace BudgetMgrWebServiceTest
{
    public static class TestCfgDataSet
    {
        public static readonly TestDataCfg testDataCfg = new()
        {
            CharsToRemove = ['\"', '�'],
            DateFilter = new DateFilterCfg(new DateTime(2017, 2, 1), new DateTime(2017, 2, 20), false),
            MiscTransactionNames = [
                "MISC TRANSACTION 1",
                "MISC TRANSACTION 2",
                "MISC TRANSACTION 3"
            ],
            MiscTransactionValue = 10,
            Months = [
                "Jan",
                "Feb",
                "Mar",
                "Apr",
                "May",
                "Jun",
                "Jul",
                "Aug",
                "Sep",
                "Oct",
                "Nov",
                "Dec"
            ],
            NumberOfTransactions = 10,
            Years = [
                "2015",
                "2016"
            ]
        };

        public static readonly GroupTypesCfgEx groupTypesCfg = new GroupTypesCfgEx(
            CreditTypesOfInterest: [
                new CreditTypeRef(1, 1)
            ],
            DebitTypesOfInterest: [
                new DebitTypeRef(1, 2),
                new DebitTypeRef(2, 3),
                new DebitTypeRef(3, 4),
                new DebitTypeRef(4, 5),
                new DebitTypeRef(5, 6),
                new DebitTypeRef(6, 7),
                new DebitTypeRef(7, 8)
            ],
            NameRefs: [
                new TransactionNameEx(1, 1, 1),
                new TransactionNameEx(2, 2, 2),
                new TransactionNameEx(3, 2, 3),
                new TransactionNameEx(4, 3, 4),
                new TransactionNameEx(5, 3, 5),
                new TransactionNameEx(6, 3, 6),
                new TransactionNameEx(7, 3, 7),
                new TransactionNameEx(8, 3, 8),
                new TransactionNameEx(9, 3, 9),
                new TransactionNameEx(10, 3, 10),
                new TransactionNameEx(11, 3, 11),
                new TransactionNameEx(12, 3, 12),
                new TransactionNameEx(13, 3, 13),
                new TransactionNameEx(14, 4, 14),
                new TransactionNameEx(15, 4, 15),
                new TransactionNameEx(16, 4, 16),
                new TransactionNameEx(17, 4, 17),
                new TransactionNameEx(20, 5, 20),
                new TransactionNameEx(22, 6, 22),
                new TransactionNameEx(23, 6, 23),
                new TransactionNameEx(24, 6, 24),
                new TransactionNameEx(25, 6, 25),
                new TransactionNameEx(26, 7, 28),
                new TransactionNameEx(27, 7, 29),
                new TransactionNameEx(28, 7, 30)
            ],
            TransGroupTypes: [
                new TransGroupType(1, "income"),
                new TransGroupType(2, "expense")
            ],
            TransGroups: [
                new TransactionGroupEx(1, "credits", 1),
                new TransactionGroupEx(2, "housekeeping", 2),
                new TransactionGroupEx(3, "bills", 2),
                new TransactionGroupEx(4, "loans", 2),
                new TransactionGroupEx(5, "savings", 2),
                new TransactionGroupEx(6, "shopping", 2),
                new TransactionGroupEx(7, "misc", 2)
            ],
            TransNames: [
                new TransName(1, "WORK"),
                new TransName(2, "SUPERMARKET GB"),
                new TransName(3, "SUPERMARKET DEU"),
                new TransName(4, "TV LICENCE"),
                new TransName(5, "DGS/RPP DD"),
                new TransName(6, "NONMAE GAS SERV"),
                new TransName(7, "HOMIE INSURANCE"),
                new TransName(8, "COUNCIL TAX"),
                new TransName(9, "NONMAE GAS ENERGY"),
                new TransName(10, "NOCARDPLUS LTD"),
                new TransName(11, "WATER COMPANY"),
                new TransName(12, "CABLE TV"),
                new TransName(13, "MAGAZINE SUBSCRIPTION"),
                new TransName(14, "PENSION"),
                new TransName(15, "LOAN"),
                new TransName(16, "MORTGAGE"),
                new TransName(17, "CHARITY"),
                new TransName(18, "BONDS"),
                new TransName(19, "HAVE NONAME"),
                new TransName(20, "SAVINGS"),
                new TransName(21, "INTERNET SHOPPING"),
                new TransName(22, "JUNGLE DEU"),
                new TransName(23, "JUNGLE FR"),
                new TransName(24, "JUNGLE SVCS POR"),
                new TransName(25, "WWW.SOMESHOP.COM"),
                new TransName(26, "JUNGLE FR RETAIL"),
                new TransName(27, "JUNGLE SVCS EUROPE-INTL"),
                new TransName(28, "MISC TRANSACTION 1"),
                new TransName(29, "MISC TRANSACTION 2"),
                new TransName(30, "MISC TRANSACTION 3")
            ],
            TransTypes: [
                new TransType(1, "BANK CREDIT"),
                new TransType(2, "DIRECT DEBIT"),
                new TransType(3, "ATM WITHDRAWAL"),
                new TransType(4, "CASH RECEIPT"),
                new TransType(5, "VISA PURCHASE"),
                new TransType(6, "STANDING ORDER"),
                new TransType(7, "UNPAID TRANSACTION FEE"),
                new TransType(8, "")
            ]
        );

        public static readonly string[] linesOfText = [
            "\"Account Name:\",\"FlexAccount ****97018\"\"",
                "\"Account Balance:\",\"£83.91\"\"",
                "\"Available Balance: \",\"�30.31\"\"",
                "",
                "\"Date\",\"Transaction type\",\"Description\",\"Paid out\",\"Paid in\",\"Balance\"\"",
                "\"02 Nov 2026\",\"BANK CREDIT\",\"WORK\",\"\",\"�1024.83\",\"�0.00\"\"",
                "\"03 Nov 2026\",\"ATM WITHDRAWAL\",\"PENSION\",\"�88.81\",\"\",\"�0.00\"\"",
                "\"07 Nov 2026\",\"STANDING ORDER\",\"NONMAE GAS SERV\",\"�192.29\",\"\",\"�0.00\"\"",
                "\"09 Nov 2026\",\"DIRECT DEBIT\",\"MORTGAGE\",\"�27.16\",\"\",\"�0.00\"\"",
                "\"12 Nov 2026\",\"VISA PURCHASE\",\"SUPERMARKET DEU\",\"�185.69\",\"\",\"�0.00\"\"",
                "\"12 Nov 2026\",\"UNPAID TRANSACTION FEE\",\"MORTGAGE\",\"�106\",\"\",\"�0.00\"\"",
                "\"16 Nov 2026\",\"ATM WITHDRAWAL\",\"JUNGLE FR\",\"�44.46\",\"\",\"�0.00\"\"",
                "\"24 Nov 2026\",\"DIRECT DEBIT\",\"NONMAE GAS ENERGY\",\"�185.36\",\"\",\"�0.00\"\"",
                "\"24 Nov 2026\",\"ATM WITHDRAWAL\",\"WATER COMPANY\",\"�79.37\",\"\",\"�0.00\"\"",
                "\"27 Nov 2026\",\"UNPAID TRANSACTION FEE\",\"COUNCIL TAX\",\"�-6.1\",\"\",\"�0.00\"\"",
                "\"21 Nov 2026\",\"VISA PURCHASE\",\"MISC TRANSACTION 1\",\"�10\",\"\",\"�0.00\"\"",
                "\"25 Nov 2026\",\"DIRECT DEBIT\",\"MISC TRANSACTION 2\",\"�10\",\"\",\"�0.00\"\"",
                "\"26 Nov 2026\",\"VISA PURCHASE\",\"MISC TRANSACTION 3\",\"�10\",\"\",\"�0.00\"\""
        ];

        public static readonly IReadOnlyList<Transaction> fixedTransactions = [
            new Transaction(Id:1, DateStamp:new DateTime(2026, 11 ,5), TransType:"BANK CREDIT", Description:"WORK", Amount:1313.0899999999997),
            new Transaction(Id:2, DateStamp:new DateTime(2026, 11, 1), TransType:"VISA PURCHASE", Description:"MAGAZINE SUBSCRIPTION", Amount:111.9),
            new Transaction(Id:3, DateStamp:new DateTime(2026, 11,11), TransType:"ATM WITHDRAWAL", Description:"JUNGLE FR", Amount:170.58),
            new Transaction(Id:4, DateStamp:new DateTime(2026, 11,12), TransType:"ATM WITHDRAWAL", Description:"WATER COMPANY", Amount:190.27),
            new Transaction(Id:5, DateStamp:new DateTime(2026, 11,12), TransType:"ATM WITHDRAWAL", Description:"CABLE TV", Amount:133.7),
            new Transaction(Id:6, DateStamp:new DateTime(2026, 11,16), TransType:"VISA PURCHASE", Description:"CABLE TV", Amount:155.85),
            new Transaction(Id:7, DateStamp:new DateTime(2026, 11,22), TransType:"VISA PURCHASE", Description:"MAGAZINE SUBSCRIPTION", Amount:71.3),
            new Transaction(Id:8, DateStamp:new DateTime(2026, 11,17), TransType:"UNPAID TRANSACTION FEE", Description:"SAVINGS", Amount:22.79),
            new Transaction(Id:9, DateStamp:new DateTime(2026, 11,21), TransType:"UNPAID TRANSACTION FEE", Description:"CABLE TV", Amount:86.25),
            new Transaction(Id:10, DateStamp:new DateTime(2026, 11,26), TransType:"ATM WITHDRAWAL", Description:"MISC TRANSACTION 2", Amount:181.12),
            new Transaction(Id:11, DateStamp:new DateTime(2026, 11,17), TransType:"UNPAID TRANSACTION FEE", Description:"MISC TRANSACTION 1", Amount:10),
            new Transaction(Id:12, DateStamp:new DateTime(2026, 11,23), TransType:"UNPAID TRANSACTION FEE", Description:"MISC TRANSACTION 2", Amount:10),
            new Transaction(Id:13, DateStamp:new DateTime(2026, 11,25), TransType:"STANDING ORDER", Description:"MISC TRANSACTION 3", Amount:10)
        ];

        public static readonly TransactionGroupCfg[] fixedCfgData = [
            new TransactionGroupCfg(Id:1, GroupName:"credits", GroupType:TransactionGroupTypeProcessor.IncomeTransGroupType, Names:["SALARY"]),
            new TransactionGroupCfg(Id:2, GroupName:"housekeeping", GroupType:TransactionGroupTypeProcessor.ExpenseTransGroupType, Names:["SUPERMARKET GB", "SUPERMARKET DEU", "SUPERMARKET FR"]),
            new TransactionGroupCfg(Id:3, GroupName:"bills", GroupType:TransactionGroupTypeProcessor.ExpenseTransGroupType, Names:["NONMAE GAS SERV", "WATER COMPANY", "CABLE TV"]),
            new TransactionGroupCfg(Id:4, GroupName:"loans", GroupType:TransactionGroupTypeProcessor.ExpenseTransGroupType, Names:["LOAN PAYMENT 1", "LOAN PAYMENT 2", "LOAN PAYMENT 3"]),
            new TransactionGroupCfg(Id:5, GroupName:"savings", GroupType:TransactionGroupTypeProcessor.ExpenseTransGroupType, Names:["CHILD TRUST", "SAVINGS 2", "ISA"]),
            new TransactionGroupCfg(Id:6, GroupName:"shopping", GroupType:TransactionGroupTypeProcessor.ExpenseTransGroupType, Names:["GROCERY STORE", "INTERNET SHOPPING", "JUNGLE DEU", "JUNGLE FR"]),
            new TransactionGroupCfg(Id:7, GroupName:TransactionGroupTypeProcessor.MiscTransGroupType, GroupType:TransactionGroupTypeProcessor.ExpenseTransGroupType, Names:[])
        ];
    }
}
