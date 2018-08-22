using System;

namespace GraphQL.Attachments
{
    public static class GraphQLAttachmentConventions
    {
        public static void RegisterInContainer(Action<Type, object> registerInstance)
        {
            Guard.AgainstNull(nameof(registerInstance), registerInstance);

        }
    }
}