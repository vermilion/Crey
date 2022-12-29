using System.ComponentModel;

namespace Psi.Exceptions;

public abstract class ErrorCodes
{
    public const int DefaultCode = -1;

    [Description("System failure")]
    public const int SystemError = 10001;

    [Description("No services found alive")]
    public const int NoService = 10007;
}
