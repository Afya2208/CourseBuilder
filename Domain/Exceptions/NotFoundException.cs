namespace Domain.Exceptions
{
    public class NotFoundException(string message, object objId, Exception? innerException = null) : Exception(message, innerException)
    {
        
    }
}