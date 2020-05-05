# SQL-Wrapper-Classes

![GitHub](https://img.shields.io/github/license/TobiHatti/SQL-Wrapper-Classes)
[![GitHub Release Date](https://img.shields.io/github/release-date/TobiHatti/SQL-Wrapper-Classes)](https://github.com/TobiHatti/SQL-Wrapper-Classes/releases)
[![GitHub release (latest by date including pre-releases)](https://img.shields.io/github/v/release/TobiHatti/SQL-Wrapper-Classes?include_prereleases)](https://github.com/TobiHatti/SQL-Wrapper-Classes/releases)
[![GitHub last commit](https://img.shields.io/github/last-commit/TobiHatti/SQL-Wrapper-Classes)](https://github.com/TobiHatti/SQL-Wrapper-Classes/commits/master)
[![GitHub issues](https://img.shields.io/github/issues-raw/TobiHatti/SQL-Wrapper-Classes)](https://github.com/TobiHatti/SQL-Wrapper-Classes/issues)
[![GitHub language count](https://img.shields.io/github/languages/count/TobiHatti/SQL-Wrapper-Classes)](https://github.com/TobiHatti/SQL-Wrapper-Classes)

Multiple wrapper-classes for several languages and DB-Types

### Supported Languages and DB-Types
- C#
  - MySQL
  - SQLite
  - ODBC
  - OleDb
- VB.NET
  - MySQL
  - SQLite
  - ODBC
  - OleDb

## Finding the right file or assembly
Navigate to "Finished" and select your desired Language, then select either the source-file or the assembly and add it to your own project.

A full documentation is included in every language-sub-folder:
- [README - C#](https://github.com/TobiHatti/SQL-Wrapper-Classes/tree/master/Finished/C%23)
- [README - VB.NET](https://github.com/TobiHatti/SQL-Wrapper-Classes/blob/master/Finished/VB.NET)

__NOTE: Some Files/Assemblies/Libraries require additional packages, such as NuGet packages, to work without any errors!__

## Quick overview
These Methods/Function exists for every language. Check corresponding readme for additional features.

```cs
// Executes a non-query statement, e.g. UPDATE, INSERT, DELETE, ...
WrapSQL.ExecuteNonQuery("UPDATE ...", parameter1, parameter2, ...);

// Returns a single value from a select-statement
WrapSQL.ExecuteScalar("SELECT COUNT(*) FROM ...", parameter1, parameter2, ...);

// Returns all effected rows retrieved by the select-statement
WrapSQL.ExecuteQuery("SELECT * FROM ...", parameter1, parameter2, ...);
```
