import { login, register, logout } from "./auth.js";
import { state } from "./state.js";
import { getRoute, setRoute } from "./router.js";
import { getMyDrafts} from "./retake.js";


const app = document.getElementById("app");
const pageTitle = document.getElementById("pageTitle");
const pageSubtitle = document.getElementById("pageSubtitle");
const userInfo = document.getElementById("userInfo");
const toast = document.getElementById("toast");
const nav = document.getElementById("nav");
const logoutButton = document.getElementById("logoutButton");
const themeToggle = document.getElementById("themeToggle");

function showToast(message) {
  toast.textContent = message;
  toast.classList.add("is-visible");
  setTimeout(() => toast.classList.remove("is-visible"), 2500);
}

function setTopbar(title, subtitle) {
  pageTitle.textContent = title;
  pageSubtitle.textContent = subtitle;
  userInfo.textContent = state.user
    ? `${state.user.email} (${state.user.role})`
    : "Не авторизован";
}

function renderLoginPage() {
  setTopbar("Вход", "Авторизация преподавателя");
  app.innerHTML = `
    <div class="auth-page">
      <div class="card">
        <form class="auth-form" id="loginForm">
          <div class="field">
            <label for="loginEmail">Email</label>
            <input class="input" id="loginEmail" type="email" required />
          </div>
          <div class="field">
            <label for="loginPassword">Пароль</label>
            <input class="input" id="loginPassword" type="password" required />
          </div>
          <div class="page-actions">
            <button class="button button--primary" type="submit">Войти</button>
            <button class="button button--secondary" type="button" id="goRegister">Регистрация</button>
          </div>
        </form>
      </div>
    </div>
  `;

  document.getElementById("loginForm").addEventListener("submit", async (e) => {
    e.preventDefault();

    const email = document.getElementById("loginEmail").value.trim();
    const password = document.getElementById("loginPassword").value.trim();

    try {
      await login(email, password);
      showToast("Успешный вход");
      setRoute("dashboard");
      render();
    } catch (error) {
      showToast(error.body?.message || "Ошибка входа");
    }
  });

  document.getElementById("goRegister").addEventListener("click", () => {
    setRoute("register");
    render();
  });
}

function renderRegisterPage() {
  setTopbar("Регистрация", "Создание аккаунта преподавателя");
  app.innerHTML = `
    <div class="auth-page">
      <div class="card">
        <form class="auth-form" id="registerForm">
          <div class="field">
            <label for="registerEmail">Email</label>
            <input class="input" id="registerEmail" type="email" required />
          </div>
          <div class="field">
            <label for="registerPassword">Пароль</label>
            <input class="input" id="registerPassword" type="password" required />
          </div>
          <div class="page-actions">
            <button class="button button--primary" type="submit">Зарегистрироваться</button>
            <button class="button button--secondary" type="button" id="goLogin">Назад ко входу</button>
          </div>
        </form>
      </div>
    </div>
  `;

  document.getElementById("registerForm").addEventListener("submit", async (e) => {
    e.preventDefault();

    const email = document.getElementById("registerEmail").value.trim();
    const password = document.getElementById("registerPassword").value.trim();

    try {
      await register(email, password, "Teacher");
      showToast("Регистрация успешна");
      setRoute("dashboard");
      render();
    } catch (error) {
      showToast(error.body?.message || "Ошибка регистрации");
    }
  });

  document.getElementById("goLogin").addEventListener("click", () => {
    setRoute("login");
    render();
  });
}

async function renderDashboardPage() {
  setTopbar("Панель", "Ваши направления");
  app.innerHTML = `
    <div class="dashboard-grid">
      <div class="card">
        <h2>Черновики</h2>
        <p>0 направлений</p>
        <button class="button button--primary" data-route="drafts">Открыть</button>
      </div>
    </div>
  `;
}

async function renderDraftsPage() {
  setTopbar("Черновики", "Ваши направления");
  app.innerHTML = `<div class="card">Загрузка...</div>`;
  
  try {
    const drafts = await getMyDrafts();
    let html = `<div class="card"><h3>Черновики (${drafts.length})</h3>`;
    if (drafts.length === 0) {
      html += "<p>Черновиков нет</p>";
    } else {
      html += "<table><tr><th>ID</th><th>Номер</th></tr>";
      drafts.forEach(d => {
        html += `<tr><td>${d.id}</td><td>${d.number}</td></tr>`;
      });
      html += "</table>";
    }
    html += "</div>";
    app.innerHTML = html;
  } catch(e) {
    app.innerHTML = `<div class="card">Ошибка: ${e.body?.message || e.message}</div>`;
  }
}

function renderPlaceholderPage(title, subtitle) {
  setTopbar(title, subtitle);
  app.innerHTML = `
    <div class="card empty-state">
      Раздел в следующем шаге будет подключён к backend.
    </div>
  `;
}

function updateNav() {
  nav.querySelectorAll(".nav__item").forEach((button) => {
    button.classList.toggle("is-active", button.dataset.route === state.currentRoute);
  });
}

function render() {
  state.currentRoute = getRoute();
  updateNav();

  switch (state.currentRoute) {
    case "login":
      renderLoginPage();
      break;
    case "register":
      renderRegisterPage();
      break;
    case "dashboard":
      renderDashboardPage();
      break;
    case "drafts":
      renderDraftsPage();
      break;
    case "create":
      renderPlaceholderPage("Создать", "Форма создания");
      break;
    default:
      renderLoginPage();
      break;
  }
}

nav.addEventListener("click", (e) => {
  const button = e.target.closest("[data-route]");
  if (!button) return;

  setRoute(button.dataset.route);
  render();
});

logoutButton.addEventListener("click", () => {
  logout();
  showToast("Вы вышли из системы");
  setRoute("login");
  render();
});

themeToggle.addEventListener("click", () => {
  const html = document.documentElement;
  const current = html.getAttribute("data-theme");
  html.setAttribute("data-theme", current === "dark" ? "light" : "dark");
});

window.addEventListener("hashchange", render);

render();