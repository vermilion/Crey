namespace Crey.Exceptions;

internal class ExceptionConverter : IExceptionConverter
{
    public MessageException Wrap(Exception ex)
    {
        return new MessageException(ex);
    }

    public FaultException Unwrap(MessageException? message)
    {
        if (message is null)
            return new FaultException(ExceptionConstants.UnknownError);

        var serviceExceptionType = Type.GetType(message.ExceptionTypeName);
        if (serviceExceptionType is null)
            return new FaultException(message.Message);

        var genericType = typeof(FaultException<>).MakeGenericType(serviceExceptionType);
        var ex = (FaultException)Activator.CreateInstance(genericType, new[] { message });
        return ex;
    }
}