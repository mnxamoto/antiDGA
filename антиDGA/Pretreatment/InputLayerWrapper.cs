using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace антиDGA.Pretreatment
{
    public class InputLayerWrapper
    {
        public string[] namesAttributes;

        public double[] valuesAttributes;
        public Dictionary<string, float> Attributes { get; set; }

        public InputLayerWrapper()
        {

        }

        public virtual void ConfigureDomainAttribute(string Domain)
        {

        }
    }
}
