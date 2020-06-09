using ServiceStack;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StockRegressionPrediction
{
    public class AlphaVantageData
    {
        public DateTime Timestamp { get; set; }
        public decimal Open { get; set; }

        public decimal High { get; set; }
        public decimal Low { get; set; }

        public decimal Close { get; set; }
        public decimal Volume { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var symbol = "MSFT";
            var apiKey = "PGN40K5YZ8S4IW3M";
            string filePath = @"..\..\..\Data\data.txt";

            var monthlyPrices = $"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={symbol}&apikey={apiKey}&datatype=csv&outputsize=full"
                .GetStringFromUrl().FromCsv<List<AlphaVantageData>>();
            monthlyPrices.PrintDump();
            File.WriteAllText(filePath, monthlyPrices.ToCsv());

            var maxPrice = monthlyPrices.Max(u => u.Close);
            var min = monthlyPrices.Min(u => u.Close);
        }
    }
}
