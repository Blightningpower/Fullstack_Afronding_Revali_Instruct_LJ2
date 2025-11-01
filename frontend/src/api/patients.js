const base = '/api/patients';

export async function fetchPatients({ q, status, page = 1, pageSize = 20, sort } = {}) {
  const params = new URLSearchParams();
  if (q) params.append('q', q);
  if (status) params.append('status', status);
  params.append('page', page);
  params.append('pageSize', pageSize);
  if (sort) params.append('sort', sort);

  const res = await fetch(`${base}?${params.toString()}`);
  if (!res.ok) throw new Error(`API error ${res.status}`);
  return res.json(); // { data: [], page, pageSize } volgens backend
}

export async function fetchPatient(id) {
  const res = await fetch(`${base}/${id}`);
  if (!res.ok) {
    if (res.status === 404) return null;
    throw new Error(`API error ${res.status}`);
  }
  return res.json();
}