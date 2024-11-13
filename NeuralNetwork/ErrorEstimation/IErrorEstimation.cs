/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetwork.Error
{
    using System;

    public interface IErrorEstimation
    {
        void Setup(INeuralNetwork neuronalNet);

        double DetermineError(StimulusSet input, StimulusSet output);
    }
}
