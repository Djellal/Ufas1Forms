using System.Text.Json;

namespace Ufas1Forms.Helpers;

public static class OptionsJsonParser
{
    public static List<SelectOption> Parse(string? optionsJson)
    {
        if (string.IsNullOrWhiteSpace(optionsJson))
            return new List<SelectOption>();

        try
        {
            var options = JsonSerializer.Deserialize<List<SelectOption>>(optionsJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return options ?? new List<SelectOption>();
        }
        catch
        {
            return new List<SelectOption>();
        }
    }
}
