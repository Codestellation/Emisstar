using System;
using System.Reflection;
using System.Reflection.Emit;
using Codestellation.Emisstar.Impl;

namespace Codestellation.Emisstar.DarkFlowIntegration
{
    //This class builds following code dynamically:
    //namespace Codestellation.Emisstar.Impl
    //{
    //    public class ExecutorDispatcher : RuleBasedDispatcher
    //    {
    //        private readonly IExecutor _executor;
    //
    //        public ExecutorDispatcher(IExecutor executor): base(new InvokeUsingExecutorRule())
    //        {
    //            if (executor == null)
    //            {
    //                throw new ArgumentNullException("executor");
    //            }
    //
    //            _executor = executor;
    //        }
    //
    //        protected override void IntervalInvoke<TMessage>(TMessage message, IHandler<TMessage> handler)
    //        {
    //            var task = new InvokeHandlerTask<TMessage>(message, handler);
    //
    //            _executor.Execute(task);
    //        }
    //    }
    //}

    internal class DispatcherBuilder
    {
        private readonly ModuleBuilder _modBuilder;
        private readonly Type _generatedTaskType;
        private TypeBuilder _dispatcherBuilder;
        private Type _executorType;
        private FieldBuilder _executorField;


        public DispatcherBuilder(ModuleBuilder modBuilder, Type generatedTaskType)
        {
            _modBuilder = modBuilder;
            _generatedTaskType = generatedTaskType;
        }

        public Type Build()
        {
            _executorType = Type.GetType("Codestellation.DarkFlow.IExecutor, Codestellation.DarkFlow");

            if (_executorType == null)
            {
                throw new InvalidOperationException("Could not find assemly 'Codestellation.DarkFlow'");
            }

            DefineType();
            DefineFields();
            DefineConstrunctor();
            DefineInvokeMethod();
            return _dispatcherBuilder.CreateType();
        }

        private void DefineInvokeMethod()
        {
            var methodName = "InternalInvoke";
            var builder = _dispatcherBuilder.DefineMethod(
                methodName,
                MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.ReuseSlot);

            var messageTypeParameter = builder.DefineGenericParameters("TMessage1")[0];

            var handlerType = typeof(IHandler<>).MakeGenericType(new Type[] { messageTypeParameter });

            builder.SetParameters(messageTypeParameter, handlerType);

            builder.DefineParameter(1, ParameterAttributes.None, "message");
            builder.DefineParameter(2, ParameterAttributes.None, "handler");

            builder.SetReturnType(typeof(void));

            var taskType = _generatedTaskType.MakeGenericType(messageTypeParameter);

            var ilgen = builder.GetILGenerator();

            ilgen.DeclareLocal(taskType);

            ilgen.Emit(OpCodes.Ldarg_1); //IL_0000:  ldarg.1
            ilgen.Emit(OpCodes.Ldarg_2); //IL_0001:  ldarg.2


            var openCtor = _generatedTaskType.GetConstructors()[0];
            var taskCtor = TypeBuilder.GetConstructor(taskType, openCtor);


            ilgen.Emit(OpCodes.Newobj, taskCtor); //IL_0002:  newobj     instance void class Codestellation.Emisstar.Impl.InvokeHandlerTask`1<!!TMessage>::.ctor(!0, class Codestellation.Emisstar.IHandler`1<!0>)
            ilgen.Emit(OpCodes.Stloc_0); //IL_0007:  stloc.0
            ilgen.Emit(OpCodes.Ldarg_0); //IL_0008:  ldarg.0
            ilgen.Emit(OpCodes.Ldfld, _executorField); //IL_0009:  ldfld      class [Codestellation.DarkFlow]Codestellation.DarkFlow.IExecutor Codestellation.Emisstar.Impl.ExecutorDispatcher::_executor
            ilgen.Emit(OpCodes.Ldloc_0); //IL_000e:  ldloc.0

            var executeMethod = _executorType.GetMethods()[0];
            ilgen.Emit(OpCodes.Callvirt, executeMethod); //IL_000f:  callvirt   instance void [Codestellation.DarkFlow]Codestellation.DarkFlow.IExecutor::Execute(class [Codestellation.DarkFlow]Codestellation.DarkFlow.ITask)
            ilgen.Emit(OpCodes.Ret); //IL_0014:  ret
        }

