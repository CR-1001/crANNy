/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using CRAI.NeuralNetwork;
    using CRAI.NeuralNetwork.Learning;
    using CRAI.NeuralNetworkHost.Configurations;

    public interface IStateDetailsFormatter
    {
        void Format(State state, LearningSession learningSession, LearningResult learningResult);

        void Format(State state, Stimulus stimulusInput, Stimulus stimulusOutput);

        void Format(State state, byte[] rawDataInput, StimulusSet stimulusSetInput, ConvertSettings convertSettings);
    }
}
