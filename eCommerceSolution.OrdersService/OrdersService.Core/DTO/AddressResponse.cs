namespace OrdersService.Core.DTO
{
    public class AddressResponse
    {
        public Guid AddressId { get; set; }
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PinCode { get; set; } = string.Empty;
        public string? Landmark { get; set; }
        public string Type { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }
}
