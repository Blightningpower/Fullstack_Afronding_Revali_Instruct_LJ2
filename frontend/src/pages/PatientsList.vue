<template>
  <div class="container">
    <h1 class="page-title">Patiënten</h1>

    <div class="controls">
      <input type="text" v-model="q" placeholder="Zoek op naam..." />
      <select v-model="statusFilter">
        <option value="">Alle statussen</option>
        <option value="IntakeGepland">Intake gepland</option>
        <option value="Actief">Actief</option>
        <option value="Afgerond">Afgerond</option>
        <option value="OnHold">On hold</option>
      </select>
      <button class="btn" @click="load">Zoeken</button>
      <div style="margin-left:auto" class="muted">Pagina {{ page }}</div>
    </div>

    <div class="card">
      <table class="table" aria-live="polite">
        <thead>
          <tr>
            <th>Naam</th>
            <th>Startdatum</th>
            <th>Status</th>
          </tr>
        </thead>
        <tbody>
          <tr v-if="!loading && patients.length === 0">
            <td class="empty" colspan="3">Geen patiënten gevonden</td>
          </tr>

          <tr v-for="p in patients" :key="p.id">
            <td data-label="Naam">
              <router-link :to="`/patients/${p.id}`">{{ p.lastName }}, {{ p.firstName }}</router-link>
            </td>
            <td data-label="Startdatum">{{ formatDate(p.startDate) }}</td>
            <td data-label="Status">{{ p.status }}</td>
          </tr>
        </tbody>
      </table>

      <div class="pagination">
        <button class="page-button" :disabled="page <= 1" @click="prevPage">Vorige</button>
        <div>Pagina {{ page }}</div>
        <button class="page-button" :disabled="!hasMore" @click="nextPage">Volgende</button>
      </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios';

export default {
  name: 'PatientsList',

  data() {
    return {
      q: '',
      statusFilter: '',
      patients: [],
      loading: false,
      page: 1,
      pageSize: 10,
      hasMore: false
    };
  },

  methods: {
    async load() {
      this.loading = true;
      try {
        const params = {
          q: this.q || undefined,
          status: this.statusFilter || undefined,
          page: this.page,
          pageSize: this.pageSize
        };

        // API base: zet VITE_API_URL in .env als je backend op andere poort draait
        const API_BASE = import.meta.env.VITE_API_URL || 'http://localhost:5000';

        const resp = await axios.get(`${API_BASE}/api/patients`, { params });

        // Mogelijke vormen: { items: [...], total } of direct array
        if (resp.data && Array.isArray(resp.data.items)) {
          this.patients = resp.data.items;
          const total = resp.data.total ?? this.patients.length;
          this.hasMore = (this.page * this.pageSize) < total;
        } else if (Array.isArray(resp.data)) {
          this.patients = resp.data;
          this.hasMore = this.patients.length === this.pageSize;
        } else {
          this.patients = [];
          this.hasMore = false;
        }
      } catch (err) {
        console.error('Load patients failed', err);
        this.patients = [];
        this.hasMore = false;
      } finally {
        this.loading = false;
      }
    },

    prevPage() {
      if (this.page > 1) {
        this.page--;
        this.load();
      }
    },

    nextPage() {
      this.page++;
      this.load();
    },

    // enkel één formatDate-functie (dd-mm-jjjj)
    formatDate(dateString) {
      if (!dateString) return '';
      const d = new Date(dateString);
      if (isNaN(d)) return dateString;
      const day = String(d.getDate()).padStart(2, '0');
      const month = String(d.getMonth() + 1).padStart(2, '0');
      const year = d.getFullYear();
      return `${day}-${month}-${year}`;
    }
  },

  mounted() {
    this.load(); // laad direct bij openen
  }
};
</script>