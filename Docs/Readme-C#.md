# WrapSQL - C# Port

## Method overview

- [Constructors](https://github.com/TobiHatti/WrapSQL/tree/master/.NET/Readme-C%23.md#constructors)
- [`Open()` and `Close()`](https://github.com/TobiHatti/WrapSQL/tree/master/.NET/Readme-C%23.md#open-and-close)
- [Transactions](https://github.com/TobiHatti/WrapSQL/tree/master/.NET/Readme-C%23.md#transactions)
- [Passing SQL-Statements and Parameters](https://github.com/TobiHatti/WrapSQL/tree/master/.NET/Readme-C%23.md#passing-sql-statements-and-parameters)
- [`ExecuteNonQuery()` and `ExecuteNonQueryACon()`](https://github.com/TobiHatti/WrapSQL/tree/master/.NET/Readme-C%23.md#executenonquery-and-executenonqueryacon)
- [`ExecuteQuery()`](https://github.com/TobiHatti/WrapSQL/tree/master/.NET/Readme-C%23.md#executequery)
- [`ExecuteScalar()` and `ExecuteScalarACon()`](https://github.com/TobiHatti/WrapSQL/tree/master/.NET/Readme-C%23.md#executescalar-and-executescalaracon)
- [`FillDataTable()`](https://github.com/TobiHatti/WrapSQL/tree/master/.NET/Readme-C%23.md#filldatatable)
- [`GetDataAdapter()`](https://github.com/TobiHatti/WrapSQL/tree/master/.NET/Readme-C%23.md#getdataadapter)

# Usage

## Constructors
Note: The constructors are the only thing between different DB-Types that are individual for every DB-Type.

__MySQL__
```cs
// Initialising using a connection-string
WrapMySQL sql = new WrapMySQL("CustomConnectionString");
```
```cs
// Initialising using pre-defined connection-string
WrapMySQL sql = new WrapMySQL("localhost","northwind","username","password");
```
```cs
// Initialising using WrapMySQLData
WrapMySQLData dbData = new WrapMySQLData("localhost","northwind","username","password")
{
    Pooling = true,
    SSLMode = "none",
    Port = 1253
};

WrapMySQL sql = new WrapMySQL(dbData);
```
__SQLite__
```cs
// Initialising using the path to your sqlite-file
WrapSQLite sql = new WrapSQLite(@"Path\To\Your\File.db");
```
```cs
// Initialising using the path to your sqlite-file
WrapSQLite sql = new WrapSQLite("CustomConnectionString", false);
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
// Passing a sql-statement without parameters (NOT RECOMMENDED!)
string memberIDNr = "ABCD-EFGH-IJKL-MNOP";
sql.ExecuteScalar($"SELECT paymentDate FROM members WHERE memberID = '{memberIDNr}'");

// Passing a sql-statement with parameters (recommended)
string memberIDNr = "ABCD-EFGH-IJKL-MNOP";
sql.ExecuteScalar("SELECT paymentDate FROM members WHERE memberID = ?", memberIDNr);
```

## ExecuteNonQuery() and ExecuteNonQueryACon()

ExecuteNonQuery-Methods are used to execute a non-query like statement, like `UPDATE`, `DELETE`, `INSERT INTO`, `ALTER TABLE`, ...

```cs
// Opening and closing the connection manually
sql.Open();
sql.ExecuteNonQuery("INSERT INTO ....");
sql.Close();
```

```cs
// Opening and closing the connection automatically
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
using(MySqlDataReader reader = (MySqlDataReader)sql.ExecuteQuery("SELECT * FROM orders"))
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
int amount = (int)sql.ExecuteScalar("SELECT COUNT(*) FROM employees ...");
sql.Close();
```

```cs
// Manual casting (ACon)
int amount = (int)sql.ExecuteScalarACon("SELECT COUNT(*) FROM employees ...");
```

```cs
// Auto casting
sql.Open();
var sum = sql.ExecuteScalar<double>("SELECT SUM(price) FROM products ...");
sql.Close();
```

```cs
// Auto casting (ACon)
var sum = sql.ExecuteScalarACon<double>("SELECT SUM(price) FROM products ...");
```

## CreateDataTable()

The CreateDataTable-Method is usefull for populating form-controlls with DB-Entries:
```cs
// e.g. WinForms listbox:
listboxProducts.DisplayMember = "NameAndPrice";
listboxProducts.ValueMember = "productID";
listboxProducts.DataSource = sql.CreateDataTable("SELECT CONCAT_WS(name, price) AS NameAndPrice, productID FROM products");
```
No Open()/Close() is required for this method to work.

## GetDataAdapter()

The GetDataAdapter-Method returns a DataAdapter-Object for further use.
```cs
MySQLDataAdapter da = sql.GetDataAdapter("SELECT * FROM ...");
```
No Open()/Close() is required.

# Application examples

### Fetching some values from a database
```cs
using(WrapSQLite sql = new WrapSQLite(@"Path/To/DB/File.db"))
{
    sql.Open();
    
    var value1 = sql.ExecuteScalar<string>("SELECT Firstname FROM customers WHERE CustomerID = ?", customerID);
    var value2 = sql.ExecuteScalar<int>("SELECT COUNT(*) FROM members");
    
    float value3 = sql.ExecuteScalar<float>("SELECT MAX(Price) FROM Items");
    
    sql.Close();
}
```

### Inserting values into a database with a transaction
```cs
using(WrapMySQL sql = new WrapMySQL(dbData))
{
    sql.Open();
    
    sql.TransactionBegin();
    try
    {
        sql.ExecuteNonQuery("UPDATE players SET balance = balance + ? WHERE playerID = ?", 300, playerID);
        sql.ExecuteNonQuery("UPDATE businesses SET balance = balance - ? WHERE businessID = ?", 300, businessID);

        sql.TransactionCommit();
    }
    catch
    {
        sql.TransactionRollback();
    }
    
    sql.Close();
}
```

### Using different database-types at the same time
```cs

static void Main(string[] args)
{
    WrapMySQL mysql = new WrapMySQL("ConnectionString");
    WrapSQLite sqlite = new WrapSQLite("ConnectionString", false);
    
    if(saveDataOnline) SaveData(mysql);
    else SaveData(sqlite);
}

static void SaveData(WrapSQL wrapSQLObject)
{
    // Since all WrapSQL sub-types are build on the same foundation (WrapSQLBase), 
    // it is possible to "switch" between db-types, e.g. MySQL and SQLite, 
    // without the need to call seperate methods for each db type
    
    wrapSQLObject.Open();
    wrapSQLObject.ExecuteNonQuery("UPDATE stats SET ....");
    wrapSQLObject.Close();
}




```

