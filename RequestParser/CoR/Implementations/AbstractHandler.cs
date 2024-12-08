using RequestParser.CoR.Contracts;

namespace RequestParser.CoR.Implementations;

public abstract class AbstractHandler : IHandler
{
    private IHandler? _nextHandler;

    public IHandler SetNext(IHandler handler)
    {
        _nextHandler = handler;
        return handler;
    }

    public virtual bool Handle(string request) => _nextHandler?.Handle(request) ?? true;
}