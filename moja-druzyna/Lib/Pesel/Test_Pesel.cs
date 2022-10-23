using System;
using System.Collections.Generic;

namespace moja_druzyna.Lib.Pesel
{
    public class Test_Pesel
    {
        public string testPesel(string data)
        {
            string output;
            Pesel p = new Pesel(data);
            if (p.isValid())
            {
                output = $"PESEL {data} jest poprawny.";
                output = output + $"Data urodzenia: {p.getBirthday()}";
                bool male = p.isMale();
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
