using SportShop.Entities;
using SportShop.Entities.Shop;
using SportShop.Models;
using SportShop.ViewModels.User; // Bổ sung namespace này
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SportShop.ViewModels.Order
{
    public class CheckoutViewModel
    {
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal TotalAmount { get; set; }
        public string AddressType { get; set; }
        public int? SelectedAddressId { get; set; }
        public UserAddress NewAddress { get; set; } = new UserAddress();
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.COD;
        public bool IsCustomized { get; set; } // Đơn gia công/Sỉ
        public bool IsPartialPayment { get; set; } // Khách chọn Cọc 30% thay vì trả 100%
        public List<UserAddress> SavedAddresses { get; set; } = new List<UserAddress>();
    }
}