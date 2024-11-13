/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetworkHost.Configurations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;
    using CRAI.Common;
    using CRAI.NeuralNetwork;

    [DataContract]
    [Serializable]
    public class TypeDescription<T> : NotifyBase<TypeDescription<T>>
    {
        public TypeDescription()
        {

        }

        public TypeDescription(Type type)
        {
            Type = type;
        }

        public TypeDescription(String typeFullName)
        {
            TypeFullName = typeFullName;
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext streamingContext)
        {
            if (TypeFullName != null && AssemblyFullName != null && Type == null)
            {
                Type = TypeResolver.GetType(TypeFullName, AssemblyFullName);
            }
        }

        #region public Type Type
        private Type _Type;
        [IgnoreDataMember]
        public Type Type
        {
            get
            {
                if (_Type == null && _TypeFullName != null && _AssemblyFullName != null)
                {
                    return TypeResolver.GetType(_TypeFullName, _AssemblyFullName);
                }

                return _Type;
            }
            set
            {
                if (_Type == value) return;

                if (!typeof(T).IsAssignableFrom(value))
                {
                    throw new NeuralNetworkException(Errors.TypeMissmatch);
                }

                _Type = value;

                TypeFullName = value == null ? null : value.FullName;
                AssemblyFullName = value == null ? null : value.Assembly.FullName;

                NotifyPropertyChanged();
            }
        }
        #endregion public Type Type

        #region public String TypeFullName
        private String _TypeFullName;
        [DataMember]
        public String TypeFullName
        {
            get
            {
                if (_TypeFullName == null && Type != null)
                {
                    return Type.FullName;
                }

                return _TypeFullName;
            }
            set
            {
                if (_TypeFullName == value) return;
                _TypeFullName = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(this, c => c.Type);
            }
        }
        #endregion public String TypeFullName

        #region public String AssemblyFullName
        private String _AssemblyFullName;
        [DataMember]
        public String AssemblyFullName
        {
            get 
            { 
                if (_AssemblyFullName == null && Type != null)
                {
                    return Type.Assembly.FullName;
                }

                return _AssemblyFullName; 
            }
            set
            {
                if (_AssemblyFullName == value) return;
                _AssemblyFullName = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public String AssemblyFullName
        

        public override bool Equals(Object obj)
        {
            var other = obj as TypeDescription<T>;

            if (other == null) return false;

            return Object.Equals(other.Type, Type)
                || (Object.Equals(other.TypeFullName, TypeFullName) 
                    && Object.Equals(other.AssemblyFullName, AssemblyFullName));
        }

        public override int GetHashCode()
        {
            if (Type != null) return Type.GetHashCode();
            if (TypeFullName != null) return Type.GetHashCode();
            return base.GetHashCode();
        }

        public override string ToString()
        {
            if (Type != null) return Type.ToString();
            if (TypeFullName != null) return TypeFullName;
            return base.ToString();
        }
    }
}
