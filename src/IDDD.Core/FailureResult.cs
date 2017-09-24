using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IDDD.Core
{
    public class FailureResult : Exception
    {
        public IEnumerable<string> Errors { get; }

        public FailureResult()
        {
        }

        public FailureResult(string message) : base(message)
        {
        }

        public FailureResult(string message, params string[] Errors) : base(message)
        {
            this.Errors = Errors;
        }

        public FailureResult(string message, Exception innerException) : base(message, innerException)
        {
        }

    }
}
