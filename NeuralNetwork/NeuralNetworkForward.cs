/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetwork
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using CRAI.Common;
    using CRAI.NeuralNetwork.Activation;
    using CRAI.NeuralNetwork.Algebra;
    using CRAI.NeuralNetwork.Error;
    using CRAI.NeuralNetwork.Randomizer;
    
    [Serializable] [DataContract] public class NeuralNetworkForward : NotifyBase<NeuralNetworkForward>, INeuralNetwork
    {
        #region public Guid Id
        private Guid _Id;
        public Guid Id
        {
            get { return _Id; }
            set
            {
                if (_Id == value) return;
                _Id = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public Guid Id

        #region public ObservableCollection<ITier> Tiers
        private ObservableCollection<ITier> _Tiers
            = new ObservableCollection<ITier>();
        public ObservableCollection<ITier> Tiers
        {
            get { return _Tiers; }
            private set
            {
                if (_Tiers == value) return;
                _Tiers = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public ObservableCollection<ITier> Tiers

        public void Setup(
            IEnumerable<IRandomizer> randomizers, 
            bool isRandomFill, 
            IEnumerable<IActivation> activations, 
            int[] tiers)
        {
            var randomizersx = randomizers.ToList();
            var activationsx = activations.ToList();

            for (var i = 0; i < tiers.Length; i++)
            {
                var neurons = tiers[i];

                var tier = new TierForward();

                var activation = activationsx[Math.Min(i, activationsx.Count - 1)];

                var randomizer = randomizersx[Math.Min(i, randomizersx.Count - 1)];

                tier.Setup(randomizer, isRandomFill, activation, neurons);

                tier.TierPrevious = TierOut;

                if (TierOut != null)
                {
                    TierOut.TierNext = tier;
                }

                tier.TierNext = null;

                Tiers.Add(tier);
            }
        }

        public Stimulus DetermineOutput(Stimulus stimulusInput)
        {
            if (TierIn.CountNeurons != stimulusInput.Dimension)
            {
                throw new NeuralNetworkException(Errors.InputNeuronsMissmatch);
            }

            TierIn.ValuesOut = stimulusInput.ToArray();

            for (var i = 0; i < Tiers.Count - 1; i++)
            {
                Tiers[i].DetermineOutputTierNext();
            }

            var stimulusOutput = new Stimulus(TierOut.ValuesOut);

            return stimulusOutput;
        }

        public int CountNeurons
        {
            get
            {
                return Tiers.Any() ? Tiers.Sum(t => t.CountNeurons) : 0;
            }
        }

        public int CountTiers
        {
            get
            {
                return Tiers.Count;
            }
        }

        public override bool Equals(Object obj)
        {
            var NeuralNetwork = obj as NeuralNetworkForward;

            if (NeuralNetwork == null)
            {
                return false;
            }

            if (CountTiers != NeuralNetwork.CountTiers)
            {
                return false;
            }

            for (var i = 0; i < CountTiers; i++)
            {
                var tier1 = Tiers[i];
                var tier2 = NeuralNetwork.Tiers[i];

                if (!tier1.Equals(tier2))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return Tiers.GetHashCode() ^ 617;
        }

        public ITier TierIn
        {
            get
            {
                return Tiers.FirstOrDefault();
            }
        }

        public ITier TierOut
        {
            get
            {
                return Tiers.LastOrDefault();
            }
        }

        public override String ToString()
        {
            return ToString(2, 10, 10);
        }

        public String ToString(int precision, int rowsMax, int columnsMax)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("Type = " + GetType().Name);
            stringBuilder.AppendLine("CountTiers = " + CountTiers);
            stringBuilder.AppendLine();
            var index = 0;
            foreach (var tier in Tiers)
            {
                stringBuilder.AppendLine("Tier " + index++ + " = ");
                stringBuilder.AppendLine(
                    tier.ToString(precision, rowsMax, columnsMax));
                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }

        public String ToStringComplete()
        {
            return ToString(-1, -1, -1);
        }
    }
}
