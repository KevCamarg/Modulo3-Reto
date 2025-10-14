document.addEventListener("DOMContentLoaded", function() {
    // Manejar el clic en el enlace de inicio de sesión
    document.getElementById("login-link").addEventListener("click", function() {
        clearContent();
        showLoginForm();
    });

    // Manejar el clic en el enlace de registro
    document.getElementById("register-link").addEventListener("click", function() {
        clearContent();
        showRegisterForm();
    });

    // Manejar el clic en el enlace de libros
    document.getElementById("books-link").addEventListener("click", function() {
        clearContent();
        showBooks();
    });

    // Manejar el clic en el enlace de préstamos
    document.getElementById("loans-link").addEventListener("click", function() {
        clearContent();
        showLoans();
    });
});

// Función para mostrar el formulario de inicio de sesión
function showLoginForm() {
    clearContent();
    const content = document.getElementById("content");
    content.innerHTML = `
        <h2>Iniciar Sesión</h2>
        <form id="login-form">
            <input type="email" id="login-email" placeholder="Correo Electrónico" required>
            <input type="password" id="login-password" placeholder="Contraseña" required>
            <button type="submit">Iniciar Sesión</button>
        </form>
    `;

    document.getElementById("login-form").addEventListener("submit", function(event) {
        event.preventDefault();
        login();
    });
}

// Función para mostrar el formulario de registro
function showRegisterForm() {
    clearContent();
    const content = document.getElementById("content");
    content.innerHTML = `
        <h2>Registrarse</h2>
        <form id="register-form">
            <input type="text" id="register-name" placeholder="Nombre" required>
            <input type="email" id="register-email" placeholder="Correo Electrónico" required>
            <input type="password" id="register-password" placeholder="Contraseña" required>
            <button type="submit">Registrarse</button>
        </form>
    `;

    document.getElementById("register-form").addEventListener("submit", function(event) {
        event.preventDefault();
        register();
    });
}

// Función para mostrar los libros
function showBooks() {
    clearContent();
    // Ocultar todos los formularios
    document.getElementById("book-form").style.display = "none";
    document.getElementById("edit-book-form").style.display = "none";

    const content = document.getElementById("content");
    content.style.display = "block"; // Mostrar la sección principal
    content.innerHTML = `<h2>Lista de Libros</h2><ul id="books-list"></ul>
        <button onclick="showBookForm()">Registrar Nuevo Libro</button>`;

    fetch('http://localhost:15663/api/books', {
        method: 'GET',
        headers: {
            'Authorization': 'Bearer ' + localStorage.getItem('token')
        }
    })
    .then(response => response.json())
    .then(books => {
        const booksList = document.getElementById('books-list');
        booksList.innerHTML = '';

        books.forEach(book => {
            const li = document.createElement('li');
            li.textContent = `Título: ${book.Title}, Autor: ${book.Author}, ISBN: ${book.ISBN}, Cantidad: ${book.Quantity}`;

            const editBtn = document.createElement('button');
            editBtn.textContent = 'Editar';
            editBtn.onclick = () => showEditBookForm(book);
            li.appendChild(editBtn);

            booksList.appendChild(li);
        });
    })
    .catch(err => console.error(err));
}

function showBookForm() {
    document.getElementById("book-form").style.display = "block";
    document.getElementById("edit-book-form").style.display = "none";
    document.getElementById("content").style.display = "none"; // ocultar la lista mientras registras
}

function showEditBookForm(book) {
    if (!book) return;

    currentBookId = book.Id;
    document.getElementById("edit-book-id").value = book.Id;
    document.getElementById("edit-book-title").value = book.Title;
    document.getElementById("edit-book-author").value = book.Author;
    document.getElementById("edit-book-isbn").value = book.ISBN;
    document.getElementById("edit-book-quantity").value = book.Quantity;

    document.getElementById("edit-book-form").style.display = "block";
    document.getElementById("book-form").style.display = "none";
    document.getElementById("content").style.display = "none";
}


let currentBookId = null;

function showEditBookForm(book) {
    // Asegúrate de que book no sea null/undefined
    if (!book) {
        alert("Error: libro no válido.");
        return;
    }

    // Asignar valores a los campos del formulario de edición
    currentBookId = book.id;
    document.getElementById("edit-book-id").value = book.Id || '';
    document.getElementById("edit-book-title").value = book.Title || '';
    document.getElementById("edit-book-author").value = book.Author || '';
    document.getElementById("edit-book-isbn").value = book.ISBN || '';
    document.getElementById("edit-book-quantity").value = book.Quantity || '';

    // Mostrar el formulario de edición y ocultar el formulario de registro
    document.getElementById("edit-book-form").style.display = "block";
    document.getElementById("book-form").style.display = "none"; // Asegúrate de ocultar el formulario de registro
    document.getElementById("content").style.display = "none"; // Ocultar el contenido principal
}

