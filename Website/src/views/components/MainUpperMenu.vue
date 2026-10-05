<script lang="ts" setup>
import router from '@/router'
import api from '@/services/api.ts'
import { useUserStore } from '@/stores/user.ts'
import {
	BNavItemDropdown,
	BDropdownItem,
	BNavbarBrand,
	BNavItem,
	BNavbarNav,
	BNavbarToggle,
	BCollapse,
	BNavbar,
	BBadge,
} from 'bootstrap-vue-next'
import { onBeforeMount, onMounted, ref } from 'vue'
import { RouterLink } from 'vue-router'
import { storeToRefs } from 'pinia'

const { user } = storeToRefs(useUserStore())
const logout = () => {
	if (confirm('Вы точно хотите выйти из профиля?')) {
		delete api.defaults.headers.common.Authorization
		useUserStore().logOut()
		router.push('/')
	}
}
let timer: number | undefined
const unreadMessagesCount = ref(0)

onMounted(async () => {
	timer = setInterval(async () => {
		await getUnreadReportsCount()
	}, 1000 * 60)
})
const getUnreadReportsCount = async () => {
	if (user.value?.role.id == 2) {
		await api
			.get<number>('feedback/count')
			.then((res) => (unreadMessagesCount.value = res.data))
	}
}
const getUserTypeAnalytics = (): string => {
	let type = 'creator-analytics'
	switch (user.value?.role.id) {
		case 1:
			type = 'creator-analytics'
			break
		case 2:
			type = 'admin/analytics'
			break
	}
	return type
}

onBeforeMount(() => {
	if (timer) {
		clearInterval(timer)
	}
})
</script>
<template>
	<BNavbar toggleable="md" variant="light">
		<BNavbarBrand href="/">
			<img src="/cube.png" width="35" height="35" />
			CourseBuilder
		</BNavbarBrand>
		<BNavbarToggle target="nav-collapse" />
		<BCollapse id="nav-collapse" is-nav>
			<BNavbarNav>
				<li class="nav-item">
					<RouterLink class="nav-link" to="/courses">Курсы</RouterLink>
				</li>
				<li class="nav-item">
					<RouterLink class="nav-link" to="/kits">Наборы</RouterLink>
				</li>
				<BNavItemDropdown
					v-if="user?.role.id == 2"
					id="admin-nav-dropdown"
					text="Администрирование"
				>
					<BDropdownItem to="/admin/users"> Пользователи </BDropdownItem>
					<BDropdownItem to="/admin/feedback">
						<span>
							Жалобы/предложения
							<BBadge variant="primary" pill v-if="unreadMessagesCount > 0">
								{{ unreadMessagesCount }}
							</BBadge>
						</span>
					</BDropdownItem>
				</BNavItemDropdown>
				<BNavItemDropdown
					v-if="user && user?.role.id != 2"
					id="education-nav-dropdown"
					text="Обучение"
				>
					<BDropdownItem  to="/my-courses"
						>Мои курсы</BDropdownItem
					>
					<BDropdownItem  to="/my-groups"
						>Учебные группы</BDropdownItem
					>
					<BDropdownItem v-if="user?.role.id == 3" to="/student-progress"
						>Прогресс</BDropdownItem
					>
				</BNavItemDropdown>
				<li class="nav-item" v-if="user?.role.id == 1">
					<RouterLink class="nav-link" :to="`/${getUserTypeAnalytics()}`"
						>Аналитика</RouterLink
					>
				</li>
			</BNavbarNav>

			<BNavbarNav class="ms-auto mb-2 mb-lg-0">
				<BNavItemDropdown v-if="user">
					<template #button-content>
						<span>
							{{ user.userInformation.lastName }}
							{{ user.userInformation.firstName }}
						</span>
						<br />
						<span>{{ user.role.name }}</span>
					</template>

					<RouterLink class="text-decoration-none text-dark dropdown-item" to="/profile"
						>Профиль</RouterLink
					>

					<BDropdownItem @click="logout">Выйти из профиля</BDropdownItem>
				</BNavItemDropdown>
				<BNavItem v-if="!user" href="/auth">Авторизация</BNavItem>
				<BNavItem v-if="!user" href="">/</BNavItem>
				<BNavItem v-if="!user" href="/reg">Регистрация</BNavItem>
			</BNavbarNav>
		</BCollapse>
	</BNavbar>
</template>
