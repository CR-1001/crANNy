/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using CRAI.Common;
    using CRAI.NeuralNetworkHost.Configurations;

    [DataContract]
    [Serializable] public class Setup : NotifyBase<Setup>
    {
        #region public NeuralNetworkConfiguration NeuralNetworkConfiguration
	    private NeuralNetworkConfiguration _NeuralNetworkConfiguration;
        [DataMember]
	    public NeuralNetworkConfiguration NeuralNetworkConfiguration
	    {
		    get { return _NeuralNetworkConfiguration; }
		    set 
		    { 
			    if(_NeuralNetworkConfiguration == value) return;
			    _NeuralNetworkConfiguration = value;
			    NotifyPropertyChanged();
		    }
	    }
	    #endregion public NeuralNetworkConfiguration NeuralNetworkConfiguration

        #region public ConverterConfiguration ConverterConfiguration
        private ConverterConfiguration _ConverterConfiguration;
        [DataMember]
        public ConverterConfiguration ConverterConfiguration
        {
            get { return _ConverterConfiguration; }
            set
            {
                if (_ConverterConfiguration == value) return;
                _ConverterConfiguration = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public ConverterConfiguration ConverterConfiguration

        #region public LearningConfiguration LearningConfiguration
        private LearningConfiguration _LearningConfiguration;
        [DataMember]
        public LearningConfiguration LearningConfiguration
        {
            get { return _LearningConfiguration; }
            set
            {
                if (_LearningConfiguration == value) return;
                _LearningConfiguration = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public LearningConfiguration LearningConfiguration
        
        #region public TimeSpan? PurgeTimeSpan
	    private TimeSpan? _PurgeTimeSpan;
        [DataMember]
	    public TimeSpan? PurgeTimeSpan
	    {
		    get { return _PurgeTimeSpan; }
		    set 
		    { 
			    if(_PurgeTimeSpan == value) return;
			    _PurgeTimeSpan = value;
			    NotifyPropertyChanged();
		    }
	    }
	    #endregion public TimeSpan? PurgeTimeSpan

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

        #region public Guid IdSuggestion
        private Guid _IdSuggestion;
        [DataMember]
        public Guid IdSuggestion
        {
            get { return _IdSuggestion; }
            set
            {
                if (_IdSuggestion == value) return;
                _IdSuggestion = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public Guid IdSuggestion

        #region public bool IsAsyncLearning
        private bool _IsAsyncLearning = true;
        [DataMember]
        public bool IsAsyncLearning
        {
            get { return _IsAsyncLearning; }
            set
            {
                if (_IsAsyncLearning == value) return;
                _IsAsyncLearning = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public bool IsAsyncLearning
    }
}
