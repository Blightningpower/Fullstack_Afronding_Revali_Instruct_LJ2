export async function apiFetch(input: RequestInfo, init?: RequestInit) {

	const token = localStorage.getItem("token") ?? "";

	const headers = new Headers(init?.headers ?? {});
	if (!headers.has("Accept")) headers.set("Accept", "application/json");
	if (!headers.has("Content-Type") && !(init?.method && init.method.toUpperCase() === "GET")) {
		headers.set("Content-Type", "application/json");
	}

	if (token) headers.set("Authorization", `Bearer ${token}`);

	const res = await fetch(input, {
		...init,
		credentials: init?.credentials ?? "same-origin",
		headers
	});

	if (!res.ok) {
  let body: any = null;
  try { body = await res.json(); } catch {}

  if (res.status === 401) {
    try { localStorage.removeItem("token"); } catch {}
    window.location.href = "/login";
  }

  const msg = body?.error ?? body?.message ?? res.statusText;
  const e: any = new Error(`API ${res.status}: ${msg}`);
  e.response = res;
  throw e;
}

	if (res.status === 204) return null;
	const text = await res.text();
	if (!text) return null;
	try { return JSON.parse(text); } catch { return text; }
}
