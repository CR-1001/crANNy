/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetwork.Learning
{
    using System;
    using System.Runtime.Serialization;
    using CRAI.Common;

    [Serializable] [DataContract] public class LearningParameterConstant : NotifyBase<LearningParameterConstant>, ILearningParameter
    {
        public LearningParameterConstant()
        {

        }

        public LearningParameterConstant(double value)
        {
            Value = value;
        }

        #region public double Value
        private double _Value;
        [DataMember]
        public double Value
        {
            get { return _Value; }
            set
            {
                if (_Value == value) return;
                _Value = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public double Value

        public double Determine(ILearning learning, ITier Tier)
        {
            return _Value;
        }

        public double Determine(ILearning learning)
        {
            return _Value;
        }

        public static implicit operator LearningParameterConstant(double value)
        {
            return new LearningParameterConstant(value);
        }
    }
}
