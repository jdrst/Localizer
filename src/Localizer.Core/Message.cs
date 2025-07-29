namespace Localizer.Core;

public enum MessageType
{
    Error,
    Info
}

public readonly record struct Message(MessageType MessageType, MessageContent Text)
{
    public static Message Error(string text) => new(MessageType.Error, new StringMessage(text));
    public static Message Info(string text) => new(MessageType.Info, new StringMessage(text));
    public static Message Error(Func<string> textFunc) => new(MessageType.Error, new LazyStringMessage(textFunc ?? (() => string.Empty)));
    public static Message Info(Func<string> textFunc) => new(MessageType.Info, new LazyStringMessage(textFunc ?? (() => string.Empty)));
};

public abstract class MessageContent {
    public abstract string Value {get;}

    public abstract override string ToString();

    public static implicit operator string (MessageContent content) => content?.ToString() ?? string.Empty;
}

internal sealed class StringMessage(string value) : MessageContent
{ 
    public override string Value => value;

    public override string ToString() => value;
}

internal sealed class LazyStringMessage(Func<string> valueFunc) : MessageContent
{ 
    public override string Value => valueFunc();

    public override string ToString() => valueFunc();
}
