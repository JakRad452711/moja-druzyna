namespace moja_druzyna.Lib.Order
{
    public class TrialClosing : IOrderElement
    {
        public string ScoutId { get; set; }
        public string ScoutPesel { get; set; }
        public string ScoutName { get; set; }
        public string ScoutSurname { get; set; }
        public string TrialType { get; set; }
        public string TrialName { get; set; }
        public string Rank { get; set; }
        public string Ability { get; set; }

        public string GetScoutId()
        {
            return ScoutId;
        }
    }
}
