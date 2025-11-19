using System.Reflection;

namespace Prolexy.Compiler.Implementations;

public class MethodSelector
{
    private readonly object _objectContext;
    private readonly string _methodName;

    public MethodSelector(object objectContext, string methodName)
    {
        _objectContext = objectContext;
        _methodName = methodName;
    }

    public MethodInfo? FindMethod(object[] args)
    {
        return _objectContext.GetType()
            .GetMethods()
            .SingleOrDefault(m =>
                m.Name == _methodName &&
                m.GetParameters().Length == args.Length &&
                m.GetParameters().Select((p, idx) =>
                        p.ParameterType.IsInstanceOfType(args[idx]))
                    .All(cond => cond));
    }
}