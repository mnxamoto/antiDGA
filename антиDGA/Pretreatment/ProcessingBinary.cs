using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace антиDGA.Pretreatment
{
    public class ProcessingBinary
    {
        public string CurrentDomain { get; set; }


        public bool DomainLengthAllow { get; set; }

        public bool DomainAllow { get; set; }

        public bool TDLength { get; set; }

        public bool DomainUnderlevel { get; set; }

        public bool SpecChars { get; set; }


        public List<string> AllowedTDLs = new List<string>
        {
            "com", "ru", "org", "co.uk", "ca", "ro", "net", "it", "co.jp", "dk", "fr",
            "jp", "ir", "com.au", "fi", "in", "az", "edu", "hk"
        };


        public ProcessingBinary(string Domain)
        {
            if (string.IsNullOrEmpty(Domain))
                return;

            CurrentDomain = Domain;

            CheckLength();
            CheckTDL();
            ContainsUnderlevel();
            ContainsSpecChars();

        }


        private void CheckLength()
        {

            DomainLengthAllow = CurrentDomain.Length >= 5 ? false : true;
            //Заглушка на данный момент
        }

        private void CheckTDL()
        {
            //Необходимо сначала вырезать нужную часть
            Regex reg = new Regex(".[a-zA-Z]{2,11}?$");

            MatchCollection collection = reg.Matches(CurrentDomain);

            if (collection.Count <= 0)
                DomainAllow = false;

            string Founded = collection[0].Value.Remove(0, 1); // Удалить точку

            if (AllowedTDLs.Contains(Founded))
                DomainAllow = true;
            else DomainAllow = false;


            //Странный домен больше чем co.jp
            TDLength = Founded.Length >= 5 ? false : true;
        }

        private void ContainsUnderlevel()
        {
            //Необходимо удостовериться в правильности данной регулярки
            Regex reg = new Regex(@"^[a-zA-Z0-9][a-zA-Z0-9.-]+[a-zA-Z0-9]$");

            MatchCollection collection = reg.Matches(CurrentDomain);

            DomainUnderlevel = collection.Count <= 0 ? true : false;

        }

        public void ContainsSpecChars()
        {
            string specialChar = @"\|!#$%&/()=?»«*@£§€{};'<>_,";

            foreach (var c in CurrentDomain)
            {
                if (specialChar.Contains(c))
                {
                    SpecChars = true;
                    break;
                }
                else SpecChars = false;
            }
        }
    }

}
