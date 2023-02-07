namespace Crey.Exceptions;

public class FaultException<T> : FaultException
{
    public FaultException(MessageException messageException) 
        : base(messageException)
    {
    }
}

public class FaultException : Exception
{
    private readonly string _source;
    private readonly string _stackTrace;

    public FaultException(string message)
        : base(message)
    {
    }

    public FaultException(MessageException messageException)
    : this(messageException.Message)
    {
        _source = messageException.Source;
        _stackTrace = messageException.StackTrace;
    }

    public override string Source => _source;
    public override string StackTrace => string.IsNullOrEmpty(_stackTrace) ? base.StackTrace : _stackTrace;
}
