# **Budget Reporter**

Ambrose Adamson 

# 

# **Overview**

The aim of this project is to simplify the calculation and managing my budget from exported bank statements in a way that can be clearly explained to my wife, as well as practice modern development tools and techniques.

# **Goals**

1. Read bank statements and convert them into income and expenditure.

2. Display the balance based upon the income and expenditure.

3. Separate the  expenditure into different groups.

4. Display the difference types of expenditure as a percentage of the income 

5. Display a summary of the different types of expenditure from the budget spent 

6. Manage the different types of expenditure represented in the statement.

# **Specifications**

## Current implementation \- Google Spreadsheets

The current means of calculating our budget is to export the bank statement from an online banking service such as Nationwide Building Society Banking Web Site or Mobile App, collect the totals from the statements  and enter them into a Google spreadsheet to calculate the income, expenditure and balance.

The process is as follows:

* Login into your online banking service  
* Export your monthly bank statement  
* Open your budget spreadsheet template  
* Enter each of the income values from the statement into the income column of the spreadsheet  
* Enter each of the expenditure values from the statement into the expenditure column of the spreadsheet  
* Ensure that the ranges around the columns cover the entered values  
* Ensure the formulas that calculates the balance uses those ranges

This is very time consuming and fraught with errors such as the following:

* Entering the values incorrectly   
* Entering the names of the income / expenditure incorrectly   
* Reconfiguration of ranges for formulas to  calculate income, expenditure and balance as additional items are added   
* Managing the different types of expenditure used in the calculations.

There is also the issue that the user has to have access to Google Sheet app, and modify the  budget spreadsheet template in order to handle the data entered from the statements to perform the task, which is very time consuming.

## Proposed Implementation 

These issues could be rectified by creating an application that performs the following :

* Automate the entry of the bank statement data by importing the bank statement into the application and performing the necessary conversions  
* Automate the Calculations of the income, expenditure and balance from the bank statement data   
* Automate the Calculations of the types of expenditure as a percentage of the income from updated receipt and statement data   
* Manage the different types of expenditure as new types or groupings appear in the statements via a  configuration file 

There is also the side benefit of practising modern development techniques and technologies to produce the application and to this end, I will be using .NET to implement the  web service and Angular to implement the client application.

This would then change the process to the following:

* Login into your online banking service  
* Export your monthly bank statement  
* Import the income values from the statement into the application  
* The application would then report the following based upon the imported statement data:  
  * Income values  
  * Expenditure Values  
  * Balance  
  * Pie Chart which summarises statement data as total income versus total expenditure  
* We would also want the following information further analysis of income expenditure:  
  * Income values grouped by income type  
  * Expenditure values grouped by types of spending  
  * From the income types:  
    * List of the income values for the given income type (date, name, amount)  
    * Total of the amounts from the income values for the given income type  
    * Pie chart to show the Total of the amounts from the income values for the given income type vs. Total expenditure  
    * From the income types:  
      * List of the income values for the given income type (date, name, amount)  
      * Total of the amounts from the income values for the given income type  
      * Pie chart to show the Total of the amounts from the income values for the given income type vs. Total expenditure  
    * From the types of spending:  
      * List of the expenditure values for the given types of spending (date, name, amount)  
      * Total of the amounts from the expenditure values for the given type of spending  
      * Pie chart to show the Total of the amounts from the income values for the given type of spending vs. Total income  
  * Pie chart to summarise the types of income in terms of the totals for the types of income  
  * Pie chart to summarise the types of spending in terms of the totals for the types of spending  
* Facility to manage the groupings for the income types and types of spending

### Generation of test data

With regards to the statement and receipt data we would also want the facility to generate randomised data sets for testing purposes.

### Proposed Technologies 

The  application will initially be built as a .net core web api service with sql lite for the database for the back end and Next.js / View for the client, with a view to porting it to an android app for easier access to the application for myself and my wife.

