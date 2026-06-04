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
		component: () => import('../views/pages/auth/AuthorizationPage.vue'),
	},
	{
		path: '/reg',
		name: 'registration',
		component: () => import('../views/pages/auth/RegistrationPage.vue'),
	},
	{
		path: '/test',
		name: 'testing',
		component: () => import('../views/pages/test/TestPage.vue'),
	},
	{
		path: '/feedback',
		name: 'feedback',
		component: () => import('../views/pages/SendReportPage.vue'),
	},
	{
		path: '/student-progress',
		name: 'student progress',
		component: () => import('../views/pages/for students/StudentProgress.vue'),
	},
	{
		path: '/courses',
		name: 'courses',
		component: () => import('../views/pages/EducationalCoursesPage.vue'),
	},
	{
		path: '/profile',
		name: 'profile',
		component: () => import('../views/pages/ProfilePage.vue'),
		meta: { requireAuth: true },
	},
	{
		path: '/kits',
		name: 'kits',
		component: () => import('../views/pages/KitsPage.vue'),
	},
	{
		path: '/my-groups',
		name: 'my-groups',
		component: () => import('../views/pages/MyGroupsPage.vue'),
		meta: { requireAuth: true },
	},
	{
		path: '/my-courses',
		name: 'myCourses',
		component: () => import('../views/pages/MyCoursesPage.vue'),
		meta: { requireAuth: true },
	},
	{
		path: '/payment/:type/:dataId',
		name: 'payment',
		component: () => import('../views/pages/test/PaymentPage.vue'),
		meta: { requireAuth: true },
	},
	{
		path: '/admin/users',
		name: 'admin users',
		component: () => import('../views/pages/for admins/UsersPage.vue'),
		meta: {
			requireAuth: true,
			requireRoleAdmin: true,
		},
	},
	{
		path: '/admin/feedback',
		name: 'admin feedback',
		component: () => import('../views/pages/for admins/FeedbackPage.vue'),
		meta: {
			requireAuth: true,
			requireRoleAdmin: true,
		},
	},
	{
		path: '/creator-analytics',
		name: 'creator analytics',
		component: () => import('../views/pages/for creators/CreatorAnalyticsPage.vue'),
		meta: {
			requireAuth: true,
			requireRoleCreator: true,
		},
	},
	{
		path: '/courses/:courseId',
		name: 'course-page',
		component: () => import('../views/pages/courses-content/CoursePage.vue'),
	},
	{
		path: '/courses/:courseId/modules/:moduleId',
		name: 'module-page',
		component: () => import('../views/pages/courses-content/ModulePage.vue'),
		meta: {
			requireAuth: true,
			requireAccess: true,
		},
	},
	{
		path: '/courses/:courseId/modules/:moduleId/lessons/:lessonId',
		name: 'lesson-page',
		component: () => import('../views/pages/courses-content/LessonPage.vue'),
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
