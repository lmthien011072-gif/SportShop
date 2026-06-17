document.addEventListener("DOMContentLoaded", function () {
    initCheckoutToggle();
});

function initCheckoutToggle() {
    const radioSaved = document.getElementById("addressTypeSaved");
    const radioNew = document.getElementById("addressTypeNew");
    const savedArea = document.getElementById("savedAddressesArea");
    const newArea = document.getElementById("newAddressFormArea");

    if (!newArea) return;

    function toggleAddressSections() {
        if (radioSaved && radioSaved.checked) {
            if (savedArea) savedArea.style.display = "block";
            newArea.style.display = "none";
            toggleInputs(newArea, true);
        } else if (radioNew && radioNew.checked) {
            if (savedArea) savedArea.style.display = "none";
            newArea.style.display = "block";
            toggleInputs(newArea, false);
        }
    }

    function toggleInputs(container, isDisabled) {
        const inputs = container.querySelectorAll("input, select, textarea");
        inputs.forEach(input => {
            input.disabled = isDisabled;
        });
    }

    if (radioSaved) radioSaved.addEventListener("change", toggleAddressSections);
    if (radioNew) radioNew.addEventListener("change", toggleAddressSections);

    toggleAddressSections();
}

window.initPaymentLogic = function (totalAmount) {
    const chkCustomized = document.getElementById('chkCustomized');
    const bankRadio = document.getElementById('payBank');
    const codRadio = document.getElementById('payCOD');
    const lblCod = document.getElementById('lblCod');
    const partialSection = document.getElementById('partialPaymentSection');

    // Thoát nếu không tìm thấy các phần tử này trên giao diện (tránh lỗi null)
    if (!chkCustomized || !bankRadio || !codRadio) return;

    // Tính tiền cọc hiển thị
    const depositDisplay = document.getElementById('depositDisplay');
    if (depositDisplay) {
        depositDisplay.innerText = (totalAmount * 0.3).toLocaleString('vi-VN');
    }

    // Xử lý đơn gia công
    chkCustomized.addEventListener('change', function () {
        if (this.checked) {
            codRadio.disabled = true;
            lblCod.style.opacity = '0.5';
            bankRadio.checked = true;
            togglePartialSection();
        } else {
            codRadio.disabled = false;
            lblCod.style.opacity = '1';
        }
    });

    // Xử lý ẩn hiện khối đặt cọc
    function togglePartialSection() {
        if (bankRadio.checked) {
            if (partialSection) partialSection.style.display = 'block';
        } else {
            if (partialSection) partialSection.style.display = 'none';
            const payFull = document.getElementById('payFull');
            if (payFull) payFull.checked = true; // Reset lại
        }
    }

    bankRadio.addEventListener('change', togglePartialSection);
    codRadio.addEventListener('change', togglePartialSection);

    // Chạy kiểm tra lần đầu
    if (chkCustomized.checked) {
        codRadio.disabled = true;
        lblCod.style.opacity = '0.5';
    }
    togglePartialSection();
};