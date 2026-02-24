document.addEventListener('DOMContentLoaded', function () {
    // Xử lý tất cả dropdown toggles
    const dropdownToggles = document.querySelectorAll('.dropdown-toggle');

    dropdownToggles.forEach(toggle => {
        toggle.addEventListener('click', function (e) {
            e.preventDefault();
            e.stopPropagation();

            // Tìm dropdown menu tương ứng
            const parent = this.closest('.dropdown');

            // Toggle class 'show'
            if (parent) {
                parent.classList.toggle('show');

                // Tìm dropdown menu và toggle nó
                const menu = parent.querySelector('.dropdown-menu');
                if (menu) {
                    menu.classList.toggle('show');
                }
            }
        });
    });

    // Đóng dropdown khi click ra ngoài
    document.addEventListener('click', function (e) {
        const dropdowns = document.querySelectorAll('.dropdown.show');
        dropdowns.forEach(dropdown => {
            if (!dropdown.contains(e.target)) {
                dropdown.classList.remove('show');
                const menu = dropdown.querySelector('.dropdown-menu');
                if (menu) {
                    menu.classList.remove('show');
                }
            }
        });
    });

    // Đảm bảo kiểu nút được áp dụng nhất quán
    const dashboardBtn = document.querySelector('a[asp-controller="Dashboard"]');
    if (dashboardBtn) {
        dashboardBtn.classList.add('dashboard-btn');
    }
});