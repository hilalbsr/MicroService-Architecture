namespace Esourcing.Sourcing.Settings
{
    public interface ISourcingDatabaseSettings
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
