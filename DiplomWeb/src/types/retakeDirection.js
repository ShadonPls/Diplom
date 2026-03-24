//DiplomWeb/src/types/retakeDirection.js
export const RetakeDirectionResponseDto = {
  id: 0,
  title: '',
  groupId: 0,
  groupName: '',
  studentId: 0,
  studentName: '',
  status: 'draft', // draft, submitted, approved
  createdAt: ''
};

export const CreateQuickDto = {
  title: '',
  groupId: 0,
  studentId: 0
};

export const CreateFormDto = {
  title: '',
  groupId: 0,
  studentId: 0,
  reason: '',
  documents: []
};
