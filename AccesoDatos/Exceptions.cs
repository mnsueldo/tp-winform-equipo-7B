using System;

namespace Negocio
{
    public class BusinessRuleException : ApplicationException
    {
        public BusinessRuleException(string message) : base(message) { }
    }
}
