/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetwork.Randomizer
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [Serializable] [DataContract] public class Randomizer : IRandomizer
    {
        private Random _Random;
        private double _Delta;
        private double _MinValue;

        public void Setup(double minValue, double maxValue, IDictionary<String, Object> parameters)
        {
            _Random = new Random();
            _Delta = maxValue - minValue;
            _MinValue = minValue;
        }

        public double Next()
        {
            return (_Delta * _Random.NextDouble()) + _MinValue;
        }
    }
}
