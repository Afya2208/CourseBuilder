<script lang="ts" setup>
import type { Course, Theme } from '@/models/main'
import api from '@/services/api'
import { useUserStore } from '@/stores/user'
import CourseForm from '@/views/pages/forms/CourseForm.vue'
import { BButton } from 'bootstrap-vue-next'
import { onMounted, ref } from 'vue'
import type { ComponentExposed } from 'vue-component-type-helpers'
import type { Group } from '@/models/group.ts'

const courses = ref<Course[]>()
const groups = ref<Group[]>()
const courseForm = ref<ComponentExposed<typeof CourseForm>>()
const themes = ref<Theme[]>()
const { user, addAvailableCourse } = useUserStore()
onMounted(async () => {
	await getDataAsync()
})
const getDataAsync = async () => {
	await api.get<Course[]>(`courses/by-user/${user?.id}`).then((res) => {
		courses.value = res.data
	})
	await api.get<Theme[]>(`themes`).then((res) => {
		themes.value = res.data
	})
	await api.get<Group[]>(`groups/by-user/${user?.id}/short`).then((res) => {
		groups.value = res.data
	})
}
const deleteCourse = async (course: Course, index: number) => {
	if (confirm(`Вы уверены, что хотите удалить курс ${course.name}?`)) {
		await api.delete(`courses/${course.id}`).then((res) => {
			courses.value!.splice(index, 1)
		})
	}
}
const startEditingCourse = (c: Course) => {
	courseForm.value?.startEditingCourse(c)
}
const addCourse = () => {
	courseForm.value?.startCreatingCourse()
}
const onSaved = async (c: Course) => {
	addAvailableCourse(c.id)
	await getDataAsync()
}
const changePublicity = async (c: Course) => {
	if (
		confirm(
			'Вы уверены, что хотите поменять доступность курса? Если он публичный, ' +
				'то его смогут увидеть и приобрести все пользователи платформы',
		)
	) {
		const newVal = !c.isPublic
		await api.post(`courses/${c.id}/change-publicity`).then((res) => {
			c.isPublic = newVal
		})
	}
}
</script>

<template>
	<div>
		<BButton class="my-2" variant="primary" @click="addCourse">Добавить новый курс</BButton>
		<div v-if="courses && courses.length > 0" class="d-flex flex-wrap">
			<div v-for="(course, index) in courses" class="card m-2" style="width: 300px">
				<div class="card-body position-relative">
					<h5 class="card-title">
						{{ course.name }}
					</h5>

					<p class="card-text">{{ course.description }}</p>
					<p class="card-text">
						Количество модулей: {{ course.modulesCount }} <br />
						Количество занятий: {{ course.lessonsCount }}
					</p>
					<p class="card-text">Стоимость: {{ course.price }} руб.</p>

					<RouterLink class="stretched-link" :to="`/courses/${course.id}`" />

					<h6 class="my-2 text-muted">
						Темы курса:
						{{ course.themes?.map((x) => x.name).join(', ') }}
					</h6>
				</div>
				<div class="card-footer">
					<BButton
						variant="outline-danger"
						class="me-2"
						@click="deleteCourse(course, index)"
						>❌</BButton
					>
					<BButton variant="outline-warning" @click="startEditingCourse(course)"
						>✏️</BButton
					>
					<button
						class="btn mx-2"
						:class="{
							'btn-outline-info': course.isPublic,
							'btn-outline-primary': !course.isPublic,
						}"
						@click="changePublicity(course)"
					>
						<span>{{ course.isPublic ? 'Публичный' : 'Непубличный' }}</span>
					</button>
				</div>
			</div>
		</div>
		<p v-if="courses?.length == 0" class="my-2">Пока на платформе нет Ваших курсов</p>
		<CourseForm
			:groups="groups ?? []"
			@saved="onSaved"
			ref="courseForm"
			:themes="themes ?? []"
		/>
	</div>
</template>
