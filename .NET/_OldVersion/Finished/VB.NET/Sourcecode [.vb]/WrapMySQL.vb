Imports System.Data
Imports MySql.Data.MySqlClient

Public Class WrapMySQL
#Region "Fields and Properties"

    Private ReadOnly connectionString As String = String.Empty
    Private ReadOnly f_Connection As MySqlConnection = Nothing
    Private transaction As MySqlTransaction = Nothing
    Private transactionActive As Boolean = False

    ''' <summary>
    ''' SQL-Connection object.
    ''' </summary>
    Public ReadOnly Property Connection As MySqlConnection
        Get
            Return f_Connection
        End Get
    End Property

#End Region

#Region "Constructors, Destructors, Interfaces"

    ''' <summary>
    ''' Creates a new SQL-Wrapper object.
    ''' </summary>
    ''' <paramname="connectionString">Connection-string for the database</param>
    Public Sub New(ByVal connectionString As String)
        ' Set connection-string
        Me.connectionString = connectionString

        ' Create connection
        f_Connection = New MySqlConnection(Me.connectionString)
    End Sub

    ''' <summary>
    ''' Creates a new SQL-Wrapper object.
    ''' </summary>
    ''' <paramname="server">Hostname or IP of the server</param>
    ''' <paramname="database">Target database</param>
    ''' <paramname="username">Login username</param>
    ''' <paramname="password">Login password</param>
    ''' <paramname="port">Server-port. Default: 3306</param>
    ''' <paramname="sslMode">SSL encryption mode</param>
    Public Sub New(ByVal server As String, ByVal database As String, ByVal username As String, ByVal password As String, ByVal Optional port As Integer = 3306, ByVal Optional sslMode As String = "none")
        ' Assemble connection-string
        connectionString = $"SERVER={server};Port={port};SslMode={sslMode};DATABASE={database};USER ID={username};PASSWORD={password}"

        ' Create connection
        f_Connection = New MySqlConnection(connectionString)
    End Sub

    ''' <summary>
    ''' Disposes the object.
    ''' </summary>
    Public Sub Dispose()
        ' Dispose the connection object
        If f_Connection IsNot Nothing Then f_Connection.Dispose()
    End Sub

#End Region

#Region "Connection Open/Close"

    ''' <summary>
    ''' Opens the SQL-connection, if the connection is closed
    ''' </summary>
    Public Sub Open()
        If f_Connection.State = Data.ConnectionState.Closed Then f_Connection.Open()
    End Sub

    ''' <summary>
    ''' Closes the SQL-Connection, if the connection is open.
    ''' </summary>
    Public Sub Close()
        If f_Connection.State = Data.ConnectionState.Open Then f_Connection.Close()
    End Sub

#End Region

#Region "Transaction Begin/Commit/Rollback"

    ''' <summary>
    ''' Starts a transaction.
    ''' </summary>
    Public Sub TransactionBegin()
        transactionActive = True
        transaction = Connection.BeginTransaction()
    End Sub

    ''' <summary>
    ''' Commits a transaction.
    ''' </summary>
    Public Sub TransactionCommit()
        transaction.Commit()
        transactionActive = False
    End Sub

    ''' <summary>
    ''' Terminates a transaction.
    ''' </summary>
    Public Sub TransactionRollback()
        transaction.Rollback()
        transactionActive = False
    End Sub

#End Region

#Region "ExecuteNonQuery"

    ''' <summary>
    ''' Executes a non-query statement. 
    ''' </summary>
    ''' <paramname="sqlQuery">SQL-query</param>
    ''' <paramname="parameters">Query-parameters</param>
    ''' <returns>NonQuery result</returns>
    Public Function ExecuteNonQuery(ByVal sqlQuery As String, ParamArray parameters As Object()) As Integer
        Return ExecuteNonQuery(sqlQuery, False, parameters)
    End Function

    ''' <summary>
    ''' Executes a non-query statement. 
    ''' Automatically opens and closes the connection.
    ''' </summary>
    ''' <paramname="sqlQuery">SQL-query</param>
    ''' <paramname="parameters">Query-parameters</param>
    ''' <returns>NonQuery result</returns>
    Public Function ExecuteNonQueryACon(ByVal sqlQuery As String, ParamArray parameters As Object()) As Integer
        Return ExecuteNonQuery(sqlQuery, True, parameters)
    End Function

    ''' <summary>
    ''' Executes a non-query statement. 
    ''' </summary>
    ''' <paramname="sqlQuery">SQL-query</param>
    ''' <paramname="aCon">Manage connection states (AutoConnect)</param>
    ''' <paramname="parameters">Query-parameters</param>
    ''' <returns>NonQuery result</returns>
    Private Function ExecuteNonQuery(ByVal sqlQuery As String, ByVal aCon As Boolean, ParamArray parameters As Object()) As Integer
        If transactionActive AndAlso aCon Then Throw New Exception("AutoConnect-methods (ACon) are not allowed durring a transaction!")
        Using command As MySqlCommand = New MySqlCommand(sqlQuery, Connection)
            If transactionActive Then command.Transaction = transaction
            Dim result As Integer
            For Each parameter As Object In parameters
                command.Parameters.AddWithValue(String.Empty, parameter)
            Next
            If aCon Then Open()
            result = command.ExecuteNonQuery()
            If aCon Then Close()
            Return result
        End Using
    End Function

#End Region

