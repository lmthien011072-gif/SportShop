using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SportShop.ViewModels.User
{
    public class UserProfileViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [MaxLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } // Trường duy nhất bắt buộc phải nhập

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string? PhoneNumber { get; set; } // Đã thêm ? để cho phép bỏ trống

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Giới tính")]
        public string? Gender { get; set; } // Đã thêm ? để cho phép chưa chọn

        public string? AvatarUrl { get; set; } // Đã thêm ?

        [Display(Name = "Tải ảnh mới")]
        public IFormFile? AvatarFile { get; set; } // Đã thêm ?
    }
}