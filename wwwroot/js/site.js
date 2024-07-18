document.addEventListener("DOMContentLoaded", () => {
    const SIB = document.getElementById("sign-in-button");
    if (!SIB) throw "sign-in-button not found";
    const SUB = document.getElementById("sign-up-button");
    if (!SUB) throw "sign-up-button not found";

    SIB.addEventListener("click", RegClick);
    SUB.addEventListener("click", LogClick);
});

LogLogin: string = 'Admin';
LogPassword: string = 'Admin';

RegName: string = 'A';
RegSurname: string = 'B';
RegNumber: string = '+1 2345 6789';
RegEmail: string = 'Example@example.com';
RegLogin: string = 'Admin';
RegPassword: string = 'Admin';

function RegClick() {
    const url = 'api/user';

    const data = {
        RegLogin: this.RegLogin,
        RegEmail: this.RegEmail,
        RegName: this.RegName,
        RegNumber: this.RegNumber,
        RegPassword: this.RegPassword,
        RegSurname: this.RegSurname
    };

    fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
    })
        .then(response => response.json())
        .then(responseData => {
            // Display the response data in the console
            console.log(responseData);
        })
        .catch(error => {
            console.error('RegClick Error:', error);
        });
}

function LogClick() {
    // Assuming this.LogLogin and this.LogPassword are your login and password values
    const login = this.LogLogin;
    const password = this.LogPassword;

    // Construct the URL with login and password as query parameters
    const url = `api/user?login=${login}&password=${password}`;

    // Make the fetch request with the GET method
    fetch(url, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            // Add any additional headers if required
        },
    })
        .then(response => response.json())
        .then(data => console.log(data))
        .catch(error => console.error('LogClick:', error));
}
 