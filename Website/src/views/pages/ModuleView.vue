<script setup lang="ts">
import type { Course, Module, Lesson, LessonType } from '@/models/main'
import api from '@/services/api'
import { onMounted, ref, type Ref } from 'vue'
import { useRoute } from 'vue-router'
import {
	BButton,
	BCard,
	BCardText,
	useToggle,
	BModal,
	BForm,
	BFormFloatingLabel,
	BFormInput,
	BFormSelect,
} from 'bootstrap-vue-next'
import { useUserStore } from '@/stores/user'

const userStore = useUserStore()
const user = ref(userStore.user)
const module = ref<Module>()
const lessons = ref<Lesson[]>([])
const lessonTypes = ref<LessonType[]>()
const moduleId = parseInt(useRoute().params.moduleId as string)
const getData = async () => {
	await api.get<Module>(`modules/${moduleId}`).then((res) => {
		module.value = res.data
	})
	await api.get<Lesson[]>(`modules/${moduleId}/lessons`).then((res) => {
		lessons.value = res.data
	})
	await api.get<LessonType[]>(`lesson-types`).then((res) => {
		lessonTypes.value = res.data
	})
}
const saveSelectedLesson = async () => {

    if (!selectedLesson.value.name) {
        alert("Укажите название")
        return
    }
    if (!selectedLesson.value.description) {
        alert("Укажите описание")
        return
    }
    if (Number.isNaN(Number.parseInt(selectedLesson.value.order.toString())) || selectedLesson.value.order < 1) {
        alert("Укажите порядок > 0")
        return
    }
    selectedLesson.value.order = Math.trunc(selectedLesson.value.order);

	if (selectedLesson.value.id == 0) {
		await api
			.post<Lesson>('lessons', selectedLesson.value)
			.then((res) => {
				alert('Занятие успешно сохранено')
				lessons.value?.push(res.data)
				selectedLesson.value = getNullLesson()
			})
			.catch((err) => {
				alert('Ошибка, повторите позже')
			})
	} else {
		await api
			.put<Lesson>('lessons', selectedLesson.value)
			.then((res) => {
				let newLesson = res.data
				lessons.value[selectedLessonIndex.value!!] = newLesson
				alert('Занятие успешно сохранено')
			})
			.catch((err) => {
				alert('Ошибка, повторите позже')
			})
		closeModal()
	}
	hideModal()
}
onMounted(async () => {
	await getData()
})
const getNullLesson = (): Lesson => {
	return {
		id: 0,
		name: '',
		description: '',
		moduleId: moduleId,
		lessonTypeId: 1,
		order: lessons.value.length + 1,
	}
}
const addLesson = () => {
	showModal()
}
const closeModal = () => {
	if (selectedLesson.value.id != 0) {
		selectedLessonIndex.value = undefined
		selectedLesson.value = getNullLesson()
	}
	hideModal()
}
const selectedLessonIndex = ref<number>()
const startEditingLesson = (lesson: Lesson, index: number) => {
	selectedLessonIndex.value = index
	selectedLesson.value = {
		id: lesson.id,
		name: lesson.name,
		description: lesson.description,
		moduleId: lesson.moduleId,
		lessonTypeId: lesson.lessonTypeId,
		order: lesson.order,
	}
	showModal()
}
const deleteLesson = async (lesson: Lesson, index: number) => {
	if (confirm(`Вы уверены, что хотите удалить занятие ${lesson.name}?`)) {
		await api.delete('lessons/' + lesson.id).then((res) => {
			lessons.value?.splice(index, 1)
		})
	}
}
const { hide: hideModal, show: showModal } = useToggle('lesson-modal')
const selectedLesson = ref<Lesson>(getNullLesson())
</script>

<template>
	<div class="container">
		<div v-if="module">
			<h1>{{ module.name }}</h1>
			<p v-if="module.description">{{ module.description }}</p>
			<p v-if="module.lessonsCount">Количество занятий: {{ module.lessonsCount }}</p>
			<RouterLink :to="`/courses/${module.courseId}`">Вернуться к списку модулей</RouterLink>
		</div>
		<h3 class="my-2">
			Занятия
			<BButton @click="addLesson" variant="primary" v-if="user?.role.id == 1"
				>Добавить новое занятие</BButton
			>
		</h3>
		<div class="d-flex flex-wrap" v-if="lessons">
			<BCard
				:title="lesson.name"
				class="m-2"
				style="max-width: 20rem"
				:class="{
					is_control: lesson.lessonTypeId == 2,
					is_study: lesson.lessonTypeId == 1,
				}"
				v-for="(lesson, index) in lessons"
				:style="{ order: lesson.order }"
			>
				<BCardText>
					<p>{{ lesson.description }}</p>
					<p>
						<RouterLink
							class="btn"
                            :class="{
                                'btn-outline-success': lesson.lessonTypeId == 1,
                                'btn-outline-danger': lesson.lessonTypeId == 2
                            }"
							:to="`/courses/modules/lessons/${lesson.id}`"
							>Перейти к занятию</RouterLink
						>
					</p>
				</BCardText>
				<template #footer v-if="user?.role.id == 1">
					<BButton
						variant="outline-danger"
						class="me-2"
						@click="deleteLesson(lesson, index)"
						>❌</BButton
					>
					<BButton variant="outline-warning" @click="startEditingLesson(lesson, index)"
						>✏️</BButton
					>
				</template>
			</BCard>
		</div>
	</div>

	<BModal
		id="lesson-modal"
		centered
		header-class="bg-dark text-white"
		header-close-class="bg-white"
		@close="closeModal"
		title="Новое занятие"
		no-close-on-backdrop
	>
		<BForm>
			<BFormFloatingLabel class="my-2" label="Название" label-for="lesson-name">
				<BFormInput
					id="lesson-name"
					v-model.trim="selectedLesson.name"
					type="text"
					placeholder="Новое занятие"
				/>
			</BFormFloatingLabel>

			<BFormFloatingLabel class="my-2" label="Описание" label-for="lesson-desc">
				<BFormInput
					id="lesson-desc"
					v-model.trim="selectedLesson.description"
					type="text"
					placeholder="Описание..."
				/>
			</BFormFloatingLabel>

			<div>
				<p>Тип занятия:</p>
				<BFormSelect
					:options="lessonTypes"
					v-model="selectedLesson.lessonTypeId"
					value-field="id"
					text-field="name"
					id="lesson-type"
				/>
			</div>

			<BFormFloatingLabel class="my-2" label="Порядок" label-for="module-order">
				<BFormInput
					id="module-order"
					v-model.number="selectedLesson.order"
					:disabled="!module?.lessonsHaveOrder"
					type="text"
                    pattern="\d*"
					placeholder="1"
				/>
			</BFormFloatingLabel>
		</BForm>
		<template #footer>
			<BButton variant="primary" @click="saveSelectedLesson()">Сохранить</BButton>
			<button class="btn btn-dark" @click="closeModal">Отмена</button>
		</template>
	</BModal>
</template>

<style scoped></style>
