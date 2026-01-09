import { createRouter, createWebHistory } from 'vue-router'
import Login from '../views/Login.vue'
import PatientsList from '../pages/PatientsList.vue'
import PatientDetail from '../pages/PatientDetail.vue'
import { authState, logout as authLogout } from '../services/AuthService'

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
  const token = localStorage.getItem('token');
  const userRole = authState.role;

  if (to.path === '/login' && token) {
    if (userRole === 'Admin') return next('/audit');
    if (userRole === 'Revalidatiearts') return next('/patients');
    return next();
  }

  // Check voor beveiligde pagina's
  if (to.meta?.requiresAuth) {
    if (!token) return next('/login');

    // Check rol-gebaseerde toegang
    if (to.meta.role && to.meta.role !== userRole) {
      return next(userRole === 'Admin' ? '/audit' : '/patients');
    }
  }

  next();
});

export default router;