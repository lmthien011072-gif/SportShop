document.addEventListener("DOMContentLoaded", function () {
    initCartQuantity();
});

function initCartQuantity() {
    const qtyForms = document.querySelectorAll('.qty-form');
    if (qtyForms.length === 0) return;

    qtyForms.forEach(form => {
        const btnMinus = form.querySelector('.btn-minus');
        const btnPlus = form.querySelector('.btn-plus');
        const inputQty = form.querySelector('.qty-input');

        if (btnMinus && btnPlus && inputQty) {
            btnMinus.addEventListener('click', function () {
                let currentVal = parseInt(inputQty.value);
                if (currentVal > 1) {
                    inputQty.value = currentVal - 1;
                    form.submit();
                }
            });

            btnPlus.addEventListener('click', function () {
                let currentVal = parseInt(inputQty.value);
                if (currentVal < parseInt(inputQty.max)) {
                    inputQty.value = currentVal + 1;
                    form.submit();
                }
            });

            inputQty.addEventListener('change', function () {
                let val = parseInt(this.value);
                if (val < 1 || isNaN(val)) this.value = 1;
                if (val > parseInt(this.max)) this.value = this.max;
                form.submit();
            });
        }
    });
}