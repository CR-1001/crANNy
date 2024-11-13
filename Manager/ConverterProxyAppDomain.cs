/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetworkHost
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using CRAI.Common;
    using CRAI.NeuralNetwork;
    using CRAI.NeuralNetworkHost;
    using CRAI.NeuralNetworkHost.Configurations;

    [Serializable] public class ConverterProxyAppDomain : IConverter
    {    

        public ConverterProxyAppDomain(
            TypeDescription<IConverter> typeDescription, bool reuseConstructedConverter, params Byte[][] assemblies)
        {
            Reuse = reuseConstructedConverter;

            TypeDescription = typeDescription;

            LoadAssemblies(assemblies);
        }

        public ConverterProxyAppDomain(
            String typeFullName, bool reuseConstructedConverter, params Byte[][] assemblies)
        {
            Reuse = reuseConstructedConverter;

            TypeDescription = new TypeDescription<IConverter>(typeFullName);

            LoadAssemblies(assemblies);
        }

        private void LoadAssemblies(Byte[][] assemblies)
        {
            AppDomainExecuter = new AppDomainExecuter();

            AppDomainExecuter.CreateTemporaryAppDomain();

            foreach (var assembly in assemblies)
            {
                AppDomainExecuter.LoadAssembly(assembly);
            }
        }

        public Guid Id { get; set; }

        public AppDomainExecuter AppDomainExecuter { get; private set; }
        
        public IDictionary<String, Object> Parameters { get; private set; }

        public TypeDescription<IConverter> TypeDescription { get; private set; }

        public bool Reuse { get; private set; }

        private Object _LockConstruct = new Object();

        public StimulusSet Convert(byte[] rawData, ConvertSettings convertSettings)
        {
            var result = AppDomainExecuter.Invoke(
                new Func<TypeDescription<IConverter>, Dictionary<String, Object>, Byte[], ConvertSettings, Boolean, Object>((t, p, r, c, x) =>
                {
                    var converter = Construct(x, t, p);

                    var stimulusSet = converter.Convert(r, c);

                    return stimulusSet;

                }), TypeDescription, Parameters, rawData, convertSettings, Reuse);

            return result as StimulusSet;
        }

        public byte[] Convert(StimulusSet stimulusSet, ConvertSettings convertSettings)
        {
            var result = AppDomainExecuter.Invoke(
                new Func<TypeDescription<IConverter>, Dictionary<String, Object>, StimulusSet, ConvertSettings, Boolean, Object>((t, p, s, c, x) =>
                {
                    var converter = Construct(x, t, p);

                    var rawData = converter.Convert(s, c);

                    return rawData;

                }), TypeDescription, Parameters, stimulusSet, convertSettings, Reuse);

            return result as byte[];
        }

        private IConverter Construct(
            bool reuseExisting, 
            TypeDescription<IConverter> typeDescription,
            Dictionary<String, Object> parameters)
        {
            IConverter converter;

            if (!reuseExisting)
            {
                converter = Construct(typeDescription, parameters);
            }
            else
            {
                lock (_LockConstruct)
                {
                    converter = AppDomain.CurrentDomain.GetData(
                        typeDescription.TypeFullName) as IConverter;

                    if (converter == null)
                    {
                        converter = Construct(typeDescription, parameters);
                        AppDomain.CurrentDomain.SetData(
                            typeDescription.TypeFullName, converter);
                    }
                }
            }

            return converter;
        }

        private IConverter Construct(
            TypeDescription<IConverter> typeDescription, 
            Dictionary<String, Object> parameters)
        {
            Type type;

            if (typeDescription.Type == null 
                || typeDescription.Type == typeof(IConverter))
            {
                var typesConverter
                    = AppDomainExecuter.AppDomainExecution
                    .GetAssemblies()
                    .SelectMany(a => a.ExportedTypes)
                    .Where(t => t.IsClass
                        && typeof(IConverter).IsAssignableFrom(t)
                        && t != typeof(ConverterProxyAppDomain))
                    .ToList();

                if (typeDescription.TypeFullName != null)
                {
                    typesConverter = typesConverter
                        .Where(t => t.FullName == typeDescription.TypeFullName)
                        .ToList();
                }

                if (typesConverter.Count != 1)
                {
                    throw new Exception("IConverter instance not specified or multiple IConverter classes found in referenced assemblies.");
                }

                type = typesConverter.First();
            }
            else
            {
                type = typeDescription.Type;
            }

            var converter = (IConverter)Activator.CreateInstance(type);

            converter.Setup(parameters);

            return converter;
        }



        public void Setup(IDictionary<String, Object> parameters)
        {
            Parameters = parameters;
        }



        public IEnumerable<KeyValuePair<String, Object>> ExtractParameters()
        {
            foreach(var item in Parameters)
            {
                yield return item;
            }
        }
    }
}
