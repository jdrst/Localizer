using Localizer.Core;
using Spectre.Console;

namespace Localizer.Application.Commands.Config.Helpers;

public static class ConsoleExtensions
{
    public const string InfoMarkup = "[bold][[Info]]:[/] ";
    public const string ErrorMarkup = "[bold][[Error]]:[/] ";
    public static void WriteMessage(this IAnsiConsole console, Message message)
    {
        console.MarkupLine(message.MessageType switch
        {
            MessageType.Info => $"{InfoMarkup}{message.Text}",
            MessageType.Error => $"{ErrorMarkup}{message.Text}",
            _ => throw new NotImplementedException(nameof(MessageType))
        });
    }
}