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

    [KnownType("GetTypesSupported")] [DataContract] [Serializable]
    public abstract class ParametersBase<T> : NotifyBase<ParametersBase<T>>
    {
        public static Type[] GetTypesSupported()
        {
            var types = TypeResolver
                .GetTypesTransmittable()
                .ToArray();

            return types;
        }
        
        #region public Dictionary<String, Object> Parameters
        private Dictionary<String, Object> _Parameters = new Dictionary<String, Object>();
        [DataMember]
        public Dictionary<String, Object> Parameters
        {
            get { return _Parameters; }
            set
            {
                if (_Parameters == value) return;
                _Parameters = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public Dictionary<String, Object> Parameters
    }
}
