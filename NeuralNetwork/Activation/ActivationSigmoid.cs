/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetwork.Activation
{
    using System;
    using System.Runtime.Serialization;
    using CRAI.Common;

    [Serializable] [DataContract] public class ActivationSigmoid : NotifyBase<ActivationSigmoid>, IActivation
    {
        #region public double Scale
        private double _Scale = 1.0;
        public double Scale
        {
            get { return _Scale; }
            set
            {
                if (_Scale == value) return;

                _Scale = (value > 0) ? value : 1.0;
                NotifyPropertyChanged();

            }
        }
        #endregion public double Scale


        public double Calculate(double value)
        {
            return (1.0 / (1 + Math.Exp(value * -_Scale)));
        }

        public double CalculateDerivate(double value)
        {
            return (_Scale * Calculate(value) * (1.0 - Calculate(value)));
        }
    }
}
