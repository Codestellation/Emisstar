using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Codestellation.Emisstar.DarkFlowIntegration
{
    /*Dynamically builds following code. 
    public class InvokeHandlerTask<TMessage> : ITask
    {
        private readonly TMessage _message;
        private readonly IHandler<TMessage> _handler;

        public InvokeHandlerTask(TMessage message, IHandler<TMessage> handler)
        {
            Debug.Assert(message != null);
            Debug.Assert(handler != null);
            _message = message;
            _handler = handler;
        }

        public void Execute()
        {
            _handler.Handle(_message);
        }
    }*/

    internal class TaskBuilder
    {
        private readonly ModuleBuilder _modBuilder;
        private TypeBuilder _invokeTaskBuilder;
        private GenericTypeParameterBuilder _messageType;
        private FieldBuilder _messageField;
        private Type _handlerType;
        private FieldBuilder _handlerField;

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
            _messageType = _invokeTaskBuilder.DefineGenericParameters("TMessage")[0];
        }

        private void DefineFields()
        {
            _messageField = _invokeTaskBuilder.DefineField("_message", _messageType, FieldAttributes.Private | FieldAttributes.InitOnly);

            _handlerType = typeof(IHandler<>).MakeGenericType(_messageType);
            _handlerField = _invokeTaskBuilder.DefineField("_handler", _handlerType, FieldAttributes.Private | FieldAttributes.InitOnly);
        }

        private void DefineConstructor()
        {
            ConstructorBuilder ctorBuilder = _invokeTaskBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { _messageType, _handlerType });
            ctorBuilder.DefineParameter(1, ParameterAttributes.None, "message");
            ctorBuilder.DefineParameter(2, ParameterAttributes.None, "handler");

            ILGenerator ilgen = ctorBuilder.GetILGenerator();
            ConstructorInfo objCtor = typeof(object).GetConstructor(Type.EmptyTypes);

            ilgen.Emit(OpCodes.Ldarg_0); //IL_0000:  ldarg.0
            ilgen.Emit(OpCodes.Call, objCtor); //IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
            //Sets _message field
            ilgen.Emit(OpCodes.Ldarg_0); //IL_0006:  ldarg.0
            ilgen.Emit(OpCodes.Ldarg_1); //IL_0007:  ldarg.1
            ilgen.Emit(OpCodes.Stfld, _messageField); //IL_0008:  stfld      !0 class Codestellation.Emisstar.Impl.InvokeHandlerTask`1<!TMessage>::_message
            //Set _handlerField
            ilgen.Emit(OpCodes.Ldarg_0); //IL_000d:  ldarg.0
            ilgen.Emit(OpCodes.Ldarg_2); //IL_000e:  ldarg.2
            ilgen.Emit(OpCodes.Stfld, _handlerField); //IL_000f:  stfld      class Codestellation.Emisstar.IHandler`1<!0> class Codestellation.Emisstar.Impl.InvokeHandlerTask`1<!TMessage>::_handler

            ilgen.Emit(OpCodes.Ret); //IL_0014:  ret
        }

        private void DefineExecuteMethod()
        {
            const MethodAttributes methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final;
            
            var executeMethodBuilder = _invokeTaskBuilder.DefineMethod("Execute", methodAttributes, CallingConventions.Standard);

            executeMethodBuilder.SetSignature(typeof(void), null, null, null, null, null);
            ILGenerator ilgen = executeMethodBuilder.GetILGenerator();

            var handleMethod = typeof(IHandler<>).GetMethod("Handle");

            ilgen.Emit(OpCodes.Ldarg_0);                //IL_0000:  ldarg.0
            ilgen.Emit(OpCodes.Ldfld, _handlerField);   //IL_0001:  ldfld      class Codestellation.Emisstar.IHandler`1<!0> class Codestellation.Emisstar.Impl.InvokeHandlerTask`1<!TMessage>::_handler
            ilgen.Emit(OpCodes.Ldarg_0);                //IL_0006:  ldarg.0
            ilgen.Emit(OpCodes.Ldfld, _messageField);   //IL_0007:  ldfld      !0 class Codestellation.Emisstar.Impl.InvokeHandlerTask`1<!TMessage>::_message
            ilgen.Emit(OpCodes.Callvirt, handleMethod); //IL_000c:  callvirt   instance void class Codestellation.Emisstar.IHandler`1<!TMessage>::Handle(!0)
            ilgen.Emit(OpCodes.Ret);                    //IL_0011:  ret
        }
    }
}