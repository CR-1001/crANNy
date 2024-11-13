/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetwork
{
    using System;
    using CRAI.NeuralNetwork.Activation;
    using CRAI.NeuralNetwork.Algebra;
    using CRAI.NeuralNetwork.Randomizer;

    public interface ITier
    {
        int CountNeurons { get; }

        void Setup(
            IRandomizer randomizer, 
            bool isRandomFill, 
            IActivation activationFunction, 
            int countNeurons);

        bool IsRandomFill { get; set; }

        IRandomizer Randomizer { get; set; }

        IActivation Activation { get; set; }

        void DetachNeuron(int neuron);

        void DetermineOutputTierNext();

        double[] ValuesOut { get; set; }

        bool IsIn { get; }

        bool IsHidden { get; }

        bool IsOut { get; }

        Matrix MatrixWeightsThresholds { get; set; }

        Vector VectorBias { get; set; }

        ITier TierNext { get; set; }

        ITier TierPrevious { get; set; }

        String ToString(int precision, int rowsMax, int columnsMax);

        String ToStringComplete();

    }
}
