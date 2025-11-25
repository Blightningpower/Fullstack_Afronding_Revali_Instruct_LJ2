import api from '../services/api';

const base = '/patients';

function normalizePatientsResponse(payload) {
  // payload might be: [] or { items: [...] } or { data: [...] } or { patients: [...] } or { data: { items: [...] } }
  let arr = [];

  if (!payload) return [];

  if (Array.isArray(payload)) arr = payload;
  else if (Array.isArray(payload.items)) arr = payload.items;
  else if (Array.isArray(payload.data)) arr = payload.data;
  else if (payload.data && Array.isArray(payload.data.items)) arr = payload.data.items;
  else if (Array.isArray(payload.patients)) arr = payload.patients;
  else {
    // fallback: probeer eerste array-property
    const firstArray = Object.values(payload).find(v => Array.isArray(v));
    if (firstArray) arr = firstArray;
  }

  // map naar consistente vorm
  return arr.map(p => {
    const firstName = p.firstName ?? p.FirstName ?? p.first_name ?? '';
    const lastName  = p.lastName ?? p.LastName ?? p.last_name ?? '';

    return {
      id: p.id ?? p.Id ?? null,
      firstName,
      lastName,
      name: `${firstName} ${lastName}`.trim(),
      status: p.status ?? p.Status ?? null,
      startDate: p.startDate ?? p.StartDate ?? null,
      assignedDoctorUserId: p.assignedDoctorUserId ?? p.AssignedDoctorUserId ?? null,
      _raw: p // originele record erbij voor debug / extra velden
    };
  });
}

// Hoofd–fetch voor lijsten
export async function fetchPatients({ q, status, page = 1, pageSize = 20, sort } = {}) {
  const params = {};
  if (q) params.q = q;
  if (status) params.status = status;
  params.page = page;
  params.pageSize = pageSize;
  if (sort) params.sort = sort;

  const response = await api.get(base, { params });
  return normalizePatientsResponse(response.data);
}

// Alias, voor het geval andere code nog getPatients gebruikt
export const getPatients = async (params) => {
  const response = await api.get(base, { params });
  return normalizePatientsResponse(response.data);
};

// Eén patiënt ophalen
export async function fetchPatient(id) {
  try {
    const response = await api.get(`${base}/${id}`);
    const list = normalizePatientsResponse([response.data]);
    return list[0] ?? null;
  } catch (err) {
    console.error('fetchPatient failed', { id, err });
    throw err; // laat de caller beslissen of er naar /login geredirect wordt
  }
}

// Alias idem
export const getPatientById = async (id) => {
  try {
    const response = await api.get(`${base}/${id}`);
    const list = normalizePatientsResponse([response.data]);
    return list[0] ?? null;
  } catch (err) {
    console.error('getPatientById failed', { id, err });
    throw err;
  }
};

// Dossier–endpoint
export const getPatientDossier = async (id) => {
  try {
    const res = await api.get(`${base}/${id}/dossier`);
    return res.data;
  } catch (err) {
    console.error('getPatientDossier failed', { id, err });
    throw err;
  }
};