/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetwork.Randomizer
{
    using System;
    using System.Collections.Generic;

    public interface IRandomizer
    {

        void Setup(
            double minValue, 
            double maxValue, 
            IDictionary<String, Object> parameters);

        double Next();
    }
}
