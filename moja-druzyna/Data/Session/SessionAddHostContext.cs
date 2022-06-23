using System;

namespace moja_druzyna.Data.Session
{
    public class SessionAddHostContext : ISessionAddHostContext
    {
        public string AddedHostName { get; set; }
        public string AddedHostCaptainPesel { get; set; }
    }
}
