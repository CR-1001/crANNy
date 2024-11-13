/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetwork.Learning.Backpropagation
{
    using System;
    using System.Runtime.Serialization;
    using CRAI.Common;
    using CRAI.NeuralNetwork;
    using CRAI.NeuralNetwork.Algebra;

    [Serializable] [DataContract] public class LearningBackPropagationTier : NotifyBase<LearningBackPropagationTier>
    {
        #region public LearningBackPropagation BackPropagation
        private LearningBackPropagation _BackPropagation;
        public LearningBackPropagation BackPropagation
        {
            get { return _BackPropagation; }
            private set
            {
                if (_BackPropagation == value) return;
                _BackPropagation = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public LearningBackPropagation BackPropagation

        #region public ITier Tier
        private ITier _Tier;
        public ITier Tier
        {
            get { return _Tier; }
            set
            {
                if (_Tier == value) return;
                _Tier = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public ITier Tier

        #region public Vector VectorErrors
        private Vector _VectorErrors;
        public Vector VectorErrors
        {
            get { return _VectorErrors; }
            set
            {
                if (_VectorErrors == value) return;
                _VectorErrors = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public Vector VectorErrors

        #region public Vector VectorErrorDeltas
        private Vector _VectorErrorDeltas;
        public Vector VectorErrorDeltas
        {
            get { return _VectorErrorDeltas; }
            private set
            {
                if (_VectorErrorDeltas == value) return;
                _VectorErrorDeltas = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public Vector VectorErrorDeltas

        #region public Matrix MatrixErrorDeltasWeightsBias
        private Matrix _MatrixErrorDeltasWeightsBias;
        public Matrix MatrixErrorDeltasWeightsBias
        {
            get { return _MatrixErrorDeltasWeightsBias; }
            private set
            {
                if (_MatrixErrorDeltasWeightsBias == value) return;
                _MatrixErrorDeltasWeightsBias = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public Matrix MatrixErrorDeltasWeightsBias

        #region public Matrix MatrixDeltasPrevious
        private Matrix _MatrixDeltasPrevious;
        public Matrix MatrixDeltasPrevious
        {
            get { return _MatrixDeltasPrevious; }
            private set
            {
                if (_MatrixDeltasPrevious == value) return;
                _MatrixDeltasPrevious = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public Matrix MatrixDeltasPrevious

        #region public int RowBias
        private int _RowBias;
        public int RowBias
        {
            get { return _RowBias; }
            set
            {
                if (_RowBias == value) return;
                _RowBias = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public int RowBias

        public void Setup(LearningBackPropagation backpropagation, ITier tier)
        {
            BackPropagation = backpropagation;
            Tier = tier;

            VectorErrors = new Vector(tier.CountNeurons);
            VectorErrorDeltas = new Vector(tier.CountNeurons);

            if (tier.TierNext != null)
            {
                var rows = tier.CountNeurons + 1;
                var columns = tier.TierNext.CountNeurons;

                MatrixErrorDeltasWeightsBias = new Matrix(rows, columns);
                MatrixDeltasPrevious = new Matrix(rows, columns);

                RowBias = tier.CountNeurons;
            }
        }

        public void DetermineError()
        {
            var backPropagationTierNext = _BackPropagation[_Tier.TierNext];

            var tierNextCountNeurons = _Tier.TierNext.CountNeurons;
            var tierCountNeurons = _Tier.CountNeurons;

            for (var indexNext = 0; indexNext < tierNextCountNeurons; indexNext++)
            {
                DetermineErrorPart(backPropagationTierNext, indexNext);

                var errorThresholdDelta = backPropagationTierNext.VectorErrorDeltas[indexNext];

                _MatrixErrorDeltasWeightsBias[_RowBias, indexNext] += errorThresholdDelta;
            }

            if (_Tier.IsHidden)
            {
                for (var indexThis = 0; indexThis < tierCountNeurons; indexThis++)
                {
                    VectorErrorDeltas[indexThis] = DetermineDelta(indexThis).Limit();
                }
            }
        }

        private void DetermineErrorPart(LearningBackPropagationTier next, int indexNext)
        {
            var tierCountNeurons = _Tier.CountNeurons;

            for (var indexThis = 0; indexThis < tierCountNeurons; indexThis++)
            {
                var valueOut = _Tier.ValuesOut[indexThis];

                var errorDelta = next.VectorErrorDeltas[indexNext];

                var matrixDelta = errorDelta * valueOut;

                _MatrixErrorDeltasWeightsBias[indexThis, indexNext] += matrixDelta;

                var error
                    = VectorErrors[indexThis]
                    + _Tier.MatrixWeightsThresholds[indexThis, indexNext]
                    * next.VectorErrorDeltas[indexNext];

                VectorErrors[indexThis] = error.Limit();
            }
        }

        public void DetermineError(Stimulus stimulusOutputShould)
        {
            for (var i = 0; i < _Tier.CountNeurons; i++)
            {
                VectorErrors[i] = (stimulusOutputShould[i] - _Tier.ValuesOut[i]).Limit();
                VectorErrorDeltas[i] = DetermineDelta(i).Limit();
            }
        }

        private double DetermineDelta(int neuron)
        {
            var value = _Tier.ValuesOut[neuron];

            var derivate = _Tier.Activation.CalculateDerivate(value);

            var error = VectorErrors[neuron];

            var delta = error * derivate;

            return delta;
        }

        public void Learn(double learningRate, double momentum)
        {
            if (_Tier.MatrixWeightsThresholds != null)
            {
                var matrixErrorDeltasWeightsBiasLearningRate
                    = _MatrixErrorDeltasWeightsBias.Multiply(learningRate);

                var matrixDeltasPreviousMomentum
                    = MatrixDeltasPrevious.Multiply(momentum);

                MatrixDeltasPrevious
                    = matrixErrorDeltasWeightsBiasLearningRate.Add(matrixDeltasPreviousMomentum);

                _Tier.MatrixWeightsThresholds
                    = _Tier.MatrixWeightsThresholds.Add(MatrixDeltasPrevious);

                _MatrixErrorDeltasWeightsBias.Fill(0);
            }
        }
    }
}