function registerBook() {
    const title = document.getElementById("book-title").value;
    const author = document.getElementById("book-author").value;
    const ISBN = document.getElementById("book-isbn").value;
    const Quantity = document.getElementById("book-quantity").value;

    const bookData = {
        title: title,
        author: author,
        ISBN: ISBN,
        Quantity: Quantity
    };

    fetch('http://localhost:15663/api/books', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + localStorage.getItem('token') // Si usas JWT, incluye el token
        },
        body: JSON.stringify(bookData)
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Error al registrar el libro');
        }
        return response.json();
    })
    .then(data => {
        alert('Libro registrado exitosamente.');
        showBooks(); // Actualizar la lista de libros
    })
    .catch(error => {
        console.error('Error:', error);
        alert('Error al registrar el libro. Por favor, intenta nuevamente.');
    });
}

// Función para editar un libro
function editBook() {
    const id = document.getElementById("edit-book-id").value;
    const title = document.getElementById("edit-book-title").value;
    const author = document.getElementById("edit-book-author").value;
    const ISBN = document.getElementById("edit-book-isbn").value;
    const Quantity = document.getElementById("edit-book-quantity").value;

    const bookData = {
        id: id,
        title: title,
        author:author,
        isbn: ISBN,
        Quantity: Quantity
    };

    fetch(`http://localhost:15663/api/books/${id}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + localStorage.getItem('token') // Si usas JWT, incluye el token
        },
        body: JSON.stringify(bookData)
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Error al actualizar el libro');
        }
        return response.json();
    })
    .then(data => {
        alert('Libro actualizado exitosamente.');
        showBooks(); // Actualizar la lista de libros
    })
    .catch(error => {
        console.error('Error:', error);
        alert('Error al actualizar el libro. Por favor, intenta nuevamente.');
        showBooks();
    });
}
function clearContent() {
    document.getElementById('content').innerHTML = "";
}

// Función para mostrar los préstamos
function showLoans() {
    clearContent();
    document.getElementById("loan-form").style.display = "none";
    document.getElementById("edit-loan-form").style.display = "none";
    

    const content = document.getElementById("content");
    content.innerHTML = `<h2>Lista de Préstamos</h2><ul id="loans-list"></ul><button onclick="showLoanForm()">Registrar Nuevo Préstamo</button>`; // Crear un contenedor para la lista de préstamos
    // Obtener la lista de préstamos desde la API
    fetch('http://localhost:15663/api/loans', { // Cambia la URL según tu configuración
        method: 'GET',
        headers: {
            'Authorization': 'Bearer ' + localStorage.getItem('token') // Si usas JWT, incluye el token
        }
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Error al obtener los préstamos');
        }
        return response.json();
    })
    .then(loans => {
        const loansList = document.getElementById('loans-list');
        loansList.innerHTML = ''; // Limpiar la lista antes de agregar nuevos elementos

        // Iterar sobre los préstamos y agregarlos a la lista
        loans.forEach(loan => {
            const listItem = document.createElement('li');

            // Crear nodo de texto
            const textNode = document.createTextNode(
                `Libro: ${loan.BookTitle}, Usuario: ${loan.Name}, Fecha de Préstamo: ${loan.LoanDate}, Fecha de Entrega: ${loan.DueDate}, Estado: ${loan.IsReturned ? 'Devuelto' : 'No devuelto'} `
            );
            listItem.appendChild(textNode);

            // Botón
            const editButton = document.createElement('button');
            editButton.textContent = 'Editar';
            editButton.onclick = () => showEditLoanForm(loan);
            listItem.appendChild(editButton);

            loansList.appendChild(listItem);
        });
    })
    .catch(error => {
        console.error('Error:', error);
        alert('Error al cargar los préstamos. Por favor, intenta nuevamente.');
    });
}

function showEditLoanForm(loan) {
    clearContent();
    // Asegúrate de que loan no sea null/undefined
    if (!loan) {
        alert("Error: préstamo no válido.");
        return;
    }

    document.getElementById("edit-loan-id").value = loan.Id || '';
    document.getElementById("edit-loan-book-id").value = loan.BookId || '';
    document.getElementById("edit-loan-user-id").value = loan.UserId || '';

    const formatISODate = (dateStr) => {
        if (!dateStr) return '';
        const date = new Date(dateStr);
        return isNaN(date.getTime()) ? '' : date.toISOString().split('T')[0];
    };

    document.getElementById("edit-loan-date").value = formatISODate(loan.LoanDate);
    document.getElementById("edit-due-date").value = formatISODate(loan.DueDate);

    // Mostrar formulario
    document.getElementById("edit-loan-form").style.display = "block";
    document.getElementById("loan-form").style.display = "none"; // Asegúrate de ocultar el formulario de registro
    document.getElementById("content").style.display = "none"; // Ocultar el contenido principal
}

function registerLoan() {
    const bookId = document.getElementById("loan-book-id").value;
    const userId = document.getElementById("loan-user-id").value;
    const loanDate = document.getElementById("loan-date").value;
    const dueDate = document.getElementById("due-date").value;

    const loanData = {
        bookId: bookId,
        userId: userId,
        loanDate: loanDate,
        dueDate: dueDate,
        isReturned: false // Inicialmente, el préstamo no está devuelto
    };

    fetch('http://localhost:15663/api/loans', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + localStorage.getItem('token') // Si usas JWT, incluye el token
        },
        body: JSON.stringify(loanData)
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Error al registrar el préstamo');
        }
        return response.json();
    })
    .then(data => {
        alert('Préstamo registrado exitosamente.');
        showLoans(); // Actualizar la lista de préstamos
        document.getElementById("tab-loans").click();
    })
    .catch(error => {
        console.error('Error:', error);
        alert('Error al registrar el préstamo. Por favor, intenta nuevamente.');
        showLoans();
        document.getElementById("tab-loans").click();
    });
}

function editLoanFromForm() {
    const loanId = document.getElementById("edit-loan-id").value;
    editLoan(loanId); // Ahora editLoan solo necesita el ID
}

function editLoan(loanId) {
    const bookId = document.getElementById("edit-loan-book-id").value;
    const userId = document.getElementById("edit-loan-user-id").value;
    const loanDate = document.getElementById("edit-loan-date").value;
    const dueDate = document.getElementById("edit-due-date").value;

    // Validación básica
    if (!bookId || !userId || !loanDate || !dueDate) {
        alert("Por favor completa todos los campos.");
        return;
    }

    const loanData = {
        id: loanId,
        bookId: parseInt(bookId, 10),
        userId: parseInt(userId, 10),
        loanDate: loanDate, // Ya está en formato YYYY-MM-DD (por input type="date")
        dueDate: dueDate
        // No incluimos isReturned para no sobrescribirlo
    };

    fetch(`http://localhost:15663/api/loans/${loanId}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + localStorage.getItem('token')
        },
        body: JSON.stringify(loanData)
    })
    .then(response => {
        if (!response.ok) {
            return response.text().then(text => {
                throw new Error(`Error ${response.status}: ${text}`);
            });
        }
        return response.json();
    })
    .then(() => {
        alert('Préstamo actualizado exitosamente.');
        showLoans(); // Recargar la lista
        document.getElementById("edit-loan-form").style.display = "none"; // Ocultar form de edición
        document.getElementById("content").style.display = "block"; // Mostrar la lista de nuevo
    })
    .catch(error => {
        console.error('Error:', error);
        alert('Error al actualizar el préstamo: ' + error.message);
    });
}

