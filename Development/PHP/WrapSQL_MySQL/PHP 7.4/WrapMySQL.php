<?php

class WrapMySQL
{
    private PDO $pdo;

    private string $dbHostname = "";
    private string $dbDatabase = "";
    private string $dbUsername = "";
    private string $dbPassword = "";

    private bool $isPersistent = true;

    private bool $isOpen = false;

    // Creates a new SQL-Wrapper object.
    public function __construct(string $hostname, string $database, string $username, string $password, bool $persistentConnection = true)
    {
        $this->dbHostname = $hostname;
        $this->dbDatabase = $database;
        $this->dbUsername = $username;
        $this->dbPassword = $password;

        $this->isPersistent = $persistentConnection;
    }

    // Disposes the object.
    public function __destruct()
    {
        if($this->isOpen)
        {
            $this->isOpen = false;
            $this->pdo = NULL;
        }
    }

    // PDO-Connection object.
    public function GetPDO() : PDO
    {
        if(!$this->isOpen)
        {
            return $this->pdo;
        }
        else throw new Exception("PDO-Connection is not established. Call WrapSQL->Open() before performing any actions.");
    }

    // Opens the SQL-connection, if the connection is closed
    public function Open()
    {
        if(!$this->isOpen)
        {
            $this->isOpen = true;
            $this->pdo = new PDO('mysql:host='.$this->dbHostname.';dbname='.$this->dbDatabase, $this->dbUsername, $this->dbPassword, array( PDO::ATTR_PERSISTENT => $this->isPersistent));
        }
    }

    // Closes the SQL-Connection, if the connection is open.
    public function Close()
    {
        if($this->isOpen)
        {
            $this->isOpen = false;
            $this->pdo = NULL;
        }
    }

    // Starts a transaction.
    public function TransactionBegin()
    {
        $this->GetPDO()->beginTransaction();
    }

    // Commits a transaction.
    public function TransactionCommit()
    {
        $this->GetPDO()->commit();
    }

    // Terminates a transaction.
    public function TransactionRollback()
    {
        $this->GetPDO()->rollBack();
    }

    // Executes a non-query statement. 
    public function ExecuteNonQuery(string $sqlStatement, ...$parameters)
    {
        $stmt = $this->GetPDO()->prepare($sqlStatement);
        $stmt->execute($parameters);
    }

    // Executes a query-statement.
    public function ExecuteQuery(string $sqlStatement, ...$parameters)
    {
        $stmt = $this->GetPDO()->prepare($sqlStatement);
        $stmt->execute($parameters);
        return $stmt->fetchAll();
    }

    // Executes a execute-scalar statement. 
    public function ExecuteScalar(string $sqlStatement, ...$parameters)
    {
        $stmt = $this->GetPDO()->prepare($sqlStatement);
        $stmt->execute($parameters);
        return $stmt->fetchColumn();
    }

}