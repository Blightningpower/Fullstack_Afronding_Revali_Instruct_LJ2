import api from '../services/api';

const base = '/api/patients';

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
		// fallback: try to find first array-valued prop
		const firstArray = Object.values(payload).find(v => Array.isArray(v));
		if (firstArray) arr = firstArray;
	}

	// map to consistent shape
	return arr.map(p => ({
		id: p.id ?? p.Id ?? null,
		firstName: p.firstName ?? p.FirstName ?? p.first_name ?? '',
		lastName: p.lastName ?? p.LastName ?? p.last_name ?? '',
		name: (p.firstName ?? p.FirstName ?? p.first_name ?? '') + ' ' + (p.lastName ?? p.LastName ?? p.last_name ?? ''),
		status: p.status ?? p.Status ?? null,
		startDate: p.startDate ?? p.StartDate ?? null,
		assignedDoctorUserId: p.assignedDoctorUserId ?? p.AssignedDoctorUserId ?? null,
		_raw: p // keep original if needed
	}));
}

export async function fetchPatients({ q, status, page = 1, pageSize = 20, sort } = {}) {
  const params = {};
  if (q) params.q = q;
  if (status) params.status = status;
  params.page = page;
  params.pageSize = pageSize;
  if (sort) params.sort = sort;

  // gebruik centrale api instance; interceptor voegt token toe indien aanwezig
  const response = await api.get('/patients', { params });
  return normalizePatientsResponse(response.data);
}

export async function fetchPatient(id) {
  const response = await api.get(`/patients/${id}`);
  const list = normalizePatientsResponse([response.data]);
  return list[0] ?? null;
}

export const getPatients = async (params) => {
  const response = await api.get('/patients', { params });
  return normalizePatientsResponse(response.data);
}

export const getPatientById = async (id) => {
  const response = await api.get(`/patients/${id}`);
  const list = normalizePatientsResponse([response.data]);
  return list[0] ?? null;
}

export const getPatientDossier = async (id) => {
  const res = await api.get(`/patients/${id}/dossier`);
  return res.data;
}