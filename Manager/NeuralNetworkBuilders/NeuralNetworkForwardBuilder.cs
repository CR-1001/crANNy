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
    [Serializable] public class NeuralNetworkForwardBuilder : INeuralNetworkBuilder<NeuralNetworkForward>
    {
        private int[] _Tiers;
        private IEnumerable<IActivation> _Activations;
        private IEnumerable<IRandomizer> _Randomizers;
        private INeuralNetwork _NeuralNetwork;
        private bool _IsFillRandom;


        public void SetupRandomizers(IEnumerable<IRandomizer> randomizers)
        {
            _Randomizers = randomizers;
        }

        public void SetupActivations(IEnumerable<IActivation> activations)
        {
            _Activations = activations;
        }

        public void SetupTiers(int[] tiers)
        {
            _Tiers = tiers;
        }
        
        public void SetupIsFillRandom(bool isFillRandom)
        {
            _IsFillRandom = isFillRandom;
        }

        public void SetupMatrixWeightsThresholds(int tier, Matrix matrix)
        {
            var TierForward 
                = (TierForward)_NeuralNetwork.Tiers[tier];

            TierForward.MatrixWeightsThresholds = matrix;

            if(_IsFillRandom)
            {
                TierForward.GenerateWeightsTresholdsMatrix();
            }
        }

        public void SetupParameters(IDictionary<String, Object> parameters)
        {
            
        }

        public void SetupInstance()
        {
            _NeuralNetwork = new NeuralNetworkForward();
            _NeuralNetwork.Setup(_Randomizers, _IsFillRandom, _Activations, _Tiers);
        }

        public INeuralNetwork GetNeuralNetwork()
        {
            return _NeuralNetwork;
        }
    }
}
