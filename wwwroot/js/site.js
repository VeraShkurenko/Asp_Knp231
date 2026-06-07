class Base64 {
    static #textEncoder = new TextEncoder();
    static #textDecoder = new TextDecoder();

    // https://datatracker.ietf.org/doc/html/rfc4648#section-4
    static encode = (str) => btoa(String.fromCharCode(...Base64.#textEncoder.encode(str)));
    static decode = (str) => Base64.#textDecoder.decode(Uint8Array.from(atob(str), c => c.charCodeAt(0)));

    // https://datatracker.ietf.org/doc/html/rfc4648#section-5
    static encodeUrl = (str) => this.encode(str).replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');
    static decodeUrl = (str) => this.decode(str.replace(/\-/g, '+').replace(/\_/g, '/'));

    static jwtEncodeBody = (header, payload) => this.encodeUrl(JSON.stringify(header)) + '.' + this.encodeUrl(JSON.stringify(payload));
    static jwtDecodePayload = (jwt) => JSON.parse(this.decodeUrl(jwt.split('.')[1]));
}

document.addEventListener('submit', e => {
    const form = e.target;
    if (form.id == 'auth-form') {
        // Подія автентифікації
        e.preventDefault();   // зупиняємо автоматичне надсилання
        const formData = new FormData(form);
        const login = formData.get("user-login");
        const password = formData.get("user-password");

        if (login.length == 0 && password.length == 0) {
            alert("Заповніть усі поля");
            return;
        }
        // Формуємо дані для передачі до серверу за RFC 7617   https://www.rfc-editor.org/rfc/rfc7617
        /*
          To receive authorization, the client

           1.  obtains the user-id and password from the user,

           2.  constructs the user-pass by concatenating the user-id, a single
               colon (":") character, and the password,

           3.  encodes the user-pass into an octet sequence (see below for a
               discussion of character encoding schemes),

           4.  and obtains the basic-credentials by encoding this octet sequence
               using Base64 ([RFC4648], Section 4) into a sequence of US-ASCII
               characters ([RFC0020]).
           
           Furthermore, a user-id containing a colon character is invalid

           use the following header field:
            Authorization: Basic QWxhZGRpbjpvcGVuIHNlc2FtZQ==
        */
        if (login.includes(':')) {
            alert("У логіні символ ':' не допускається");
            return;
        }
        const userPass = login + ':' + password;
        const basicCredentials = Base64.encode(userPass);
        fetch("/User/SignIn", {
            headers: {
                "Authorization": `Basic ${basicCredentials}`
            }
        }).then(r => r.json()).then(j => {
            if (j.status != 200) {
                alert("У вході відмовлено")
            }
            else {
                window.location.reload();
            }            
        });
    }
});
/*
Д.З. Реалізувати відображення помилок (повідомлень) автентифікації
у складі модального вікна
Рекомендація: використати Bootstrap-Alerts https://getbootstrap.com/docs/5.3/components/alerts/
у лівій нижній частині діалогу (ліворуч від кнопок)
*/