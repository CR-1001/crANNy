/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using CRAI.Common;
    using CRAI.NeuralNetwork;
    using CRAI.NeuralNetwork.Learning;
    using CRAI.NeuralNetworkHost.Configurations;

    public class StateDetailsFormatter 
        : NotifyBase<StateDetailsFormatter>, IStateDetailsFormatter
    {
        #region public int PrecisionStimulus
        private int _PrecisionStimulus = 2;
        public int PrecisionStimulus
        {
            get { return _PrecisionStimulus; }
            set
            {
                if (_PrecisionStimulus == value) return;
                _PrecisionStimulus = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public int PrecisionStimulus

        #region public int ItemsToTakeMaxLearningSubjects
        private int _ItemsToTakeMaxLearningSubjects = 10;
        public int ItemsToTakeMaxLearningSubjects
        {
            get { return _ItemsToTakeMaxLearningSubjects; }
            set
            {
                if (_ItemsToTakeMaxLearningSubjects == value) return;
                _ItemsToTakeMaxLearningSubjects = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public int ItemsToTakeMaxLearningSubjects

        #region public int ItemsToTakeMaxStimulusSet
        private int _ItemsToTakeMaxStimulusSet = 10;
        public int ItemsToTakeMaxStimulusSet
        {
            get { return _ItemsToTakeMaxStimulusSet; }
            set
            {
                if (_ItemsToTakeMaxStimulusSet == value) return;
                _ItemsToTakeMaxStimulusSet = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public int ItemsToTakeMaxStimulusSet

        #region public int ItemsToTakeMaxParameters
        private int _ItemsToTakeMaxParameters = 10;
        public int ItemsToTakeMaxParameters
        {
            get { return _ItemsToTakeMaxParameters; }
            set
            {
                if (_ItemsToTakeMaxParameters == value) return;
                _ItemsToTakeMaxParameters = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public int ItemsToTakeMaxParameters

        #region public int ValuesToTakeMaxParameter
        private int _ValuesToTakeMaxParameter = 50;
        public int ValuesToTakeMaxParameter
        {
            get { return _ValuesToTakeMaxParameter; }
            set
            {
                if (_ValuesToTakeMaxParameter == value) return;
                _ValuesToTakeMaxParameter = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public int ValuesToTakeMaxParameter

        #region public int ValuesToTakeMaxStimulus
        private int _ValuesToTakeMaxStimulus = 10;
        public int ValuesToTakeMaxStimulus
        {
            get { return _ValuesToTakeMaxStimulus; }
            set
            {
                if (_ValuesToTakeMaxStimulus == value) return;
                _ValuesToTakeMaxStimulus = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public int ValuesToTakeMaxStimulus

        #region public int ValuesToTakeMaxRawData
        private int _ValuesToTakeMaxRawData = 10;
        public int ValuesToTakeMaxRawData
        {
            get { return _ValuesToTakeMaxRawData; }
            set
            {
                if (_ValuesToTakeMaxRawData == value) return;
                _ValuesToTakeMaxRawData = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public int ValuesToTakeMaxRawData
        

        public void Format(
            State state, 
            LearningSession learningSession, 
            LearningResult learningResult)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(
                    "LearningSession.LearningSubjects.Count = " + learningSession.LearningSubjects.Count);

            var learningSubjects
                = learningSession.LearningSubjects
                .Take(ItemsToTakeMaxLearningSubjects)
                .ToList();

            var index = 0;

            foreach(var learningSubject in learningSubjects)
            {
                var input 
                    = learningSubject.Input.ToStringFormat(
                        PrecisionStimulus,
                        ValuesToTakeMaxStimulus,
                        -1);

                stringBuilder.AppendLine(
                    "LearningSubject[" + index + "].Input = " + input);

                var output 
                    = learningSubject.Output.ToStringFormat(
                        PrecisionStimulus,
                        ValuesToTakeMaxStimulus,
                        -1);

                stringBuilder.AppendLine(
                    "LearningSubject[" + index + "].Output = " + output);

                index++;
            }

            stringBuilder.AppendLine(
                "LearningResult.Duration = " + learningResult.Duration);
            stringBuilder.AppendLine(
                "LearningResult.DurationMaximum = " + learningResult.DurationMaximum);
            stringBuilder.AppendLine(
                "LearningResult.ErrorMaximum = " + learningResult.ErrorMaximum);
            stringBuilder.AppendLine(
                "LearningResult.ErrorResult = " + learningResult.ErrorResult);
            stringBuilder.AppendLine(
                "LearningResult.Finished = " + learningResult.Finished);
            stringBuilder.AppendLine(
                "LearningResult.IsInterrupted = " + learningResult.IsInterrupted);
            stringBuilder.AppendLine(
                "LearningResult.Started = " + learningResult.Started);
            stringBuilder.AppendLine(
                "LearningResult.StepsMaximum = " + learningResult.StepsMaximum);
            stringBuilder.AppendLine(
                "LearningResult.StepsResult = " + learningResult.StepsResult);

            state.Details = stringBuilder.ToString();
        }

        public void Format(
            State state, 
            Stimulus stimulusInput, 
            Stimulus stimulusOutput)
        {
            var stringBuilder = new StringBuilder();

            var input 
                = stimulusInput.ToStringFormat(
                    PrecisionStimulus,
                    ValuesToTakeMaxStimulus,
                    -1);

            var output 
                = stimulusOutput.ToStringFormat(
                    PrecisionStimulus,
                    ValuesToTakeMaxStimulus,
                    -1);

            stringBuilder.AppendLine(
                "StimulusInput = " + input);

            stringBuilder.AppendLine(
                "StimulusOutput = " + output);

            state.Details = stringBuilder.ToString();
        }

        public void Format(
            State state, 
            byte[] rawData, 
            StimulusSet stimulusSet,
            ConvertSettings convertSettings)
        {
            var stringBuilder = new StringBuilder();

            var stimulusSetText
                = stimulusSet.ToStringFormat(
                    ItemsToTakeMaxStimulusSet, 
                    PrecisionStimulus,
                    ValuesToTakeMaxStimulus,
                    -1)
                .Replace("Values[", "StimulusSet.Values[");

            stringBuilder.AppendLine(stimulusSetText);

            stringBuilder.AppendLine(
                "RawData.Count = " + rawData.Length);

            if(rawData.Length != 0 && ValuesToTakeMaxRawData > 0)
            {
                stringBuilder.AppendLine(
                "RawData = " + rawData
                    .Take(ValuesToTakeMaxRawData)
                    .Select(v => String.Format("{0:000}", v))
                    .Aggregate((v1, v2) => String.Format("{0} {1}", v1, v2)) + " ...");
            }

            stringBuilder.AppendLine(
                "ConvertSettings.IsLearningMode = " 
                + convertSettings.IsLearningMode);

            foreach(var parameter in convertSettings.Parameters)
            {
                var value = String.Format("{0}", parameter.Value);

                if(value.Length > _ValuesToTakeMaxParameter)
                {
                    value = value.Substring(0, _ValuesToTakeMaxParameter) + " ...";
                }

                stringBuilder.AppendLine(
                "ConvertSettings.Parameters[ " + parameter.Key + "] = " 
                + value);
            }

            state.Details = stringBuilder.ToString();
        }
    }
}
