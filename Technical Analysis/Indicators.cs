using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technical_Analysis
{
    public static class Indicators
    {
        const int INTERVAL_COUNT = 5;
        //вспомогательный метод
        private static double[] movingAverageCalculate(double[] close, int closeLength)
        {
            double[] result = new double[closeLength];

            for (int i = 0; i < closeLength; i++)
            {
                if (i < INTERVAL_COUNT) result[i] = close[i];
                else
                {
                    for (int j = i - INTERVAL_COUNT; j <= i; j++)
                    {
                        result[i] += close[j];
                    }

                    result[i] /= INTERVAL_COUNT;
                }
            }

            return result;
        }

        static double[] SMA(double[] close)
        {
            int closeLength = close.Length;
            return movingAverageCalculate(close, closeLength);
        }

        static double[] EMA(double[] close, int intervalValue)
        {
            int closeLength = close.Length;
            double alpha = 2 / (closeLength + 1);
            double[] result = new double[closeLength];
            for (int i = 0; i < closeLength; i++)
            {
                if (i <= intervalValue) result = movingAverageCalculate(close, closeLength);
                else
                {
                    result[i] = alpha * close[i - intervalValue]
                                + (1 - alpha) * result[i - 1];
                }
            }

            return result;
        }


        static double[] OBV(double[] close, double[] volume)
        {
            int closeLength = close.Length;
            double[] result = new double[closeLength];

            result[0] = 0;
            for (int i = 1; i < closeLength; i++)
            {
                if (close[i] > close[i - 1]) result[i] = result[i-1] + volume[i];
                else if (close[i] == close[i - 1]) result[i] = result[i - 1] + 0;
                else result[i] = result[i - 1] - volume[i];
            }

            return result;
        }

        static double[] MACD(double[] close)
        {
            return EMA(close, 12).Except(EMA(close, 26)).ToArray();
        }

        static double[] RSI(double[] close)
        {
            int closeLength = close.Length;
            double[] U = new double[closeLength];
            double[] D = new double[closeLength];
            double[] result = new double[closeLength];

            U[0] = D[0] = close[0];
            for (int i = 1; i < closeLength; i++)
            {
                if (close[i] > close[i - 1]) U[i] += close[i];
                else D[i] += close[i];
            }

            for (int i = 0; i < closeLength; i++)
            {
                result[i] = 100 - (100 / (1 + U[i] / D[i]));
            }

            return result;
        }

    }
}
