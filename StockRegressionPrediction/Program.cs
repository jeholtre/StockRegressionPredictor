using Microsoft.ML;
using ServiceStack;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Microsoft.ML.DataOperationsCatalog;

namespace StockRegressionPrediction
{
    class Program
    {
        public const string APIKEY = "PGN40K5YZ8S4IW3M";
        public const double TESTFRACTION = 0.9;
        public static readonly string priceDataPath = @"..\..\..\Data\data.csv";
        
        public static TrainTestData trainTestData; 
        public static IDataView trainDataView;
        public static IDataView testDataView;

        static void Main(string[] args)
        {


            MLContext mlContext = new MLContext();
            IDataView dataView = mlContext.Data.LoadFromTextFile<AlphaVantageData>(priceDataPath, hasHeader: true, separatorChar: ',');
            createTestTrainDataViews(mlContext, dataView, TESTFRACTION);
            var model = Train(mlContext);
            Evaluate(mlContext, model);
            //TestSinglePrediction(mlContext, model);
            //symbol, function, size
            //var dailyPrices = getPrices("GOOGL", "TIME_SERIES_DAILY", "full");
            //createCSVFile(filePath, dailyPrices);
            //var maxPrice = dailyPrices.Max(u => u.Close);
            //var min = dailyPrices.Min(u => u.Close);

        }


        public static ITransformer Train(MLContext mlContext)
        {
            var pipeline = mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: "Close")
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "TimestampEncoded", inputColumnName: "Timestamp"))
                .Append(mlContext.Transforms.Concatenate("Features", "TimestampEncoded", "Open", "High", "Low", "Volume"))
                .Append(mlContext.Regression.Trainers.FastTree());
            var model = pipeline.Fit(trainDataView);
            return model;
            
        }

        private static void Evaluate(MLContext mlContext, ITransformer model)
        {
            var predictions = model.Transform(testDataView);
            var metrics = mlContext.Regression.Evaluate(predictions, "Label", "Score");

            Console.WriteLine();
            Console.WriteLine($"*************************************************");
            Console.WriteLine($"*       Model quality metrics evaluation         ");
            Console.WriteLine($"*------------------------------------------------");


            Console.WriteLine($"*       RSquared Score:      {metrics.RSquared:0.##}");
            Console.WriteLine($"*       Root Mean Squared Error:      {metrics.RootMeanSquaredError:#.##}");
            Console.ReadLine();
        }

        private static void TestSinglePrediction(MLContext mlContext, ITransformer model)
        {
            var predictionFunction = mlContext.Model.CreatePredictionEngine<AlphaVantageData, AlphaVantagePrediction>(model);
        }

        public static void createTestTrainDataViews(MLContext mlContext, IDataView dataView, double testFraction)
        {
            trainTestData = mlContext.Data.TrainTestSplit(dataView, testFraction: testFraction);
            trainDataView = trainTestData.TrainSet;
            testDataView = trainTestData.TestSet;

    }

        public static void createCSVFile(string filePath, List<AlphaVantageData> priceData)
        {
            File.WriteAllText(filePath, priceData.ToCsv());
        }
        public static List<AlphaVantageData> getPrices(string symbol, string function, string size)
        {
            var prices = $"https://www.alphavantage.co/query?function={function}&symbol={symbol}&apikey={APIKEY}&datatype=csv&outputsize={size}"
                .GetStringFromUrl().FromCsv<List<AlphaVantageData>>();
            return prices;
        }
    }
}
