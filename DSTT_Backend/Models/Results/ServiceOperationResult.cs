namespace DSTT_Backend.Models.Results
{
    public class ServiceOperationResult
    {
        public bool Success { get; set; }

        public string? ErrorMessage { get; set; }

        public int? StatusCode { get; set; }
    }
}
