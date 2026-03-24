import { useState, useEffect } from 'react';
import { useAuthContext } from '../context/AuthContext.jsx';
import {
  getMyDrafts,
  getGroups,
  createQuick,
  getGroupStudents
} from '../api/retakeDirection.js';

const Dashboard = () => {
  const { user, logout } = useAuthContext();
  const [drafts, setDrafts] = useState([]);
  const [groups, setGroups] = useState([]);
  const [students, setStudents] = useState([]);
  const [loading, setLoading] = useState(true);
  const [selectedGroup, setSelectedGroup] = useState('');

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    setLoading(true);
    try {
      const [draftsRes, groupsRes] = await Promise.all([
        getMyDrafts(),
        getGroups()
      ]);
      
      // 🔥 ФИКС: .data для axios ответа
      setDrafts(draftsRes.data || []);
      setGroups(groupsRes.data || []);
    } catch (error) {
      console.error('Ошибка загрузки:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleGroupChange = async (groupId) => {
    setSelectedGroup(groupId);
    if (groupId) {
      try {
        const studentsRes = await getGroupStudents(groupId);
        setStudents(studentsRes.data || []);
      } catch (error) {
        setStudents([]);
      }
    } else {
      setStudents([]);
    }
  };

  if (loading) return <div className="loading">⏳ Загрузка...</div>;

  return (
    <div className="dashboard">
      <header>
        <h1>📋 Направления ({drafts.length})</h1>
        <div>
          <span>👤 {user?.id}</span>
          <button onClick={logout}>Выход</button>
        </div>
      </header>

      {/* Быстрое создание */}
      <section className="quick-create">
        <h2>⚡ Быстрое создание</h2>
        <form>
          <input placeholder="Название" />
          <select onChange={(e) => handleGroupChange(e.target.value)}>
            <option>Группа</option>
            {groups.map(group => (
              <option key={group.value} value={group.value}>{group.text}</option>
            ))}
          </select>
          <select disabled={!selectedGroup}>
            <option>Студент</option>
            {students.map(student => (
              <option key={student.value} value={student.value}>{student.text}</option>
            ))}
          </select>
          <button>Создать</button>
        </form>
      </section>

      {/* Черновики */}
      <section className="drafts">
        <h2>📄 Черновики ({drafts.length})</h2>
        <div className="drafts-grid">
          {drafts.map(draft => (
            <div key={draft.id} className="draft-card">
              <h3>{draft.number} — {draft.disciplineName}</h3>
              <p>Группа: {draft.groupName}</p>
              <p>Преподаватель: {draft.teacherFullName}</p>
              <p>Студентов: {draft.students?.length || 0}</p>
              <p>Статус: <span className={`status-${draft.status}`}>{draft.status}</span></p>
              <div>
                <a href={`/api/RetakeDirections/${draft.id}/pdf`}>📄 PDF</a>
                <button>✏️ Редактировать</button>
              </div>
            </div>
          ))}
        </div>
      </section>
    </div>
  );
};

export default Dashboard;
