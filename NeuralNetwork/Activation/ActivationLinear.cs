/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetwork.Activation
{
    using System;
    using System.Runtime.Serialization;
    using CRAI.Common;

    [Serializable] [DataContract] public class ActivationLinear : NotifyBase<ActivationLinear>, IActivation
    {
        #region public double Factor
        private double _Factor = 1.0;
        public double Factor
        {
            get { return _Factor; }
            set
            {
                if (_Factor == value) return;

                _Factor = value > 0 ? value : 1.0;
                Threshold = 0.5 / _Factor;

                NotifyPropertyChanged();
            }
        }
        #endregion public double Factor

        #region public double Threshold
        private double _Threshold = 0.5;
        public double Threshold
        {
            get { return _Threshold; }
            set
            {
                if (_Threshold == value) return;
                _Threshold = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public double Threshold

        public double Calculate(double value)
        {
            if (Threshold < value)
            {
                return 1;
            }
            else if (-Threshold > value)
            {
                return 0;
            }

            return value * Factor + 0.5;
        }

        public double CalculateDerivate(double value)
        {
            if (Threshold < value)
            {
                return 0;
            }
            else if (-Threshold > value)
            {
                return 0;
            }

            return Factor;
        }
    }
}
