<script setup lang="ts">
import type { Module, Lesson, LessonType } from '@/models/main'
import api from '@/services/api'
import { onMounted, ref } from 'vue'
import { RouterLink, useRoute } from 'vue-router'
import { BButton } from 'bootstrap-vue-next'
import { useUserStore } from '@/stores/user'
import LessonForm from '@/views/pages/forms/LessonForm.vue'
import type { ComponentExposed } from 'vue-component-type-helpers'

const userStore = useUserStore()
const user = ref(userStore.user)
const userIsOwner = ref(false)
const module = ref<Module>()
const lessonForm = ref<ComponentExposed<typeof LessonForm>>()
const lessons = ref<Lesson[]>([])
const userProgressLessonsIds = ref<number[]>([])
const lessonTypes = ref<LessonType[]>([])
const moduleId = parseInt(useRoute().params.moduleId as string)
const courseId = parseInt(useRoute().params.courseId as string)
const getData = async () => {
	await api.get<Module>(`modules/${moduleId}`).then((res) => {
		module.value = res.data
	})
	await getLessons()
	await api.get<boolean>(`courses/${courseId}/is-created-by/${user.value?.id}`).then((res) => {
		userIsOwner.value = res.data
	})
	await api.get<LessonType[]>(`lesson-types`).then((res) => {
		lessonTypes.value = res.data
	})
	if (user.value) {
		await api.get<number[]>(`progress/${user.value?.id}/for-module/${moduleId}`).then((res) => {
			userProgressLessonsIds.value = res.data
		})
	}
}

const syncOrders = () => {
	lessons.value.forEach((card, i) => {
		card.order = i + 1
	})
}

function move(fromIndex: number, toIndex: number) {
	if (toIndex < 0 || toIndex >= lessons.value.length || fromIndex === toIndex) return
	const arr = lessons.value
	const [item] = arr.splice(fromIndex, 1)
	arr.splice(toIndex, 0, item)
	orderChanged.value = true
}

const getLessons = async () => {
	await api.get<Lesson[]>(`modules/${moduleId}/lessons`).then((res) => {
		lessons.value = res.data
	})
}
onMounted(async () => {
	await getData()
})
const deleteLesson = async (lesson: Lesson, index: number) => {
	if (confirm(`Вы уверены, что хотите удалить занятие ${lesson.name}?`)) {
		await api.delete('lessons/' + lesson.id).then((res) => {
			lessons.value?.splice(index, 1)
		})
	}
}

const saveOrder = async () => {
	syncOrders()
	await api
		.post(
			'/lessons/order',
			lessons.value.map((x) => {
				return { id: x.id, order: x.order }
			}),
		)
		.then((res) => {
			alert('Порядок занятий сохранен')
			orderChanged.value = false
		})
}

const orderChanged = ref(false)

const addLesson = () => {
	lessonForm.value?.startCreatingLesson()
}
const editLesson = (l: Lesson) => {
	lessonForm.value?.startEditingLesson(l)
}
const onSaved = async () => {
	orderChanged.value = false
	await getLessons()
}
const loadError = ref(false)
const getTypeName = (id: number) => {
	if (id == 1) return 'Обучающее занятие'
	if (id == 2) return 'Тест/контрольная'
}
</script>

<template>
	<div class="container" v-if="module">
		<h3 class="my-3">
			{{ module.name }}
		</h3>
		<p v-if="module.description">{{ module.description }}</p>
		<p v-if="module.lessonsCount">Количество занятий: {{ module.lessonsCount }}</p>

		<RouterLink class="btn btn-outline-dark" :to="`/courses/${module.courseId}`"
			>Вернуться к списку модулей</RouterLink
		>

		<h4 class="my-3">
			Занятия
			<BButton @click="addLesson" variant="primary" v-if="userIsOwner"
				>Добавить новое занятие</BButton
			>
			<button @click="saveOrder" v-if="orderChanged" class="btn btn-outline-info mx-2">
				Сохранить порядок занятий
			</button>
		</h4>

		<div v-if="!lessons && !loadError">
			<p>Загрузка данных, подождите, пожалуйста</p>
		</div>

		<div v-else-if="!lessons && loadError">
			<p>Ошибка загрузки данных, попробуйте позже</p>
		</div>

		<div v-else-if="lessons && lessons.length > 0" class="row row-cols-1 row-cols-md-4 g-4 p-2">
			<div class="col" v-for="(lesson, index) in lessons" :key="lesson.id">
				<div class="card h-100" style="min-height: 200px">
					<div
						class="card-body d-flex flex-column card-clickable position-relative flex-grow-1"
					>
						<div class="card-title">
							<h5>{{ lesson.name }}</h5>
							<p class="p-0 m-0">
								<span
									class="small text-secondary"
									:class="{
										type1: lesson.lessonTypeId == 1,
										type2: lesson.lessonTypeId == 2,
									}"
								>
									{{ getTypeName(lesson.lessonTypeId) }}
								</span>
								<br />
								<span class="small text-secondary" v-if="lesson.isRequired">
									Занятие обязательно для зачета
								</span>
							</p>
						</div>

						<p class="card-text my-1">
							{{ lesson.description }}
						</p>
						<RouterLink
							class="stretched-link"
							:to="`/courses/${courseId}/modules/${moduleId}/lessons/${lesson.id}`"
						/>
					</div>

					<div class="card-footer" v-if="userIsOwner">
						<div class="d-flex justify-content-between gap-2">
							<div class="d-flex gap-2">
								<BButton
									variant="outline-danger"
									@click="deleteLesson(lesson, index)"
									>❌</BButton
								>
								<BButton variant="outline-warning" @click="editLesson(lesson)"
									>✏️</BButton
								>
							</div>
							<div class="d-flex gap-2">
								<BButton
									v-if="module.lessonsHaveOrder"
									variant="outline-dark"
									:disabled="index === 0"
									@click="move(index, index - 1)"
									>⬅️</BButton
								>
								<BButton
									v-if="module.lessonsHaveOrder"
									variant="outline-dark"
									:disabled="index === lessons.length - 1"
									@click="move(index, index + 1)"
									>➡️</BButton
								>
							</div>
						</div>
					</div>
				</div>
			</div>
		</div>

		<div v-else>
			<p class="text-center p-1">К сожалению, пока модулей у этого курса нет</p>
		</div>

		<LessonForm
			ref="lessonForm"
			:lesson-types="lessonTypes"
			:lessons-count="lessons.length"
			:module-id="moduleId"
			:lessons-have-order="module?.lessonsHaveOrder || true"
			@saved="onSaved"
		/>
	</div>
</template>

<style scoped>
.card-title {
	border-bottom: 1px solid #f0f0f0;
}
span.type1::after {
	content: '📄';
}
span.type2::after {
	content: '✏️';
}
</style>
