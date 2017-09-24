﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IDDD.Common
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

        protected FailureResult(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
