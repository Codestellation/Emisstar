using Castle.MicroKernel;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Codestellation.Emisstar.Impl;

namespace Codestellation.Emisstar.CastleWindsor.Facility
{
    //
    public class PublisherSpecificCollectionResolver : CollectionResolver
    {
        public PublisherSpecificCollectionResolver(IKernel kernel) : base(kernel, true)
        {
        }

        protected override bool CanSatisfy(System.Type itemType)
        {
            return (itemType == typeof(IHandlerSource) || itemType == typeof(IDispatcher)) && base.CanSatisfy(itemType);
        }
    }
}