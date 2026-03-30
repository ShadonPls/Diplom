// js/retake.js
import { api } from "./api.js";
import { state } from "./state.js";
import { showToast } from "../app.js"; 

// Направления
export function getMyDrafts() {
  return Promise.resolve([]);
}
export async function getDirection(id) {
  return await api.get(`/retake-directions/${id}`);
}

export async function createDirection(data) {
  return await api.post("/retake-directions", data);
}

export async function updateDirection(id, data) {
  return await api.put(`/retake-directions/${id}`, data);
}

export async function publishDirection(id) {
  return await api.post(`/retake-directions/${id}/publish`);
  showToast("Направление опубликовано!");
}

// PDF
export async function downloadPdf(id) {
  try {
    const blob = await api.get(`/documents/retake-directions/${id}/pdf`);
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `napravlenie-${id}.pdf`;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
    showToast("PDF скачан");
  } catch (error) {
    showToast("Ошибка PDF: " + error.body?.message);
  }
}

// Справочники
export async function getGroups() {
  return await api.get("/groups");
}

export async function getGroupStudents(groupId) {
  return await api.get(`/groups/${groupId}/students`);
}

export async function getDisciplines() {
  return await api.get("/lookup/disciplines");
}

export async function getAttestTypes() {
  return await api.get("/lookup/attest-types");
}