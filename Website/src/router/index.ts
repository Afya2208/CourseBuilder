import { useUserStore } from '@/stores/user'
import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
	history: createWebHistory(import.meta.env.BASE_URL),
	routes: [
		{
			path: '/auth',
			name: 'authorization',
			component: () => import('../views/pages/AuthorizationPageView.vue'),
		},
		{
			path: '/',
			name: 'main',
			component: () => import('../views/pages/MainPage.vue'),
		},
		{
			path: '/admin',
			name: 'administration',
			component: () => import('../views/pages/AdministrationPageView.vue'),
			meta: {
				requireAuth: true,
				requireAdmin: true,
			},
		},
		{
			path: '/courses',
			name: 'courses',
			component: () => import('../views/pages/CoursesPageView.vue'),
		},
		{
			path: '/courses/:courseId',
			name: 'course-page',
			component: () => import('../views/pages/CoursePageView.vue'),
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
router.beforeEach(async (to, from) => {
	let userStore = useUserStore()
	if (to.meta.requireAuth && !userStore.isInitialized) {
		await userStore.init()
	}
	if (to.meta.requireAuth && !userStore.user) {
		return '/auth'
	}
	if (to.meta.requireAdmin && userStore.user?.role?.id !== 2) {
		return '/'
	} else {
	}
})

export default router
