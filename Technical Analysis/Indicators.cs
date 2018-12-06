using System.Linq;

namespace Technical_Analysis
{
    public static class Indicators
    {
        //вспомогательный метод
        private static double[] movingAverageCalculate(double[] close, int closeLength)
        {
            double[] result = new double[closeLength];

            for (int i = 0; i < closeLength; i++)
            {
                if (i < closeLength) result[i] = close[i];
                else
                {
                    for (int j = i - closeLength; j <= i; j++)
                    {
                        result[i] += close[j];
                    }

                    result[i] /= closeLength;
                }
            }

            return result;
        }

        internal static double[] SMA(double[] close)
        {
            int closeLength = close.Length;
            return movingAverageCalculate(close, closeLength);
        }

        internal static double[] EMA(double[] close, int intervalValue)
        {
            int closeLength = close.Length;
            double alpha = 2 / ((double)intervalValue + 1);
            double[] result = new double[closeLength];
            for (int i = 0; i < closeLength; i++)
            {
                if (i < intervalValue) result[i] = close[i];
                else if (i == intervalValue) result[i] = result.Take(intervalValue).Sum()/intervalValue;
                else
                {
                    result[i] = alpha * close[i - intervalValue]
                                + (1 - alpha) * result[i - 1];
                }
            }

            return result;
        }


        internal static double[] OBV(double[] close, double[] volume)
        {
            int closeLength = close.Length;
            double[] result = new double[closeLength];

            result[0] = 0;
            for (int i = 1; i < closeLength; i++)
            {
                if (close[i] > close[i - 1]) result[i] = result[i-1] + volume[i];
                else if (close[i].Equals(close[i - 1])) result[i] = result[i - 1] + 0;
                else result[i] = result[i - 1] - volume[i];
            }

            return result;
        }

        internal static double[] MACD(double[] close)
        {
            double[] result = new double[close.Length];
            double[] ema12 = EMA(close, 12);
            double[] ema26 = EMA(close, 26);
            for (int i = 0; i < close.Length; i++)
            {
                result[i] = ema12[i] - ema26[i];
            }
            return result;
        }

        internal static double[] RSI(double[] close)
        {
            int closeLength = close.Length;
            double[] U = new double[closeLength];
            double[] D = new double[closeLength];
            double[] result = new double[closeLength];
            double[] RS = new double[closeLength];
            const int N = 15;
            U[0] = close[0];
            D[0] = close[0];
            for (int i = 1; i < closeLength; i++)
            {
                U[i] += U[i - 1];
                D[i] += D[i - 1];
                if (close[i] > close[i - 1]) U[i] += close[i];
                else D[i] += close[i];
            }

            for (int i = N; i < closeLength; ++i)
            {
                result[i] = 100 - (100 / (1 + (U[i] - U[i - N]) / (D[i] - D[i - N])));
            }

            return result;
        }

        //internal static double NRMSE(double[] f, double[] g)
        //{
        //    double sqrDiff = 0;
        //    for (int i = 0; i < f.Length; i++)
        //    {
        //        sqrDiff += Math.Pow((f[i] - g[i]), 2);
        //    }

        //    return Math.Pow(sqrDiff, 2)/(f.Max() - f.Min());
        //}

    }
}
