namespace Localizer.Application;

public enum MessageType
{
    Error,
    Info
}

public readonly record struct Message(MessageType MessageType, string Text)
{
    public static Message Error(string Text) => new(MessageType.Error, Text);
    public static Message Info(string Text) => new(MessageType.Info, Text);
};

