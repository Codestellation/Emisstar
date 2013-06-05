using System;
using System.Reflection;
using System.Reflection.Emit;
using Codestellation.Emisstar.Impl;

namespace Codestellation.Emisstar.DarkFlowIntegration
{
    //Dynamically builds following code. 
    //public class InvokeHandlerTaskTemplate : ITask
    //{
    //    private MessageHandlerTuple _tuple;

    //    public InvokeHandlerTaskTemplate(MessageHandlerTuple tuple)
    //    {
    //        _tuple = tuple;
    //    }

    //    public void Execute()
    //    {
    //        HandlerInvoker.Invoke(ref _tuple);
    //    }
    //}

    internal class TaskBuilder
    {
        private readonly ModuleBuilder _modBuilder;
        private TypeBuilder _invokeTaskBuilder;
        private FieldBuilder _tupleField;


        public TaskBuilder(ModuleBuilder modBuilder)
        {
            _modBuilder = modBuilder;
        }

        public Type Build()
        {
            BuildType();
            DefineFields();
            DefineConstructor();
            DefineExecuteMethod();

            var type = _invokeTaskBuilder.CreateType();
            return type;
        }

        private void BuildType()
        {
            var taskType = Type.GetType("Codestellation.DarkFlow.ITask, Codestellation.DarkFlow");
            if (taskType == null)
            {
                throw new InvalidOperationException("Could not find assembly 'Codestellation.DarkFlow'");
            }

            const string generatedTaskName = IntegrationAssemblyBuilder.IntegrationAssemblyName + ".InvokeHandlerTask";

            _invokeTaskBuilder = _modBuilder.DefineType(generatedTaskName, TypeAttributes.Class | TypeAttributes.Public);
            _invokeTaskBuilder.AddInterfaceImplementation(taskType);
            _invokeTaskBuilder.SetParent(typeof(object));
        }

        private void DefineFields()
        {
            _tupleField = _invokeTaskBuilder.DefineField("_tuple", typeof(MessageHandlerTuple), FieldAttributes.Private);
        }

        private void DefineConstructor()
        {
            ConstructorBuilder ctorBuilder = _invokeTaskBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { typeof(MessageHandlerTuple) });
            ctorBuilder.DefineParameter(1, ParameterAttributes.None, "tuple");

            ILGenerator ilgen = ctorBuilder.GetILGenerator();
            ConstructorInfo objCtor = typeof(object).GetConstructor(Type.EmptyTypes);

            //Invokes base constructor.
            ilgen.Emit(OpCodes.Ldarg_0); //IL_0000:  ldarg.0
            ilgen.Emit(OpCodes.Call, objCtor); //IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
            
            //Sets tuple field
            ilgen.Emit(OpCodes.Ldarg_0); //IL_0006:  ldarg.0
            ilgen.Emit(OpCodes.Ldarg_1); //IL_0007:  ldarg.1
            ilgen.Emit(OpCodes.Stfld, _tupleField); //IL_0008:  stfld      
            //Set _handlerField

            ilgen.Emit(OpCodes.Ret); //IL_0014:  ret
        }

        private void DefineExecuteMethod()
        {
            const MethodAttributes methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final;
            
            var executeMethodBuilder = _invokeTaskBuilder.DefineMethod("Execute", methodAttributes, CallingConventions.Standard);

            executeMethodBuilder.SetSignature(typeof(void), null, null, null, null, null);
            ILGenerator ilgen = executeMethodBuilder.GetILGenerator();

            var invokeMethod = typeof(HandlerInvoker).GetMethod("Invoke");

            ilgen.Emit(OpCodes.Ldarg_0);               
            ilgen.Emit(OpCodes.Ldflda, _tupleField);   
            
            ilgen.Emit(OpCodes.Call, invokeMethod); 
            ilgen.Emit(OpCodes.Ret);                    
        }
    }
}