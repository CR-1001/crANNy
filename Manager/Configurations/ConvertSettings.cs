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

    [Serializable]
    [DataContract]
    public class ConvertSettings : ParametersBase<ConvertSettings>
    {
        #region public bool IsLearningMode
        private bool _IsLearningMode;
        [DataMember]
        public bool IsLearningMode
        {
            get { return _IsLearningMode; }
            set
            {
                if (_IsLearningMode == value) return;
                _IsLearningMode = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public bool IsLearningMode

    }
}
