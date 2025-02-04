// URL của API Backend
const BASE_URL = "http://localhost:5001"; // Thay bằng URL của server Backend

// Lưu Access Token trong memory
let accessToken = null;

// Lưu Refresh Token trong localStorage
const saveTokens = (tokens) => {
    accessToken = tokens.token; // Lưu Access Token vào memory
    localStorage.setItem("refreshToken", tokens.refreshToken); // Lưu Refresh Token mới vào localStorage
};

// Gửi yêu cầu Refresh Token đến server
const refreshToken = async () => {
    try {
        const refreshToken = localStorage.getItem("refreshToken");
        if (!refreshToken) {
            console.error("No refresh token found. Redirecting to login...");
            window.location.href = "/login";
            return;
        }

        const response = await fetch(`${BASE_URL}/api/login/refresh-token`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                token: accessToken,
                refreshToken: refreshToken,
            }),
        });

        if (response.ok) {
            const tokens = await response.json();
            saveTokens(tokens); // Lưu Access Token và Refresh Token mới
        } else {
            console.error("Failed to refresh token. Redirecting to login...");
            window.location.href = "/login";
        }
    } catch (error) {
        console.error("Error refreshing token:", error);
        window.location.href = "/login";
    }
};


// Tự động Refresh Token mỗi 10 phút (600000ms)
document.addEventListener("DOMContentLoaded", () => {
    console.log("Starting automatic token refresh every 10 minutes...");
    setInterval(refreshToken, 10 * 60 * 1000); // Gọi hàm refreshToken mỗi 10 phút
});

