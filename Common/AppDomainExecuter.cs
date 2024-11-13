/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.Common
{
    using System;
    using System.Linq;
    using System.Diagnostics;
    using System.Reflection;

    [Serializable] public class AppDomainExecuter
    {
        public AppDomain AppDomainExecution { get; set; }

        public void CreateTemporaryAppDomain()
        {
            var appDomainSetup = new AppDomainSetup();
            
            appDomainSetup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;

            var name = String.Format("AppDomain_Temporary_{0}", Guid.NewGuid());

            AppDomainExecution = AppDomain.CreateDomain(
                name,
                null,
                appDomainSetup);

            LoadAssembliesFromCurrentAppDomain();
        }

        public void LoadAssembly(String assemblyPath)
        {
            if (assemblyPath == null) return;

            var assemblyName = AssemblyName.GetAssemblyName(assemblyPath);

            LoadAssembly(assemblyName);
        }

        public void LoadAssembly(byte[] assembly)
        {
            if (assembly == null) return;

            Invoke(new Action<Byte[]>(d => AppDomainExecution.Load(d)), assembly);
        }

        public void LoadAssembly(AssemblyName assemblyName)
        {
            if (assemblyName == null) return;

            Invoke(new Action<AssemblyName>(d => AppDomainExecution.Load(d)), assemblyName);
        }

        public void LoadAssembly(Assembly assembly)
        {
            if (assembly == null) return;

            LoadAssembly(assembly.GetName());
        }

        public void LoadAssembliesFromAppDomain(AppDomain appDomain)
        {
            var assemblies = appDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                LoadAssembly(assembly);
            }
        }

        public void LoadAssembliesFromCurrentAppDomain()
        {
            LoadAssembliesFromAppDomain(AppDomain.CurrentDomain);
        }

        public void Unload()
        {
            AppDomain.Unload(AppDomainExecution);
        }

        public Object Invoke(Delegate invoke, params Object[] arguments)
        {
            var dataKey = Guid.NewGuid().ToString();

            var invoker = new Invoker(dataKey, invoke, arguments);

            AppDomainExecution.DoCallBack(new CrossAppDomainDelegate(invoker.DynamicInvoke));

            var result = AppDomainExecution.GetData(dataKey);

            return result;
        }
    }

    [Serializable] public class Invoker
    {
        public Delegate Invoke { get; private set; }
        public Object[] Arguments { get; private set; }
        public String Key { get; private set; }

        public Invoker(String key, Delegate invoke, Object[] arguments)
        {
            Key = key;
            Invoke = invoke;
            Arguments = arguments;
        }

        public void DynamicInvoke()
        {
            Debug.WriteLine("Invoke {0}", AppDomain.CurrentDomain.FriendlyName);

            var result = Invoke.DynamicInvoke(Arguments);

            AppDomain.CurrentDomain.SetData(Key, result);
        }
    }
}
