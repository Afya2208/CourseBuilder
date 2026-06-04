<script lang="ts" setup>
import type { Role, User } from '@/models/main'
import type { UsersAndTotalCount } from '@/models/usersAndTotalCount'
import api from '@/services/api'
import UserForm from '@/views/pages/forms/UserForm.vue'
import {
	BFormSelect,
	BDropdown,
	BDropdownItem,
	BInputGroup,
	BInputGroupText,
	BDropdownText,
} from 'bootstrap-vue-next'
import { onMounted, ref, watch } from 'vue'
import type { ComponentExposed } from 'vue-component-type-helpers'
import PaginationView from '@/views/pages/components/PaginationView.vue'

const loadError = ref(false)
const users = ref<User[]>([])
const roles = ref<Role[]>([])
const userForm = ref<ComponentExposed<typeof UserForm>>()

const clearSearch = () => {
	searchOptions.value = getDefaultSearchOptions()
}
const getDefaultSearchOptions = () => {
	return {
		pageNumber: 1,
		pageSize: 30,
		text: '',
	}
}
const searchOptions = ref<{
	pageNumber: number
	roleId?: number
	pageSize: number
	text?: string
}>(getDefaultSearchOptions())

const totalCount = ref<number>(0)
const searchChanged = ref<boolean>(false)

const searchUsers = async () => {
	loadError.value = false
	let requestPath = 'users/search?'
	requestPath += `pageSize=${searchOptions.value.pageSize}`
	if (searchOptions.value.text) {
		requestPath += `&text=${searchOptions.value.text}`
	}
	if (searchOptions.value.roleId) {
		requestPath += `&roleId=${searchOptions.value.roleId}`
	}
	if (searchChanged.value) {
		requestPath += `&pageNumber=1`
		searchOptions.value.pageNumber = 1
		searchChanged.value = false
	} else {
		requestPath += `&pageNumber=${searchOptions.value.pageNumber}`
	}
	await api
		.get<UsersAndTotalCount>(requestPath)
		.then((res) => {
			users.value = res.data.users
			totalCount.value = res.data.totalCount
		})
		.catch((err) => (loadError.value = true))
}

watch(
	() => searchOptions.value.roleId,
	() => {
		searchChanged.value = true
	},
)
watch(
	() => searchOptions.value.text,
	() => {
		searchChanged.value = true
	},
)

const loadRolesAsync = async () => {
	await api.get<Role[]>('roles').then((res) => {
		roles.value = res.data
	})
}

const changeEmail = async (user: User) => {
	if (confirm(`Вы уверены, что хотите сменить почту у ${user.email}?`)) {
		const newEmail = prompt('Введите новый email')
		if (newEmail) {
			api.post('users/change-email', {
				userId: user.id,
				email: newEmail,
			})
				.then((res) => {
					alert('Почта успешно изменена')
					user.email = newEmail
				})
				.catch((err) => {
					if (err.status == 400) {
						alert(`Почта ${newEmail} занята, введите другую новую почту`)
					}
				})
		}
	}
}
const deleteUser = async (user: User, index: number) => {
	if (confirm(`Вы уверены, что хотите удалить пользователя ${user.email}?`)) {
		await api.delete('users/' + user.id).then((res) => {
			users.value?.splice(index, 1)
		})
	}
}

const importStudentsCsv = async () => {
	const inp = document.createElement('input')
	inp.type = 'file'
	inp.accept = '.csv'
	document.body.appendChild(inp)
	inp.click()
	inp.onchange = async (e) => {
		if (inp.files && inp.files[0]) {
			const file = inp.files[0]
			const ext = file.name.split('.').pop() ?? ''
			if (ext != 'csv') {
				alert('Неправильный формат файла импорта - требуется файл с расширением .csv')
				return
			}
			const formData = new FormData()
			formData.append('FormFile', file)
			await api
				.post(`users/import/csv`, formData, {
					headers: {
						'Content-Type': 'multipart/form-data',
					},
				})
				.then( async (res) => {
					alert('Студенты успешно добавлены')
					await searchUsers()
				})
				.catch((err) => {
					if (err.status == 400) {
						alert(err.response.data)
					}
				})
		}
	}
	document.body.removeChild(inp)
}

