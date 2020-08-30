using System;
using System.Collections.Generic;
using System.Text;

namespace WrapSQL
{
    public enum OpenStatus
    {
        Success = 0,
        ConnectionError = 500,
        DatabaseBusy = 501,
        GeneralError = 555,
    }
}
