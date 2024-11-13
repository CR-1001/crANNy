/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetworkHost
{
    using System;
    using System.Collections.Generic;
    using CRAI.NeuralNetwork;
    using CRAI.NeuralNetworkHost.Configurations;

    public interface IConverter
    {
        void Setup(IDictionary<String, Object> parameters);

        StimulusSet Convert(byte[] rawData, ConvertSettings convertSettings);

        byte[] Convert(StimulusSet stimulusSet, ConvertSettings convertSettings);

        IEnumerable<KeyValuePair<String, Object>> ExtractParameters();
    }


}
