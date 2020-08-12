# SQL-Wrapper - PHP Port

Currently supported DB-Types:
- MySQL (requires PDO, usually installed by default)

## Method overview

- [Constructors](https://github.com/TobiHatti/SQL-Wrapper-Classes/tree/master/Finished/PHP#constructors)
- [`Open()` and `Close()`](https://github.com/TobiHatti/SQL-Wrapper-Classes/tree/master/Finished/PHP#open-and-close)
- [Transactions](https://github.com/TobiHatti/SQL-Wrapper-Classes/tree/master/Finished/PHP#transactions)
- [Passing SQL-Statements and Parameters](https://github.com/TobiHatti/SQL-Wrapper-Classes/tree/master/Finished/PHP#passing-sql-statements-and-parameters)
- [`ExecuteNonQuery()`](https://github.com/TobiHatti/SQL-Wrapper-Classes/tree/master/Finished/PHP#executenonquery)
- [`ExecuteQuery()`](https://github.com/TobiHatti/SQL-Wrapper-Classes/tree/master/Finished/PHP#executequery)
- [`ExecuteScalar()`](https://github.com/TobiHatti/SQL-Wrapper-Classes/tree/master/Finished/PHP#executescalar)


# Usage

## Constructors

__MySQL__
```php
$sql = new WrapMySQL("localhost","northwind","username","password");
```

## Open() and Close()

The following Methods require you to open the connection before performing any action:
- ExecuteNonQuery
- ExecuteScalar
- ExecuteQuery

_NOTE: This class uses PDO, which does not have a way to close the connection right away. (Unlike the C# or VB.Net Ports of WrapSQL)_

## Transactions

```php
$sql->Open();
$sql->TransactionBegin();
try
{
  $sql->ExecuteNonQuery("UPDATE ...");
  $sql->ExecuteNonQuery("DELETE ...");

  $sql->TransactionCommit();
}
catch
{
  $sql->TransactionRollback();
}
$sql->Close();
```
__NOTE: Methods with the suffix `ACon` are not allowed durring a transaction and will throw an exception!__

## Passing SQL-Statements and Parameters

_It is recommended to pass sql-queries using parameters, protecting them against SQL-Injection attacks._

The following applies for every method which requires a SQL-query:
```php
// Passing a sql-statement without parameters
$memberIDNr = "AAAA-AAAA-AAAA-AAAA";
$sql->ExecuteScalar("SELECT paymentDate FROM members WHERE memberID = '".$memberIDNr."'");

// Passing a sql-statement with parameters (recommended)
$memberIDNr = "AAAA-AAAA-AAAA-AAAA";
$sql->ExecuteScalar("SELECT paymentDate FROM members WHERE memberID = ?", $memberIDNr);
```

## ExecuteNonQuery()

The ExecuteNonQuery-Method is used to execute a non-query like statement, like `UPDATE`, `DELETE`, `INSERT INTO`, ...

```php
$sql->Open();
$sql->ExecuteNonQuery("INSERT INTO ....");
$sql->Close();
```

## ExecuteQuery()

The ExecuteQuery-Method provides a Method for cycling through all results the query retrieves.

```php
$sql->Open();
foreach($sql->ExecuteQuery("SELECT * FROM orders") as $row)
{
  echo $row['orderID'].' '.$row['orderName']."\n";
}
$sql->Close();
```

## ExecuteScalar()

The ExecuteScalar-Method is used to return a single "cell" or a single result from a query.

```php
$sql->Open();
$amount = $sql->ExecuteScalar("SELECT COUNT(*) FROM employees ...");
$sql->Close();
```


