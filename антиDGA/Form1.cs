using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using weka.classifiers;
using weka.classifiers.trees;
using weka.core;
using weka.core.converters;
using weka.filters;
using weka.filters.supervised.instance;
using антиDGA.Helpers;
using антиDGA.Pretreatment;

namespace антиDGA
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Instances dataSet;

        Instances trainDataSet;
        Instances testDataSet;

        public struct InfaForExperimentClassification
        {
            public Instances trainDataSet;
            public Instances testDataSet;
            public string fileNameModel;
        }

        public struct ResultExperimentClassification
        {
            public Evaluation evaluation;
            public Stopwatch timeTrain;
            public Stopwatch timeTest;
            public Stopwatch timeСlassification;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {

            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string[] rows = File.ReadAllLines(openFileDialog1.FileName);

                int countRows = rows.Length;

                string[] domains = new string[countRows];
                string[] labelsClass = new string[countRows];

                progressBar1.Maximum = countRows;

                await Task.Factory.StartNew(() =>
                {
                    for (int i = 0; i < countRows; i++)
                    {
                        string[] row = rows[i].Split('\t');

                        domains[i] = row[0];
                        labelsClass[i] = row[1];

                        progressBar1.Invoke(new Action(() =>
                        {
                            progressBar1.Value = i;
                            labelStatus.Text = $"{i}/{countRows}";
                        }));
                    }

                    dataSet = WekaHelper.GetInstancesForWeka(domains, labelsClass, labelStatus, progressBar1);
                });

                labelStatus.Text = "Данные обработались";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dataSet = ReadArff();
        }

        public ResultExperimentClassification ExperimentClassification(object a)
        {
            InfaForExperimentClassification infa = (InfaForExperimentClassification)a;

            ResultExperimentClassification result = new ResultExperimentClassification();
            result.timeTrain = new Stopwatch();
            result.timeTest = new Stopwatch();
            result.timeСlassification = new Stopwatch();

            RandomForest RF = new RandomForest();

            RF.setNumIterations(100); //Количество деревьев

            result.timeTrain.Start();

            RF.buildClassifier(infa.trainDataSet); //Обучние

            result.timeTrain.Stop();
            result.timeTest.Start();

            result.evaluation = evaluateModel(RF, infa.trainDataSet, infa.testDataSet); //Тестирование

            result.timeTest.Stop();
            result.timeСlassification.Start();

            RF.classifyInstance(infa.testDataSet.instance(0)); //Замер времени классификации 1 экземпляра

            result.timeСlassification.Stop();

            return result;
        }

        public RandomForest TrainClassification(object a)
        {
            InfaForExperimentClassification infa = (InfaForExperimentClassification)a;

            RandomForest RF = new RandomForest();
            RF.setNumIterations(100); //Количество деревьев
            RF.buildClassifier(infa.trainDataSet); //Обучние

            SerializationHelper.write(infa.fileNameModel, RF); //Сохранение модели

            return RF;
        }

        public ResultExperimentClassification TestClassification(object a)
        {
            InfaForExperimentClassification infa = (InfaForExperimentClassification)a;

            ResultExperimentClassification result = new ResultExperimentClassification();
            result.timeTrain = new Stopwatch();
            result.timeTest = new Stopwatch();
            result.timeСlassification = new Stopwatch();

            RandomForest RF = (RandomForest)SerializationHelper.read(infa.fileNameModel);  //Загрузка модели
            //RandomForest RF = new RandomForest();

            result.timeTest.Start();

            result.evaluation = evaluateModel(RF, infa.trainDataSet, infa.testDataSet); //Тестирование

            result.timeTest.Stop();
            result.timeСlassification.Start();

            RF.classifyInstance(infa.testDataSet.instance(0)); //Замер времени классификации 1 экземпляра

            result.timeСlassification.Stop();

            return result;
        }

        public Evaluation evaluateModel(Classifier model, Instances trainDataSet, Instances testDataSet)
        {
            Evaluation eval = null;
            try
            {
                //Evaluate classifier with test dataset
                eval = new Evaluation(trainDataSet);
                eval.evaluateModel(model, testDataSet);
            }
            catch (Exception ex)
            {
                textBoxLog.Text += ex + "\r\n";
            }

            return eval;
        }


        private void button4_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "arff файлы (*.arff)|*.arff|Все файлы (*.*)|*.*";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ArffSaver arffSaver = new ArffSaver();
                arffSaver.setInstances(dataSet);
                arffSaver.setFile(new java.io.File(saveFileDialog1.FileName));
                arffSaver.writeBatch();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private async void button6_Click(object sender, EventArgs e)
        {
            InfaForExperimentClassification infa = new InfaForExperimentClassification(); //Инфа для второго потока

            Instances experimentdataSet = dataSet;

            experimentdataSet.setClassIndex(experimentdataSet.numAttributes() - 1); //Установка указателя на атрибут с классами

            java.util.Random rnd = new java.util.Random();
            experimentdataSet.randomize(rnd); //Перемешивание исходного набора данных

            int trainSize = (int)Math.Round((double)(experimentdataSet.numInstances() * 0.66));  //0.66 - Процент даннхы на обучение
            int testSize = experimentdataSet.numInstances() - trainSize;

            infa.trainDataSet = new Instances(experimentdataSet, 0, trainSize);
            infa.testDataSet = new Instances(experimentdataSet, trainSize, testSize);

            labelStatus.Text = "Проводится эксперимент...";

            ResultExperimentClassification result = await Task.Factory.StartNew(() => ExperimentClassification(infa));

            Evaluation evaluation = result.evaluation;

            string[] row = new string[10];
            row[0] = evaluation.weightedPrecision().ToString(); //Точность
            row[1] = evaluation.weightedRecall().ToString(); //Полнота
            row[2] = evaluation.weightedFMeasure().ToString(); //F-мера //Accuracy
            row[3] = (evaluation.pctCorrect() / 100).ToString(); //Достоверноесть
            row[4] = evaluation.weightedTruePositiveRate().ToString(); //TPR
            row[5] = evaluation.weightedFalsePositiveRate().ToString(); //FPR
            row[6] = evaluation.weightedAreaUnderROC().ToString(); //ROC

            row[7] = result.timeTrain.ElapsedTicks.ToString(); //Время обучения
            row[8] = result.timeTest.ElapsedTicks.ToString(); //Время тестирования
            row[9] = result.timeСlassification.ElapsedTicks.ToString(); //Время классификации 1 экзепляра

            dataGridView5.Rows.Add(row);

            labelStatus.Text = "Эксперимент завершён";
        }

        private async void button5_Click_1(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string[] rows = File.ReadAllLines(openFileDialog1.FileName);

                int countRows = rows.Length;

                string[] domains = new string[countRows];
                string[] labelsClass = new string[countRows];

                for (int i = 0; i < countRows; i++)
                {
                    string[] row = rows[i].Split('\t');

                    domains[i] = row[0];
                    labelsClass[i] = row[1];
                }

                Ngram ngram = Ngram.getInstance();

                progressBar1.Maximum = countRows;

                await Task.Factory.StartNew(() =>
                {
                    for (int i = 4; i < 6; i++)
                    {
                        ngram.CreateDictionary(domains, labelsClass, i, labelStatus, progressBar1);
                    }
                });
            }
        }

        private async void button7_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "model файлы (*.model)|*.model|Все файлы (*.*)|*.*";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                InfaForExperimentClassification infa = new InfaForExperimentClassification(); //Инфа для второго потока

                trainDataSet.setClassIndex(trainDataSet.numAttributes() - 1); //Установка указателя на атрибут с классами

                infa.trainDataSet = trainDataSet;
                infa.fileNameModel = saveFileDialog1.FileName;

                labelStatus.Text = "Проводится обучение...";

                RandomForest RF = await Task.Factory.StartNew(() => TrainClassification(infa));

                labelStatus.Text = "Обучение прошло успешно.";
            }
        }

        private async void button8_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "model файлы (*.model)|*.model|Все файлы (*.*)|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                InfaForExperimentClassification infa = new InfaForExperimentClassification(); //Инфа для второго потока

                trainDataSet.setClassIndex(trainDataSet.numAttributes() - 1); //Установка указателя на атрибут с классами
                testDataSet.setClassIndex(testDataSet.numAttributes() - 1);

                infa.trainDataSet = trainDataSet;
                infa.testDataSet = testDataSet;
                infa.fileNameModel = openFileDialog1.FileName;

                labelStatus.Text = "Проводится тестирование...";

                ResultExperimentClassification result = await Task.Factory.StartNew(() => TestClassification(infa));

                Evaluation evaluation = result.evaluation;

                string[] row = new string[10];
                row[0] = evaluation.weightedPrecision().ToString(); //Точность
                row[1] = evaluation.weightedRecall().ToString(); //Полнота
                row[2] = evaluation.weightedFMeasure().ToString(); //F-мера //Accuracy
                row[3] = (evaluation.pctCorrect() / 100).ToString(); //Достоверноесть
                row[4] = evaluation.weightedTruePositiveRate().ToString(); //TPR
                row[5] = evaluation.weightedFalsePositiveRate().ToString(); //FPR
                row[6] = evaluation.weightedAreaUnderROC().ToString(); //ROC

                row[7] = result.timeTrain.ElapsedTicks.ToString(); //Время обучения
                row[8] = result.timeTest.ElapsedTicks.ToString(); //Время тестирования
                row[9] = result.timeСlassification.ElapsedTicks.ToString(); //Время классификации 1 экзепляра

                dataGridView5.Rows.Add(row);

                labelStatus.Text = "Тестирование прошло успешно.";
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            trainDataSet = ReadArff();
        }

        private Instances ReadArff()
        {
            openFileDialog1.Filter = "arff файлы (*.arff)|*.arff|Все файлы (*.*)|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ArffLoader arffReader = new ArffLoader();

                java.io.File arffFile = new java.io.File(openFileDialog1.FileName);
                arffReader.setFile(arffFile);

                return arffReader.getDataSet();
            }
            else
            {
                return null;
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            testDataSet = ReadArff();
        }
    }
}
