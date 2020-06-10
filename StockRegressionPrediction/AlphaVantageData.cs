using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockRegressionPrediction
{
    public class AlphaVantageData
    {
        [LoadColumn(0)]
        public DateTime Timestamp { get; set; }
        [LoadColumn(1)]
        public Single Open { get; set; }
        [LoadColumn(2)]
        public Single High { get; set; }
        [LoadColumn(3)]
        public Single Low { get; set; }
        [LoadColumn(4)]
        public Single Close { get; set; }
        [LoadColumn(5)]
        public Single Volume { get; set; }
    }
    public class AlphaVantagePrediction
    {
        [ColumnName("Score")]
        public Single Close;

    }
}
