/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetwork.Algebra
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public static class VectorOperations
    {
        public static void AssertDimensionsEqual(params Vector[] vectors)
        {
            var areDimensionsEqual
                = vectors
                .Select(g => g.Dimension)
                .Distinct()
                .Count() == 1;

            if (!areDimensionsEqual)
            {
                throw new NeuralNetworkException(Errors.VectorDimensionsNotEqual);
            }
        }

        public static void Fill(this Vector vector, double value)
        {
            for (var i = 0; i < vector.Dimension; i++) 
            {
                vector[i] = value;
            }
        }

        public static Vector Add(this Vector vector1, Vector vector2)
        {
            return Add(new Vector[] { vector1, vector2 });
        }

        public static Vector Add(params Vector[] vectors)
        {
            if(!vectors.Any())
            {
                throw new Exception();
            }

            AssertDimensionsEqual(vectors);

            var values = new double[vectors.First().Dimension];

            for (var i = 0; i < values.Length; i++) 
            {
                foreach (var vector in vectors)
                {
                    values[i] += vector[i];
                }
            }

            return new Vector(values);
        }

        public static Vector Add(this Vector vector, double add)
        {
            var values = new double[vector.Dimension];

            for (var i = 0; i < values.Length; i++) 
            {
                values[i] = vector[i] + add;
            }

            return new Vector(values);
        }

        public static Vector Subtract(this Vector vectorMinuend, Vector vectorSubtrahend)
        {
            AssertDimensionsEqual(vectorMinuend, vectorSubtrahend);

            var values = new double[vectorMinuend.Dimension];

            for (var i = 0; i < values.Length; i++) 
            {
                values[i] = vectorMinuend[i] - vectorSubtrahend[i];
            }

            return new Vector(values);
        }

        public static Vector Subtract(this Vector vector, double minus)
        {
            var values = new double[vector.Dimension];

            for (var i = 0; i < values.Length; i++) 
            {
                values[i] = vector[i] - minus;
            }

            return new Vector(values);
        }

        public static double MultiplyScalar(this Vector vector1, Vector vector2)
        {
            AssertDimensionsEqual(vector1, vector2);

            var scalar = 0.0;

            for (var i = 0; i < vector2.Dimension; i++) 
            {
                scalar += vector1[i] * vector2[i];
            }

            return scalar;
        }

        public static Vector Negate(this Vector vector)
        {
            var values = new double[vector.Dimension];

            for (var i = 0; i < values.Length; i++)
            {
                values[i] = -vector[i];
            }

            return new Vector(values);
        }

        public static Vector Normalize(this Vector vector, double extent)
        {
            var valuesNormalized = new double[vector.Dimension];

            if(valuesNormalized.Length != 0) 
            {
                var factor = vector.Select(v => v * v).Sum();

                if (factor != 0) 
                {
                    factor = Math.Sqrt(extent / factor);

                    for (var i = 0; i < vector.Values.Length; i++) 
                    {
                        valuesNormalized[i] = vector[i] * factor;
                    }
                }
            }

            return new Vector(valuesNormalized);
        }
        
        public static Vector Scale(this Vector vector, double scalar)
        {
            var values = new double[vector.Dimension];

            for (var i = 0; i < values.Length; i++) 
            {
                values[i] = vector[i] * scalar;
            }

            return new Vector(values);
        }
        
        public static double Limit(this double value, double limit = 1.0E40)
        {
            value = Math.Max(value, limit * -1);
            value = Math.Min(value, limit);

            return value;
        }

        public static double LimitDownTop(this double value, double limitDown = 0.0, double limitTop = 1.0)
        {
            value = Math.Max(limitDown, value);
            value = Math.Min(limitTop, value);

            return value;
        }

        private static readonly char[] separators 
            = new char[] { '|', ' ', '/' };

        public static double[] FromString(String valuesSequence, bool useCurrentCulture = true)
        {
            var cultureInfo 
                = useCurrentCulture 
                ? CultureInfo.CurrentUICulture 
                : CultureInfo.InvariantCulture;

            var values
                = valuesSequence
                .Replace(Environment.NewLine, String.Empty)
                .Split(separators, StringSplitOptions.RemoveEmptyEntries)
                .Select(v => Double.Parse(v, cultureInfo))
                .ToArray();

            return values;
        }

        public static String ToString(IEnumerable<double> values)
        {
            return ToString(values, -1, -1, -1);
        }

        public static String ToString(
            IEnumerable<double> values, 
            int valuesToTakeMax, 
            int precision, 
            int valuesLineBreak)
        {
            var stringBuilder = new StringBuilder();

            if (values != null)
            {
                var valueFormat 
                    = (precision < 0 
                    ? "{0}" 
                    : "{0:0." + new String('0', precision) + "}") 
                    + separators[0];

                var valueSubSet = (valuesToTakeMax >= 0) 
                    ? values.Take(valuesToTakeMax) : values;

                var index = 0;
                foreach (var value in valueSubSet)
                {
                    stringBuilder.AppendFormat(CultureInfo.InvariantCulture, valueFormat, value);

                    if(valuesLineBreak > 0)
                    {
                        index++;
                        if(valuesLineBreak == index)
                        {
                            stringBuilder.AppendLine();
                            index = 0;
                        }
                    }
                }

                if (valuesToTakeMax >= 0)
                {
                    stringBuilder.Append("...");
                }
            }

            return stringBuilder.ToString();
        }
    }
}
