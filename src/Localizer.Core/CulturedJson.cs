using System.Globalization;
using System.Text.Json.Nodes;

namespace Localizer;

public readonly record struct CulturedJson(string FilePath, JsonObject Json, CultureInfo CultureInfo);