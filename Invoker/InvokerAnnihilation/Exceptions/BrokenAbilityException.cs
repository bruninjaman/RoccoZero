using System.Collections;
using System.Runtime.Serialization;

namespace InvokerAnnihilation.Exceptions;

[Serializable]
public class BrokenAbilityException : InvokeException
{
    public BrokenAbilityException()
        : base()
    {
    }

    public BrokenAbilityException(string message)
        : base(message)
    {
    }

    public BrokenAbilityException(string message, System.Exception innerException)
        : base(message, innerException)
    {
    }

    protected BrokenAbilityException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    public override IDictionary Data { get; } = new Dictionary<string, object>();
}

[Serializable]
public class InvokeException : System.Exception
{
    public InvokeException()
        : base()
    {
    }

    public InvokeException(string message)
        : base(message)
    {
    }

    public InvokeException(string message, System.Exception innerException)
        : base(message, innerException)
    {
    }

    protected InvokeException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    public override IDictionary Data { get; } = new Dictionary<string, object>();
}