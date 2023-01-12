namespace Crey.Session;

public abstract class ContextProvider<T>
    where T : new()
{
    private static readonly AsyncLocal<T> _asyncLocal = new();

    public static T Current
    {
        get
        {
            _asyncLocal.Value ??= new T();

            return _asyncLocal.Value;
        }
    }

    public static void Set(T value)
    {
        _asyncLocal.Value = value;
    }

    public static void Reset()
    {
        _asyncLocal.Value = new T();
    }
}