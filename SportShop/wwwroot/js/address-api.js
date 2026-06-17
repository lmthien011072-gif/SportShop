document.addEventListener("DOMContentLoaded", function () {
    initAddressFetch();
});

/* --- CHỌN ĐỊA CHỈ & GỌI API --- */
function initAddressFetch() {
    // Lưu ý: Các trang sử dụng file này phải đảm bảo thẻ select có đúng ID dưới đây
    const provinceSelect = document.getElementById("provinceSelect");
    const communeSelect = document.getElementById("communeSelect");

    // Nếu trang không có 2 dropdown này thì ngưng chạy để tránh lỗi
    if (!provinceSelect || !communeSelect) return;

    const API_BASE_URL = '/api/address';

    // 1. Load Tỉnh/Thành ban đầu
    fetch(`${API_BASE_URL}/provinces`)
        .then(response => {
            if (!response.ok) throw new Error("Lỗi mạng hoặc API chưa chạy");
            return response.json();
        })
        .then(data => {
            let options = '<option value="">-- Chọn Tỉnh/Thành phố --</option>';
            data.forEach(p => {
                options += `<option value="${p.name}" data-code="${p.code}">${p.name}</option>`;
            });
            provinceSelect.innerHTML = options;
        })
        .catch(error => {
            console.error("Lỗi khi tải danh sách Tỉnh/Thành:", error);
            provinceSelect.innerHTML = '<option value="">-- Lỗi tải dữ liệu --</option>';
        });

    // 2. Bắt sự kiện đổi Tỉnh -> Load Xã/Phường
    provinceSelect.addEventListener('change', function () {
        const selectedOption = this.options[this.selectedIndex];
        const provinceCode = selectedOption.getAttribute('data-code');

        if (!provinceCode) {
            communeSelect.innerHTML = '<option value="">-- Vui lòng chọn Tỉnh/Thành trước --</option>';
            communeSelect.disabled = true;
            return;
        }

        communeSelect.disabled = false;
        communeSelect.innerHTML = '<option value="">-- Đang tải danh sách Xã/Phường... --</option>';

        fetch(`${API_BASE_URL}/provinces/${provinceCode}/communes`)
            .then(response => {
                if (!response.ok) throw new Error("Lỗi mạng hoặc API chưa chạy");
                return response.json();
            })
            .then(data => {
                let options = '<option value="">-- Chọn Quận/Huyện, Xã/Phường --</option>';
                data.forEach(commune => {
                    options += `<option value="${commune.name}">${commune.name}</option>`;
                });
                communeSelect.innerHTML = options;
            })
            .catch(error => {
                console.error("Lỗi khi tải danh sách Xã/Phường:", error);
                communeSelect.innerHTML = '<option value="">-- Lỗi tải dữ liệu --</option>';
            });
    });
}