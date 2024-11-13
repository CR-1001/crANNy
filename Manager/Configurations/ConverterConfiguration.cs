/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetworkHost.Configurations
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    using CRAI.Common;
    using CRAI.NeuralNetwork;

    [DataContract] [Serializable]
    public class ConverterConfiguration 
        : ParametersBase<ConverterConfiguration>
    {
        #region public TypeDescription<IConverter> TypeDescription
        private TypeDescription<IConverter> _TypeDescription
            = new TypeDescription<IConverter>();
        [DataMember]
        public TypeDescription<IConverter> TypeDescription
        {
            get { return _TypeDescription; }
            set
            {
                if (_TypeDescription == value) return;
                _TypeDescription = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public TypeDescription<IConverter> TypeDescription

        #region public ObservableCollection<AssemblyDescription> AssemblyDescriptions
        private ObservableCollection<AssemblyDescription> _AssemblyDescriptions
            = new ObservableCollection<AssemblyDescription>();
        [DataMember]
        public ObservableCollection<AssemblyDescription> AssemblyDescriptions
        {
            get { return _AssemblyDescriptions; }
            set
            {
                if (_AssemblyDescriptions == value) return;
                _AssemblyDescriptions = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public ObservableCollection<AssemblyDescription> AssemblyDescriptions

        #region public String AssembliesDescription
        private String _AssembliesDescription = String.Empty;
        [DataMember]
        public String AssembliesDescription
        {
            get { return _AssembliesDescription; }
            set
            {
                if (_AssembliesDescription == value) return;
                _AssembliesDescription = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(this, c => c.AssemblyDescriptions);
            }
        }
        #endregion public String AssembliesDescription

        public Object this[String key]
        {
            get 
            {
                Object value = null;
                
                Parameters.TryGetValue(key, out value);
                
                return value; 
            }
            set 
            { 
                Parameters[key] = value; 
            }
        }
    }
}
