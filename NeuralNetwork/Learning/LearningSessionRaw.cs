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
    using System.Text;
    using System.Threading.Tasks;
    
    [Serializable] [DataContract] public class LearningSessionRaw : LearningSessionBase
    {
        #region public ObservableCollection<LearningSubjectRaw> LearningSubjects
        private ObservableCollection<LearningSubjectRaw> _LearningSubjects
            = new ObservableCollection<LearningSubjectRaw>();
        [DataMember]
        public ObservableCollection<LearningSubjectRaw> LearningSubjects
        {
            get { return _LearningSubjects; }
            set
            {
                if (_LearningSubjects == value) return;
                _LearningSubjects = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public ObservableCollection<LearningSubjectRaw> LearningSubjects
    }
}
