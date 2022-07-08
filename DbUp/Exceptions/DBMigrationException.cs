using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUp.Exceptions
{
    class DBMigrationException : Exception
    {
        protected string _customMessage;
        public string CustomMessage
        {
            get { return _customMessage; }
            set { _customMessage = value; }
        }

        public DBMigrationException(string message)
        {
            CustomMessage = message;
        }
    }
}
