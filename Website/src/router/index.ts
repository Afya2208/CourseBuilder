import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/auth',
      name: 'authorization',
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import('../views/pages/AuthorizationView.vue'),
    },
    {
      path: '/',
      name: 'main',
      component: () => import('../views/pages/MainView.vue'),
    },
    {
      path: '/courses/:courseId',
      name: 'course-page',
      component: () => import('../views/pages/CourseView.vue'),
    },
    {
      path: '/courses/modules/:moduleId',
      name: 'module-page',
      component: () => import('../views/pages/ModuleView.vue'),
    },
    {
      path: '/courses/modules/lessons/:lessonId',
      name: 'lesson-page',
      component: () => import('../views/pages/LessonView.vue'),
    },
  ],
})

export default router
