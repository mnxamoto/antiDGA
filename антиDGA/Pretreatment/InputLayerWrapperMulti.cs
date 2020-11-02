using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace антиDGA.Pretreatment
{
    class InputLayerWrapperMulti :InputLayerWrapper
    {
        public InputLayerWrapperMulti()
        {
            namesAttributes = new string[22];
            namesAttributes[0] = "DomainLength";
            namesAttributes[1] = "NumberDigits";
            namesAttributes[2] = "NumberVowels";
            namesAttributes[3] = "NumberConsonants";
            namesAttributes[4] = "NumberSpecialCharacters";
            namesAttributes[5] = "NumberUniqueCharacter";
            namesAttributes[6] = "NumberRepetitions";
            namesAttributes[7] = "LengthDigitsSequence";
            namesAttributes[8] = "LengthVowelsSequence";
            namesAttributes[9] = "LengthConsonantSequence";
            namesAttributes[10] = "LengthSpecialCharactersSequence";
            namesAttributes[11] = "RatioDigitToLength";
            namesAttributes[12] = "RatioVowelsToLength";
            namesAttributes[13] = "RatioConsonantsToLength";
            namesAttributes[14] = "RatioSpecialCharactersToLength";
            namesAttributes[15] = "RatioVowelsToConsonants";
            namesAttributes[16] = "RatioUniqueCharacterToLength";
            namesAttributes[17] = "FrequencyRepeat";
            namesAttributes[18] = "FrequencyNgram2";
            namesAttributes[19] = "FrequencyNgram3";
            namesAttributes[20] = "FrequencyNgram4";
            namesAttributes[21] = "FrequencyNgram5";
        }

        public override void ConfigureDomainAttribute(string Domain)
        {
            ProcessingMulti proc = new ProcessingMulti(Domain);
            valuesAttributes = new double[22];
            valuesAttributes[0] = proc.DomainLength;
            valuesAttributes[1] = proc.NumberDigits;
            valuesAttributes[2] = proc.NumberVowels;
            valuesAttributes[3] = proc.NumberConsonants;
            valuesAttributes[4] = proc.NumberSpecialCharacters;
            valuesAttributes[5] = proc.NumberUniqueCharacter;
            valuesAttributes[6] = proc.NumberRepetitions;
            valuesAttributes[7] = proc.LengthDigitsSequence;
            valuesAttributes[8] = proc.LengthVowelsSequence;
            valuesAttributes[9] = proc.LengthConsonantSequence;
            valuesAttributes[10] = proc.LengthSpecialCharactersSequence;
            valuesAttributes[11] = proc.RatioDigitToLength;
            valuesAttributes[12] = proc.RatioVowelsToLength;
            valuesAttributes[13] = proc.RatioConsonantsToLength;
            valuesAttributes[14] = proc.RatioSpecialCharactersToLength;
            valuesAttributes[15] = proc.RatioVowelsToConsonants;
            valuesAttributes[16] = proc.RatioUniqueCharacterToLength;
            valuesAttributes[17] = proc.FrequencyRepeat;
            valuesAttributes[18] = proc.FrequencyNgram2;
            valuesAttributes[19] = proc.FrequencyNgram3;
            valuesAttributes[20] = proc.FrequencyNgram4;
            valuesAttributes[21] = proc.FrequencyNgram5;
        }
    }
}
