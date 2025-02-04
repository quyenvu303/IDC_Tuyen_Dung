(function () {
    let idleTime = 0; // Thời gian không hoạt động (tính bằng phút)
    const maxIdleTime = 15; // Giới hạn không hoạt động là 15 phút

    // Làm mới token
    async function refreshToken() {
        const response = await fetch('/Login/RefreshToken', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            credentials: 'include', // Gửi cookie trong request
            body: JSON.stringify({
                Token: getCookie('JwtToken'),
                RefreshToken: getCookie('RefreshToken'),
            }),
        });

        const data = await response.json();
        if (response.ok) {
            console.log('Token refreshed successfully:', data);
        } else {
            console.error('Failed to refresh token:', data);
            // Nếu làm mới token thất bại, chuyển hướng đến login
            window.location.href = '/Login';
        }
    }

    // Lấy cookie
    function getCookie(name) {
        const cookies = document.cookie.split(';');
        for (let cookie of cookies) {
            const [key, value] = cookie.trim().split('=');
            if (key === name) return value;
        }
        return null;
    }

    // Đặt lại thời gian không hoạt động khi có tương tác
    function resetIdleTime() {
        idleTime = 0;
    }

    // Tăng thời gian không hoạt động mỗi phút
    function trackIdleTime() {
        idleTime++;
        if (idleTime >= maxIdleTime) {
            console.log('User is idle for 15 minutes. Redirecting to login...');
            window.location.href = '/Login';
        }
    }

    // Lắng nghe các sự kiện để đặt lại idleTime
    window.onload = function () {
        document.addEventListener('mousemove', resetIdleTime);
        document.addEventListener('keydown', resetIdleTime);
        document.addEventListener('scroll', resetIdleTime);
        document.addEventListener('click', resetIdleTime);

        // Kiểm tra thời gian không hoạt động mỗi phút
        setInterval(trackIdleTime, 60 * 1000);

        // Làm mới token mỗi 14 phút nếu người dùng đang hoạt động
        setInterval(() => {
            if (idleTime < maxIdleTime) {
                refreshToken();
            }
        }, 14 * 60 * 1000); // 14 phút
    };
})();