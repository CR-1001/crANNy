/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetwork.Learning
{
    using CRAI.NeuralNetwork;

    public interface ILearningSubject
    {
        Stimulus Input { get; }

        Stimulus Output { get; }
    }
}
