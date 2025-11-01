import { createRouter, createWebHistory } from 'vue-router'
import PatientsList from '../pages/PatientsList.vue'
import PatientDetail from '../pages/PatientDetail.vue'

const routes = [
  { path: '/', redirect: '/patients' },
  { path: '/patients', component: PatientsList },
  { path: '/patients/:id', component: PatientDetail, props: true },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router