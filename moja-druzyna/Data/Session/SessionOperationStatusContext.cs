namespace moja_druzyna.Data.Session
{
    public class SessionOperationStatusContext : ISessionOperationStatusContext
    {
        public bool OperationSucceeded { get; set; }
        public bool OperationFailed { get; set; }
    }
}
