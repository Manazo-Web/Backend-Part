using Manazo.Models.Product;

namespace Manazo.Models.User
{
    public class UserFormModel
    {
        public String Type { get; set; } = "user";
        public Guid Id { get; set; }
        public String RegName { get; set; } = null!;
        public String? RegSurname { get; set; }
        public String Photo { get; set; } = null!;
        public String RegLogin { get; set; } = null!;
        public String RegPassword { get; set; } = null!;
        public String RegEmail { get; set; } = null!;
        public int RegNumber { get; set; }
        public Address RegAdress { get; set; } = null!;
        public List<OrderFormModel>? Cart { get; set; }
        public List<OrderFormModel>? Wishlist { get; set; }
        public BankCard? BankCards { get; set; }

        public class Address
        {
            public string Street { get; set; } = null!;
            public string City { get; set; } = null!;
            public string ZipCode { get; set; } = null!;
            public string Country { get; set; } = null!;
        }

        public class OrderFormModel
        {
            public ProductFormModel product = null!;
            public uint quantity;
            public DateTime? OrderDT;
            public string OrderState = null!;  //  Wishlist, Cart

            public void ChangeOrderState(string newState)
            {
                OrderState = newState;
            }

            public void SetOrderDateTime()
            {
                OrderDT = DateTime.Now;
            }

            public decimal GetPrice()
            {
                return product.Price * quantity;
            }
        }

        public class BankCard
        {
            public string CardNumber { get; set; }
            public DateTime ExpiryDate { get; set; }
            public string Cvv { get; set; }

            public BankCard(string cardNumber, DateTime expiryDate, string cvv)
            {
                CardNumber = cardNumber;
                ExpiryDate = expiryDate;
                Cvv = cvv;
            }
        }
        bool IsDeleted;

        public void DeleteMe()
        {
            IsDeleted = true;
        }
    }
}
