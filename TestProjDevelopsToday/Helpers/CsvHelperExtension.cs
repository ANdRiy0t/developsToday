using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace TestProjDevelopsToday.Helpers;

public static class CsvHelperExtension
{
    public static List<T> GetRecords<T>(string filePath)
    {
        var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);

        using var reader = new StreamReader(filePath);
        using var csvReader = new CsvReader(reader, csvConfiguration);

        return csvReader.GetRecords<T>().ToList();
    }
    
    public static void WriteToCsv<T>(IEnumerable<T> records, string filePath)
    {
        using var writer = new StreamWriter(filePath);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));
        
        csv.WriteHeader<T>();
        csv.NextRecord();
        
        csv.WriteRecords(records);
    }
}