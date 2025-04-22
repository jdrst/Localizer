using Microsoft.Extensions.Logging;

namespace Localizer.Logging;

public static partial class LoggerMessages
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "Could not find any locale files for {baseLocaleFile}")]
    public static partial void CouldNotFindLocales(this ILogger logger, string baseLocaleFile);
    
    [LoggerMessage(Level = LogLevel.Debug, Message = "File does not exist {filePath}")]
    public static partial void FileDoesntExist(this ILogger logger, string filePath);
}