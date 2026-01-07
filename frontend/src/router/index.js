import { createRouter, createWebHistory } from 'vue-router'
import Login from '../views/Login.vue'
import PatientsList from '../pages/PatientsList.vue'
import PatientDetail from '../pages/PatientDetail.vue'
import { authState } from '../services/AuthService'

const routes = [
  { path: '/', redirect: '/patients' }, 
  { path: '/login', name: 'Login', component: Login },
  { 
    path: '/patients', 
    name: 'Patients', 
    component: PatientsList, 
    meta: { requiresAuth: true, role: 'Revalidatiearts' } 
  },
  { 
    path: '/patients/:id', 
    name: 'PatientDetail', 
    component: PatientDetail, 
    props: true, 
    meta: { requiresAuth: true, role: 'Revalidatiearts' } 
  },
  { 
    path: '/audit', 
    name: 'AuditLogView', 
    component: () => import('../components/admin/AuditLogView.vue'), 
    meta: { requiresAuth: true, role: 'Admin' } 
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('token')
  const userRole = authState.role 

  // Redirect als ingelogde gebruiker naar juiste startpagina
  if (to.path === '/login' && token) {
    return userRole === 'Admin' ? next('/audit') : next('/patients')
  }

  // Authenticatie check
  if (to.meta?.requiresAuth && !token) {
    return next('/login')
  }

  // Rol-gebaseerde check
  if (to.meta?.role && to.meta.role !== userRole) {
    console.warn(`Toegang geweigerd: Rol ${userRole} heeft geen rechten voor ${to.path}`)
    
    return userRole === 'Admin' ? next('/audit') : next('/patients')
  }

  next()
})

export default router