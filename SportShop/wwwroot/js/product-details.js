// =========================================================
// SPORTSHOP LOCAL JAVASCRIPT - TRANG CHI TIẾT SẢN PHẨM
// =========================================================

// --- 1. HÀM THAY ĐỔI ẢNH CHÍNH TỪ THUMBNAIL ---
window.changeMainImage = function (src) {
    const mainImage = document.getElementById('mainImage');
    if (!mainImage) return;

    mainImage.src = src;

    let thumbs = document.querySelectorAll('.img-thumbnail');
    thumbs.forEach(t => t.style.borderColor = '#dee2e6');

    if (window.event && window.event.target) {
        window.event.target.style.borderColor = 'var(--accent-color)';
    }
}

// --- 2. LOGIC CHỌN BIẾN THỂ (MÀU SẮC & SIZE) ---
document.addEventListener("DOMContentLoaded", function () {
    if (typeof productVariants === 'undefined' || productVariants.length === 0) return;

    const variantInput = document.getElementById('selectedVariantId');
    if (!variantInput) return;

    let selectedColor = null;
    let selectedSize = null;

    const colorBtns = document.querySelectorAll('.color-btn');
    const sizeBtns = document.querySelectorAll('.size-btn');
    const btnAddToCart = document.getElementById('btnAddToCart');
    const stockStatus = document.getElementById('stockStatus');
    const quantityInput = document.getElementById('quantityInput');

    // Xử lý khi click nút Màu
    colorBtns.forEach(btn => {
        btn.addEventListener('click', function () {
            if (this.classList.contains('active')) {
                this.classList.remove('active');
                selectedColor = null;
            } else {
                colorBtns.forEach(b => b.classList.remove('active'));
                this.classList.add('active');
                selectedColor = this.getAttribute('data-color');
            }
            checkVariantAvailability();
        });
    });

    // Xử lý khi click nút Size
    sizeBtns.forEach(btn => {
        btn.addEventListener('click', function () {
            if (this.classList.contains('disabled-variant')) return;

            if (this.classList.contains('active')) {
                this.classList.remove('active');
                selectedSize = null;
            } else {
                sizeBtns.forEach(b => b.classList.remove('active'));
                this.classList.add('active');
                selectedSize = this.getAttribute('data-size');
            }
            checkVariantAvailability();
        });
    });

    // Hàm kiểm tra tổng thể xem tổ hợp Màu - Size có hợp lệ và còn hàng không
    function checkVariantAvailability() {
        let matchedVariant = null;

        if (selectedColor && selectedSize) {
            matchedVariant = productVariants.find(v => v.Color === selectedColor && v.Size === selectedSize && v.StockQuantity > 0);

            if (matchedVariant) {
                // CÒN HÀNG
                variantInput.value = matchedVariant.VariantId;
                btnAddToCart.disabled = false;
                stockStatus.innerHTML = `<span class="text-success"><i class="fas fa-check-circle"></i> Sản phẩm có sẵn (Còn ${matchedVariant.StockQuantity} sản phẩm)</span>`;
                quantityInput.max = matchedVariant.StockQuantity;
            } else {
                // HẾT HÀNG
                variantInput.value = "";
                btnAddToCart.disabled = true;
                stockStatus.innerHTML = `<span class="text-danger"><i class="fas fa-times-circle"></i> Phân loại hàng này hiện không có sẵn.</span>`;
            }
        } else {
            // Chưa chọn đủ
            variantInput.value = "";
            btnAddToCart.disabled = true;
            stockStatus.innerHTML = `<span class="text-muted"><i class="fas fa-info-circle"></i> Vui lòng chọn Màu sắc và Kích thước.</span>`;
        }
        updateVariantUIStatus();
    }

    // Làm mờ các lựa chọn không tồn tại hoặc hết kho
    function updateVariantUIStatus() {
        sizeBtns.forEach(btn => {
            const sizeVal = btn.getAttribute('data-size');
            if (selectedColor) {
                const isValid = productVariants.some(v => v.Color === selectedColor && v.Size === sizeVal && v.StockQuantity > 0);
                if (!isValid) {
                    btn.classList.add('disabled-variant');
                    if (selectedSize === sizeVal) {
                        btn.classList.remove('active');
                        selectedSize = null;
                    }
                } else {
                    btn.classList.remove('disabled-variant');
                }
            } else {
                btn.classList.remove('disabled-variant');
            }
        });

        colorBtns.forEach(btn => {
            const colorVal = btn.getAttribute('data-color');
            if (selectedSize) {
                const isValid = productVariants.some(v => v.Size === selectedSize && v.Color === colorVal && v.StockQuantity > 0);
                if (!isValid) {
                    btn.classList.add('disabled-variant');
                    if (selectedColor === colorVal) {
                        btn.classList.remove('active');
                        selectedColor = null;
                    }
                } else {
                    btn.classList.remove('disabled-variant');
                }
            } else {
                btn.classList.remove('disabled-variant');
            }
        });
    }

    // Form validation CỤC BỘ: Ngăn submit nếu chưa chọn đủ Variant
    const addToCartForm = document.getElementById('addToCartForm');
    if (addToCartForm) {
        addToCartForm.addEventListener('submit', function (e) {
            if (!variantInput.value) {
                e.preventDefault();
                alert('Vui lòng chọn đầy đủ Phân loại hàng (Màu sắc & Kích thước) trước khi thêm vào giỏ.');

                // Hủy bỏ trạng thái disabled của nút chống double-click do site.js vừa thêm vào
                setTimeout(() => {
                    btnAddToCart.classList.remove('disabled');
                    btnAddToCart.innerHTML = '<i class="fas fa-cart-plus me-2"></i> Thêm Vào Giỏ Hàng';
                }, 100);
            }
        });
    }
});