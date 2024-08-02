namespace DSTT_Backend.Models.Results
{
    public class ServiceDataOperationResult<T> : ServiceOperationResult
    {
        public List<T>? Data { get; set; }
    }
}
