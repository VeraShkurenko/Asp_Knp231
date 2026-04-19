document.addEventListener("DOMContentLoaded", () => {
    const loginButton = document.getElementById("login-button");
    if (loginButton) {
        loginButton.addEventListener('click', loginButtonClick);
    }
});

function loginButtonClick() {
    const loginInput = document.getElementById("login-input");
    const passwordInput = document.getElementById("password-input");
    const alertContainer = document.getElementById("login-alert-container");

    if (!loginInput || !passwordInput || !alertContainer) return;

    const login = loginInput.value.trim();
    const password = passwordInput.value.trim();

    // Очищуємо попередні повідомлення
    alertContainer.innerHTML = "";

    if (login.length === 0 || password.length === 0) {
        showLoginAlert(alertContainer, "Заповніть усі поля.", "danger");
        return;
    }

    // Надсилаємо реальний запит на сервер
    const formData = new FormData();
    formData.append("user-login", login);
    formData.append("user-password", password);

    fetch("/Home/Auth", {
        method: "POST",
        body: formData
    })
    .then(r => r.json())
    .then(data => {
        if (data.success) {
            showLoginAlert(alertContainer, "Успішно!", "success");
            setTimeout(() => location.reload(), 1000);
        } else {
            showLoginAlert(alertContainer, data.message, "warning");
        }
    })
    .catch(err => {
        console.error(err);
        showLoginAlert(alertContainer, "Помилка сервера", "danger");
    });
}

function showLoginAlert(container, message, type) {
    container.innerHTML = `
        <div class="alert alert-${type} alert-dismissible fade show mb-0 p-2" style="font-size: 0.85rem;" role="alert">
            ${message}
            <button type="button" class="btn-close p-2" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>`;
}
