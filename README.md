# Localizer

A little CLI helper for [kli.Localize](https://github.com/kl1mm/localize/). 
You can pass your [default localization file](https://github.com/kl1mm/localize/?tab=readme-ov-file#create-json-files-for-your-localized-texts)
 and missing nodes in other localization files will be added with their values set depending on the configured provider.

_Example:_

`locale.json`:
```json
{
    "Foo": "Bar",
    "Ipsum": "Lorem"
}
```
`locale_en.json`:
```json
{
    "Foo": "Bar"
}
```
Running `localizer translate locale.json` with default configuration will change the `locale_en.json` to
```json
{
    "Foo": "Bar",
    "Ipsum": "<<replaceme>> Lorem"
}
```

## Available providers

### ReplaceMe

Adds missing localizations with the value `<<replaceme>> <value from your default localization file>`

### Prompt

Prompts you for the translation for each missing localization.

### DeepL

Gets the translations from the [DeepL api](https://www.deepl.com/en/pro-api) (You need an account but 500k characters/month are free).

See [here](#configuration) for information regarding configuration.


## Installation

```
dotnet tool install -g jdrst.Localizer
```

## Usage

Navigate to the directory with your locale files:

`localizer translate <default locale file>`

For further options check `localizer translate --help`

## Configuration

You can get/set/unset local or global (`-g`) config settings.  
Local config takes precedence over global config.

See `localizer config get --help`, `localizer config set --help` or `localizer config list`.

Local config settings will be saved in your __working directory__ in a `.localizer` file.

### Currently available config settings:

`TranslationProvider`: One of `ReplaceMe`, `Prompt` or `DeepL`

`DeepL:Context`: See [translation context](https://developers.deepl.com/docs/best-practices/working-with-context)  
`DeepL:AuthKey`: Your api key for the DeepL api (in case of `DeepL` you __need__ to set this).  
`DeepL:SourceLanguage` The language you are translating from so DeepL doesn't have to guess. 

DeepL only supports certain languages. See [here](https://developers.deepl.com/docs/api-reference/languages) for a list.

You can also edit your config files manually. Just make sure it's valid JSON.   
The global config file is `appsettings.json` next to the executable.