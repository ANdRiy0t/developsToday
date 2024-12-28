namespace TestProjDevelopsToday.Base;

public static class AppConfiguration
{
    public static string ConnectionString = "Host=localhost;Port=5432;Database=usersdb;Username=postgres;Password=mysecurepassword";
    public static string DirectoryPathForDuplicatesFile = AppContext.BaseDirectory;
}