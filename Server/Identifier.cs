/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CRAI.Common;

namespace CRAI.Server
{
    [DataContract]
    [Serializable] public class Identifier : NotifyBase<Identifier>
    {
        #region public String Name
        private String _Name;
        [DataMember]
        public String Name
        {
            get { return _Name; }
            set
            {
                if (_Name == value) return;
                _Name = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public String Name

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

        #region public DateTime Created
        private DateTime _Created;
        [DataMember]
        public DateTime Created
        {
            get { return _Created; }
            set
            {
                if (_Created == value) return;
                _Created = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public DateTime Created

        #region public int CountCompute
        private int _CountCompute = 1;
        [DataMember]
        public int CountCompute
        {
            get { return _CountCompute; }
            set
            {
                if (_CountCompute == value) return;
                _CountCompute = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public int CountCompute

        #region public int CountConvert
        private int _CountConvert = 1;
        [DataMember]
        public int CountConvert
        {
            get { return _CountConvert; }
            set
            {
                if (_CountConvert == value) return;
                _CountConvert = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public int CountConvert
        
        
        public override bool Equals(Object obj)
        {
            var other = obj as Identifier;

            if (other == null) return false;

            if(Id != other.Id) return false;
           
            return true;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static Identifier New(Guid id)
        {
            return new Identifier()
            {
                Id = id
            };
        }

        public static Identifier New(Setup setup)
        {
            return new Identifier()
            {
                Id = setup.IdSuggestion,
                Name = setup.Name,
                Description = setup.Description,
                CountCompute = setup.CountCompute,
                CountConvert = setup.CountConvert,
                Created = DateTime.Now,
            };
        }
    }
}
