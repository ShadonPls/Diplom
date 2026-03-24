//DiplomWeb/src/dashboard.js
const form = document.getElementById('loginForm');
const token = localStorage.getItem('token');

if (token) {
  initDashboard();
} else {
  form.onsubmit = (e) => {  // Простой обработчик без module проблем
    e.preventDefault();
    const user = document.getElementById('username').value;
    const pass = document.getElementById('password').value;
    if (user === 'admin' && pass === '123') {
      localStorage.setItem('token', 'valid');
      initDashboard();
    } else {
      alert('Неверный логин/пароль');
    }
  };
}

function initDashboard() {
  document.getElementById('app').innerHTML = `
    <h1>Dashboard</h1>
    <p>Добро пожаловать!</p>
    <button id="logout">Выход</button>
  `;
  document.getElementById('logout').onclick = () => {
    localStorage.removeItem('token');
    location.reload();
  };
}
