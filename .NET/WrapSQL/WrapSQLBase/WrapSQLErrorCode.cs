using System;
using System.Collections.Generic;
using System.Text;

namespace WrapSQL
{
    public enum WrapSQLErrorCode
    {
        None = 0,
        Success = 200,

        ConnectionCreationFailed = 1100,
        ConnectionCloseFailed = 1101,

        TransactionBeginFailed = 1201,
        TransactionCommitFailed = 1202,
        TransactionRollbackFailed = 1203,

        OperationNonQueryFailed = 2100,
        OperationScalarFailed = 2200,
        OperationQueryFailed = 2300,

        OperationDataAdapterFailed = 3100,
        OperationDataTableFailed = 3200
    }
}
