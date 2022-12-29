namespace Crey.Exceptions;

public class FaultException : Exception
{
    public int Code { get; set; }

    public FaultException(string message, int code = 500) : base(message)
    {
        Code = code;
    }
}
