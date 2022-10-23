using System;
using System.IO;

namespace moja_druzyna.Lib.Pesel
{
    public class Pesel
    {
        public string pesel;
        private static int[] multipliers = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };

        public Pesel(string pesel)
        {
            this.pesel = pesel;
        }

        public string PESEL
        {
            get { return pesel; }
            set { }
        }

        public bool isPesel()
        {
            if (PESEL.Length != 11)
            {
                return false;
            }
            try
            {
                long temp = long.Parse(PESEL);
                return true;
            }
            catch { return false; }
        }

        public int getDay()
        {
            /*
            char[] digits = new char[PESEL.Length];
            using (StringReader sr = new StringReader(PESEL))
            {
                sr.Read(digits, 0, PESEL.Length);
            }*/
            int d10 = PESEL[4] - '0';
            int d1 = PESEL[5] - '0';
            int day = 10 * d10 + d1;
            return day;
        }

        public int getMonth()
        {
            int month;
            int d10 = PESEL[2] - '0';
            int d1 = PESEL[3] - '0';
            if (d10 > 1)
            {
                month = 10 * d10 + d1 - 20;
            }
            else
            {
                month = 10 * d10 + d1;
            }
            return month;
        }

        public int getYear()
        {
            int d1 = PESEL[1] - '0';
            int d10 = PESEL[0] - '0';
            int age = PESEL[2] - '0';
            int year;
            if (age > 1)
            {
                year = 2000 + d10 * 10 + d1;
            }
            else
            {
                year = 1900 + d10 * 10 + d1;
            }
            return year;
        }

        public int summary()
        {
            int sum = 0;
            for (int i = 0; i < 10; i++)
            {
                sum = sum + (PESEL[i] - '0') * multipliers[i];
            }
            return sum % 10;
        }

        public bool isValid()
        {
            bool valid = true;
            if (!isPesel())
            {
                valid = false;
            }
            else
            {
                int rest = summary();
                if (rest == 0 & PESEL[10] - '0' != 0)
                {
                    valid = false;
                }
                else if (PESEL[10] - '0' != 10 - rest)
                {
                    valid = false;
                }
                else if (getDay() > 31 | getMonth() > 12)
                {
                    valid = false;
                }
                else if (getMonth() == 4 | getMonth() == 6 | getMonth() == 9 | getMonth() == 11)
                {
                    if (getDay() == 31)
                    {
                        valid = false;
                    }
                }
                else if (getMonth() == 2)
                {
                    if (getDay() == 31 | getDay() == 30)
                    {
                        valid = false;
                    }
                    else if (getDay() == 29)
                    {
                        if (getYear() % 4 != 0)
                        {
                            valid = false;
                        }
                    }
                }
            }
            return valid;
        }

        public DateTime getBirthday()
        {
            DateTime birthday = new DateTime();
            birthday = new DateTime(getYear(), getMonth(), getDay());
            //string birthday = $"{getDay()}.{getMonth()}.{getYear()}";
            return birthday;
        }

        public bool isMale()
        {
            if ((PESEL[9] - '0') % 2 != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
