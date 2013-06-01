using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Codestellation.Emisstar.DarkFlowIntegration
{
    internal class IntegrationAssemblyBuilder
    {
        private Type _generatedTaskType;
        private Type _generatedDispatcherType;
        private bool _built;
        public const string IntegrationAssemblyName = "Codestellation.Emisstar.DarkFlowIntergration";

        public Type GeneratedTaskType
        {
            get
            {
                Build();
                return _generatedTaskType;
            }
        }

        public Type GeneratedDispatcherType
        {
            get
            {
                Build();
                return _generatedDispatcherType;
            }
        }

        public void Build()
        {
            if(_built)return;
            var assemblyName = new AssemblyName(IntegrationAssemblyName);

            AssemblyBuilder asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder modBuilder = asmBuilder.DefineDynamicModule(IntegrationAssemblyName);

            _generatedTaskType = new TaskBuilder(modBuilder).Build();
            _generatedDispatcherType = new DispatcherBuilder(modBuilder, _generatedTaskType).Build();
            _built = true;
        }
    }
}