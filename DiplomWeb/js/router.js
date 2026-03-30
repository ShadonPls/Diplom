import { state } from "./state.js";

export function setRoute(route) {
  state.currentRoute = route;
  window.location.hash = route;
}

export function getRoute() {
  const hash = window.location.hash.replace("#", "").trim();
  return hash || "login";
}