#Region "ExecuteQuery"

    ''' <summary>
    ''' Executes a query-statement.
    ''' </summary>
    ''' <paramname="sqlQuery">SQL-query</param>
    ''' <paramname="parameters">Query-parameters</param>
    ''' <returns>DataReader fetching the query-results</returns>
    Public Function ExecuteQuery(ByVal sqlQuery As String, ParamArray parameters As Object()) As MySqlDataReader
        Dim command As MySqlCommand = New MySqlCommand(sqlQuery, Connection)
        For Each parameter As Object In parameters
            command.Parameters.AddWithValue(String.Empty, parameter)
        Next
        Return command.ExecuteReader()
    End Function

#End Region

#Region "ExecuteScalar"

    ''' <summary>
    ''' Executes a execute-scalar statement. 
    ''' </summary>
    ''' <paramname="sqlQuery">SQL-query</param>
    ''' <paramname="parameters">Query-parameters</param>
    ''' <returns>Result of the scalar-query</returns>
    Public Function ExecuteScalar(ByVal sqlQuery As String, ParamArray parameters As Object()) As Object
        Return ExecuteScalar(Of Object)(sqlQuery, False, parameters)
    End Function

    ''' <summary>
    ''' Executes a execute-scalar statement. 
    ''' Automatically opens and closes the connection.
    ''' </summary>
    ''' <paramname="sqlQuery">SQL-query</param>
    ''' <paramname="parameters">Query-parameters</param>
    ''' <returns>Result of the scalar-query</returns>
    Public Function ExecuteScalarACon(ByVal sqlQuery As String, ParamArray parameters As Object()) As Object
        Return ExecuteScalar(Of Object)(sqlQuery, True, parameters)
    End Function

    ''' <summary>
    ''' Executes a execute-scalar statement. 
    ''' </summary>
    ''' <typeparamname="T">Target-datatype of the result</typeparam>
    ''' <paramname="sqlQuery">SQL-query</param>
    ''' <paramname="parameters">Query-parameters</param>
    ''' <returns>Result of the scalar-query</returns>
    Public Function ExecuteScalar(Of T)(ByVal sqlQuery As String, ParamArray parameters As Object()) As T
        Return ExecuteScalar(Of T)(sqlQuery, False, parameters)
    End Function

    ''' <summary>
    ''' Executes a execute-scalar statement.
    ''' Automatically opens and closes the connection.
    ''' </summary>
    ''' <typeparamname="T">Target-datatype of the result</typeparam>
    ''' <paramname="sqlQuery">SQL-query</param>
    ''' <paramname="parameters">Query-parameters</param>
    ''' <returns>Result of the scalar-query</returns>
    Public Function ExecuteScalarACon(Of T)(ByVal sqlQuery As String, ParamArray parameters As Object()) As T
        Return ExecuteScalar(Of T)(sqlQuery, True, parameters)
    End Function

    ''' <summary>
    ''' Executes a execute-scalar statement.
    ''' </summary>
    ''' <typeparamname="T">Target-datatype of the result</typeparam>
    ''' <paramname="sqlQuery">SQL-query</param>
    ''' <paramname="aCon">Manage connection states (AutoConnect)</param>
    ''' <paramname="parameters">Query-parameters</param>
    ''' <returns>Result of the scalar-query</returns>
    Private Function ExecuteScalar(Of T)(ByVal sqlQuery As String, ByVal aCon As Boolean, ParamArray parameters As Object()) As T
        If transactionActive AndAlso aCon Then Throw New Exception("AutoConnect-methods (ACon) are not allowed durring a transaction!")
        Using command As MySqlCommand = New MySqlCommand(sqlQuery, Connection)
            If transactionActive Then command.Transaction = transaction
            For Each parameter As Object In parameters
                command.Parameters.AddWithValue(String.Empty, parameter)
            Next
            If aCon Then Open()
            Dim retval As Object = command.ExecuteScalar()
            If aCon Then Close()
            Return Convert.ChangeType(retval, GetType(T))
        End Using
    End Function

#End Region

#Region "DataAdapter"

    ''' <summary>
    ''' Fills a DataTable with the results of a query-statement.
    ''' </summary>
    ''' <paramname="sqlQuery">SQL-query</param>
    ''' <paramname="parameters">Query-parameters</param>
    ''' <returns>Results of a query-statement</returns>
    Public Function FillDataTable(ByVal sqlQuery As String, ParamArray parameters As Object()) As DataTable
        Using command As MySqlCommand = New MySqlCommand(sqlQuery, Connection)
            For Each parameter As Object In parameters
                command.Parameters.AddWithValue(String.Empty, parameter)
            Next
            Using da As MySqlDataAdapter = New MySqlDataAdapter(command)
                Dim dt As DataTable = New DataTable()
                da.Fill(dt)
                Return dt
            End Using
        End Using
    End Function

    ''' <summary>
    ''' Creates a DataAdapter on the given query-statement.
    ''' </summary>
    ''' <paramname="sqlQuery">SQL-query</param>
    ''' <paramname="parameters">Query-parameters</param>
    ''' <returns>DataAdapter of the given query-statement</returns>
    Public Function GetDataAdapter(ByVal sqlQuery As String, ParamArray parameters As Object()) As MySqlDataAdapter
        Using command As MySqlCommand = New MySqlCommand(sqlQuery, Connection)
            For Each parameter As Object In parameters
                command.Parameters.AddWithValue(String.Empty, parameter)
            Next
            Return New MySqlDataAdapter(command)
        End Using
    End Function

#End Region
End Class
