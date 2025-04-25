namespace Localizer.Core;

public enum MessageType
{
    Error,
    Info
}

public readonly record struct Message(MessageType MessageType, string Text)
{
    public static Message Error(string text) => new(MessageType.Error, text);
    public static Message Info(string text) => new(MessageType.Info, text);
};

