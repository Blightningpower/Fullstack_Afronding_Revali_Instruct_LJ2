<template>
  <div class="audit-container">
    <header class="audit-header">
      <h2 class="title">🛡️ Systeem Audit Trail</h2>
      <p class="subtitle">Audit Overzicht</p>
    </header>

    <div class="audit-card card shadow-sm">
      <div class="table-responsive">
        <table class="audit-table">
          <thead>
            <tr>
              <th>Datum & Tijd</th>
              <th>Gebruiker (ID)</th>
              <th>Actie</th>
              <th>Tabel</th>
              <th>Record ID</th>
              <th>Details</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="log in logs" :key="log.id" :class="getActionClass(log.action)">
              <td class="timestamp">{{ formatDateTime(log.timestamp) }}</td>
              <td class="user-id">👤 Arts #{{ log.userId }}</td>
              <td>
                <span class="action-badge">{{ log.action }}</span>
              </td>
              <td class="table-name"><code>{{ log.tableName }}</code></td>
              <td>#{{ log.recordId }}</td>
              <td class="details">{{ log.details }}</td>
            </tr>
          </tbody>
        </table>
      </div>
      
      <div v-if="logs.length === 0" class="empty-state">
        Geen audit-logs gevonden in het systeem.
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import api from '../../api/axios';

const logs = ref([]);

const fetchLogs = async () => {
  try {
    const response = await api.get('/auditlogs');
    logs.value = response.data;
  } catch (err) {
    console.error("Geen toegang tot audit logs:", err);
  }
};

const formatDateTime = (d) => new Date(d).toLocaleString('nl-NL');
const getActionClass = (action) => `row-${action.toLowerCase()}`;

onMounted(fetchLogs);
</script>

<style scoped>
.audit-container { padding: 2rem; max-width: 1200px; margin: 0 auto; }
.audit-header { margin-bottom: 2rem; }
.title { color: #2a7ca3; font-weight: 800; }
.subtitle { color: #718096; }

.audit-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.9rem;
}

.audit-table th {
  background: #f8fafc;
  padding: 12px;
  text-align: left;
  border-bottom: 2px solid #e2e8f0;
  color: #4a5568;
}

.audit-table td {
  padding: 12px;
  border-bottom: 1px solid #edf2f7;
}

/* Kleurcodering per actie conform AC6 */
.row-insert { border-left: 4px solid #48bb78; }
.row-update { border-left: 4px solid #ecc94b; }
.row-delete { border-left: 4px solid #e53e3e; }

.action-badge {
  font-weight: 800;
  font-size: 0.75rem;
  text-transform: uppercase;
  padding: 4px 8px;
  border-radius: 4px;
  background: #edf2f7;
}

.table-name code {
  background: #f1f5f9;
  padding: 2px 6px;
  border-radius: 4px;
  color: #2d3748;
}

.timestamp { font-weight: 600; color: #2d3748; }
.details { font-style: italic; color: #4a5568; }
</style>