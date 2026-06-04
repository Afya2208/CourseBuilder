<script lang="ts" setup>
import type { Group } from '@/models/group'
import type { Course, UserInformation } from '@/models/main'
import api from '@/services/api'
import { useUserStore } from '@/stores/user'
import GroupForm from '@/views/pages/forms/GroupForm.vue'

import { BButton } from 'bootstrap-vue-next'
import { onMounted, ref } from 'vue'
import type { ComponentExposed } from 'vue-component-type-helpers'

const { user } = useUserStore()
const groups = ref<Group[]>()
const courses = ref<Course[]>()

const loadDataAsync = async () => {
	await loadGroupsAsync()
	await api.get<Course[]>(`courses/by-user/${user?.id}`).then((res) => {
		courses.value = res.data
	})
}
const loadGroupsAsync = async () => {
	await api.get<Group[]>(`groups/by-user/${user?.id}`).then((res) => {
		groups.value = res.data
	})
}
const loadGroupUsersAsync = async () => {
	if (selectedGroup.value) {
		await api.get(`groups/${selectedGroup.value?.id}/users`).then((res) => {
			selectedGroup.value.usersInfo = res.data
		})
	}
}
const groupForm = ref<ComponentExposed<typeof GroupForm>>()

const addGroup = () => groupForm.value?.startCreatingGroup()
const editGroup = (g: Group) => groupForm.value?.startEditingGroup(g)
const deleteGroup = async (g: Group, i: number) => {
	if (confirm(`Вы уверены, что хотите удалить группу ${g.name}?`)) {
		await api
			.delete(`groups/${g.id}`)
			.then((res) => {
				groups.value!.splice(i, 1)
			})
			.catch((err) => {
				if (err.status == 400) {
					alert(
						'Ошибка! Нельзя удалить группу, если к ней привязаны курсы или в ней есть студенты',
					)
				}
			})
	}
}

const onSaved = async (g: Group) => {
	await loadGroupsAsync()
}

const selectedGroup = ref<Group>()
const userIdToAdd = ref<string>()

const addUserToGroup = async () => {
	if (userIdToAdd.value) {
		await api
			.post(`groups/${selectedGroup.value?.id}/add-user/${userIdToAdd.value}`)
			.then(async (res) => {
				alert('Студент добавлен в группу!')
				await loadGroupUsersAsync()
				userIdToAdd.value = ''
			})
			.catch((err) => {
				if (err.status == 404) {
					alert('Вы указали не существующего пользователя')
				}
				if (err.status == 400) {
					alert('Пользователь уже есть в группе')
				}
			})
	} else {
		alert('Укажите id пользователя, которого вы хотите добавить в группу')
	}
}
const deleteUser = async (uId: number) => {
	if (confirm('Вы уверены, что хотите удалить пользователя ' + uId + '?')) {
		await api
			.delete(`groups/${selectedGroup.value?.id}/delete-user/${uId}`)
			.then(async (res) => {
				alert('Студент удален из группы')
				await loadGroupUsersAsync()
			})
	}
}
const selectedCourseId = ref()
const uploadProgress = async () => {
	if (selectedCourseId.value && selectedGroup.value) {
		await api
			.get(`progress/for-group/${selectedGroup.value?.id}/course/${selectedCourseId.value}`, {
				responseType: 'blob',
			})
			.then((res) => {
				let fileURL = URL.createObjectURL(res.data)
				const link = document.createElement('a')
				link.href = fileURL
				link.download = `Успеваемость ${selectedGroup.value?.name} ${courses.value?.find((x) => x.id == selectedCourseId.value)?.name}.xlsx`
				document.body.appendChild(link)
				link.click()
				document.body.removeChild(link)
				setTimeout(() => URL.revokeObjectURL(fileURL), 100)
			})
			.catch((err) => {
				alert('Ошибка загрузки, повторите позже')
			})
	} else {
		alert('Выберите группу и курс для экспорта успеваемости')
	}
}

onMounted(async () => {
	await loadDataAsync()
})
</script>
<template>
	<div class="my-3">
		<button :disabled="!courses" class="my-2 btn btn-primary" @click="addGroup">
			Добавить новую группу
		</button>

		<div class="d-flex flex-wrap gap-2">
			<div :key="group.id" v-for="(group, index) in groups" class="card" style="width: 300px">
				<div
					@click="selectedGroup = group"
					class="card-body position-relative card-clickable"
				>
					<h5 class="card-title">
						<span>
							{{ group.name }}
						</span>
						<br />
						<span class="text-secondary small">
							С {{ group.dateStart }} по {{ group.dateEnd }}
						</span>
					</h5>
				</div>
				<div class="card-footer">
					<BButton
						variant="outline-danger"
						class="me-2"
						@click="deleteGroup(group, index)"
						>❌</BButton
					>
					<BButton
						:disabled="!courses"
						variant="outline-warning"
						@click="editGroup(group)"
						>✏️</BButton
					>
				</div>
			</div>
		</div>
		<div v-if="groups?.length == 0">
			<p>Пока нет групп/потоков</p>
		</div>

		<GroupForm ref="groupForm" @saved="onSaved" :courses="courses!!" />

		<div v-if="selectedGroup">
			<hr />
			<h5>Группа {{ selectedGroup.name }}</h5>
			<p>
				Связь с куратором: {{ selectedGroup.curatorFeedback }}<br />
				Дата начала: {{ selectedGroup.dateStart }}<br />
				Дата окончания: {{ selectedGroup.dateEnd }}<br />
			</p>

			<div class="d-flex flex-wrap gap-2">
				Курсы, прикреплённые к группе:
				<div class="card p-2" v-for="course in selectedGroup.coursesInfo">
					<h6 class="card-title">{{ course.name }}</h6>
					<RouterLink class="stretched-link" :to="`/courses/${course.id}`" />
				</div>
			</div>

			<h4 class="my-3">
				Студенты группы
				<span>{{ selectedGroup.name }}</span>
			</h4>
			<div class="d-flex flex-wrap gap-2 my-2">
				<button class="btn btn-outline-primary" @click="addUserToGroup">
					Добавить студента в группу
				</button>
				UID:
				<input type="text" class="form-control w-25" v-model="userIdToAdd" />
			</div>
			<div class="d-flex flex-wrap gap-2">
				<button class="btn btn-outline-info" @click="uploadProgress">
					Экспорт успеваемости группы по курсу
				</button>
				<select v-model="selectedCourseId" class="form-control w-50">
					<option v-for="course in selectedGroup.coursesInfo" :value="course.id">
						{{ course.name }}
					</option>
				</select>
			</div>
			<p>Всего студентов: {{ selectedGroup.usersInfo?.length }}</p>
			<table class="table table-bordered table-hover align-middle my-2">
				<thead class="table-light">
					<tr>
						<th>UID</th>
						<th>ФИО</th>
						<th style="width: 100px">Действия</th>
					</tr>
				</thead>
				<tbody>
					<tr v-for="student in selectedGroup.usersInfo" :key="student.userId">
						<td>{{ student.userId }}</td>
						<td>
							{{ student.lastName }} {{ student.firstName }}
							<span v-if="student.middleName">{{ student.middleName }}</span>
						</td>
						<td>
							<button
								class="btn btn-outline-danger btn-sm"
								@click="deleteUser(student.userId)"
							>
								❌
							</button>
						</td>
					</tr>
				</tbody>
			</table>
		</div>
	</div>
</template>
