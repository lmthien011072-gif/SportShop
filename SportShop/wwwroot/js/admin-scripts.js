// Highlight menu dựa trên đường dẫn hiện tại
document.addEventListener("DOMContentLoaded", function () {
    const currentPath = window.location.pathname;
    const navLinks = document.querySelectorAll('.nav-link');

    navLinks.forEach(link => {
        if (link.getAttribute('href') === currentPath) {
            link.classList.add('active');
        }
    });
});

// Hàm xác nhận xóa chung (dùng cho mọi trang)
function confirmDelete(message = "Bạn có chắc chắn muốn xóa mục này?") {
    return confirm(message);
}

// Tự động ẩn Alert sau 3 giây
setTimeout(function () {
    let alerts = document.querySelectorAll('.alert');
    alerts.forEach(function (alert) {
        alert.style.display = 'none';
    });
}, 3000);