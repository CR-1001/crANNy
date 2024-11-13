/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetwork.Learning
{
    using System;
    using System.Runtime.Serialization;
    using CRAI.NeuralNetwork;
    using CRAI.NeuralNetwork.Algebra;

    [Serializable] [DataContract] public class LearningSubject : ILearningSubject
    {
        public LearningSubject(double[] input, double[] output)
        {
            Input = new Stimulus(input);
            Output = new Stimulus(output);
        }

        [DataMember]
        public Stimulus Input { get; set; }
        
        [DataMember]
        public Stimulus Output { get; set; }

        public static LearningSubject FromString(
            String valuesSequenceInput, String valuesSequenceOutput, bool useCurrentCulture = true)
        {
            return new LearningSubject(
                VectorOperations.FromString(valuesSequenceInput, useCurrentCulture),
                VectorOperations.FromString(valuesSequenceOutput, useCurrentCulture));
        }
    }
}
