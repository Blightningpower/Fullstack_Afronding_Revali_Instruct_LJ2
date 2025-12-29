import api from '../services/api';

const base = '/patients';

function normalizePatientsResponse(payload) {
  let arr = [];
  if (!payload) return [];
  if (Array.isArray(payload)) arr = payload;
  else if (Array.isArray(payload.data)) arr = payload.data;
  else arr = Object.values(payload).find(v => Array.isArray(v)) || [];

  return arr.map(p => {
    const fName = p.firstName ?? p.FirstName ?? '';
    const lName = p.lastName ?? p.LastName ?? '';

    return {
      id: p.id ?? p.Id ?? null,
      firstName: fName,
      lastName: lName,
      status: p.status ?? p.Status ?? 0,
      startDate: p.startDate ?? p.StartDate ?? null,
      _raw: p 
    };
  });
}

export async function getPatients(params) {
  const response = await api.get(base, { params });
  return normalizePatientsResponse(response.data);
}

export async function getPatientDossier(id) {
  const res = await api.get(`${base}/${id}/dossier`);
  return res.data;
}