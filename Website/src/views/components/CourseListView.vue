<script setup lang="ts">
import type { Course, CourseEditable, Theme } from '@/models/main'
import api from '@/services/api'
import { useUserStore } from '@/stores/user'
import {
	BButton,
	BModal,
	useToggle,
	BForm,
	BFormFloatingLabel,
	BFormInput,
	BFormSelect,
	BFormCheckbox,
} from 'bootstrap-vue-next'
import { storeToRefs } from 'pinia'
import { ref } from 'vue'
import { RouterLink } from 'vue-router'

const selectedThemeIdForSearch = ref<Number>()
const searchText = ref('')
const { user } = storeToRefs(useUserStore())
const props = defineProps<{
	themes: Theme[]
	courses: Course[]
}>()
const themes = ref(props.themes ?? [])
const courses = ref(props.courses ?? [])

const deleteCourse = async (course: Course, index: number) => {
	if (confirm(`Вы уверены, что хотите удалить курс ${course.name}?`)) {
		await api.delete(`courses/${course.id}`).then((res) => {
			courses.value.splice(index, 1)
		})
	}
}
const { hide: hideCourseModal, show: showCourseModal } = useToggle('add-course-modal')

const addCourse = () => showCourseModal()
const getNullCourse = (): CourseEditable => {
	return {
		id: 0,
		name: '',
		description: '',
		authorId: user.value?.id ?? 0,
		themesIds: [],
		minimalCompletionPercentage: 100,
		modulesHaveOrder: true,
	}
}
const close = () => {
	if (editableCourse.value.id != 0) {
		selectedCourseIndex.value = undefined
		editableCourse.value = getNullCourse()
		hideCourseModal()
	}
}

const editableCourse = ref<CourseEditable>(getNullCourse())
const selectedCourseIndex = ref<number>()
const saveCourse = async () => {
	if (editableCourse.value.id == 0) {
		await api
			.post<Course>('courses', editableCourse.value)
			.then((res) => {
				courses.value.push({ ...res.data, lessonsCount: 0, modulesCount: 0 })
				editableCourse.value = getNullCourse()
				hideCourseModal()
				alert('Курс успешно создан')
			})
			.catch((err) => {
				hideCourseModal()
				alert('Ошибка, повторите позже')
			})
	} else {
		await api
			.put<Course>('courses', editableCourse.value)
			.then((res) => {
				let oldCourse = courses.value[selectedCourseIndex.value!!]
				let newCourse = res.data
				newCourse.modulesCount = oldCourse?.modulesCount
				newCourse.lessonsCount = oldCourse?.lessonsCount
				courses.value[selectedCourseIndex.value!!] = newCourse
				hideCourseModal()
				alert('Курс успешно изменен')
			})
			.catch((err) => {
				hideCourseModal()
				alert('Ошибка, повторите позже')
			})
		editableCourse.value = getNullCourse()
		selectedCourseIndex.value = undefined
	}
}
const startEditingCourse = async (course: Course, index: number) => {
	selectedCourseIndex.value = index
	editableCourse.value = {
		id: course.id,
		name: course.name,
		price: course.price,
		description: course.description,
		authorId: course.authorId ?? user.value?.id ?? 0,
		themesIds: [],
		minimalCompletionPercentage: course.minimalCompletionPercentage ?? 100,
		modulesHaveOrder: course.modulesHaveOrder ?? true,
	}
	course.themes?.forEach((x) => editableCourse.value.themesIds.push(x.id))
	showCourseModal()
}
const clearSearch = () => {
	selectedThemeIdForSearch.value = 0
	searchText.value = ''
}
const searchCourses = async () => {
	let text = searchText.value.trim()
	let id = selectedThemeIdForSearch.value ?? null
	let requestPath = 'courses/search?text=' + text
	if (id && id != 0) {
		requestPath += '&themeId=' + id
	}
	await api.get<Course[]>(requestPath).then((res) => {
		courses.value = res.data
	})
}
</script>

