using System;
using Castle.MicroKernel.Registration;

namespace Codestellation.Emisstar.CastleWindsor.Facility
{
    public static class AllHandlers
    {
        public static BasedOnDescriptor WithServiceAllHandlers(this BasedOnDescriptor self)
        {
            return self.WithServiceSelect((type, baseTypes) => type.HandlerServices());
        }

        public static Type[] HandlerServices(this Type self)
        {
            return self.ImplementationOfGenericInterface(typeof(IHandler<>));
        }

        public static Type[] ImplementationOfGenericInterface(this Type self, Type genericInterface)
        {
            return self.FindInterfaces(
                (@interface, nomatter) => @interface.IsGenericType &&
                                          @interface.GetGenericTypeDefinition() == genericInterface, null);
        }

        public static bool IsHandler(this Type self)
        {
            return self.HandlerServices().Length > 0;
        }
    }
}