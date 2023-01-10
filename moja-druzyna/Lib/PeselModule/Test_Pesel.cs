namespace moja_druzyna.Lib.PeselModule
{
    public class Test_Pesel
    {
        public string testPesel(string data)
        {
            string output;
            Pesel p = new Pesel(data);
            if (p.IsValid())
            {
                output = $"PESEL {data} jest poprawny.";
                output = output + $"Data urodzenia: {p.GetBirthday()}";
                bool male = p.IsMale();
                if (male == true)
                {
                    output = output + "Płeć: Mężczyzna";
                }
                else
                {
                    output = output + "Płeć: Kobieta";
                }
            }
            else
            {
                output = $"PESEL {data} jest niepoprawny!";
            }
            return output;

        }
    }
}
