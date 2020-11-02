using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using weka.core;

using антиDGA.Pretreatment;

namespace антиDGA.Helpers
{
    public static class WekaHelper
    {
        public static Instances GetInstancesForWeka(string[] domains, string[] labelsClass, Label status, ProgressBar progress)
        {
            java.util.ArrayList attributes = new java.util.ArrayList();

            //Добавление атрибутов
            InputLayerWrapperMulti wrapper = new InputLayerWrapperMulti();
            for (int i = 0; i < wrapper.namesAttributes.Length; i++)
            {
                attributes.Add(new weka.core.Attribute(wrapper.namesAttributes[i]));
            }

            java.util.ArrayList classes = new java.util.ArrayList();

            foreach (var item in labelsClass.ToList().Distinct())
            {
                classes.add(item);
            }

            attributes.Add(new weka.core.Attribute("class", classes)); //Метка класса

            Instances instances = new Instances($"Domains {DateTime.Now}", attributes, 0);

            int numAttributes = instances.numAttributes();
            int countRows = domains.Length;

            for (int i = 0; i < countRows; i++)
            {
                instances.add(GetInstanceForWeka(domains[i], labelsClass[i], numAttributes));

                progress.Invoke(new Action(() =>
                {
                    progress.Value = i;
                    status.Text = $"{i}/{countRows}";
                }));
            }

            return instances;
        }

        //Предобработка конкретного домена
        public static Instance GetInstanceForWeka(string domain, string labelClass, int numAttributes)
        {
            DenseInstance instance = new DenseInstance(numAttributes);

            //Выделение атрибутов instance.setValue(<индекс атрибута>, <значение>);
            //instance.setValue(0, domain.Length);

            InputLayerWrapperMulti wrapper = new InputLayerWrapperMulti();
            wrapper.ConfigureDomainAttribute(domain);
            double[] values = wrapper.valuesAttributes;

            for (int i = 0; i < values.Length; i++)
            {
                instance.setValue(i, values[i]);
            }

            instance.setValue(numAttributes - 1, Convert.ToDouble(labelClass)); //Метка класса

            return instance;
        }
    }
}
