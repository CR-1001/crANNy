/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.Threading;
    using System.Threading.Tasks;
    using CRAI.Common;
    using CRAI.NeuralNetwork;
    using CRAI.NeuralNetwork.Learning;
    using CRAI.NeuralNetworkHost;
    using CRAI.NeuralNetworkHost.Configurations;

    [Serializable]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Service : NotifyBase<Service>, IService
    {
        public Service()
        {
            StateHistory = new StateHistory();
        }

        private Object _Lock = new Object();

        #region public IStateDetailsFormatter StateDetailsFormatter
        private IStateDetailsFormatter _StateDetailsFormatter;
        public IStateDetailsFormatter StateDetailsFormatter
        {
            get { return _StateDetailsFormatter; }
            set
            {
                if (_StateDetailsFormatter == value) return;
                _StateDetailsFormatter = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public IStateDetailsFormatter StateDetailsFormatter

        #region public StateHistory StateHistory
        private StateHistory _StateHistory;
        public StateHistory StateHistory
        {
            get { return _StateHistory; }
            private set
            {
                if (_StateHistory == value) return;
                _StateHistory = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public StateHistory StateHistory

        #region public Dictionary<Identifier, Object> Locks
        private Dictionary<Identifier, Object> _Locks
            = new Dictionary<Identifier, Object>();
        public Dictionary<Identifier, Object> Locks
        {
            get { return _Locks; }
            set
            {
                if (_Locks == value) return;
                _Locks = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public Dictionary<Identifier, Object> Locks

        #region public Dictionary<Identifier, Semaphore> SemaphoresCompute
        private Dictionary<Identifier, Semaphore> _SemaphoresCompute
            = new Dictionary<Identifier, Semaphore>();
        public Dictionary<Identifier, Semaphore> SemaphoresCompute
        {
            get { return _SemaphoresCompute; }
            set
            {
                if (_SemaphoresCompute == value) return;
                _SemaphoresCompute = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public Dictionary<Identifier, Semaphore> SemaphoresCompute

        #region public Dictionary<Identifier, Semaphore> SemaphoresConvert
        private Dictionary<Identifier, Semaphore> _SemaphoresConvert
            = new Dictionary<Identifier, Semaphore>();
        public Dictionary<Identifier, Semaphore> SemaphoresConvert
        {
            get { return _SemaphoresConvert; }
            set
            {
                if (_SemaphoresConvert == value) return;
                _SemaphoresConvert = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public Dictionary<Identifier, Semaphore> SemaphoresConvert

        #region public Dictionary<Identifier, INeuralNetwork> NeuralNetworks
        private Dictionary<Identifier, INeuralNetwork> _NeuralNetworks
            = new Dictionary<Identifier, INeuralNetwork>();
        public Dictionary<Identifier, INeuralNetwork> NeuralNetworks
        {
            get { return _NeuralNetworks; }
            set
            {
                if (_NeuralNetworks == value) return;
                _NeuralNetworks = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public Dictionary<Identifier, INeuralNetwork> NeuralNetworks

        #region public Dictionary<Identifier, IConverter> Converters
        private Dictionary<Identifier, IConverter> _Converters
            = new Dictionary<Identifier, IConverter>();
        public Dictionary<Identifier, IConverter> Converters
        {
            get { return _Converters; }
            set
            {
                if (_Converters == value) return;
                _Converters = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public Dictionary<Identifier, IConverter> Converters

        #region public Dictionary<Identifier, ILearning> Learnings
        private Dictionary<Identifier, ILearning> _Learnings
            = new Dictionary<Identifier, ILearning>();
        public Dictionary<Identifier, ILearning> Learnings
        {
            get { return _Learnings; }
            set
            {
                if (_Learnings == value) return;
                _Learnings = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public Dictionary<Identifier, ILearning> Learnings


        public Identifier Setup(Setup setup)
        {
            var identifier = Identifier.New(setup);

            using (var state = StateHistory.AppendState(identifier, Types.Setup))
            {
                try
                {
                    var lockx = new Object();

                    lock (_Lock)
                    {
                        var identifierExisting
                            = ObtainInstances().FirstOrDefault(l => l.Id == identifier.Id);

                        if (identifierExisting != null)
                        {
                            return identifierExisting;
                        }

                        Locks[identifier] = lockx;
                    }

                    lock (lockx)
                    {
                        var director = new Director();

                        var NeuralNetwork = director.Construct(setup.NeuralNetworkConfiguration);
                        var converter = director.Construct(setup.ConverterConfiguration);

                        NeuralNetworks[identifier] = NeuralNetwork;
                        Converters[identifier] = converter;

                        SemaphoresCompute[identifier] = new Semaphore(
                            setup.CountCompute, setup.CountCompute);

                        SemaphoresConvert[identifier] = new Semaphore(
                            setup.CountConvert, setup.CountConvert);

                        if (setup.PurgeTimeSpan.HasValue)
                        {
                            Scheduler.RunDelayed(() =>
                            {
                                Release(identifier);
                            },
                            setup.PurgeTimeSpan.Value);
                        }

                        if (setup.LearningConfiguration != null)
                        {
                            if (setup.IsAsyncLearning)
                            {
                                ThreadPool.QueueUserWorkItem(delegate
                                {
                                    Learn(identifier, setup.LearningConfiguration);
                                });
                            }
                            else
                            {
                                Learn(identifier, setup.LearningConfiguration);
                            }
                        }

                        return identifier;
                    }
                }
                catch (Exception e)
                {
                    state.Exceptions.Add(e);

                    return null;
                }
            }
        }

        public Setup ExtractSetup(Identifier identifier)
        {
            using (var state = StateHistory.AppendState(identifier, Types.ExtractSetup))
            {
                try
                {
                    Object lockx;
                    INeuralNetwork NeuralNetwork;
                    IConverter converter;

                    lock (_Lock)
                    {
                        identifier = ObtainInstances().FirstOrDefault(l => l.Id == identifier.Id);

                        if (identifier == null)
                        {
                            return null;
                        }

                        lockx = Locks[identifier];
                        NeuralNetwork = NeuralNetworks[identifier];
                        converter = Converters[identifier];

                        if (lockx == null || NeuralNetwork == null || converter == null)
                        {
                            var exception = new NeuralNetworkException(Errors.ResourceNotFoundForIdentifier);

                            state.Exceptions.Add(exception);

                            return null;
                        }
                    }

                    lock (lockx)
                    {
                        var director = new Director();

                        var NeuralNetworkConfiguration = director.Extract(NeuralNetwork);
                        var converterConfiguration = director.Extract(converter);

                        var setup = new Setup()
                        {
                            IdSuggestion = identifier.Id,

                            Name = identifier.Name,
                            Description = identifier.Description,

                            CountCompute = identifier.CountCompute,
                            CountConvert = identifier.CountConvert,
                            
                            NeuralNetworkConfiguration = NeuralNetworkConfiguration,
                            ConverterConfiguration = converterConfiguration
                        };

                        return setup;
                    }
                }
                catch (Exception e)
                {
                    state.Exceptions.Add(e);

                    return null;
                }
            }
        }

        public void Release(Identifier identifier)
        {
            using (var state = StateHistory.AppendState(identifier, Types.Release))
            {
                try
                {
                    lock (_Lock)
                    {
                        identifier = ObtainInstances().FirstOrDefault(l => l.Id == identifier.Id);

                        if (identifier == null)
                        {
                            return;
                        }

                        NeuralNetworks[identifier] = null;
                        Converters[identifier] = null;
                        Learnings[identifier] = null;
                        SemaphoresCompute[identifier] = null;
                        SemaphoresConvert[identifier] = null;
                        Locks[identifier] = null;
                    }
                }
                catch (Exception e)
                {
                    state.Exceptions.Add(e);
                }
            }
        }

        public LearningResult Learn(
            Identifier identifier,
            LearningConfiguration learningConfiguration)
        {
            using (var state = StateHistory.AppendState(identifier, Types.Learning))
            {
                try
                {
                    learningConfiguration.LearningSessionRawConvertSettings
                        .IsLearningMode = true;

                    Object lockx;
                    INeuralNetwork NeuralNetwork;
                    IConverter converter;

                    lock (_Lock)
                    {
                        identifier = ObtainInstances().FirstOrDefault(l => l.Id == identifier.Id);

                        if (identifier == null)
                        {
                            return null;
                        }

                        lockx = Locks[identifier];
                        NeuralNetwork = NeuralNetworks[identifier];
                        converter = Converters[identifier];

                        if (lockx == null || NeuralNetwork == null || converter == null)
                        {
                            var exception = new NeuralNetworkException(Errors.ResourceNotFoundForIdentifier);

                            state.Exceptions.Add(exception);

                            return null;
                        }
                    }

                    lock (lockx)
                    {
                        var director = new Director();

                        var learningSession = MakeLearningSession(learningConfiguration, converter);

                        var learning = director.Construct(learningConfiguration, NeuralNetwork);

                        var learningResult = learning.Learn(learningSession);

                        StateDetailsFormatter.Format(
                            state, learningSession, learningResult);

                        return learningResult;
                    }
                }
                catch (Exception e)
                {
                    state.Exceptions.Add(e);

                    return null;
                }
            }
        }

        private LearningSession MakeLearningSession(
            LearningConfiguration learningConfiguration,
            IConverter converter)
        {
            if (learningConfiguration.LearningSessionRaw == null
                || !learningConfiguration.LearningSessionRaw.LearningSubjects.Any())
            {
                return learningConfiguration.LearningSession;
            }

            var learningSessionCombined = new LearningSession(
                learningConfiguration.LearningSessionRaw.CountInputNeurons,
                learningConfiguration.LearningSessionRaw.CountOutputNeurons);

            foreach (var learningSubjectRaw in
                learningConfiguration.LearningSessionRaw.LearningSubjects)
            {
                var stimulusSet = converter.Convert(
                    learningSubjectRaw.InputRaw,
                    learningConfiguration.LearningSessionRawConvertSettings);

                if (stimulusSet.Values.Length != 1)
                {
                    var exception = new NeuralNetworkException(Errors.LearningSubjectRawConversion);

                    throw exception;
                }

                var learningSubject = new LearningSubject(
                    stimulusSet.Values[0].Values,
                    learningSubjectRaw.Output.Values);

                learningSessionCombined.CountInputNeurons
                    = stimulusSet.Values[0].Values.Length;

                learningSessionCombined.CountOutputNeurons
                    = learningSubjectRaw.Output.Values.Length;

                learningSessionCombined.Append(learningSubject);
            }

            foreach (var learningSubject in learningConfiguration.LearningSession.LearningSubjects)
            {
                learningSessionCombined.Append(learningSubject);
            }


            var countInputNeurons = learningSessionCombined.LearningSubjects
                .Select(l => l.Input.Values.Length).Distinct();

            if (countInputNeurons.Count() != 1)
            {
                throw new NeuralNetworkException(Errors.LearningSubjectNeuronsCountMissmatch);
            }

            var countOutputNeurons = learningSessionCombined.LearningSubjects
                .Select(l => l.Output.Values.Length).Distinct();

            if (countOutputNeurons.Count() != 1)
            {
                throw new NeuralNetworkException(Errors.LearningSubjectNeuronsCountMissmatch);
            }

            return learningSessionCombined;
        }

        public LearningResult InterruptLearn(Identifier identifier)
        {
            using (var state = StateHistory.AppendState(identifier, Types.LearningInterrupted))
            {
                try
                {
                    Object lockx;
                    ILearning learning;

                    lock (_Lock)
                    {
                        identifier = ObtainInstances().FirstOrDefault(l => l.Id == identifier.Id);

                        if (identifier == null)
                        {
                            return null;
                        }

                        lockx = Locks[identifier];
                        learning = Learnings[identifier];

                        if (lockx == null || learning == null)
                        {
                            var exception = new NeuralNetworkException(Errors.ResourceNotFoundForIdentifier);

                            state.Exceptions.Add(exception);

                            return null;
                        }
                    }

                    //lock (lockx)
                    {
                        var learningResult = learning.Interrupt();

                        return learningResult;
                    }

                }
                catch (Exception e)
                {
                    state.Exceptions.Add(e);

                    return null;
                }
            }
        }

        public Stimulus Compute(Identifier identifier, Stimulus stimulusInput)
        {
            using (var state = StateHistory.AppendState(identifier, Types.Compute))
            {
                try
                {
                    Semaphore semaphoresCompute;
                    INeuralNetwork NeuralNetwork;

                    lock (_Lock)
                    {
                        identifier = ObtainInstances().FirstOrDefault(l => l.Id == identifier.Id);

                        if (identifier == null)
                        {
                            return null;
                        }

                        semaphoresCompute = SemaphoresCompute[identifier];
                        NeuralNetwork = NeuralNetworks[identifier];

                        if (semaphoresCompute == null || NeuralNetwork == null)
                        {
                            var exception = new NeuralNetworkException(Errors.ResourceNotFoundForIdentifier);

                            state.Exceptions.Add(exception);

                            return null;
                        }
                    }

                    try
                    {
                        semaphoresCompute.WaitOne();

                        var stimulusOutput = NeuralNetwork.DetermineOutput(stimulusInput);

                        StateDetailsFormatter.Format(
                               state, stimulusInput, stimulusOutput);

                        return stimulusOutput;
                    }
                    finally
                    {
                        semaphoresCompute.Release();
                    }

                }
                catch (Exception e)
                {
                    state.Exceptions.Add(e);

                    return null;
                }
            }
        }

        public byte[] ConvertAndCompute(
            Identifier identifier,
            byte[] rawDataInput,
            ConvertSettings convertSettings)
        {
            using (var state = StateHistory.AppendState(identifier, Types.ConvertAndCompute))
            {
                try
                {
                    Semaphore semaphoreConvert;
                    IConverter converter;

                    lock (_Lock)
                    {
                        identifier = ObtainInstances().FirstOrDefault(l => l.Id == identifier.Id);

                        if (identifier == null)
                        {
                            return null;
                        }

                        semaphoreConvert = SemaphoresConvert[identifier];
                        converter = Converters[identifier];

                        if (semaphoreConvert == null || converter == null)
                        {
                            var exception = new NeuralNetworkException(Errors.ResourceNotFoundForIdentifier);

                            state.Exceptions.Add(exception);

                            return null;
                        }
                    }

                    try
                    {
                        semaphoreConvert.WaitOne();

                        var stimulusSetInput = ConvertInput(
                            identifier, rawDataInput, converter, convertSettings);

                        var stimulusSetOutput = new StimulusSet(stimulusSetInput.Values.Length);

                        Parallel.For(0, stimulusSetInput.Values.Length, i =>
                        {
                            Stimulus simulusInput;

                            lock (stimulusSetInput)
                            {
                                simulusInput = stimulusSetInput.Values[i];
                            }

                            var stimulusOutput = Compute(identifier, simulusInput);

                            lock (stimulusSetOutput)
                            {
                                stimulusSetOutput.Values[i] = stimulusOutput;
                            }
                        });

                        var rawDataOutput = ConvertOutput(
                            identifier, converter, stimulusSetOutput, convertSettings);

                        return rawDataOutput;
                    }
                    finally
                    {
                        semaphoreConvert.Release();
                    }
                }
                catch (Exception e)
                {
                    state.Exceptions.Add(e);

                    return null;
                }
            }
        }

        private byte[] ConvertOutput(
            Identifier identifier,
            IConverter converter,
            StimulusSet stimulusSetOutput,
            ConvertSettings convertSettings)
        {
            using (var state = StateHistory.AppendState(identifier, Types.ConvertOutput))
            {
                try
                {
                    lock (_Lock)
                    {
                        identifier = ObtainInstances().FirstOrDefault(l => l.Id == identifier.Id);

                        if (identifier == null)
                        {
                            return null;
                        }
                    }

                    var rawDataOutput = converter.Convert(
                        stimulusSetOutput, convertSettings);

                    StateDetailsFormatter.Format(
                        state, rawDataOutput, stimulusSetOutput, convertSettings);

                    return rawDataOutput;
                }
                catch (Exception e)
                {
                    state.Exceptions.Add(e);

                    return null;
                }
            }
        }

        private StimulusSet ConvertInput(
            Identifier identifier,
            byte[] rawDataInput,
            IConverter converter,
            ConvertSettings convertSettings)
        {
            using (var state = StateHistory.AppendState(identifier, Types.ConvertInput))
            {
                try
                {
                    lock (_Lock)
                    {
                        identifier = ObtainInstances().FirstOrDefault(l => l.Id == identifier.Id);

                        if (identifier == null)
                        {
                            return null;
                        }
                    }

                    var stimulusSetInput = converter.Convert(
                        rawDataInput, convertSettings);

                    StateDetailsFormatter.Format(
                        state, rawDataInput, stimulusSetInput, convertSettings);

                    return stimulusSetInput;
                }
                catch (Exception e)
                {
                    state.Exceptions.Add(e);

                    return null;
                }
            }
        }

        public StateSet Diagnose(Identifier identifier)
        {
            lock (_Lock)
            {
                identifier = ObtainInstances().FirstOrDefault(l => l.Id == identifier.Id);

                if (identifier == null)
                {
                    return null;
                }

                return StateHistory.GetStateSet(identifier);
            }
        }


        public Identifier[] ObtainInstances()
        {
            lock (_Lock)
            {
                return Locks
                    .Where(l => l.Value != null)
                    .Select(l => l.Key)
                    .ToArray();
            }
        }
    }
}
