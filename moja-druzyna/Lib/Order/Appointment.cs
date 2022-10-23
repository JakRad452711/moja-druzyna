namespace moja_druzyna.Lib.Order
{
    public class Appointment : IOrderElement
    {
        public string ScoutId { get; set; }
        public string ScoutPesel { get; set; }
        public string ScoutName { get; set; }
        public string ScoutSurname { get; set; }
        public string Role { get; set; }
        public string RoleName { get; set; }
        public string Host { get; set; }

        public string GetScoutId()
        {
            return ScoutId;
        }
    }
}
