/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetworkHost.Configurations
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.Serialization;
    using CRAI.Common;
    using CRAI.NeuralNetwork;
    using CRAI.NeuralNetwork.Error;
    using CRAI.NeuralNetwork.Learning;
    using CRAI.NeuralNetwork.Learning.Backpropagation;

    [DataContract] [Serializable] 
    public class LearningConfiguration 
        : ParametersBase<LearningConfiguration>
    {

        #region public LearningSessionRaw LearningSessionRaw
        private LearningSessionRaw _LearningSessionRaw;
        [DataMember]
        public LearningSessionRaw LearningSessionRaw
        {
            get { return _LearningSessionRaw; }
            set
            {
                if (_LearningSessionRaw == value) return;
                _LearningSessionRaw = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public LearningSessionRaw LearningSessionRaw

        #region public ConvertSettings LearningSessionRawConvertSettings
        private ConvertSettings _LearningSessionRawConvertSettings
            = new ConvertSettings();
        [DataMember]
        public ConvertSettings LearningSessionRawConvertSettings
        {
            get { return _LearningSessionRawConvertSettings; }
            set
            {
                if (_LearningSessionRawConvertSettings == value) return;
                _LearningSessionRawConvertSettings = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public ConvertSettings LearningSessionRawConvertSettings
        

        #region public LearningSession LearningSession
        private LearningSession _LearningSession;
        [DataMember]
        public LearningSession LearningSession
        {
            get { return _LearningSession; }
            set
            {
                if (_LearningSession == value) return;
                _LearningSession = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public LearningSession LearningSession
    
        #region public TypeDescription<ILearning> TypeDescription
        private TypeDescription<ILearning> _TypeDescription
            = new TypeDescription<ILearning>();
        [DataMember]
        public TypeDescription<ILearning> TypeDescription
        {
            get { return _TypeDescription; }
            set
            {
                if (_TypeDescription == value) return;
                _TypeDescription = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public TypeDescription<ILearning> TypeDescription

        #region public IErrorEstimation ErrorEstimation
        private IErrorEstimation _ErrorEstimation;
        [DataMember]
        public IErrorEstimation ErrorEstimation
        {
            get { return _ErrorEstimation; }
            set
            {
                if (_ErrorEstimation == value) return;
                _ErrorEstimation = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public IErrorEstimation ErrorEstimation

        #region public ILearningParameter StepsMaximum
        private ILearningParameter _StepsMaximum;
        [DataMember]
        public ILearningParameter StepsMaximum
        {
            get { return _StepsMaximum; }
            set
            {
                if (_StepsMaximum == value) return;
                _StepsMaximum = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public ILearningParameter StepsMaximum

        #region public ILearningParameter ErrorMaximum
        private ILearningParameter _ErrorMaximum;
        [DataMember]
        public ILearningParameter ErrorMaximum
        {
            get { return _ErrorMaximum; }
            set
            {
                if (_ErrorMaximum == value) return;
                _ErrorMaximum = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public ILearningParameter ErrorMaximum

        #region public ILearningParameter LearningRate
        private ILearningParameter _LearningRate;
        [DataMember]
        public ILearningParameter LearningRate
        {
            get { return _LearningRate; }
            set
            {
                if (_LearningRate == value) return;
                _LearningRate = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public ILearningParameter LearningRate

        #region public ILearningParameter Momentum
        private ILearningParameter _Momentum;
        [DataMember]
        public ILearningParameter Momentum
        {
            get { return _Momentum; }
            set
            {
                if (_Momentum == value) return;
                _Momentum = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public ILearningParameter Momentum

        public Object this[String key]
        {
            get 
            {
                if(key == Notify.GetPropertyName(this, l => l.ErrorEstimation))
                {
                    return ErrorEstimation;
                }
                else if(key == Notify.GetPropertyName(this, l => l.StepsMaximum))
                {
                    return StepsMaximum;
                }
                else if(key == Notify.GetPropertyName(this, l => l.ErrorMaximum))
                {
                    return ErrorMaximum;
                }
                else if(key == Notify.GetPropertyName(this, l => l.LearningRate))
                {
                    return LearningRate;
                }
                else if(key == Notify.GetPropertyName(this, l => l.Momentum))
                {
                    return Momentum;
                }

                Object value = null;
                
                Parameters.TryGetValue(key, out value);
                
                return value; 
            }
            set 
            { 
                Parameters[key] = value; 
            }
        }
    }
}
