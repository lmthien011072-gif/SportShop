using System.ComponentModel.DataAnnotations;

namespace SportShop.ViewModels.User
{
    public class AddressViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên người nhận.")]
        [MaxLength(100, ErrorMessage = "Tên người nhận không được vượt quá 100 ký tự.")]
        [Display(Name = "Tên người nhận")]
        public string ReceiverName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})$", ErrorMessage = "Số điện thoại không hợp lệ (Ví dụ: 0987654321).")]
        [MaxLength(15, ErrorMessage = "Số điện thoại quá dài.")]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Tỉnh / Thành phố.")]
        [Display(Name = "Tỉnh / Thành phố")]
        public string Province { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Quận / Huyện, Xã / Phường.")]
        [Display(Name = "Xã / Phường")]
        public string Commune { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ chi tiết.")]
        [MaxLength(200, ErrorMessage = "Địa chỉ chi tiết không được vượt quá 200 ký tự.")]
        [Display(Name = "Địa chỉ chi tiết")]
        public string SpecificAddress { get; set; }

        [Display(Name = "Đặt làm địa chỉ mặc định")]
        public bool IsDefault { get; set; }
    }
}