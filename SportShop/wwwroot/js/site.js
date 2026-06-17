// =========================================================
// SPORTSHOP UNIFIED JAVASCRIPT - BỘ XỬ LÝ GIAO DIỆN TỔNG (GLOBAL)
// =========================================================

document.addEventListener("DOMContentLoaded", function () {
    console.log("Hệ thống SportShop đã khởi chạy!");

    // Khởi chạy các khối logic chức năng TOÀN CỤC
    initPreventDoubleSubmit(); // Logic khóa nút, chống double-click toàn hệ thống
});

/* =========================================================
   1. TIỆN ÍCH DÙNG CHUNG (UTILITIES)
   ========================================================= */
function formatCurrencyVN(number) {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(number);
}

function showToastMessage(message, type = 'success') {
    alert(message); // Tạm thời dùng alert, sau này đổi thành thư viện Toast UI
}

/* =========================================================
   2. HIỆU ỨNG KHÓA NÚT CHỐNG DOUBLE-CLICK (LOADING EFFECT)
   ========================================================= */
function initPreventDoubleSubmit() {
    // Xử lý cho Form Submit
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function (e) {
            // Kiểm tra validation của jquery-validate nếu có
            if (typeof $(this).valid === "function" && !$(this).valid()) {
                return;
            }

            const btn = form.querySelector('.btn-prevent-double');
            if (btn) {
                if (btn.classList.contains('disabled')) {
                    e.preventDefault();
                    return;
                }

                let originalHtml = btn.innerHTML;
                let originalWidth = btn.offsetWidth;

                btn.classList.add('disabled');
                btn.style.width = originalWidth + 'px';
                btn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i> Đang xử lý...';

                setTimeout(() => {
                    btn.classList.remove('disabled');
                    btn.innerHTML = originalHtml;
                    btn.style.width = 'auto';
                }, 8000); // Mở khóa sau 8s phòng lỗi mạng
            }
        });
    });

    // Xử lý cho Thẻ <a> (Nút bấm chuyển trang)
    const linkBtns = document.querySelectorAll('a.btn-prevent-double, button[type="button"].btn-prevent-double');
    linkBtns.forEach(btn => {
        btn.addEventListener('click', function (e) {
            if (this.tagName === 'A' && this.getAttribute('href') === '#') {
                e.preventDefault();
            }

            if (!this.classList.contains('disabled')) {
                let originalHtml = this.innerHTML;
                let originalWidth = this.offsetWidth;

                this.classList.add('disabled');
                this.style.width = originalWidth + 'px';
                this.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i> Vui lòng chờ...';

                setTimeout(() => {
                    this.classList.remove('disabled');
                    this.innerHTML = originalHtml;
                    this.style.width = 'auto';
                }, 5000);
            } else {
                e.preventDefault();
            }
        });
    });
}

/* =========================================================
   3. HÀM TIỆN ÍCH DÙNG CHUNG KHÁC (UTILITIES)
   ========================================================= */

// Hàm hiển thị trước hình ảnh khi chọn file (Dùng chung cho Avatar, Upload Sản phẩm...)
// Cách dùng: onchange="previewImage(this, 'id-cua-the-img')"
window.previewImage = function (inputElement, targetImageId) {
    if (inputElement.files && inputElement.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            var imgElement = document.getElementById(targetImageId);
            if (imgElement) {
                imgElement.src = e.target.result;
            }
        }

        reader.readAsDataURL(inputElement.files[0]);
    }
}