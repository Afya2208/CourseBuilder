import { useUserStore } from '@/stores/user'
import { createRouter, createWebHistory } from 'vue-router'


const routes = [
	{
		path: '/',
		name: 'main',
		redirect: '/courses',
	},
	{
		path: '/auth',
		name: 'authorization',
		component: () => import('../views/AuthorizationPage.vue'),
	},
	{
		path: '/reg',
		name: 'registration',
		component: () => import('../views/RegistrationPage.vue'),
	},
	{
		path: '/feedback',
		name: 'feedback',
		component: () => import('../views/SendReportPage.vue'),
	},
	{
		path: '/student-progress',
		name: 'student progress',
		component: () => import('../views/students/StudentProgress.vue'),
	},
	{
		path: '/courses',
		name: 'courses',
		component: () => import('../views/EducationalCoursesPage.vue'),
	},
	{
		path: '/profile',
		name: 'profile',
		component: () => import('../views/ProfilePage.vue'),
		meta: { requireAuth: true },
	},
	{
		path: '/kits',
		name: 'kits',
		component: () => import('../views/KitsPage.vue'),
	},
	{
		path: '/my-groups',
		name: 'my-groups',
		component: () => import('../views/MyGroupsPage.vue'),
		meta: { requireAuth: true },
	},
	{
		path: '/my-courses',
		name: 'myCourses',
		component: () => import('../views/MyCoursesPage.vue'),
		meta: { requireAuth: true },
	},
	{
		path: '/payment/:type/:dataId',
		name: 'payment',
		component: () => import('../views/test/PaymentPage.vue'),
		meta: { requireAuth: true },
	},
	{
		path: '/admin/users',
		name: 'admin users',
		component: () => import('../views/admins/UsersPage.vue'),
		meta: {
			requireAuth: true,
			requireRoleAdmin: true,
		},
	},
	{
		path: '/admin/feedback',
		name: 'admin feedback',
		component: () => import('../views/admins/FeedbackPage.vue'),
		meta: {
			requireAuth: true,
			requireRoleAdmin: true,
		},
	},
	{
		path: '/creator-analytics',
		name: 'creator analytics',
		component: () => import('../views/developers/CreatorAnalyticsPage.vue'),
		meta: {
			requireAuth: true,
			requireRoleCreator: true,
		},
	},
	{
		path: '/courses/:courseId',
		name: 'course-page',
		component: () => import('../views/courses-content/CoursePage.vue'),
	},
	{
		path: '/courses/:courseId/modules/:moduleId',
		name: 'module-page',
		component: () => import('../views/courses-content/ModulePage.vue'),
		meta: {
			requireAuth: true,
			requireAccess: true,
		},
	},
	{
		path: '/courses/:courseId/modules/:moduleId/lessons/:lessonId',
		name: 'lesson-page',
		component: () => import('../views/courses-content/LessonPage.vue'),
		meta: {
			requireAuth: true,
			requireAccess: true,
		},
	},
]

const router = createRouter({
	history: createWebHistory(import.meta.env.BASE_URL),
	routes: routes
})

router.beforeEach(async (to, from) => {
    const userStore = useUserStore()
    await userStore.init()
    const noAccountStr = localStorage.getItem('no-account')
    let noAccount = false;
    if (noAccountStr == 'true') {
        noAccount = true;
    }
    if (!to.fullPath.includes('auth') && !noAccount && !userStore.user) {
        return {
            path: '/auth',
            query: {redirect: to.fullPath}
        }
    }
    if (to.meta.requireAuth && !userStore.user) {
        return {
            path: '/auth',
            query: {redirect: to.fullPath}
        }
    }

    if (to.meta.requireAccess) {
        const courseId = to.params.courseId;
        if (courseId) {
            const intCourseId = Number.parseInt(courseId as string);
            if (userStore.availableCoursesIds?.includes(intCourseId)) {
                return true
            }
            else {
                alert("Для просмотра данного курса нужно его приобрести")
                return `/courses/${to.params.courseId}`
            }
        }
        else {
            return '/'
        }
    }

    if (to.meta.requireRoleAdmin && userStore.user?.role?.id !== 2
        || to.meta.requireRoleCreator && userStore.user?.role?.id !== 1
    ) {
        return '/'
    }
    else {

	}
})

export default router
