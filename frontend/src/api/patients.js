import api from '../services/api';

const base = '/patients';

function normalizePatientsResponse(payload) {
  let arr = [];
  if (!payload) return [];
  if (Array.isArray(payload)) arr = payload;
  else if (Array.isArray(payload.data)) arr = payload.data;
  else arr = Object.values(payload).find(v => Array.isArray(v)) || [];

  return arr.map(p => {
    // Pak de gecombineerde naam van de API (FullName)
    const combinedName = p.fullName ?? p.FullName ?? '';
    
    // Splits de naam voor de frontend velden firstName/lastName (optioneel)
    const nameParts = combinedName.split(' ');
    const fName = nameParts[0] || '';
    const lName = nameParts.slice(1).join(' ') || '';

    return {
      id: p.id ?? p.Id ?? null,
      firstName: fName, // Nu gevuld vanuit FullName
      lastName: lName,
      name: combinedName.trim() || 'Onbekende patiënt',
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