<template>
	<div class="d-flex">
		<input
			type="search"
			style="width: 350px"
			class="form-control-sm me-2"
			v-model="searchText"
			placeholder="Программирование..."
		/>
		<BFormSelect
			class="form-control-sm"
			:options="themes"
			value-field="id"
			text-field="name"
			v-model="selectedThemeIdForSearch"
		/>
		<button type="button" class="btn mx-2 btn-outline-primary" @click="searchCourses">
			Поиск
		</button>
		<BButton variant="outline-secondary" @click="clearSearch">Очистить</BButton>
	</div>
	<div>
		<BButton class="my-2" v-if="user?.role.id == 1" variant="primary" @click="addCourse"
			>Добавить новый курс</BButton
		>
	</div>

	<BModal
		id="add-course-modal"
		centered
		header-class="bg-dark text-white"
		header-close-class="bg-white"
		@close="close"
		title="Новый курс"
		no-close-on-backdrop
	>
		<BForm>
			<BFormFloatingLabel class="my-2" label="Название" label-for="course-name">
				<BFormInput
					id="course-name"
					v-model="editableCourse.name"
					type="text"
					placeholder="Новый курс"
				/>
			</BFormFloatingLabel>

			<BFormFloatingLabel class="my-2" label="Описание" label-for="course-desc">
				<BFormInput
					id="course-desc"
					v-model="editableCourse.description"
					type="text"
					placeholder="Описание..."
				/>
			</BFormFloatingLabel>

			<BFormFloatingLabel
				class="my-2"
				label="Стоимость (если необходимо)"
				label-for="course-price"
			>
				<BFormInput v-model="editableCourse.price" id="course-price" type="number" />
			</BFormFloatingLabel>

			<BFormCheckbox id="course-modules-have-order" v-model="editableCourse.modulesHaveOrder">
				У модулей есть порядок
			</BFormCheckbox>

			<BFormFloatingLabel
				class="my-2"
				label="Минимальный процент прохождения для сдачи"
				label-for="course-min-percentage"
			>
				<BFormInput
					id="course-min-percentage"
					type="number"
					v-model="editableCourse.minimalCompletionPercentage"
				/>
			</BFormFloatingLabel>

			<div>
				<p>Темы:</p>
				<BFormSelect
					:options="themes"
					select-size="7"
					v-model="editableCourse.themesIds"
					value-field="id"
					text-field="name"
					multiple
					id="course-themes"
				/>
			</div>
		</BForm>
		<template #footer>
			<BButton variant="primary" @click="saveCourse()">Сохранить</BButton>
			<BButton variant="dark" @click="close">Отмена</BButton>
		</template>
	</BModal>

	<div class="d-flex">
		<div v-for="(course, index) in courses" class="card m-2">
			<div class="card-body" style="max-width: 18rem">
				<h5 class="card-title">
					{{ course.name }}
				</h5>
				<p class="card-text">{{ course.description }}</p>
				<p class="card-text">
					Количество модулей: {{ course.modulesCount }}
					<br />
					Количество занятий: {{ course.lessonsCount }}
				</p>
				<p class="card-text" v-if="course.price && course.price > 0">
					Стоимость: {{ course.price }} руб.
				</p>
				<RouterLink class="btn btn-outline-primary" :to="`courses/${course.id}`"
					>Перейти к курсу</RouterLink
				>
				<h6 class="card-subtitle my-2 text-muted">Темы курса:</h6>
				<ul>
					<li v-for="theme in course.themes">
						<p>{{ themes.find((x) => x.id == theme.id)?.name }}</p>
					</li>
				</ul>
			</div>
			<div v-if="user?.role.id == 1" class="card-footer">
				<BButton variant="outline-danger" class="me-2" @click="deleteCourse(course, index)"
					>❌</BButton
				>
				<BButton variant="outline-warning" @click="startEditingCourse(course, index)"
					>✏️</BButton
				>
			</div>
		</div>
	</div>
</template>
