/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetwork.Learning
{
    using System;
    using System.Runtime.Serialization;
    using CRAI.NeuralNetwork.Algebra;

    [Serializable] [DataContract] public class LearningSubjectNormalized : ILearningSubject
    {
        public LearningSubjectNormalized(Stimulus input, Stimulus output, double extent)
        {
            Input = new Stimulus(input.Normalize(extent));
            Output = new Stimulus(output.Normalize(extent));
        }

        public LearningSubjectNormalized(double[] input, double[] output, double extent)
            : this(new Stimulus(input), new Stimulus(output), extent)
        {
        }

        public LearningSubjectNormalized(ILearningSubject learningSubject, double extent)
            : this(learningSubject.Input, learningSubject.Output, extent)
        {
        }

        [DataMember]
        public Stimulus Input { get; private set; }

        [DataMember]
        public Stimulus Output { get; private set; }

        public static LearningSubjectNormalized FromString(
            String valuesSequenceInput, String valuesSequenceOutput, double extent, bool useCurrentCulture = true)
        {
            return new LearningSubjectNormalized(
                VectorOperations.FromString(valuesSequenceInput, useCurrentCulture),
                VectorOperations.FromString(valuesSequenceOutput, useCurrentCulture),
                extent);
        }
    }
}