        private void DefineFields()
        {
            _executorField = _dispatcherBuilder.DefineField("_executor", _executorType, FieldAttributes.Private | FieldAttributes.InitOnly);
        }

        private void DefineConstrunctor()
        {
            //Implements this:


            const MethodAttributes construnctorAttributes = MethodAttributes.Public | MethodAttributes.HideBySig;
            var ctorBuilder = _dispatcherBuilder.DefineConstructor(construnctorAttributes, CallingConventions.Standard, new[] { _executorType });

            const string strParamName = "executor";
            ctorBuilder.DefineParameter(1, ParameterAttributes.None, strParamName);

            var ilgen = ctorBuilder.GetILGenerator();
            ilgen.DeclareLocal(typeof(IDispatchRule[]));
            var okLabel = ilgen.DefineLabel();
            ilgen.Emit(OpCodes.Ldarg_0);//IL_0000:  ldarg.0
            ilgen.Emit(OpCodes.Ldc_I4_1); //IL_0001:  ldc.i4.1
            ilgen.Emit(OpCodes.Newarr, typeof(IDispatchRule)); //IL_0002:  newarr     Codestellation.Emisstar.Impl.IDispatchRule
            ilgen.Emit(OpCodes.Stloc_0); //IL_0007:  stloc.0
            ilgen.Emit(OpCodes.Ldloc_0); //IL_0008:  ldloc.0
            ilgen.Emit(OpCodes.Ldc_I4_0); //IL_0009:  ldc.i4.0

            var ruleCtor = typeof(InvokeUsingExecutorRule).GetConstructor(Type.EmptyTypes);
            ilgen.Emit(OpCodes.Newobj, ruleCtor); //IL_000a:  newobj     instance void Codestellation.Emisstar.Impl.InvokeUsingExecutorRule::.ctor()
            ilgen.Emit(OpCodes.Stelem_Ref); //IL_000f:  stelem.ref
            ilgen.Emit(OpCodes.Ldloc_0); //IL_0010:  ldloc.0

            var baseDispatcherCtor = typeof(RuleBasedDispatcher).GetConstructors(BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)[0];

            ilgen.Emit(OpCodes.Call, baseDispatcherCtor); //IL_0011:  call       instance void Codestellation.Emisstar.Impl.RuleBasedDispatcher::.ctor(class Codestellation.Emisstar.Impl.IDispatchRule[])
            ilgen.Emit(OpCodes.Ldarg_1); //IL_0016:  ldarg.1
            ilgen.Emit(OpCodes.Brtrue_S, okLabel); //IL_0017:  brtrue.s   IL_0024
            ilgen.Emit(OpCodes.Ldstr, strParamName); //IL_0019:  ldstr      "executor"

            var exceptionCtor = typeof(ArgumentNullException).GetConstructor(new[] { typeof(string) });
            ilgen.Emit(OpCodes.Newobj, exceptionCtor); //IL_001e:  newobj     instance void [mscorlib]System.ArgumentNullException::.ctor(string)
            ilgen.Emit(OpCodes.Throw); //IL_0023:  throw
            ilgen.MarkLabel(okLabel);
            ilgen.Emit(OpCodes.Ldarg_0); //IL_0024:  ldarg.0
            ilgen.Emit(OpCodes.Ldarg_1); //IL_0025:  ldarg.1
            ilgen.Emit(OpCodes.Stfld, _executorField); //IL_0026:  stfld      class [Codestellation.DarkFlow]Codestellation.DarkFlow.IExecutor Codestellation.Emisstar.Impl.ExecutorDispatcher::_executor
            ilgen.Emit(OpCodes.Ret); //IL_002b:  ret
        }

        private void DefineType()
        {
            const string dispatcherName = IntegrationAssemblyBuilder.IntegrationAssemblyName + ".ExecutorDispatcher";

            _dispatcherBuilder = _modBuilder.DefineType(dispatcherName, TypeAttributes.Class | TypeAttributes.Public);
            _dispatcherBuilder.SetParent(typeof(RuleBasedDispatcher));
        }
    }
}