// Función para iniciar sesión
function login() {
    const email = document.getElementById("login-email").value;
    const password = document.getElementById("login-password").value;

    if (!email || !password) {
        alert("Por favor, completa todos los campos.");
        return;
    }

    const loginData = { email, password };

    fetch('http://localhost:15663/api/auth/login', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'X-Email': email
        },
        body: JSON.stringify(loginData)
    })
    .then(response => {
        if (response.status === 429) {
            alert("Demasiados intentos. Por favor, espera un minuto antes de volver a intentar.");
            throw new Error("Too many requests");
        }
        if (!response.ok) {
            throw new Error('Error en la autenticación');
        }
        return response.json(); // Solo se llama una vez
    })
    .then(data => {
        localStorage.setItem('token', data.Token);
        alert("Inicio de sesión exitoso. Token recibido:\n\n" + data.Token);
        window.location.href = 'index.html';
    })
    .catch(error => {
        console.error('Error:', error);
        alert(error.message);
    });
}


// Función para registrarse
function register() {
    const name = document.getElementById("register-name").value;
    const email = document.getElementById("register-email").value;
    const password = document.getElementById("register-password").value;

    // Validar que los campos no estén vacíos
    if (!name ||!email || !password) {
        alert("Por favor, completa todos los campos.");
        return;
    }

    // Crear el objeto de datos para enviar
    const registerData = {
        email: email,
        password: password,
        name: name
    };

    // Enviar la solicitud de registro a la API
    fetch('http://localhost:15663/api/auth/register', { // Cambia la URL según tu configuración
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(registerData)
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Error en el registro');
        }
        return response.json();
    })
    .then(data => {
        alert('Registro exitoso. Ahora puedes iniciar sesión.');
        showLogin(); // Mostrar el formulario de inicio de sesión después del registro
    })
    .catch(error => {
        console.error('Error:', error);
        alert('Error al registrarse. Por favor, intenta nuevamente.' + error);
    });
}
