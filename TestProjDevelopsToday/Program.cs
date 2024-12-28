using TestProjDevelopsToday.Base;
using TestProjDevelopsToday.Helpers;
using TestProjDevelopsToday.Models;

namespace TestProjDevelopsToday;

/// <summary>
/// 
/// 9. Assume your program will be used on much larger data files. 
/// Describe in a few sentences what you would change if you knew it would be used for a 10GB CSV input file.
///
/// Answer:
/// 
/// The first of all, we need to use multithreading where each thread processes its own chunk of memory.
/// After processing, all results should be written to the database.
/// Additionally, we can add an entity named "Status" to store information about the number of rows that have been processed/scraped:
/// For example,this entity will contain 2 prop: startFrom = 0 and finishTo = 1_000_000. Thanks to this data, we won't need to reprocess the previously handled rows.
/// The last greater performance, we can combine parallelism with asynchronous programming
/// 
/// </summary>
/// <param name="args"></param>



static class Program
{
    private const int ChunkSize = 5000;
    private const string DuplicatesFileName = "\\duplicates.csv";
    static async Task Main(string[] args)
    {
        Console.Write("Enter file path: ");
        string? filePath = Console.ReadLine();
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
        {
            Console.WriteLine("File not found");
            return;
        }
        
        var records = CsvHelperExtension.GetRecords<TripRecord>(filePath);
        
        var duplicateRecords = records
            .GroupBy(r => new { r.PickupDate, r.DropoffDate, r.PassengerCount })
            .Where(g => g.Count() > 1)
            .SelectMany(g => g).DistinctBy(r => new { r.PickupDate, r.DropoffDate, r.PassengerCount })
            .ToList();

        var recordsWithoutDuplicate = records
            .GroupBy(r => new { r.PickupDate, r.DropoffDate, r.PassengerCount })
            .Select(g => g.First())
            .ToList();

        Console.WriteLine("Duplicate records: {0}", duplicateRecords.Count);
        if (duplicateRecords.Any())
        {
            CsvHelperExtension.WriteToCsv(duplicateRecords,  Path.Combine(AppConfiguration.DirectoryPathForDuplicatesFile, DuplicatesFileName));
        }

        await SaveRecordsInDatabase(recordsWithoutDuplicate, ChunkSize);
    }


    private static async Task SaveRecordsInDatabase(List<TripRecord> records, int chunkSize)
    {
        var chunkedRecords = records.Chunk(chunkSize);
        
        List<Task> tasks = new List<Task>();
        foreach (var recordChunk in chunkedRecords)
        {
            tasks.Add(Task.Run(async () =>
            {
                using var scopedDbContext = new ApplicationContext(new());

                await scopedDbContext.TripRecords.AddRangeAsync(recordChunk);
                await scopedDbContext.SaveChangesAsync();
            }));
        }

        await Task.WhenAll(tasks);
    }
}