/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetwork
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using CRAI.NeuralNetwork.Activation;
    using CRAI.NeuralNetwork.Algebra;
    using CRAI.NeuralNetwork.Error;
    using CRAI.NeuralNetwork.Randomizer;

    public interface INeuralNetwork
    {
        int CountTiers { get; }

        int CountNeurons { get; }

        ITier TierIn { get; }

        ITier TierOut { get; }

        Stimulus DetermineOutput(Stimulus input);

        ObservableCollection<ITier> Tiers { get; }

        void Setup(
            IEnumerable<IRandomizer> randomizers, 
            bool isRandomFill, 
            IEnumerable<IActivation> activations, 
            int[] tiers);


        String ToString(int precision, int rowsMax, int columnsMax);

        String ToStringComplete();
    }
}
