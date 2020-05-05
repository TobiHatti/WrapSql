# SQL-Wrapper - C# Port

Currently supported DB-Types:
- MySQL
- SQLite
- ODBC
- OleDb

## Method overview

- [Constructors](https://github.com/TobiHatti/SQL-Wrapper-Classes/tree/master/Finished/C%23#constructors)
- [`Open()` and `Close()`](https://github.com/TobiHatti/SQL-Wrapper-Classes/tree/master/Finished/C%23#cpen-and-close)
- [Transactions](https://github.com/TobiHatti/SQL-Wrapper-Classes/tree/master/Finished/C%23#transactions)
- [Passing SQL-Statements and Parameters](https://github.com/TobiHatti/SQL-Wrapper-Classes/tree/master/Finished/C%23#passing-sql-statements-and-parameters)
- [`ExecuteNonQuery()` and `ExecuteNonQueryACon()`](https://github.com/TobiHatti/SQL-Wrapper-Classes/tree/master/Finished/C%23#executenonquery-and-executenonqueryacon)
- [`ExecuteQuery()`](https://github.com/TobiHatti/SQL-Wrapper-Classes/tree/master/Finished/C%23#executequery)
- [`ExecuteScalar()` and `ExecuteScalarACon()`](https://github.com/TobiHatti/SQL-Wrapper-Classes/tree/master/Finished/C%23#executescalar-and-executescalaracon)
- [`FillDataTable()`](https://github.com/TobiHatti/SQL-Wrapper-Classes/tree/master/Finished/C%23#filldatatable)
- [`GetDataAdapter()`](https://github.com/TobiHatti/SQL-Wrapper-Classes/tree/master/Finished/C%23#getdataadapter)

# Usage

## Constructors

__MySQL__
```cs
// Initialising using a custom connection-string
WrapMySQL sql = new WrapMySQL("CustomConnectionString");

// Initialising using pre-defined connection-string
WrapMySQL sql = new WrapMySQL("localhost","northwind","username","password");
```
__SQLite__
```cs
// Initialising using a custom connection-string
WrapSQLite sql = new WrapSQLite("CustomConnectionString");

// Initialising using the path to your sqlite-file
WrapSQLite sql = new WrapSQLite(@"Path\To\Your\File.db", true);
```
__ODBC__
```cs
// Initialising using a custom connection-string
WrapODBC sql = new WrapODBC("CustomConnectionString");
```

__OleDb__
```cs
// Initialising using a custom connection-string
WrapOleDb sql = new WrapOleDb("CustomConnectionString");
```

## Open() and Close()

The following Methods require you to open the connection before performing any action:
- ExecuteNonQuery
- ExecuteScalar
- ExecuteQuery

The connection should be kept open as short as possible.
```cs
sql.Open();
sql.ExecuteNonQuery("UPDATE ....");
sql.Close();
```

Methods with the suffix `ACon` open and close the connection automatically, this can however cause problems when running them several times after each other (e.g. in a loop). 

```cs
sql.ExecuteScalarACon("SELECT ID FROM customers WHERE ...");
```

## Transactions

```cs
sql.Open();
sql.TransactionBegin();
try
{
  sql.ExecuteNonQuery("UPDATE ...");
  sql.ExecuteNonQuery("DELETE ...");

  sql.TransactionCommit();
}
catch
{
  sql.TransactionRollback();
}
sql.Close();
```
__NOTE: Methods with the suffix `ACon` are not allowed durring a transaction and will throw an exception!__

## Passing SQL-Statements and Parameters

_It is recommended to pass sql-queries using parameters, protecting them against SQL-Injection attacks._

The following applies for every method which requires a SQL-query:
```cs
// Passing a sql-statement without parameters
string memberIDNr = "AAAA-AAAA-AAAA-AAAA";
sql.ExecuteScalar($"SELECT paymentDate FROM members WHERE memberID = '{memberIDNr}'");

// Passing a sql-statement with parameters (recommended)
string memberIDNr = "AAAA-AAAA-AAAA-AAAA";
sql.ExecuteScalar($"SELECT paymentDate FROM members WHERE memberID = ?", memberIDNr);
```

## ExecuteNonQuery() and ExecuteNonQueryACon()

ExecuteNonQuery-Methods are used to execute a non-query like statement, like `UPDATE`, `DELETE`, `INSERT INTO`, ...

```cs
sql.Open();
sql.ExecuteNonQuery("INSERT INTO ....");
sql.Close();
```

```cs
sql.ExecuteNonQueryACon("INSERT INTO ...");
```

## ExecuteQuery()

The ExecuteQuery-Method provides a SQLReader for cycling through all results the query retrieves.

Make sure to use the correct SQLReader:
- MySQL: MySQLDataReader
- SQLite: SQLiteDataReader
- ODBC: ODBCDataReader
- OleDb: OleDbDataReader

```cs
sql.Open();
using(MySQLDataReader reader = sql.ExecuteQuery("SELECT * FROM orders"))
{
  while(reader.Read())
  {
    Console.WriteLine(reader["orderID"] + " " + reader["orderName"]);
  }
}
sql.Close();
```

## ExecuteScalar() and ExecuteScalarACon()

ExecuteScalar-Methods are used to return a single "cell" or a single result from a query.
The ExecuteScalar-Method has Normal and ACon variants, as well as auto-casting methods.

```cs
// Manual casting
sql.Open();
int amount = (int)sql.ExecuteNonQuery("SELECT COUNT(*) FROM employees ...");
sql.Close();
```

```cs
// Manual casting (ACon)
int amount = (int)sql.ExecuteNonQueryACon("SELECT COUNT(*) FROM employees ...");
```

```cs
// Auto casting
sql.Open();
double sum = sql.ExecuteNonQuery<double>("SELECT SUM(price) FROM products ...");
sql.Close();
```

```cs
// Auto casting (ACon)
double sum = sql.ExecuteNonQueryACon<double>("SELECT SUM(price) FROM products ...");
```

## FillDataTable()

The FillDataTable-Method is usefull for populating form-controlls with DB-Entries:
```cs
listboxProducts.DisplayMember = "NameAndPrice";
listboxProducts.ValueMember = "productID";
listboxProducts.DataSource = FillDataTable("SELECT CONCAT_WS(name, price) AS NameAndPrice, productID FROM products");
```
No Open()/Close() is required.

## GetDataAdapter()

The GetDataAdapter-Method returns a DataAdapter-Object for further use.
```cs
MySQLDataAdapter da = sql.GetDataAdapter("SELECT * FROM ...");
```
No Open()/Close() is required.
