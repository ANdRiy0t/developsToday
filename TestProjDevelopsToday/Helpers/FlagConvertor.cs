using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace TestProjDevelopsToday.Helpers;

public class FlagConvertor : ITypeConverter
{
    public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        return text switch
        {
            "Y" => "Yes",
            "N" => "No",
            _ => text
        };
    }

    public string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
    {
        return value?.ToString() switch
        {
            "Yes" => "Y",
            "No" => "N",
            _ => value?.ToString()
        };
    }
}