/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetwork.Learning
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using CRAI.Common;
    using CRAI.NeuralNetwork;

    [KnownType(typeof(LearningSubject))]
    [Serializable] [DataContract] public class LearningSession : LearningSessionBase
    {
        #region public ObservableCollection<ILearningSubject> LearningSubjects
        private ObservableCollection<ILearningSubject> _LearningSubjects = new ObservableCollection<ILearningSubject>();
        [DataMember]
        public ObservableCollection<ILearningSubject> LearningSubjects
        {
            get { return _LearningSubjects; }
            set
            {
                if (_LearningSubjects == value) return;
                _LearningSubjects = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public ObservableCollection<ILearningSubject> LearningSubjects

        public StimulusSet GetInput()
        {
            return new StimulusSet(LearningSubjects.Select(l => l.Input));
        }

        public StimulusSet GetOutput()
        {
            return new StimulusSet(LearningSubjects.Select(l => l.Output));
        }

        public LearningSession()
        {

        }

        public LearningSession(int countInputNeurons, int countOutputNeurons)
        {
            CountInputNeurons = countInputNeurons;
            CountOutputNeurons = countOutputNeurons;
        }

        public void Append(ILearningSubject learningSubject)
        {
            if (learningSubject.Input.Values.Length != CountInputNeurons 
                || learningSubject.Output.Values.Length != CountOutputNeurons)
            {
                throw new NeuralNetworkException(Errors.LearningSubjectNeuronsCountMissmatch);
            }

            _LearningSubjects.Add(learningSubject);
        }

        public ILearningSubject this[int i]
        {
            get { return LearningSubjects[i]; }
            set { LearningSubjects[i] = value; }
        }
    }
}
