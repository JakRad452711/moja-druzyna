namespace moja_druzyna.Lib.Order
{
    public class ReprimendsAndPraises : IOrderElement
    {
        public string ScoutId { get; set; }
        public string ScoutPesel { get; set; }
        public string ScoutName { get; set; }
        public string ScoutSurname { get; set; }
        public string Type { get; set; }
        public string Explanation { get; set; }

        public string GetScoutId()
        {
            return ScoutId;
        }
    }
}
