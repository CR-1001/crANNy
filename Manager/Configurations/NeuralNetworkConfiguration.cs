/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetworkHost.Configurations
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Linq;
    using CRAI.Common;
    using CRAI.NeuralNetwork;
    using CRAI.NeuralNetwork.Activation;
    using CRAI.NeuralNetwork.Algebra;
    using CRAI.NeuralNetwork.Randomizer;
    using CRAI.NeuralNetworkHost.NeuralNetworkBuilders;
    using NeuralNetworkHost;

    [DataContract] [Serializable] 
    public class NeuralNetworkConfiguration 
        : ParametersBase<NeuralNetworkConfiguration>
    {

        #region public int[] Tiers
        private int[] _Tiers = new int[0];
        [DataMember]
        public int[] Tiers
        {
            get { return _Tiers; }
            set
            {
                if (_Tiers == value) return;
                _Tiers = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public int[] Tiers

        #region public ObservableCollection<Matrix> MatrixWeightsThresholds
        private ObservableCollection<Matrix> _MatrixWeightsThresholds = new ObservableCollection<Matrix>();
        [DataMember]
        public ObservableCollection<Matrix> MatrixWeightsThresholds
        {
            get { return _MatrixWeightsThresholds; }
            set
            {
                if (_MatrixWeightsThresholds == value) return;
                _MatrixWeightsThresholds = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public ObservableCollection<Matrix> MatrixWeightsThresholds

        #region public TypeDescription<INeuralNetworkBuilder> TypeDescriptionNeuralNetworkBuilder
        private TypeDescription<INeuralNetworkBuilder> _TypeDescriptionNeuralNetworkBuilder;
        [DataMember]
        public TypeDescription<INeuralNetworkBuilder> TypeDescriptionNeuralNetworkBuilder
        {
            get { return _TypeDescriptionNeuralNetworkBuilder; }
            set
            {
                if (_TypeDescriptionNeuralNetworkBuilder == value) return;
                _TypeDescriptionNeuralNetworkBuilder = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public TypeDescription<INeuralNetworkBuilder> TypeDescriptionNeuralNetworkBuilder

        #region public ObservableCollection<TypeDescription<IActivation>> TypeDescriptionsActivation
        private ObservableCollection<TypeDescription<IActivation>> _TypeDescriptionsActivation
            = new ObservableCollection<TypeDescription<IActivation>>();
        [DataMember]
        public ObservableCollection<TypeDescription<IActivation>> TypeDescriptionsActivation
        {
            get { return _TypeDescriptionsActivation; }
            set
            {
                if (_TypeDescriptionsActivation == value) return;
                _TypeDescriptionsActivation = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public ObservableCollection<TypeDescription<IActivation>> TypeDescriptionsActivation

        #region public ObservableCollection<TypeDescription<IRandomizer>> TypeDescriptionsRandomizer
        private ObservableCollection<TypeDescription<IRandomizer>> _TypeDescriptionsRandomizer
            = new ObservableCollection<TypeDescription<IRandomizer>>();
        [DataMember]
        public ObservableCollection<TypeDescription<IRandomizer>> TypeDescriptionsRandomizer
        {
            get { return _TypeDescriptionsRandomizer; }
            set
            {
                if (_TypeDescriptionsRandomizer == value) return;
                _TypeDescriptionsRandomizer = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public ObservableCollection<TypeDescription<IRandomizer>> TypeDescriptionsRandomizer
    }
}
