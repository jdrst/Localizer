using Microsoft.Extensions.Logging;

namespace Localizer.Infrastructure.Logging;

public static partial class LoggerMessages
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "Billed DeepL-API characters during this invocation: {charactersUsed}")]
    public static partial void DebugCharactersUsed(this ILogger logger, int charactersUsed);
}