### 

### Event Workflows

This would result in the following Workflows With Regard to the use of a MVC architecture :

* Import statement or cash receipt data into the application   
* Produce summary transaction page (total income, total expenditure, balance, income versus expenditure summary)  
* Produce details  page (total income, total expenditure, balance, income versus expenditure summary) based upon type of spending group   
* Import ,update and  export configuration of types of income and expenditure 


## Application Views 

The application will consist of four views:

* Import view  
* Details view   
* Configuration view 

### 

### 

### Details view 

This gives a summary of the budget, based upon the imported transactions from the statements and cash receipts for a given type of transactions:

* Browse Control to browse for statement or cash receipt files  
* Import button that triggers the Import request for transactions and receipts  
* Date range displaying the range of transaction data processed  
* Total income from the transaction data processed   
* Total expenditure from the transaction data processed  
* Balance from the transaction data processed   
* Pie chart to summarise the income vs expenditure from the transaction data processed 

### Configuration view 

This gives a tool to configure the different types  of transactions used in details views:

* Tree view showing the grouping of the types of transactions   
* List of the different types of transactions available   
* Add button to add a new grouping for certain types of transactions   
* Add button to add a type of transaction to a grouping   
* Remove button to remove a type of transaction from a grouping 

# 

# 

# **Milestones**

1. ## Implementing the service layer

   This will be initially implemented as set of classes to represent the model / state in the service layer:

*  the import data types (cash receipt transactions and bank statement transactions) \-  *completed*   
* transaction data types to be used in calculation of balance, income and expenditure totals \-  *completed*   
* The configuration data types to represent the types of income and expenditure *\- completed*  
* The Transaction Credit Details types to be used in representing the income and expenditure content in the summary and details views:  
  * Collection of transaction rows \- *completed*  
  * Total amount for the aforementioned rows \- *completed*  
* The Transaction Summary types consisting of:  
  * Transaction Credit Details Type to Represent the income \- *completed*  
  * Transaction Credit Details Type to Represent the expenditure \- *completed*  
  * Balance value representing Income \- Expenditure \- *completed*  
  * Expenditure Summary as a Percentage of the Income \- *completed*  
    * They will be used for all income and expenditure with regards to the Summary View  
    * They will also be used for subsets of income and expenditure via Transaction Group Types in the Details View 

  In addition, the generators that generate:

* The test cash receipt data sets \- *completed*  
* The test statement data sets \- *completed*  
* The transaction data sets by transforming the received cash receipt and statement data sets \- *completed*  
* Unit tests that verify in the implementation of the aforementioned generators \- *completed*

And the processors that perform the following:

* Provide utility functions to aid in converting the cash receipt data to transaction data \- *completed*    
* Provide utility functions to aid in converting the statement data to transaction data \- *completed*   
* Add new sets of income and types of spending to the transaction group types configuration \- *completed*   
* Update sets of income and types of spending to the transaction group types configuration \- *completed*   
* Provide utility functions to aid in Filtering the transaction data sets on the sets of income and types of spending found in the transaction group types configuration \- *completed*  
* Provide utility functions to aiding the generation of the summary and details response data by leveraging the transaction filtering functionality \- *to be completed*  
* Implement the reading of the configuration from the app configuration set on service startup \- *to be completed*   
* Implement the export of the configuration to the app configuration set (and src json file) \- *to be completed*   
* Unit tests to verify the implementation of this configuration startup and export *\- to be completed*   
* Unit tests to verify the implementation of these processors \- *Unit tests for the for the generation of the summary and detail response to be completed*

2. ## Implementing the client layer

This would essentially be:

* Controllers which are mappings of the requests to the service layer  and the responses from the service layer  to forwarded to the relevant views, with the views being updated via the relevant data bindings to properties in the model from the service layer.  
* Implementations of the Views:  
  * Import View  
  * Details View  
  * Configuration View


