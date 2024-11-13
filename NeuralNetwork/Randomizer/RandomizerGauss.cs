/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetwork.Randomizer
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using CRAI.Common;

    [Serializable] [DataContract] public class RandomizerGauss : IRandomizer
    {
        private Random _Random;
        private double _Delta;
        private double _MinValue;

        public double Sigma { get; private set; }

        public double Peak { get; private set; }

        public double Mean { get; private set; }

        public void Setup(
            double minValue, 
            double maxValue, 
            IDictionary<String, Object> parameters)
        {
            _Random = new Random();
            _Delta = maxValue - minValue;
            _MinValue = minValue;

            Sigma = parameters.TryGetValueFallback("Sigma", 1.0);
            Peak = parameters.TryGetValueFallback("Peak", _MinValue + _Delta / 2);

            if(Peak < _MinValue || Peak > maxValue)
            {
                throw new NeuralNetworkException(Errors.SetupInvalid);
            }

            Mean = Peak / _Delta;
        }

        public double Next()
        {
            var random = 0.0;
            do
            {
                random = Math.Abs(GetNextGaussian());
            }
            while(random < 0 || random > 1);

            return (_Delta * random) + _MinValue;
        }

        private double GetNextGaussian()
        {
            double value1 = 1 - _Random.NextDouble();
            double value2 = 1 - _Random.NextDouble();

            double y1 
                = Math.Sqrt(-2.0 * Math.Log(value1)) 
                * Math.Cos(2.0 * Math.PI * value2);

            return y1 * Sigma + Mean;
        }
    }
}
