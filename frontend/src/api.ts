export async function apiFetch(input: RequestInfo, init?: RequestInit) {
	// haal token uit localStorage (of waar jij 'm opslaat)
	const token = localStorage.getItem("token") ?? "";

	const headers = new Headers(init?.headers ?? {});
	// Zorg dat we JSON verwachten (pas aan indien je andere content gebruikt)
	if (!headers.has("Accept")) headers.set("Accept", "application/json");
	if (!headers.has("Content-Type") && !(init?.method && init.method.toUpperCase() === "GET")) {
		headers.set("Content-Type", "application/json");
	}

	// Voeg Authorization only when token present
	if (token) headers.set("Authorization", `Bearer ${token}`);

	const res = await fetch(input, {
		...init,
		credentials: init?.credentials ?? "same-origin", // of "include" als je cookies gebruikt
		headers
	});

	// optionele handige foutafhandeling
	if (!res.ok) {
		// probeer JSON message te lezen
		let body: any = null;
		try { body = await res.json(); } catch { }
		const msg = body?.error ?? body?.message ?? res.statusText;
		const e = new Error(`API ${res.status}: ${msg}`);
		// @ts-ignore voeg response mee
		e.response = res;
		throw e;
	}

	// als geen content
	if (res.status === 204) return null;
	const text = await res.text();
	if (!text) return null;
	try { return JSON.parse(text); } catch { return text; }
}
