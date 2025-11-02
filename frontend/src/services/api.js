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

// voorbeeld fetch + render (vanuit browser)
async function fetchAndRenderPatients() {
  try {
    const res = await fetch('/api/patients'); // controleer dat dit pad klopt
    if (!res.ok) throw new Error(`HTTP ${res.status}`);
    const patients = await res.json();

    const container = document.getElementById('patients');
    container.innerHTML = '';

    patients.forEach(p => {
      // Pas hier velden aan naar jouw API response
      const id = p.Id ?? p.id;
      const first = p.FirstName ?? p.firstName;
      const last = p.LastName ?? p.lastName;
      const dobRaw = p.DateOfBirth ?? p.dateOfBirth; // verwacht ISO string of yyyy-mm-dd
      const dob = formatDDMMYYYY(dobRaw);

      const el = document.createElement('div');
      el.className = 'patient';
      el.innerHTML = `<strong>${first} ${last}</strong> — Geb: ${dob}`;
      container.appendChild(el);
    });
  } catch (err) {
    console.error('Fetch patients failed', err);
    document.getElementById('patients').textContent = 'Fout bij ophalen patiënten: ' + err.message;
  }
}

function formatDDMMYYYY(dateInput) {
  if (!dateInput) return '';
  const d = (dateInput instanceof Date) ? dateInput : new Date(dateInput);
  if (isNaN(d)) return '';
  const dd = String(d.getDate()).padStart(2,'0');
  const mm = String(d.getMonth()+1).padStart(2,'0');
  const yyyy = d.getFullYear();
  return dd + mm + yyyy; // ddmmyyyy zoals je vroeg
}

// run on load
document.addEventListener('DOMContentLoaded', fetchAndRenderPatients);