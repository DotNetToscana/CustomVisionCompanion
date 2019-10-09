namespace CustomVisionCompanion.Services
{
    public interface ISettingsService
    {
        string Region { get; set; }

        string ProjectName { get; set; }

        string PredictionKey { get; set; }

        string IterationId { get; set; }
    }
}
