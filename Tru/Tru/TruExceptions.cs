// For System.Exception, which new exception classes must extend from.
using System;

namespace Tru {
    /// A parsing exception for the Tru language
    public class TruSyntaxError : Exception {
        public TruSyntaxError() {}
        public TruSyntaxError(string message) : base(message) {}
    }

    /// A runtime exception for the Tru language
    public class TruRuntimeException : Exception {
        public TruRuntimeException() {}
        public TruRuntimeException(string message) : base(message) {}
    }
}