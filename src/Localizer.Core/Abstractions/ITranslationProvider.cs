﻿using System.Globalization;

namespace Localizer.Core.Abstractions;

public interface ITranslationProvider
{
    public IReadOnlyList<Message> Messages { get; }
    public bool UsesConsole { get; }

    public Task<string[]> GetTranslationsAsync(string[] texts, CultureInfo cultureInfo,
        CancellationToken ct = default);
}