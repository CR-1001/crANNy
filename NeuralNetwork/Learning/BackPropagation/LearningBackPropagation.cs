/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetwork.Learning.Backpropagation
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using CRAI.Common;
    using CRAI.NeuralNetwork;
    using CRAI.NeuralNetwork.Algebra;
    using CRAI.NeuralNetwork.Error;

    public sealed class LearningBackPropagation : NotifyBase<LearningBackPropagation>, ILearning
    {
        private Object _LockLearn = new Object();

        #region public IDictionary<String, Object> Parameters
        private IDictionary<String, Object> _Parameters = new Dictionary<String, Object>();
        public IDictionary<String, Object> Parameters
        {
            get { return _Parameters; }
            set
            {
                if (_Parameters == value) return;
                _Parameters = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public IDictionary<String, Object> Parameters
       
        #region public DateTime Started
        private DateTime? _Started;
        public DateTime? Started
        {
            get { return _Started; }
            private set
            {
                if (_Started == value) return;
                _Started = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(this, l => l.Duration);
            }
        }

        public TimeSpan Duration
        {
            get { return !_Started.HasValue ? TimeSpan.Zero : DateTime.Now - _Started.Value; }
        }

        #endregion public DateTime Started

        #region public ObservableCollection<LearningBackPropagationTier> Tiers
        private ObservableCollection<LearningBackPropagationTier> _Tiers = new ObservableCollection<LearningBackPropagationTier>();
        public ObservableCollection<LearningBackPropagationTier> Tiers
        {
            get { return _Tiers; }
            set
            {
                if (_Tiers == value) return;
                _Tiers = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public ObservableCollection<LearningBackPropagationTier> Tiers

        #region public TimeSpan DurationMax
        private TimeSpan _DurationMaximum = Timeout.InfiniteTimeSpan;
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
        #endregion public TimeSpan DurationMax

        #region public LearningResult LearningResultCurrent
        private LearningResult _LearningResultCurrent;
        public LearningResult LearningResultCurrent
        {
            get
            {
                return _LearningResultCurrent;
            }
            set
            {
                if (_LearningResultCurrent == value) return;
                _LearningResultCurrent = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public LearningResult LearningResultCurrent

        #region public ILearningParameter ErrorMaximum
        private ILearningParameter _ErrorMaximum;
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

        #region public uint Steps
        private uint _Steps;
        public uint Steps
        {
            get { return _Steps; }
            set
            {
                if (_Steps == value) return;
                _Steps = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public uint Steps

        #region public ILearningParameter StepsMaximum
        private ILearningParameter _StepsMaximum;
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

        #region public INeuralNetwork NeuralNetwork
        private INeuralNetwork _NeuralNetwork;
        public INeuralNetwork NeuralNetwork
        {
            get { return _NeuralNetwork; }
            set
            {
                if (_NeuralNetwork == value) return;
                _NeuralNetwork = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public INeuralNetwork NeuralNetwork

        #region public ILearningParameter LearningRate
        private ILearningParameter _LearningRate;
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

        #region public double ErrorMaximumLast
        private double _ErrorMaximumLast;
        public double ErrorMaximumLast
        {
            get { return _ErrorMaximumLast; }
            private set
            {
                if (_ErrorMaximumLast == value) return;
                _ErrorMaximumLast = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public double ErrorMaximumLast

        #region public double StepsMaximumLast
        private double _StepsMaximumLast;
        public double StepsMaximumLast
        {
            get { return _StepsMaximumLast; }
            private set
            {
                if (_StepsMaximumLast == value) return;
                _StepsMaximumLast = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public double StepsMaximumLast

        #region public IErrorEstimation ErrorEstimation
        private IErrorEstimation _ErrorEstimation;
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
        
        #region public double Error
        private double _Error;
        public double Error
        {
            get { return _Error; }
            set
            {
                if (_Error == value) return;
                _Error = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public double Error

        #region public LearningSession LearningSession
        private LearningSession _LearningSession;
        public LearningSession LearningSession
        {
            get { return _LearningSession; }
            private set
            {
                if (_LearningSession == value) return;
                _LearningSession = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public LearningSession LearningSession

        public void Setup(
            INeuralNetwork NeuralNetwork, 
            IErrorEstimation errorEstimation, 
            ILearningParameter stepsMaximum, 
            ILearningParameter errorMaximum, 
            ILearningParameter learningRate, 
            ILearningParameter momentum,
            IDictionary<String, Object> parametersAdditional)
        {
            NeuralNetwork = NeuralNetwork;

            StepsMaximum = stepsMaximum;
            ErrorMaximum = errorMaximum;
            LearningRate = learningRate;
            Momentum = momentum;
            Parameters = parametersAdditional ?? new Dictionary<String, Object>();

            Tiers.Clear();

            foreach (var tier in NeuralNetwork.Tiers)
            {
                var learningTier = new LearningBackPropagationTier();
                learningTier.Setup(this, tier);
                Tiers.Add(learningTier);
            }

            ErrorEstimation = errorEstimation;
            ErrorEstimation.Setup(NeuralNetwork);
        }

        private void LearnStep()
        {
            foreach (var learningSubject in LearningSession.LearningSubjects)
            {
                NeuralNetwork.DetermineOutput(learningSubject.Input);
                DetermineError(learningSubject.Output);
            }

            foreach (var tier in Tiers)
            {
                var Tier = tier.Tier;

                var momentum = Momentum.Determine(this, Tier)
                    .LimitDownTop(0, 1);

                var learningRate = LearningRate.Determine(this, Tier)
                    .LimitDownTop(0, 1);

                tier.Learn(learningRate, momentum);
            }

            var input = LearningSession.GetInput();
            var output = LearningSession.GetOutput();

            Error = ErrorEstimation.DetermineError(input, output);

            Steps++;
        }

        public LearningResult Learn(LearningSession learningSession)
        {
            _Started = DateTime.Now;

            LearningSession = learningSession;

            lock(_LockLearn)
            {
                while (DurationMaximum == Timeout.InfiniteTimeSpan || Duration < DurationMaximum)
                {
                    LearnStep();

                    ErrorMaximumLast = ErrorMaximum.Determine(this);
                    StepsMaximumLast = StepsMaximum.Determine(this);

                    RefreshLearningResult();

                    if (Steps > StepsMaximumLast || Error < ErrorMaximumLast || _InterruptNextIteration) break;
                }
            }


            return LearningResultCurrent;
        }

        private void RefreshLearningResult()
        {
            LearningResultCurrent = new LearningResult()
            {
                Started = _Started.Value,
                Finished = DateTime.Now,
                DurationMaximum = DurationMaximum,
                ErrorMaximum = ErrorMaximumLast,
                ErrorResult = Error,
                StepsMaximum = (uint)StepsMaximumLast,
                StepsResult = Steps,
                IsInterrupted = _InterruptNextIteration,
            };

            Debug.WriteLine(
                "Steps={0}; Error={1} / {2}; Time={3}", 
                LearningResultCurrent.StepsResult,
                LearningResultCurrent.ErrorResult,
                LearningResultCurrent.ErrorMaximum,
                DateTime.Now - LearningResultCurrent.Started);
        }

        public void DetermineError(Stimulus stimulusOutputShould)
        {
            if (stimulusOutputShould.Dimension != _NeuralNetwork.TierOut.CountNeurons)
            {
                throw new NeuralNetworkException(Errors.InputNeuronsMissmatch);
            }

            foreach (var tier in Tiers)
            {
                tier.VectorErrors.Fill(0);
            }

            for (var i = _NeuralNetwork.CountTiers - 1; i >= 0; i--)
            {
                var tier = _NeuralNetwork.Tiers[i];

                if (!tier.IsOut)
                {
                    Tiers[i].DetermineError();
                }
                else
                {
                    Tiers[i].DetermineError(stimulusOutputShould);
                }
            }
        }

        public LearningBackPropagationTier this[ITier Tier]
        {
            get
            {
                return Tiers.FirstOrDefault(t => t.Tier == Tier);
            }
        }

        public LearningBackPropagationTier this[int index]
        {
            get
            {
                return Tiers.ElementAtOrDefault(index);
            }
        }

        public LearningResult Interrupt()
        {
            _InterruptNextIteration = true;

            lock (_LockLearn)
            {
                return LearningResultCurrent;
            }
        }

        private bool _InterruptNextIteration = false;
    }
}