const addUser = () => userForm.value?.startCreatingUser()
const editUser = (u: User) => userForm.value?.startEditingUser(u)

const onSaved = async () => {
	await searchUsers()
}

onMounted(async () => {
	await loadRolesAsync()
	await searchUsers()
})

const onSelectedPage = async () => {
	await searchUsers()
}
</script>

<template>
	<div class="container">
		<h3 class="mt-3" id="header">Пользователи</h3>

		<div class="d-flex flex-wrap justify-content-center gap-2">
			<button class="btn btn-primary" @click="addUser">Добавить пользователя</button>
			<BDropdown text="Импортировать пользователей">
				<BDropdownText textClass="small">Добавление студентов</BDropdownText>
				<BDropdownItem @click="importStudentsCsv">Из файла .csv</BDropdownItem>
			</BDropdown>
		</div>

		<div class="d-flex flex-wrap justify-content-center m-1">
			<input
				type="search"
				style="width: 350px"
				class="m-1 form-control"
				v-model.trim="searchOptions.text"
				placeholder="Поиск..."
			/>
			<BInputGroup class="m-1" style="width: 250px">
				<BInputGroupText>Роль:</BInputGroupText>
				<BFormSelect
					class="d-inline"
					:options="roles"
					value-field="id"
					text-field="name"
					v-model="searchOptions.roleId"
				/>
			</BInputGroup>

			<button type="button" class="btn m-1 btn-outline-primary" @click="searchUsers">
				Поиск
			</button>
			<button type="button" class="btn m-1 btn-outline-secondary" @click="clearSearch">
				Очистить поля
			</button>
		</div>

		<div v-if="!users && !loadError">
			<p>Загрузка данных, подождите, пожалуйста</p>
		</div>

		<div v-else-if="!users && loadError">
			<p>Ошибка загрузки данных, попробуйте позже</p>
		</div>

		<div class="d-flex flex-wrap justify-content-center" v-else-if="users && users.length > 0">
			<div class="table-responsive">
				<table class="table table-striped table-hover table-bordered">
					<thead>
						<tr>
							<th>UID</th>
							<th>Email</th>
							<th>Роль</th>
							<th>Фамилия</th>
							<th>Имя</th>
							<th>Отчество</th>
							<th>Телефон</th>
							<th>Должность</th>
							<th>Действия</th>
						</tr>
					</thead>
					<tbody>
						<tr v-for="(user, index) in users" :key="user.id">
							<td>
								<p>
									{{ user.id }}
								</p>
							</td>
							<td>
								<p>
									{{ user.email }}
								</p>
							</td>
							<td>
								<p>
									{{ user.role.name }}
								</p>
							</td>
							<td>
								<p>
									{{ user.userInformation.lastName }}
								</p>
							</td>
							<td>
								<p>
									{{ user.userInformation.firstName }}
								</p>
							</td>
							<td>
								<p v-if="user.userInformation.middleName">
									{{ user.userInformation.middleName }}
								</p>
							</td>
							<td>
								<p>
									{{ user.userInformation.phone }}
								</p>
							</td>
							<td>
								<p>
									{{ user.userInformation.position }}
								</p>
							</td>
							<td>
								<div class="d-flex gap-1">
									<button
										@click="changeEmail(user)"
										class="btn btn-outline-secondary btn-sm"
									>
										Сменить email
									</button>
									<button
										@click="editUser(user)"
										class="btn btn-outline-warning btn-sm"
									>
										✏️
									</button>
									<button
										@click="deleteUser(user, index)"
										class="btn btn-outline-danger btn-sm"
									>
										❌
									</button>
								</div>
							</td>
						</tr>
					</tbody>
				</table>
			</div>
		</div>

		<div v-else>
			<p class="text-center p-1">
				К сожалению, нет пользователей
				<span v-if="searchOptions.roleId || searchOptions.text">по данному запросу</span>
			</p>
		</div>

		<UserForm ref="userForm" :roles="roles" @saved="onSaved" />

		<PaginationView
			v-if="users && users.length > 0"
			:search-options="searchOptions"
			:total-count="totalCount"
			@selected-page="onSelectedPage"
		/>
	</div>
</template>
