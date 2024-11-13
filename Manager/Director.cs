/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CRAI.Common;
using CRAI.Logging;
using CRAI.NeuralNetwork;
using CRAI.NeuralNetwork.Activation;
using CRAI.NeuralNetwork.Algebra;
using CRAI.NeuralNetwork.Learning;
using CRAI.NeuralNetwork.Randomizer;
using CRAI.NeuralNetworkHost.Configurations;
using CRAI.NeuralNetworkHost.NeuralNetworkBuilders;

namespace CRAI.NeuralNetworkHost
{
    [Serializable] public class Director
    {
        private static Object _LockExportAssemblies = new Object();

        public NeuralNetworkConfiguration Extract(INeuralNetwork NeuralNetwork)
        {
            var NeuralNetworkConfiguration = new NeuralNetworkConfiguration();           

            NeuralNetworkConfiguration.TypeDescriptionNeuralNetworkBuilder 
                = new TypeDescription<INeuralNetworkBuilder>(
                    ObtainTypeNeuralNetworkBuilder(NeuralNetwork));

            NeuralNetworkConfiguration.Tiers 
                = NeuralNetwork.Tiers
                .Select(t => t.CountNeurons)
                .ToArray();

            NeuralNetworkConfiguration.MatrixWeightsThresholds
                = NeuralNetwork.Tiers
                .Select(t => t.MatrixWeightsThresholds == null 
                    ? null : t.MatrixWeightsThresholds.Copy())
                .ToObservableCollection();

            NeuralNetworkConfiguration.TypeDescriptionsActivation
                = NeuralNetwork.Tiers
                .Select(t => new TypeDescription<IActivation>(t.Activation.GetType()))
                .TrimEnd()
                .ToObservableCollection();

            NeuralNetworkConfiguration.TypeDescriptionsRandomizer
                = NeuralNetwork.Tiers
                .Select(t => new TypeDescription<IRandomizer>(t.Randomizer.GetType()))
                .TrimEnd()
                .ToObservableCollection();

            return NeuralNetworkConfiguration;
        }

        public ConverterConfiguration Extract(IConverter converter)
        {
            var converterConfiguration = new ConverterConfiguration();

            foreach(var parameter in converter.ExtractParameters())
            {
                converterConfiguration.Parameters[parameter.Key] 
                    = Serialization.CloneBinary(parameter.Value);
            }

            converterConfiguration.TypeDescription 
                = new TypeDescription<IConverter>(converter.GetType());

            return converterConfiguration;
        }

        public static Type ObtainTypeNeuralNetworkBuilder(INeuralNetwork NeuralNetwork)
        {
            var typeNeuralNetworkBuilders
                = Assembly.GetExecutingAssembly()
                .ExportedTypes
                .Where(
                    delegate(Type t)
                    {
                        if (!t.IsClass || !typeof(INeuralNetworkBuilder).IsAssignableFrom(t))
                        {
                            return false;
                        }

                        var hasMarkerInterface
                            = t.GetInterfaces()
                            .Any(i => i.IsGenericType
                                && i.GetGenericTypeDefinition().Equals(typeof(INeuralNetworkBuilder<>))
                                && i.GetGenericArguments().SequenceEqual(new[] { NeuralNetwork.GetType() }));

                        return hasMarkerInterface;
                    })
                .ToList();

            if (typeNeuralNetworkBuilders.Count != 1)
            {
                throw new NeuralNetworkException(Errors.TypeMissmatch);
            }

            var typeNeuralNetworkBuilder = typeNeuralNetworkBuilders.First();

            return typeNeuralNetworkBuilder;
        }

        public INeuralNetwork Construct(NeuralNetworkConfiguration NeuralNetworkConfiguration)
        {
            var NeuralNetworkBuilder = CreateInstance<INeuralNetworkBuilder>(
                NeuralNetworkConfiguration.TypeDescriptionNeuralNetworkBuilder);

            var randomizers = new List<IRandomizer>();
            foreach (var typeRandomizer in NeuralNetworkConfiguration.TypeDescriptionsRandomizer)
            {
                var randomizer = CreateInstance<IRandomizer>(typeRandomizer);
                randomizers.Add(randomizer);
            }
            NeuralNetworkBuilder.SetupRandomizers(randomizers);
            
            var activations = new List<IActivation>();
            foreach (var typeActivation in NeuralNetworkConfiguration.TypeDescriptionsActivation)
            {
                var activation = CreateInstance<IActivation>(typeActivation);
                activations.Add(activation);
            }
            NeuralNetworkBuilder.SetupActivations(activations);
            
            NeuralNetworkBuilder.SetupTiers(NeuralNetworkConfiguration.Tiers);

            NeuralNetworkBuilder.SetupIsFillRandom(
                !NeuralNetworkConfiguration.MatrixWeightsThresholds.Any()
                || NeuralNetworkConfiguration.MatrixWeightsThresholds.All(m => m == null));
            
            NeuralNetworkBuilder.SetupParameters(NeuralNetworkConfiguration.Parameters);

            NeuralNetworkBuilder.SetupInstance();

            for (var i = 0; i < NeuralNetworkConfiguration.MatrixWeightsThresholds.Count; i++)
            {
                var matrixWeightsThresholds = NeuralNetworkConfiguration.MatrixWeightsThresholds[i];
                NeuralNetworkBuilder.SetupMatrixWeightsThresholds(i, matrixWeightsThresholds);
            }

            var NeuralNetwork = NeuralNetworkBuilder.GetNeuralNetwork();

            return NeuralNetwork;
        }

        public IConverter Construct(ConverterConfiguration converterConfiguration)
        {
            IConverter converter;

            //if (converterConfiguration.Assemblies != null 
            //    && converterConfiguration.Assemblies.Any())
            //{
            //    converter = new ConverterProxyAppDomain(
            //        converterConfiguration.TypeDescription,
            //        true,
            //        converterConfiguration.Assemblies.ToArray());
            //}
            //else
            //{
                ExportAssemblies(converterConfiguration.AssemblyDescriptions);

                converter = CreateInstance<IConverter>(
                    converterConfiguration.TypeDescription);
            //}

            converter.Setup(converterConfiguration.Parameters);

            return converter;
        }

        private void ExportAssemblies(IEnumerable<AssemblyDescription> assemblyDescriptions)
        {
            lock (_LockExportAssemblies)
            {
                var directoryExecutingAssembly = new FileInfo(
                    Assembly.GetExecutingAssembly().Location).DirectoryName;

                var directoryDependencies = Path.Combine(directoryExecutingAssembly /*, "Dependencies" */);

                if (!Directory.Exists(directoryDependencies))
                {
                    Directory.CreateDirectory(directoryDependencies);
                }

                var filesNotLoaded = new List<String>();

                foreach (var assemblyDescription in assemblyDescriptions)
                {
                    var file = Path.Combine(
                        directoryDependencies,
                        assemblyDescription.Name);

                    var isToIgnore = assemblyDescription.Name
                        .IsIn(GetAssembliesToIgnore());

                    if (isToIgnore)
                    {
                        continue;
                    }

                    var isExisting = File.Exists(file);

                    var isLoaded = isExisting
                        && TypeResolver.GetAssembliesAppDomain()
                            .Any(a => a.Location == file);

                    var isEqual = isExisting
                        && File.ReadAllBytes(file)
                            .SequenceEqual(assemblyDescription.Raw);

                    if (!isLoaded && isEqual)
                    {
                        filesNotLoaded.Add(file);
                    }
                    else if (!isLoaded && !isEqual)
                    {
                        File.Delete(file);
                        File.WriteAllBytes(file, assemblyDescription.Raw);
                        filesNotLoaded.Add(file);
                    }
                    else if (isLoaded && !isEqual)
                    {
                        throw new NeuralNetworkException(
                            Errors.ConverterAssemblyExistingButDifferent, null, file);
                    }
                }

                foreach (var file in filesNotLoaded)
                {
                    try
                    {
                        AppDomain.CurrentDomain.Load(file);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }

        private String[] GetAssembliesToIgnore()
        {
            return new[]
                {
                    typeof(Notify),    // Common
                    typeof(ILogging),  // Logging
                    typeof(Director),  // Manager
                    typeof(INeuralNetwork) // NeuralNetwork
                }
                .Select(t => new FileInfo(t.Assembly.Location).Name)
                .ToArray();
        }

        public ILearning Construct(LearningConfiguration learningConfiguration, INeuralNetwork NeuralNetwork)
        {
            var learning = CreateInstance<ILearning>(learningConfiguration.TypeDescription);

            learning.Setup(
                NeuralNetwork,
                learningConfiguration.ErrorEstimation,
                learningConfiguration.StepsMaximum,
                learningConfiguration.ErrorMaximum,
                learningConfiguration.LearningRate,
                learningConfiguration.Momentum,
                learningConfiguration.Parameters);

            return learning;
        }

        private T CreateInstance<T>(TypeDescription<T> typeDescription)
        {
            var instance = AppDomain.CurrentDomain.CreateInstanceAndUnwrap(
                typeDescription.AssemblyFullName,
                typeDescription.TypeFullName);

            return (T)instance;
        }
    }



}
