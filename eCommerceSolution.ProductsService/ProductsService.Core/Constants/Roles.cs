namespace ProductsService.Core.Constants
{
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string User = "User";
        public const string DeliveryPartner = "DeliveryPartner";
        public static readonly string[] All = { Admin, User, DeliveryPartner };
    }
}
