// frontend/src/services/api.js
export async function fetchPatients({ q, status, page = 1, pageSize = 20, sort } = {}) {
  const params = new URLSearchParams();
  if (q) params.append('q', q);
  if (status) params.append('status', status);
  params.append('page', page);
  params.append('pageSize', pageSize);
  if (sort) params.append('sort', sort);

  const res = await fetch(`/api/patients?${params.toString()}`);
  if (!res.ok) throw new Error('Network error fetching patients');
  const json = await res.json();
  // verwacht object { data, page, pageSize } (zoals jouw controller return)
  return json;
}

export async function fetchPatientById(id) {
  const res = await fetch(`/api/patients/${id}`);
  if (!res.ok) throw new Error('Network error fetching patient');
  return res.json();
}