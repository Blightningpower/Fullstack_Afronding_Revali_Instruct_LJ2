<template>
  <div class="info-grid-container">
    <div class="patient-info-grid">
      <div class="info-item">
        <div class="info-label">
          <span class="info-icon">🎂</span>
          <span>Geboortedatum</span>
        </div>
        <div class="info-value">
          {{ (patient.birthDate || patient.BirthDate) 
             ? formatDateLong(patient.birthDate || patient.BirthDate) 
             : 'Niet ingevuld' }}
        </div>
      </div>

      <div class="info-item">
        <div class="info-label">
          <span class="info-icon">📅</span>
          <span>Startdatum behandeling</span>
        </div>
        <div class="info-value">
          {{ (patient.startDate || patient.StartDate) 
             ? formatDateLong(patient.startDate || patient.StartDate) 
             : 'Niet ingevuld' }}
        </div>
      </div>

      <div class="info-item">
        <div class="info-label">
          <span class="info-icon">📧</span>
          <span>E-mail</span>
        </div>
        <div class="info-value">
          <template v-if="patient?.email || patient?.Email">
            <a :href="`mailto:${patient.email || patient.Email}`" class="contact-link">
              {{ patient.email || patient.Email }}
            </a>
          </template>
          <template v-else><span class="muted">Niet ingevuld</span></template>
        </div>
      </div>

      <div class="info-item">
        <div class="info-label">
          <span class="info-icon">📞</span>
          <span>Telefoon</span>
        </div>
        <div class="info-value">
          {{ patient?.phone || patient?.Phone || 'Niet ingevuld' }}
        </div>
      </div>

      <div class="info-item">
        <div class="info-label">
          <span class="info-icon">🩺</span>
          <span>Verwijzend arts</span>
        </div>
        <div class="info-value highlight">
          {{ patient?.referringDoctor?.firstName || patient?.ReferringDoctor?.FirstName }}
          {{ patient?.referringDoctor?.lastName || patient?.ReferringDoctor?.LastName }}
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
const props = defineProps(['patient']);

const formatDateLong = (dateString) => {
  if (!dateString) return 'Nog niet bekend';
  const date = new Date(dateString);
  return date.toLocaleDateString('nl-NL', {
    day: 'numeric',
    month: 'long',
    year: 'numeric'
  });
};
</script>

<style scoped>
.info-grid-container {
  margin-top: 24px;
  padding: 20px;
  background: #fafdff;
  border-radius: 12px;
  border: 1px solid #eaf6fb;
}

.patient-info-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 20px 40px;
}

.info-item {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.info-label {
  display: flex;
  align-items: center;
  gap: 10px;
  font-size: 0.9rem;
  font-weight: 600;
  color: #2a7ca3;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.info-icon {
  font-size: 1.1rem;
  filter: grayscale(0.2);
}

.info-value {
  font-size: 1.15rem;
  color: #234;
  font-weight: 500;
  padding-left: 32px;
}

.contact-link {
  color: #3bb3ce;
  text-decoration: none;
  border-bottom: 1px solid transparent;
  transition: border-color 0.2s;
}

.contact-link:hover {
  border-bottom-color: #3bb3ce;
}

.highlight {
  color: #111827;
  font-weight: 600;
}

.muted {
  color: #9ca3af;
  font-style: italic;
}

@media (max-width: 600px) {
  .patient-info-grid {
    grid-template-columns: 1fr;
    gap: 16px;
  }
  
  .info-value {
    padding-left: 0;
    font-size: 1.05rem;
  }
}
</style>