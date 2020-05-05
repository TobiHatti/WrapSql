# SQL-Wrapper - VB.NET Port

Currently supported DB-Types:
- MySQL
- SQLite
- ODBC
- OleDb

## Method overview

- [Constructors](https://github.com/TobiHatti/SQL-Wrapper-Classes/tree/master/Finished/VB.NET#constructors)
- [`Open()` and `Close()`](https://github.com/TobiHatti/SQL-Wrapper-Classes/tree/master/Finished/VB.NET#open-and-close)
- [Transactions](https://github.com/TobiHatti/SQL-Wrapper-Classes/tree/master/Finished/VB.NET#transactions)
- [Passing SQL-Statements and Parameters](https://github.com/TobiHatti/SQL-Wrapper-Classes/tree/master/Finished/VB.NET#passing-sql-statements-and-parameters)
- [`ExecuteNonQuery()` and `ExecuteNonQueryACon()`](https://github.com/TobiHatti/SQL-Wrapper-Classes/tree/master/Finished/VB.NET#executenonquery-and-executenonqueryacon)
- [`ExecuteQuery()`](https://github.com/TobiHatti/SQL-Wrapper-Classes/tree/master/Finished/VB.NET#executequery)
- [`ExecuteScalar()` and `ExecuteScalarACon()`](https://github.com/TobiHatti/SQL-Wrapper-Classes/tree/master/Finished/VB.NET#executescalar-and-executescalaracon)
- [`FillDataTable()`](https://github.com/TobiHatti/SQL-Wrapper-Classes/tree/master/Finished/VB.NET#filldatatable)
- [`GetDataAdapter()`](https://github.com/TobiHatti/SQL-Wrapper-Classes/tree/master/Finished/VB.NET#getdataadapter)

# Usage

## Constructors

__MySQL__
```vbnet
' Initialising using a custom connection-string
Dim sql As WrapMySQL = New WrapMySQL("CustomConnectionString")

' Initialising using pre-defined connection-string
Dim sql As WrapMySQL = New WrapMySQL("localhost", "northwind", "username", "password")
```
__SQLite__
```vbnet
' Initialising using a custom connection-string
Dim sql As WrapSQLite = New WrapSQLite("CustomConnectionString")

' Initialising using the path to your sqlite-file
Dim sql As WrapSQLite = New WrapSQLite("Path\To\Your\File.db", True)
```
__ODBC__
```vbnet
' Initialising using a custom connection-string
Dim sql As WrapODBC = New WrapODBC("CustomConnectionString")
```

__OleDb__
```vbnet
' Initialising using a custom connection-string
Dim sql As WrapOleDb = New WrapOleDb("CustomConnectionString")
```

## Open() and Close()

The following Methods require you to open the connection before performing any action:
- ExecuteNonQuery
- ExecuteScalar
- ExecuteQuery

The connection should be kept open as short as possible.
```vbnet
sql.Open()
sql.ExecuteNonQuery("UPDATE ....")
sql.Close()
```

Methods with the suffix `ACon` open and close the connection automatically, this can however cause problems when running them several times after each other (e.g. in a loop). 

```vbnet
sql.ExecuteScalarACon("SELECT ID FROM customers WHERE ...")
```

## Transactions

```vbnet
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
```vbnet
' Passing a sql-statement without parameters
Dim memberIDNr As String = "AAAA-AAAA-AAAA-AAAA"
sql.ExecuteScalar($"SELECT paymentDate FROM members WHERE memberID = '{memberIDNr}'")

' Passing a sql-statement with parameters (recommended)
Dim memberIDNr As String = "AAAA-AAAA-AAAA-AAAA"
sql.ExecuteScalar($"SELECT paymentDate FROM members WHERE memberID = ?", memberIDNr)
```

## ExecuteNonQuery() and ExecuteNonQueryACon()

ExecuteNonQuery-Methods are used to execute a non-query like statement, like `UPDATE`, `DELETE`, `INSERT INTO`, ...

```vbnet
sql.Open()
sql.ExecuteNonQuery("INSERT INTO ....")
sql.Close()
```

```vbnet
sql.ExecuteNonQueryACon("INSERT INTO ...")
```

## ExecuteQuery()

The ExecuteQuery-Method provides a SQLReader for cycling through all results the query retrieves.

Make sure to use the correct SQLReader:
- MySQL: MySQLDataReader
- SQLite: SQLiteDataReader
- ODBC: ODBCDataReader
- OleDb: OleDbDataReader

```vbnet
sql.Open()

Using reader As MySQLDataReader = sql.ExecuteQuery("SELECT * FROM orders")
    While reader.Read()
        Console.WriteLine(reader("orderID") & " " + reader("orderName"))
    End While
End Using

sql.Close()
```

## ExecuteScalar() and ExecuteScalarACon()

ExecuteScalar-Methods are used to return a single "cell" or a single result from a query.
The ExecuteScalar-Method has Normal and ACon variants, as well as auto-casting methods.

```vbnet
' Manual casting
sql.Open()
Dim amount As Integer = CInt(sql.ExecuteNonQuery("SELECT COUNT(*) FROM employees ..."))
sql.Close()
```

```vbnet
' Manual casting (ACon)
Dim amount As Integer = CInt(sql.ExecuteNonQueryACon("SELECT COUNT(*) FROM employees ..."))
```

```vbnet
' Auto casting
sql.Open()
Dim sum As Double = sql.ExecuteNonQuery(Of Double)("SELECT SUM(price) FROM products ...")
sql.Close()
```

```vbnet
' Auto casting (ACon)
Dim sum As Double = sql.ExecuteNonQueryACon(Of Double)("SELECT SUM(price) FROM products ...")
```

## FillDataTable()

The FillDataTable-Method is usefull for populating form-controlls with DB-Entries:
```vbnet
listboxProducts.DisplayMember = "NameAndPrice"
listboxProducts.ValueMember = "productID"
listboxProducts.DataSource = FillDataTable("SELECT CONCAT_WS(name, price) AS NameAndPrice, productID FROM products")
```
No Open()/Close() is required.

## GetDataAdapter()

The GetDataAdapter-Method returns a DataAdapter-Object for further use.
```vbnet
Dim da As MySQLDataAdapter = sql.GetDataAdapter("SELECT * FROM ...")
```
No Open()/Close() is required.
