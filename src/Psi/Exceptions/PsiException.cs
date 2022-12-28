namespace Psi.Exceptions;

public class PsiException : Exception
{
    public int Code { get; set; }

    public PsiException(string message, int code = 500) : base(message)
    {
        Code = code;
    }
}
