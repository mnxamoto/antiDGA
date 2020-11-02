using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace антиDGA
{
    class Gram
    {
        public string text;
        public int legitimate;
        public int noLegitimate;
        public double weight;
    }

    public class Ngram
    {
        //Синглтон
        private static Ngram instance;

        private Ngram()
        { }

        public static Ngram getInstance()
        {
            if (instance == null)
                instance = new Ngram();
            return instance;
        }

        private Dictionary<int, List<Gram>> dictionarys = new Dictionary<int, List<Gram>>();

        public double CalculateFrequencyNgram(string domain, int lengthGram)
        {
            List<Gram> grams;

            //Получения словаря грамм
            if (dictionarys.ContainsKey(lengthGram))
            {
                grams = dictionarys[lengthGram];
            }
            else
            {
                grams = new List<Gram>();

                string[] rows = File.ReadAllLines($"{Directory.GetCurrentDirectory()}\\Слоаврь Ngram - {lengthGram}.csv");

                for (int i = 0; i < rows.Length; i++)
                {
                    string[] row = rows[i].Split(';');

                    Gram gram = new Gram();
                    gram.text = row[0];
                    gram.weight = Convert.ToDouble(row[1]);

                    grams.Add(gram);
                }

                dictionarys.Add(lengthGram, grams);

            }

            //Вычисление частоты
            double frequency = 0;

            int lengthDomain = domain.Length;

            for (int k = 0; k < lengthDomain - lengthGram; k++)
            {
                string text = domain.Substring(k, lengthGram);
                Gram gram = grams.Find(g => g.text == text);

                if (gram == null)
                {
                    continue;
                }
                else
                {
                    frequency += gram.weight;
                }
            }

            return frequency;
        }

        public void CreateDictionary(string[] domains, string[] labelsClass, int lengthGram)
        {
           List<Gram> grams = new List<Gram>();

            int countDomains = domains.Length;

            //Выделение грамм
            for (int i = 0; i < countDomains; i++)
            {
                string domain = domains[i].Substring(0, domains[i].LastIndexOf('.'));
                int lengthDomain = domain.Length;

                for (int k = 0; k < lengthDomain - lengthGram; k++)
                {
                    string text = domain.Substring(k, lengthGram);
                    Gram gram = grams.Find(g => g.text == text);

                    if (gram == null)
                    {
                        gram = new Gram();
                        gram.text = text;
                        grams.Add(gram);
                    }
                    else 
                    {

                    }

                    if (labelsClass[i] == "0")
                    {
                        gram.legitimate++;
                    }
                    else
                    {
                        gram.noLegitimate++;
                    }
                }
            }

            //Вычисление весов грамм
            for (int i = 0; i < grams.Count; i++)
            {
                Gram gram = grams[i];
                int legitimate = gram.legitimate;
                int noLegitimate = gram.noLegitimate;

                if ((legitimate > 0) && (noLegitimate == 0))
                {
                    gram.weight = -1;
                    continue;
                }

                if ((legitimate == 0) && (noLegitimate > 0))
                {
                    gram.weight = 1;
                    continue;
                }

                gram.weight = (noLegitimate / (double)(legitimate + noLegitimate)) * 2 - 1;
            }

            //Сохранение словаря
            string dictionary = "";

            foreach (Gram gram in grams)
            {
                dictionary += $"{gram.text};{gram.weight}\r\n";
            }

            File.WriteAllText($"{Directory.GetCurrentDirectory()}\\Слоаврь Ngram - {lengthGram}.csv", dictionary);
        }
    }
}
