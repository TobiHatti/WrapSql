<img align="right" width="80" height="80" data-rmimg src="https://endev.at/content/projects/WrapSQL/EndevLibsLogo.svg">

# WrapSQL

![GitHub](https://img.shields.io/github/license/TobiHatti/WrapSQL)
[![GitHub Release Date](https://img.shields.io/github/release-date-pre/TobiHatti/WrapSQL)](https://github.com/TobiHatti/WrapSQL/releases)
[![GitHub release (latest by date including pre-releases)](https://img.shields.io/github/v/release/TobiHatti/WrapSQL?include_prereleases)](https://github.com/TobiHatti/WrapSQL/releases)
![Nuget](https://img.shields.io/nuget/v/endev.wrapsql.wrapsqlbase?label=release&logo=nuget)
[![GitHub last commit](https://img.shields.io/github/last-commit/TobiHatti/WrapSQL)](https://github.com/TobiHatti/WrapSQL/commits/master)
[![GitHub issues](https://img.shields.io/github/issues-raw/TobiHatti/WrapSQL)](https://github.com/TobiHatti/WrapSQL/issues)
[![GitHub language count](https://img.shields.io/github/languages/count/TobiHatti/WrapSQL)](https://github.com/TobiHatti/WrapSQL)

![image](https://endev.at/content/projects/WrapSQL/WrapSQL_Banner_300.svg)

![Nuget](https://img.shields.io/nuget/dt/endev.wrapsql.wrapsqlbase?label=downloads%20%28WrapSQLBase%29&logo=nuget)
![Nuget](https://img.shields.io/nuget/dt/endev.wrapsql.wrapmysql?label=downloads%20%28WrapMySQL%29&logo=nuget)
![Nuget](https://img.shields.io/nuget/dt/endev.wrapsql.wrapsqlite?label=downloads%20%28WrapSQLite%29&logo=nuget)
![Nuget](https://img.shields.io/nuget/dt/endev.wrapsql.wrapodbc?label=downloads%20%28WrapODBC%29&logo=nuget)
![Nuget](https://img.shields.io/nuget/dt/endev.wrapsql.wrapoledb?label=downloads%20%28WrapOleDB%29&logo=nuget)


Multiple wrapper-classes for several languages and DB-Types

### Supported Languages and DB-Types
- .Net (C#, F#, VB.Net)
  - MySQL
  - SQLite
  - ODBC
  - OleDb
- PHP
  - MySQL

## How to get the right version for your project

__.Net:__

Download the latest version of WrapSQL as a nuget-package from [nuget.org](https://www.nuget.org/profiles/TobiHatti) or [GitHub Packages](https://github.com/TobiHatti/WrapSQL/packages).

- __WrapMySQL:__
  - via command-line: `Install-Package Endev.WrapSQL.WrapMySQL`
  - Download on [nuget.org](https://www.nuget.org/packages/Endev.WrapSQL.WrapMySQL/)

- __WrapSQLite:__
  - via command-line: `Install-Package Endev.WrapSQL.WrapSQLite`
  - Download on [nuget.org](https://www.nuget.org/packages/Endev.WrapSQL.WrapSQLite/)

- __WrapODBC:__
  - via command-line: `Install-Package Endev.WrapSQL.WrapODBC`
  - Download on [nuget.org](https://www.nuget.org/packages/Endev.WrapSQL.WrapODBC/)

- __WrapOleDB:__
  - via command-line: `Install-Package Endev.WrapSQL.WrapOleDB`
  - Download on [nuget.org](https://www.nuget.org/packages/Endev.WrapSQL.WrapOleDB/)


__PHP:__

Get the latest version from the [releases page](https://github.com/TobiHatti/WrapSQL/releases/latest)


## Documentation

- [README - C#](https://github.com/TobiHatti/WrapSQL/tree/master/.NET/Readme-C%23.md)
- README - F# (TBA)
- [README - VB.NET](https://github.com/TobiHatti/WrapSQL/tree/master/.NET/Readme-VB.md)
- [README - PHP](https://github.com/TobiHatti/WrapSQL/tree/master/PHP/Readme.md)


## Quick overview
The following methods/function exists for every port of WrapSQL in every language. Check corresponding readme for additional features.

```cs
// Creating a WrapSQL-Object (e.g. WrapMySQL)
using(WrapMySQL sql = new WrapMySQL("--- MySQL Connection String ---")
{
    // Executes a non-query statement, e.g. UPDATE, INSERT, DELETE, ...
    sql.ExecuteNonQuery("UPDATE customers SET Firstname = ? WHERE ID = ?", firstName, customerID);

    // Returns a single value from a select-statement
    int completedOrderCount = sql.ExecuteScalar<int>("SELECT COUNT(*) FROM orders WHERE completed = ?", true);

    // Returns all effected rows retrieved by the select-statement
    using (MySQLDataReader reader = (MySQLDataReader)sql.ExecuteQuery("SELECT * FROM orders"))
    {
        while (reader.Read())
        {
            Console.WriteLine(reader["orderStatus"]);
        }
    }
}
```
