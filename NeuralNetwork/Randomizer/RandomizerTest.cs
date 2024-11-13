/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetwork.Randomizer
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [Serializable] [DataContract] public class RandomizerTest : IRandomizer
    {
        private static readonly double[] _Values = 
        { 
            0.10, 0.23, 0.06, 0.17, 0.09, 0.57, 0.59, 0.35, 0.58, 0.29,
            0.07, 0.17, 0.89, 0.42, 0.80, 0.49, 0.68, 0.08, 0.49, 0.74,
            0.91, 0.32, 0.95, 0.03, 0.37, 0.97, 0.74, 0.13, 0.47, 0.30,
            0.61, 0.99, 0.20, 0.45, 0.83, 0.15, 0.87, 0.74, 0.45, 0.64,
            0.72, 0.83, 0.29, 0.28, 0.74, 0.98, 0.17, 0.36, 0.33, 0.47,
            0.32, 0.21, 0.80, 0.88, 0.68, 0.14, 0.45, 0.23, 0.07, 0.19,
            0.29, 0.46, 0.77, 0.84, 0.52, 0.09, 0.68, 0.82, 0.35, 0.72,
            0.31, 0.58, 0.00, 0.51, 0.14, 0.32, 0.78, 0.97, 0.19, 0.35,
            0.02, 0.99, 0.20, 0.65, 0.53, 0.01, 0.35, 0.26, 0.31, 0.55,
            0.53, 1.00, 0.42, 0.24, 0.85, 0.61, 0.09, 0.78, 0.62, 0.31,
            0.95, 0.57, 0.10, 0.22, 0.49, 0.51, 0.97, 0.97, 0.72, 0.39,
            0.67, 0.70, 0.97, 0.35, 0.57, 0.90, 0.04, 0.45, 0.78, 0.49,
            0.81, 0.13, 0.82, 0.83, 0.66, 0.59, 0.96, 0.78, 0.51, 0.99,
            0.86, 0.85, 0.95, 0.57, 0.46, 0.15, 0.02, 0.70, 0.59, 0.10,
            0.47, 0.56, 0.22, 0.24, 0.21, 0.85, 0.93, 0.59, 0.97, 0.62,
            0.14, 0.76, 0.29, 0.19, 0.84, 0.02, 0.89, 0.17, 0.06, 0.77,
            0.25, 0.24, 0.45, 0.65, 0.69, 0.46, 0.26, 0.04, 0.81, 0.63,
            0.05, 0.58, 0.86, 0.19, 0.15, 0.92, 0.45, 0.84, 0.10, 0.27,
            0.41, 0.40, 0.85, 0.42, 0.65, 0.43, 0.80, 0.70, 1.00, 0.11,
            0.69, 0.39, 0.66, 0.82, 0.07, 0.25, 0.49, 0.41, 0.99, 0.34,
        };

        private double _MinValue;
        private double _MaxValue;
        private double _Delta;
        private int _Index;

        public void Setup(double minValue, double maxValue, IDictionary<String, Object> parameters)
        {
            _MinValue = minValue;
            _MaxValue = maxValue;
            _Delta = maxValue - minValue;
            _Index = 0;
        }

        public double Next()
        {
            var value = _Values[_Index++];

            if (_Index >= _Values.Length)
            {
                _Index = 0;
            }

            return (_Delta * value) + _MinValue;
        }
    }

}
