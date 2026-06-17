// =========================================================
// SPORTSHOP ADMIN JAVASCRIPT - TRANG THÊM MỚI SẢN PHẨM
// =========================================================

// --- LOGIC 1: ĐỊNH DẠNG GIÁ TIỀN CÓ DẤU PHÂN CÁCH HÀNG NGHÌN ---
document.addEventListener("DOMContentLoaded", function () {
    const priceDisplay = document.getElementById('priceDisplay');
    const priceRaw = document.getElementById('Price'); // asp-for="Price" tự động tạo id="Price"

    if (priceDisplay && priceRaw) { // Bổ sung kiểm tra an toàn
        priceDisplay.addEventListener('input', function (e) {
            // 1. Loại bỏ tất cả các ký tự không phải là số (chữ cái, dấu chấm, dấu phẩy...)
            let value = this.value.replace(/\D/g, '');

            // 2. Cập nhật con số thật (không có dấu phẩy) vào thẻ input ẩn để gửi lên Server
            priceRaw.value = value;

            // 3. Format lại số có dấu chấm hàng nghìn (kiểu Việt Nam) để hiển thị cho người dùng xem
            if (value !== '') {
                this.value = parseInt(value, 10).toLocaleString('en-US');
            } else {
                this.value = '';
            }
        });
    }
});

// --- LOGIC 2: THÊM DÒNG BIẾN THỂ ĐỘNG ---
// Biến đếm số lượng biến thể hiện tại (Bắt đầu từ 1 vì Index 0 đã có ở HTML)
let variantIndex = 1;

// Đưa hàm ra Global (window) để thuộc tính onclick="" trong HTML có thể gọi được
window.addVariantRow = function () {
    const container = document.getElementById('variants-container');
    if (!container) return; // Bổ sung kiểm tra an toàn

    // Khởi tạo HTML cho dòng mới
    const html = `
        <div class="row mb-3 variant-row align-items-center bg-light p-2 rounded mx-0">
            <div class="col-4">
                <input type="text" name="Variants[${variantIndex}].Color" class="form-control" placeholder="VD: Đỏ, Xanh..." required />
            </div>
            <div class="col-4">
                <input type="text" name="Variants[${variantIndex}].Size" class="form-control" placeholder="VD: M, L, XL, 42..." required />
            </div>
            <div class="col-3">
                <input type="number" name="Variants[${variantIndex}].StockQuantity" class="form-control" placeholder="0" min="0" required />
            </div>
            <div class="col-1 text-end">
                <button type="button" class="btn btn-outline-danger btn-sm" onclick="removeVariantRow(this)"><i class="fas fa-times"></i></button>
            </div>
        </div>
    `;

    container.insertAdjacentHTML('beforeend', html);
    variantIndex++; // Tăng index cho lần thêm tiếp theo
}

window.removeVariantRow = function (button) {
    button.closest('.variant-row').remove();
}