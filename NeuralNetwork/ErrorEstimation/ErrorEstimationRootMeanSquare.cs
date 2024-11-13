/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetwork.Error
{
    using System;
    using System.Runtime.Serialization;

    [Serializable] [DataContract] public class ErrorEstimationRootMeanSquare : IErrorEstimation
    {
        private INeuralNetwork _NeuralNetwork;

        public void Setup(INeuralNetwork NeuralNetwork)
        {
            _NeuralNetwork = NeuralNetwork;
        }

        public double DetermineError(StimulusSet input, StimulusSet output)
        {
            if (_NeuralNetwork == null)
            {
                throw new NeuralNetworkException(Errors.ErrorCalculationNotSetUp);
            }

            var sum = 0.0;

            var countElements
                = output.Values.Length
                * _NeuralNetwork.TierOut.CountNeurons;

            for (var i = 0; i < output.Values.Length; i++)
            {
                _NeuralNetwork.DetermineOutput(input[i]);

                for (var j = 0; j < _NeuralNetwork.TierOut.CountNeurons; j++)
                {
                    var add
                        = output[i][j]
                        - _NeuralNetwork.TierOut.ValuesOut[j];

                    sum += Math.Pow(add, 2.0);
                }
            }

            var result = countElements == 0 ? 0 : Math.Sqrt(sum / countElements);

            return result;
        }
    }
}
