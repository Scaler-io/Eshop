namespace Eshop.Shared.Constants
{
    public class EventBusQueueNames
    {
        public class ProductQueueNames
        {
            public const string CreateOrUpdate = "create_update_product";
            public const string Delete = "delete_product";
        }

        public class UserQueueNames{
            public const string CreateOrUpdate = "create_update_user";
            public const string Delete = "delete_user";
        }
    }
}
