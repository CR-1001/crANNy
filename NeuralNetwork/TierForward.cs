/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetwork
{
    using System;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using CRAI.Common;
    using CRAI.NeuralNetwork.Activation;
    using CRAI.NeuralNetwork.Algebra;
    using CRAI.NeuralNetwork.Randomizer;

    [Serializable] [DataContract] public class TierForward : NotifyBase<TierForward>, ITier
    {
        #region public double[] ValuesOut
        private double[] _ValuesOut;
        public double[] ValuesOut
        {
            get { return _ValuesOut; }
            set
            {
                if (_ValuesOut == value) return;
                _ValuesOut = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public double[] ValuesOut

        public void Setup(
            IRandomizer randomizer,
            bool isRandomFill, 
            IActivation activationFunction,
            int countNeurons)
        {
            ValuesOut = new double[countNeurons];
            Activation = activationFunction;
            Randomizer = randomizer;
            IsRandomFill = isRandomFill;
        }

        public void DetermineOutputTierNext()
        {
            var vectorInput = Vector.Construct(ValuesOut.Concat(new [] {1.0}));

            var tierNextCountNeurons = TierNext.CountNeurons;

            for (var i = 0; i < tierNextCountNeurons; i++)
            {
                var vectorColumn = _MatrixWeightsThresholds.GetColumn(i);

                double scalarProduct = vectorColumn.MultiplyScalar(vectorInput);

                var activation = Activation.Calculate(scalarProduct);

                _TierNext.ValuesOut[i] = activation;
            }
        }

        public void DetachNeuron(int neuron)
        {
            if(CountNeurons == 0 || neuron >= CountNeurons) return;

            if (MatrixWeightsThresholds != null)
            {
                MatrixWeightsThresholds = MatrixOperations.RemoveRows(MatrixWeightsThresholds, neuron);
            }

            if (TierPrevious != null && TierPrevious.MatrixWeightsThresholds != null)
            {
                TierPrevious.MatrixWeightsThresholds = MatrixOperations.RemoveColumns(
                    TierPrevious.MatrixWeightsThresholds, neuron);
            }

            ValuesOut = new double[ValuesOut.Length - 1];
        }

        public bool IsIn
        {
            get { return (TierPrevious == null); }
        }

        public bool IsHidden
        {
            get
            {
                return !IsIn && !IsOut;
            }
        }

        public bool IsOut
        {
            get { return (TierNext == null); }
        }

        #region public Matrix MatrixWeightsThresholds
        private Matrix _MatrixWeightsThresholds;
        public Matrix MatrixWeightsThresholds
        {
            get { return _MatrixWeightsThresholds; }
            set
            {
                if (_MatrixWeightsThresholds == value) return;
                _MatrixWeightsThresholds = value;

                if (value != null && CountNeurons != value.CountRows - 1)
                {
                    throw new NeuralNetworkException(Errors.MatrixDimensionsMissmatch);
                }

                NotifyPropertyChanged();
            }
        }
        #endregion public Matrix MatrixWeightsThresholds

        public int CountNeurons
        {
            get
            {
                return ValuesOut.Length;
            }
        }

        #region public ITier TierNext
        private ITier _TierNext;
        public ITier TierNext
        {
            get { return _TierNext; }
            set
            {
                if (_TierNext == value) return;
                _TierNext = value;

                GenerateWeightsTresholdsMatrix();

                NotifyPropertyChanged();
                NotifyPropertyChanged(this, n => n.IsHidden);
                NotifyPropertyChanged(this, n => n.IsOut);
            }
        }
        #endregion public ITier TierNext

        #region public ITier TierPrevious
        private ITier _TierPrevious;
        public ITier TierPrevious
        {
            get { return _TierPrevious; }
            set
            {
                if (_TierPrevious == value) return;
                _TierPrevious = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public ITier TierPrevious

        public void GenerateWeightsTresholdsMatrix()
        {
            if (TierNext == null)
            {
                MatrixWeightsThresholds = null;
            }
            else
            {
                MatrixWeightsThresholds = new Matrix(
                    CountNeurons + 1, TierNext.CountNeurons);

                if (IsRandomFill)
                {
                    MatrixOperations.Random(
                        MatrixWeightsThresholds, 
                        Randomizer, 
                        -1, 
                        1, 
                        null);
                }
            }
        }

        #region public bool IsRandomFill
        private bool _IsRandomFill;
        public bool IsRandomFill
        {
            get { return _IsRandomFill; }
            set
            {
                if (_IsRandomFill == value) return;
                _IsRandomFill = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public bool IsRandomFill
        
        #region public IRandomizer Randomizer
        private IRandomizer _Randomizer;
        public IRandomizer Randomizer
        {
            get { return _Randomizer; }
            set
            {
                if (_Randomizer == value) return;
                _Randomizer = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public IRandomizer Randomizer

        #region public IActivation Activation
        private IActivation _Activation;
        public IActivation Activation
        {
            get { return _Activation; }
            set
            {
                if (_Activation == value) return;
                _Activation = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public IActivation Activation
        
        public Vector VectorBias
        {
            get
            {
                return MatrixWeightsThresholds == null 
                    ? null 
                    : MatrixWeightsThresholds.GetRow(MatrixWeightsThresholds.CountRows - 1);
            }
            set
            {
                MatrixWeightsThresholds.SetRow(MatrixWeightsThresholds.CountRows - 1, value);
            }
        }

        public override bool Equals(Object obj)
        {
            if (Object.ReferenceEquals(this, obj)) return true;

            var Tier = obj as TierForward;

            if (Tier == null) return false;

            if (CountNeurons != Tier.CountNeurons) return false;

            if (TierNext != Tier.TierNext) return false;
            if (TierPrevious != Tier.TierPrevious) return false;

            if (IsHidden != Tier.IsHidden) return false;
            if (IsIn != Tier.IsIn) return false;
            if (IsOut != Tier.IsOut) return false;

            if (Activation != Tier.Activation) return false;
            if (Randomizer != Tier.Randomizer) return false;

            if (MatrixWeightsThresholds != null)
            {
                if (Tier.MatrixWeightsThresholds == null) return false;
                if (!MatrixWeightsThresholds.Equals(Tier.MatrixWeightsThresholds)) return false;
            }
            else
            {
                if (Tier.MatrixWeightsThresholds != null) return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 397
                    ^ CountNeurons.GetHashCode()
                    ^ IsHidden.GetHashCode()
                    ^ IsIn.GetHashCode()
                    ^ IsOut.GetHashCode();

                if(Activation != null) hashCode ^= Activation.GetHashCode();
                if(Randomizer != null) hashCode ^= Randomizer.GetHashCode();

                return hashCode;
            }
        }

        public string ToString(
            int precision,
            int rowsMax,
            int columnsMax)
        {
            var stringBuilder = new StringBuilder();

            var kind = (IsIn ? "Input" : IsOut ? "Output" : "Hidden");
            stringBuilder.AppendLine("Type = " + GetType().Name);
            stringBuilder.AppendLine("Kind = " + kind);
            stringBuilder.AppendLine("CountNeurons = " + CountNeurons);

            if (MatrixWeightsThresholds != null)
            {
                var valuesSequence
                    = MatrixWeightsThresholds.ToString(precision, rowsMax, columnsMax);

                stringBuilder.AppendLine("Weights/Thresholds = ");
                stringBuilder.AppendLine(valuesSequence);
            }

            return stringBuilder.ToString();
        }

        public String ToStringComplete()
        {
            return ToString(-1, -1, -1);
        }
    }
}
