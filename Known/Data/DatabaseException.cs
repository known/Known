using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;

namespace Known.Data
{
    public class DatabaseException : DataException
    {
        public DatabaseException(List<Command> commands)
        {
            Commands = commands;
        }

        public DatabaseException(List<Command> commands, string message) 
            : base(message)
        {
            Commands = commands;
        }

        public DatabaseException(List<Command> commands, string message, Exception innerException)
            : base(message, innerException)
        {
            Commands = commands;
        }

        protected DatabaseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public List<Command> Commands { get; }
    }
}
