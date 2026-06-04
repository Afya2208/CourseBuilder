using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Exceptions
{
    public class NotFoundException(string message, Exception? innerException = null) : Exception(message, innerException)
    {
        
    }
}