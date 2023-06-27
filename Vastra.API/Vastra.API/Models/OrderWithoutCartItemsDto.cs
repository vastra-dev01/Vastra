namespace Vastra.API.Models
{
    public class OrderWithoutCartItemsDto
    {
        public int OrderId { get; set; }
        public int Value { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime DateAdded { get; }
        public DateTime DateModified { get; }
    }
}
