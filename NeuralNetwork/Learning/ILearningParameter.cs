/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetwork.Learning
{
    public interface ILearningParameter
    {
        double Determine(ILearning learning, ITier Tier);

        double Determine(ILearning learning);
    }
}
