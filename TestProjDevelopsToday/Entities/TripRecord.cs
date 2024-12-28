using CsvHelper.Configuration.Attributes;
using TestProjDevelopsToday.Base;
using TestProjDevelopsToday.Helpers;

namespace TestProjDevelopsToday.Models;

public class TripRecord
{
    [Ignore] public int Id { get; set; }
    
    [Name("tpep_pickup_datetime")] 
    [UtcDateTime]
    public DateTime PickupDate { get; set; }
    [Name("tpep_dropoff_datetime")] 
    [UtcDateTime] 
    public DateTime DropoffDate { get; set; }
    [Name("passenger_count")] public int? PassengerCount { get; set; }
    [Name("trip_distance")] public double? TripDistance { get; set; }
    
    [Name("store_and_fwd_flag")] 
    [TypeConverter(typeof(FlagConvertor))]
    public string StoreAndFwdFlag { get; set; }
    
    [Name("PULocationID")] public int? PULocationID { get; set; }
    [Name("DOLocationID")] public int? DOLocationID { get; set; }
    [Name("fare_amount")] public double? FareAmount { get; set; }
    [Name("tip_amount")] public double? TipAmount { get; set; }
}

