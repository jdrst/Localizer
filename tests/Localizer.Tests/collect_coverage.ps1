dotnet-coverage collect -f cobertura "dotnet run"

reportgenerator -reports:./output.cobertura.xml -targetdir:"../../coveragereport" -reporttypes:Html -assemblyfilters:"+Localizer*;-Localizer.Tests*"

Remove-Item output.cobertura.xml
