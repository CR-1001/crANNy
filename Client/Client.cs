/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.Client
{
    using System;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using CRAI.NeuralNetwork;
    using CRAI.NeuralNetwork.Learning;
    using CRAI.NeuralNetworkHost.Configurations;
    using CRAI.Server;

    public class Client : ClientBase<IService>, IService
    {
        public Client(Uri baseAddress, Binding binding)
            : base(new ServiceEndpoint(
                ContractDescription.GetContract(typeof(IService)),
                binding,
                new EndpointAddress(baseAddress)))
        {

        }

        public Identifier Setup(Setup setup)
        {
            return Channel.Setup(setup);
        }

        public Setup ExtractSetup(Identifier identifier)
        {
            return Channel.ExtractSetup(identifier);
        }

        public LearningResult Learn(Identifier identifier, LearningConfiguration learningConfiguration)
        {
            return Channel.Learn(identifier, learningConfiguration);
        }

        public LearningResult InterruptLearn(Identifier identifier)
        {
            return Channel.InterruptLearn(identifier);
        }

        public void Release(Identifier identifier)
        {
            Channel.Release(identifier);
        }

        public byte[] ConvertAndCompute(Identifier identifier, byte[] rawData, ConvertSettings convertSettings)
        {
            return Channel.ConvertAndCompute(identifier, rawData, convertSettings);
        }

        public Stimulus Compute(Identifier identifier, Stimulus stimulus)
        {
            return Channel.Compute(identifier, stimulus);
        }

        public StateSet Diagnose(Identifier identifier)
        {
            return Channel.Diagnose(identifier);
        }


        public Identifier[] ObtainInstances()
        {
            return Channel.ObtainInstances();
        }
    }
}
