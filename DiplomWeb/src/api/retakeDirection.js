import apiClient from './apiClient.js';

export const getMyDrafts = () => apiClient.get('RetakeDirections/my');
export const createQuick = (dto) => apiClient.post('RetakeDirections/quick', dto);
export const createForm = (dto) => apiClient.post('RetakeDirections/form', dto);
export const getGroups = () => apiClient.get('RetakeDirections/groups');
export const getGroupStudents = (groupId) => apiClient.get(`RetakeDirections/groups/${groupId}/students`);
export const downloadPdf = (id) => apiClient.get(`RetakeDirections/${id}/pdf`, { responseType: 'blob' });
