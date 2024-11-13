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

    [DataContract]
    [Serializable] public class StateHistory : NotifyBase<StateHistory>
    {
        private Object _Lock = new Object();

        #region public ObservableCollection<StateSet> Items
        private ObservableCollection<StateSet> _Items 
            = new ObservableCollection<StateSet>();
        [DataMember]
        public ObservableCollection<StateSet> Items
        {
            get { return _Items; }
            private set
            {
                if (_Items == value) return;
                _Items = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public ObservableCollection<StateSet> Items

        public State AppendState(Identifier identifier, Types type)
        {
            var state = new State(type);

            lock(_Lock)
            {
                var stateSet = GetStateSet(identifier);

                if(stateSet == null)
                {
                    stateSet = new StateSet(identifier);
                    Items.Add(stateSet);
                }

                stateSet.Items.Add(state);
            }

            return state;
        }

        public StateSet GetStateSet(Identifier identifier)
        {
            var stateSet = Items
                .FirstOrDefault(s => s.Identifier.Equals(identifier));

            return stateSet;
        }

        public IEnumerable<Identifier> GetIdentifiers()
        {
            return 
                Items
                .Select(s => s.Identifier)
                .Distinct()
                .ToList();
        }
    }
}
