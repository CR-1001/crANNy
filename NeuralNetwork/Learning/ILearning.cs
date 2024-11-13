/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetwork.Learning
{
    using System;
    using System.Collections.Generic;
    using CRAI.NeuralNetwork.Error;

    public interface ILearning
    {
        void Setup(
            INeuralNetwork NeuralNetwork, 
            IErrorEstimation errorEstimation, 
            ILearningParameter stepsMaximum, 
            ILearningParameter errorMaximum,
            ILearningParameter learningRate, 
            ILearningParameter momentum,
            IDictionary<String, Object> parametersAdditional);

        IDictionary<String, Object> Parameters { get; set; }

        uint Steps { get; }

        ILearningParameter StepsMaximum { get; set; }

        TimeSpan Duration { get; }

        DateTime? Started { get; }

        TimeSpan DurationMaximum { get; set; }

        IErrorEstimation ErrorEstimation { get; }

        double Error { get; }

        ILearningParameter ErrorMaximum { get; set; }

        LearningResult Learn(LearningSession learningSession);

        LearningResult Interrupt();
    }
}
