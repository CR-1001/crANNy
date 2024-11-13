/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetwork
{
    using System;
    using System.Runtime.Serialization;

    public enum Errors
    {
        ErrorCalculationNotSetUp,
        PersistanceLoad,
        PersistanceSave,
        LayerNotExistent,
        InputNeuronsMissmatch,
        LearningSubjectNeuronsCountMissmatch,
        VectorDimensionsNotEqual,
        MatrixDimensionsMissmatch,
        TypeMissmatch,
        SetupInvalid,
        ResourceNotFoundForIdentifier,
        LearningSubjectRawConversion,
        ConverterAssemblyExistingButDifferent
    }

    [Serializable] [DataContract] public class NeuralNetworkException : Exception
    {
        public Errors Error { get; set; }

        public NeuralNetworkException(Errors error) 
            : this(error, null)
        {
        }

        public NeuralNetworkException(Errors error, Exception inner) 
            : this(error, inner, null)
        {
        }

        public NeuralNetworkException(Errors error, Exception inner, String message) 
            : base(message, inner)
        {
            Error = error;
        }
    }
}
