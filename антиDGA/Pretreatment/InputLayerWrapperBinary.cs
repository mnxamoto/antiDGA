using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace антиDGA.Pretreatment
{
    class InputLayerWrapperBinary : InputLayerWrapper
    {
        public InputLayerWrapperBinary()
        {
            namesAttributes = new string[5];
            namesAttributes[0] = "Длина домена";
            namesAttributes[1] = "После точки в словаре?";
            namesAttributes[2] = "Длина после точки";
            namesAttributes[3] = "Есть поддомен?";
            namesAttributes[4] = "Спец. символы";
        }

        public override void ConfigureDomainAttribute(string Domain)
        {
            ProcessingBinary proc = new ProcessingBinary(Domain);
            valuesAttributes = new double[5];
            valuesAttributes[0] = Convert.ToDouble(proc.DomainLengthAllow);
            valuesAttributes[1] = Convert.ToDouble(proc.DomainAllow);
            valuesAttributes[2] = Convert.ToDouble(proc.TDLength);
            valuesAttributes[3] = Convert.ToDouble(proc.DomainUnderlevel);
            valuesAttributes[4] = Convert.ToDouble(proc.SpecChars);
        }
    }
}
