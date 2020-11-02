using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace антиDGA.Pretreatment
{
    class ProcessingMulti
    {
        public string CurrentDomain { get; set; }
        public double DomainLength { get; set; }
        public double NumberDigits { get; set; }
        public double NumberVowels { get; set; }
        public double NumberConsonants { get; set; }
        public double NumberSpecialCharacters { get; set; }
        public double NumberRepetitions { get; set; }
        public double LengthDigitsSequence { get; set; }
        public double LengthVowelsSequence { get; set; }
        public double LengthConsonantSequence { get; set; }
        public double LengthSpecialCharactersSequence { get; set; }
        public double RatioDigitToLength  { get; set; }
        public double RatioVowelsToLength { get; set; }
        public double RatioConsonantsToLength { get; set; }
        public double RatioSpecialCharactersToLength { get; set; }
        public double RatioVowelsToConsonants { get; set; }
        public double NumberUniqueCharacter { get; set; }
        public double RatioUniqueCharacterToLength { get; set; }
        public double FrequencyRepeat { get; set; }
        public double FrequencyNgram2 { get; set; }
        public double FrequencyNgram3 { get; set; }
        public double FrequencyNgram4 { get; set; }
        public double FrequencyNgram5 { get; set; }

        private static string Digits = "0123456789";
        private static string Vowels = "eyuioa";
        private static string Consonants = "qwrtpsdfghjklzxcvbnm";
        private static string SpecialCharacters = "\\|!#$%&/()=?»«*@£§€{};'<>_,";

        public ProcessingMulti(string domain)
        {
            if (string.IsNullOrEmpty(domain))
                return;

            CurrentDomain = domain;

            DomainLength = domain.Length;
            NumberDigits = Regex.Matches(domain, @"[0123456789]", RegexOptions.IgnoreCase).Count;
            NumberVowels = Regex.Matches(domain, @"[eyuioa]", RegexOptions.IgnoreCase).Count;
            NumberConsonants = Regex.Matches(domain, @"[qwrtpsdfghjklzxcvbnm]", RegexOptions.IgnoreCase).Count;
            NumberSpecialCharacters = Regex.Matches(domain, @"[\|!#$%&/()=?»«*@£§€{};'<>_,]", RegexOptions.IgnoreCase).Count;
            NumberUniqueCharacter = domain.Distinct().Count();
            NumberRepetitions = DomainLength - NumberConsonants;
            LengthDigitsSequence = CalculateSequence(Digits);
            LengthVowelsSequence = CalculateSequence(Vowels);
            LengthConsonantSequence = CalculateSequence(Consonants);
            LengthSpecialCharactersSequence = CalculateSequence(SpecialCharacters);
            RatioDigitToLength = NumberDigits / DomainLength;
            RatioVowelsToLength = NumberVowels / DomainLength;
            RatioConsonantsToLength = NumberConsonants / DomainLength;
            RatioSpecialCharactersToLength = NumberSpecialCharacters / DomainLength;
            RatioVowelsToConsonants = NumberVowels / NumberConsonants;
            RatioUniqueCharacterToLength = NumberUniqueCharacter / DomainLength;
            FrequencyRepeat = CalculateFrequencyRepeat();

            Ngram ngram = Ngram.getInstance();
            FrequencyNgram2 = ngram.CalculateFrequencyNgram(domain, 2);
            FrequencyNgram3 = ngram.CalculateFrequencyNgram(domain, 3);
            FrequencyNgram4 = ngram.CalculateFrequencyNgram(domain, 4);
            FrequencyNgram5 = ngram.CalculateFrequencyNgram(domain, 5);
        }

        private double CalculateFrequencyRepeat() 
        {
            double frequency = 0;

            for (int i = 0; i < DomainLength - 1; i++)
            {
                string gram = CurrentDomain.Substring(i, 2);  //2 - Размер граммы

                frequency += Regex.Matches(CurrentDomain, gram).Count;
            }

            return frequency / (DomainLength - 1);
        }

        private double CalculateSequence(string characters)
        {
            double counter = 0;
            double max = 0;

            for (int i = 0; i < DomainLength; i++)
            {
                if (characters.IndexOf(CurrentDomain[i]) > -1)
                {
                    counter++;
                }
                else
                {
                    counter = 0;
                }

                if (counter > max)
                {
                    max = counter;
                }
            }

            return max;
        }
    }
}
