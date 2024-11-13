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
    using System.ServiceModel;
    using System.Text;
    using CRAI.NeuralNetwork;
    using CRAI.NeuralNetwork.Learning;
    using CRAI.NeuralNetworkHost.Configurations;

    [ServiceContract(Namespace = Various.Namespace)]
    public interface IService
    {
        [OperationContract]
        Identifier Setup(Setup setup);

        [OperationContract]
        Setup ExtractSetup(Identifier identifier);

        [OperationContract]
        LearningResult Learn(Identifier identifier, LearningConfiguration learningConfiguration);

        [OperationContract]
        LearningResult InterruptLearn(Identifier identifier);

        [OperationContract]
        void Release(Identifier identifier);

        [OperationContract]
        byte[] ConvertAndCompute(Identifier identifier, byte[] rawData, ConvertSettings convertSettings);

        [OperationContract]
        Stimulus Compute(Identifier identifier, Stimulus stimulus);

        [OperationContract]
        StateSet Diagnose(Identifier identifier);

        [OperationContract]
        Identifier[] ObtainInstances();
    }

}
