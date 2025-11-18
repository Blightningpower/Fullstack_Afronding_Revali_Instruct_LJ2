import { createRouter, createWebHistory } from 'vue-router'
import Login from '../views/Login.vue'
import PatientsList from '../pages/PatientsList.vue'
import PatientDetail from '../pages/PatientDetail.vue'

const routes = [
  { path: '/login', name: 'Login', component: Login },
  { path: '/patients', name: 'Patients', component: PatientsList, meta: { requiresAuth: true } },
  { path: '/patients/:id', component: PatientDetail, props: true, meta: { requiresAuth: true } }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// eenvoudige guard: redirect naar /login wanneer token ontbreekt
router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('token')
  if (to.meta?.requiresAuth && !token) return next('/login')
  next()
})

export default router