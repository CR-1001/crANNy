/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.Server
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;
    using CRAI.Common;
    using CRAI.NeuralNetwork;
    using CRAI.NeuralNetwork.Learning;

    public enum Types
    {
        Setup,
        ExtractSetup,
        Learning,
        LearningInterrupted,
        ConvertInput,
        ConvertOutput,
        Compute,
        Release,
        ConvertAndCompute,
        Diagnose,
    }

    [DataContract]
    [Serializable] public class StateSet : NotifyBase<StateSet>
    {
        public StateSet()
        {

        }

        public StateSet(Identifier identifier)
        {
            Identifier = identifier;
        }

        #region public ObservableCollection<State> Items
        private ObservableCollection<State> _Items 
            = new ObservableCollection<State>();
        [DataMember]
        public ObservableCollection<State> Items
        {
            get { return _Items; }
            private set
            {
                if (_Items == value) return;
                _Items = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public ObservableCollection<State> Items

        #region public Identifier Identifier
        private Identifier _Identifier;
        [DataMember]
        public Identifier Identifier
        {
            get { return _Identifier; }
            private set
            {
                if (_Identifier == value) return;
                _Identifier = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public Identifier Identifier

        public static StateSet Empty()
        {
            return new StateSet();
        }
    }

    [DataContract]
    [Serializable] public class State : NotifyBase<State>, IDisposable
    {
        public State()
        {

        }

        public State(Types type)
        {
            Started = DateTime.Now;
            Type = type;
        }

        #region public Types Type
        private Types _Type;
        [DataMember]
        public Types Type
        {
            get { return _Type; }
            private set
            {
                if (_Type == value) return;
                _Type = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public Types Type

        #region public DateTime Started
        private DateTime _Started;
        [DataMember]
        public DateTime Started
        {
            get { return _Started; }
            private set
            {
                if (_Started == value) return;
                _Started = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public DateTime Started

        #region public DateTime? Finished
        private DateTime? _Finished;
        [DataMember]
        public DateTime? Finished
        {
            get { return _Finished; }
            set
            {
                if (_Finished == value) return;
                _Finished = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public DateTime? Finished

        #region public ObservableCollection<Guid> IdsData
        private ObservableCollection<Guid> _IdsData
            = new ObservableCollection<Guid>();
        [DataMember]
        public ObservableCollection<Guid> IdsData
        {
            get { return _IdsData; }
            private set
            {
                if (_IdsData == value) return;
                _IdsData = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public ObservableCollection<Guid> IdsData

        #region public String Details
        private String _Details;
        [DataMember]
        public String Details
        {
            get { return _Details; }
            set
            {
                if (_Details == value) return;
                _Details = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public String Details
        
        #region public ObservableCollection<Exception> Exceptions
        private ObservableCollection<Exception> _Exceptions
            = new ObservableCollection<Exception>();
        [DataMember]
        public ObservableCollection<Exception> Exceptions
        {
            get { return _Exceptions; }
            private set
            {
                if (_Exceptions == value) return;
                _Exceptions = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public ObservableCollection<Exception> Exceptions

        public void Dispose()
        {
            Finished = DateTime.Now;
        }

        
    }
}
