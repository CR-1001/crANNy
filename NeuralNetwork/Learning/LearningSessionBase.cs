/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetwork.Learning
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;
    using CRAI.Common;

    [Serializable] [DataContract] public abstract class LearningSessionBase : NotifyBase<LearningSessionBase>
    {
        
        #region public Guid Id
        private Guid _Id;
        [DataMember]
        public Guid Id
        {
            get { return _Id; }
            set
            {
                if (_Id == value) return;
                _Id = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public Guid Id

        #region public int CountInputNeurons
        private int _CountInputNeurons;
        [DataMember]
        public int CountInputNeurons
        {
            get { return _CountInputNeurons; }
            set
            {
                if (_CountInputNeurons == value) return;
                _CountInputNeurons = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public int CountInputNeurons

        #region public int CountOutputNeurons
        private int _CountOutputNeurons;
        [DataMember]
        public int CountOutputNeurons
        {
            get { return _CountOutputNeurons; }
            set
            {
                if (_CountOutputNeurons == value) return;
                _CountOutputNeurons = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public int CountOutputNeurons

        #region public IDictionary<String, Object> Parameters
        private IDictionary<String, Object> _Parameters = new Dictionary<String, Object>();
        [DataMember]
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

        #region public String Description
        private String _Description;
        [DataMember]
        public String Description
        {
            get { return _Description; }
            set
            {
                if (_Description == value) return;
                _Description = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public String Description
        
    }
}
