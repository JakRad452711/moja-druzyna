namespace moja_druzyna.Data.Session
{
    public interface ISessionOperationStatusContext
    {
        public bool OperationSucceeded { get; set; }
        public bool OperationFailed { get; set; }
    }
}
