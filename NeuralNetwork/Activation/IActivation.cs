/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetwork.Activation
{
    using System;

    public interface IActivation
    {
        double Calculate(double x);

        double CalculateDerivate(double x);
    }
}
