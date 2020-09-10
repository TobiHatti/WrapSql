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
```vb.net
'' Initialising using a connection-string
Dim sql As WrapMySQL = New WrapMySQL("CustomConnectionString")
```
```vb.net
'' Initialising using pre-defined connection-string
Dim sql As WrapMySQL = New WrapMySQL("localhost", "northwind", "username", "password")
```
```vb.net
'' Initialising using WrapMySQLData
Dim dbData As WrapMySQLData = New WrapMySQLData("localhost", "northwind", "username", "password") With 
{
    .Pooling = True,
    .SSLMode = "none",
    .Port = 1253
}

WrapMySQL sql = new WrapMySQL(dbData);
```
__SQLite__
```vb.net
'' Initialising using the path to your sqlite-file
Dim sql As WrapSQLite = New WrapSQLite("Path\To\Your\File.db")
```
```vb.net
'' Initialising using the path to your sqlite-file
Dim sql As WrapSQLite = New WrapSQLite("CustomConnectionString", False)
```
__ODBC__
```vb.net
'' Initialising using a custom connection-string
Dim sql As WrapODBC = New WrapODBC("CustomConnectionString")
```

__OleDb__
```vb.net
'' Initialising using a custom connection-string
Dim sql As WrapOleDb = New WrapOleDb("CustomConnectionString")
```

## Open() and Close()

The connection should be kept open as short as possible.
```vb.net
sql.Open()
sql.ExecuteNonQuery("UPDATE ....")
sql.Close()
```

Methods with the suffix `ACon` open and close the connection automatically, this can however cause problems when running them several times after each other (e.g. in a loop). 

```vb.net
sql.ExecuteScalarACon("SELECT ID FROM customers WHERE ...")
```

## Transactions

```vb.net
sql.Open()
sql.TransactionBegin()

Try
    sql.ExecuteNonQuery("UPDATE ...")
    sql.ExecuteNonQuery("DELETE ...")
    
    sql.TransactionCommit()
Catch
    sql.TransactionRollback()
End Try

sql.Close()
```
__NOTE: Methods with the suffix `ACon` are not allowed durring a transaction and will throw an exception!__

## Passing SQL-Statements and Parameters

_It is recommended to pass sql-queries using parameters, protecting them against SQL-Injection attacks._

The following applies for every method which requires a SQL-query:
```vb.net
'' Passing a sql-statement without parameters (NOT RECOMMENDED!)
Dim memberIDNr As String = "ABCD-EFGH-IJKL-MNOP"
sql.ExecuteScalar($"SELECT paymentDate FROM members WHERE memberID = '{memberIDNr}'")

'' Passing a sql-statement with parameters (recommended)
Dim memberIDNr As String = "ABCD-EFGH-IJKL-MNOP"
sql.ExecuteScalar("SELECT paymentDate FROM members WHERE memberID = ?", memberIDNr)
```

## ExecuteNonQuery() and ExecuteNonQueryACon()

ExecuteNonQuery-Methods are used to execute a non-query like statement, like `UPDATE`, `DELETE`, `INSERT INTO`, `ALTER TABLE`, ...

```vb.net
'' Opening and closing the connection manually
sql.Open()
sql.ExecuteNonQuery("INSERT INTO ....")
sql.Close()
```

```vb.net
'' Opening and closing the connection automatically
sql.ExecuteNonQueryACon("INSERT INTO ...")
```

## ExecuteQuery()

The ExecuteQuery-Method provides a SQLReader for cycling through all results the query retrieves.

Make sure to use the correct SQLReader:
- MySQL: MySQLDataReader
- SQLite: SQLiteDataReader
- ODBC: ODBCDataReader
- OleDb: OleDbDataReader

```vb.net
sql.Open()

Using reader As MySQLDataReader = CType(sql.ExecuteQuery("SELECT * FROM orders"), MySQLDataReader)

    While reader.Read()
        Console.WriteLine(reader("orderID") & " " + reader("orderName"))
    End While
End Using

sql.Close()
```

## ExecuteScalar() and ExecuteScalarACon()

ExecuteScalar-Methods are used to return a single "cell" or a single result from a query.
The ExecuteScalar-Method has Normal and ACon variants, as well as auto-casting methods.

```vb.net
'' Manual casting
sql.Open()
Dim amount As Integer = CInt(sql.ExecuteScalar("SELECT COUNT(*) FROM employees ..."))
sql.Close()
```

```vb.net
'' Manual casting (ACon)
Dim amount As Integer = CInt(sql.ExecuteScalarACon("SELECT COUNT(*) FROM employees ..."))
```

```vb.net
'' Auto casting
sql.Open()
Dim sum = sql.ExecuteScalar(Of Double)("SELECT SUM(price) FROM products ...")
sql.Close()
```

```vb.net
'' Auto casting (ACon)
Dim sum = sql.ExecuteScalarACon(Of Double)("SELECT SUM(price) FROM products ...")
```

## CreateDataTable()

The CreateDataTable-Method is usefull for populating form-controlls with DB-Entries:
```vb.net
'' e.g. WinForms listbox:
listboxProducts.DisplayMember = "NameAndPrice"
listboxProducts.ValueMember = "productID"
listboxProducts.DataSource = sql.CreateDataTable("SELECT CONCAT_WS(name, price) AS NameAndPrice, productID FROM products")
```
No Open()/Close() is required for this method to work.

## GetDataAdapter()

The GetDataAdapter-Method returns a DataAdapter-Object for further use.
```vb.net
Dim da As MySQLDataAdapter = sql.GetDataAdapter("SELECT * FROM ...")
```
No Open()/Close() is required.

# Application examples

### Fetching some values from a database
```vb.net
Using sql As WrapSQLite = New WrapSQLite("Path/To/DB/File.db")
    sql.Open()
    Dim value1 = sql.ExecuteScalar(Of String)("SELECT Firstname FROM customers WHERE CustomerID = ?", customerID)
    Dim value2 = sql.ExecuteScalar(Of Integer)("SELECT COUNT(*) FROM members")
    Dim value3 As Single = sql.ExecuteScalar(Of Single)("SELECT MAX(Price) FROM Items")
    sql.Close()
End Using
```

### Inserting values into a database with a transaction
```vb.net
Using sql As WrapMySQL = New WrapMySQL(dbData)
    sql.Open()
    sql.TransactionBegin()

    Try
        sql.ExecuteNonQuery("UPDATE players SET balance = balance + ? WHERE playerID = ?", 300, playerID)
        sql.ExecuteNonQuery("UPDATE businesses SET balance = balance - ? WHERE businessID = ?", 300, businessID)
        sql.TransactionCommit()
    Catch
        sql.TransactionRollback()
    End Try

    sql.Close()
End Using
```

### Using different database-types at the same time
```vb.net

Private Shared Sub Main(ByVal args As String())
    Dim mysql As WrapMySQL = New WrapMySQL("ConnectionString")
    Dim sqlite As WrapSQLite = New WrapSQLite("ConnectionString", False)

    If saveDataOnline Then
        SaveData(mysql)
    Else
        SaveData(sqlite)
    End If
End Sub

Private Shared Sub SaveData(ByVal wrapSQLObject As WrapSQL)

    '' Since all WrapSQL sub-types are build on the same foundation (WrapSQLBase), 
    '' it is possible to "switch" between db-types, e.g. MySQL and SQLite, 
    '' without the need to call seperate methods for each db type
    
    wrapSQLObject.Open()
    wrapSQLObject.ExecuteNonQuery("UPDATE stats SET ....")
    wrapSQLObject.Close()
End Sub
```
