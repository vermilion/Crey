namespace Spear.ProxyGenerator.Abstractions;

public interface IResolver
{
    void Register(Type type, object value, object key = null);

    object Resolve(Type type, object key = null);
}