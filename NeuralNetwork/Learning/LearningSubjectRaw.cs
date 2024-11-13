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

    [Serializable] [DataContract] public class LearningSubjectRaw
    {
        public LearningSubjectRaw()
        {
        }

        public LearningSubjectRaw(byte[] inputRaw, double[] output)
            : this(null, inputRaw, output)
        {
        }

        public LearningSubjectRaw(String path, byte[] inputRaw, double[] output)
        {
            InputRawPath = path;
            InputRaw = inputRaw;
            Output = new Stimulus(output);
        }

        [DataMember]
        public String InputRawPath { get; set; }

        [DataMember]
        public byte[] InputRaw { get; set; }
        
        [DataMember]
        public Stimulus Output { get; set; }
    }
}
