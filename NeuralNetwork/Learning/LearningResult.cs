/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetwork.Learning
{
    using System;
    using System.Runtime.Serialization;
    using Common;

    [Serializable] [DataContract] public class LearningResult : NotifyBase<LearningResult>
    {
        public TimeSpan Duration
        {
            get { return Finished - Started; }
        }

        #region public TimeSpan DurationMaximum
        private TimeSpan _DurationMaximum = TimeSpan.MaxValue;
        [DataMember]
        public TimeSpan DurationMaximum
        {
            get { return _DurationMaximum; }
            set
            {
                if (_DurationMaximum == value) return;
                _DurationMaximum = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public TimeSpan DurationMaximum

        #region public bool IsInterrupted
        private bool _IsInterrupted;
        [DataMember]
        public bool IsInterrupted
        {
            get { return _IsInterrupted; }
            set
            {
                if (_IsInterrupted == value) return;
                _IsInterrupted = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public bool IsInterrupted

        #region public double ErrorResult
        private double _ErrorResult;
        [DataMember]
        public double ErrorResult
        {
            get { return _ErrorResult; }
            set
            {
                if (_ErrorResult == value) return;
                _ErrorResult = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public double ErrorResult

        #region public double ErrorMaximum
        private double _ErrorMaximum;
        [DataMember]
        public double ErrorMaximum
        {
            get { return _ErrorMaximum; }
            set
            {
                if (_ErrorMaximum == value) return;
                _ErrorMaximum = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public double ErrorMaximum

        #region public uint StepsMaximum
        private uint _StepsMaximum;
        [DataMember]
        public uint StepsMaximum
        {
            get { return _StepsMaximum; }
            set
            {
                if (_StepsMaximum == value) return;
                _StepsMaximum = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public int StepsMaximum

        #region public int StepsResult
        private uint _StepsResult;
        [DataMember]
        public uint StepsResult
        {
            get { return _StepsResult; }
            set
            {
                if (_StepsResult == value) return;
                _StepsResult = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public int StepsResult

        #region public DateTime Started
        private DateTime _Started;
        [DataMember]
        public DateTime Started
        {
            get { return _Started; }
            set
            {
                if (_Started == value) return;
                _Started = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(this, l => l.Duration);
            }
        }
        #endregion public DateTime Started

        #region public DateTime Finished
        private DateTime _Finished;
        [DataMember]
        public DateTime Finished
        {
            get { return _Finished; }
            set
            {
                if (_Finished == value) return;
                _Finished = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(this, l => l.Duration);
            }
        }
        #endregion public DateTime Finished
    }
}
