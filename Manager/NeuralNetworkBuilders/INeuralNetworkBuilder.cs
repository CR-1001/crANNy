/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRAI.NeuralNetwork;
using CRAI.NeuralNetwork.Activation;
using CRAI.NeuralNetwork.Algebra;
using CRAI.NeuralNetwork.Randomizer;

namespace CRAI.NeuralNetworkHost.NeuralNetworkBuilders
{
    public interface INeuralNetworkBuilder
    {
        void SetupRandomizers(IEnumerable<IRandomizer> randomizer);
        void SetupActivations(IEnumerable<IActivation> activation);
        void SetupTiers(int[] tiers);
        void SetupMatrixWeightsThresholds(int tier, Matrix observableCollection);
        void SetupInstance();
        void SetupIsFillRandom(bool isFillRandom);
        void SetupParameters(IDictionary<String, Object> parameters);

        INeuralNetwork GetNeuralNetwork();
    }

    public interface INeuralNetworkBuilder<T> : INeuralNetworkBuilder
        where T : INeuralNetwork
    {
    
    }
}
