<script lang="ts" setup>
import type {
	Correlation,
	Course,
	CourseEditable,
	Task,
	TaskAnswer,
	TaskEditable,
	TaskType,
	Theme,
} from '@/models/main.ts'
import api from '@/services/api.ts'
import { useUserStore } from '@/stores/user.ts'
import { Validator } from '@/util/validator.ts'
import {
	BModal,
	BForm,
	BFormInput,
	BFormCheckbox,
	BFormFloatingLabel,
	BFormSelect,
	BButton,
	useToggle,
	BFormTextarea,
} from 'bootstrap-vue-next'
import { ref } from 'vue'
import type { Group } from '@/models/group.ts'

const props = defineProps<{
	themes: Theme[]
	groups: Group[]
}>()
const emits = defineEmits<{
	closed: []
	saved: [course: Course]
}>()
const { user } = useUserStore()
const { hide: hideCourseModal, show: showCourseModal } = useToggle('course-modal')
const getNewCourse = (): CourseEditable => {
	return {
		id: 0,
		name: '',
		description: '',
		authorId: user?.id ?? 0,
		themesIds: [],
		price: 0,
		isPublic: false,
		modulesHaveOrder: true,
	}
}
const course = ref<CourseEditable>(getNewCourse())
const startCreatingCourse = () => {
	course.value = getNewCourse()
	showCourseModal()
}
const startEditingCourse = async (courseToEdit: Course) => {
	course.value = {
		id: courseToEdit.id,
		name: courseToEdit.name,
		price: courseToEdit.price,
		isPublic: courseToEdit.isPublic,
		linkedGroupId: courseToEdit.linkedGroupId,
		description: courseToEdit.description,
		authorId: courseToEdit.authorId ?? user?.id ?? 0,
		themesIds: [],
		modulesHaveOrder: courseToEdit.modulesHaveOrder ?? true,
	}
	courseToEdit.themes?.forEach((x) => course.value.themesIds.push(x.id))
	await showCourseModal()
}
const checkFields = () => {
	const val: Validator = new Validator()
	val.validate(!course.value.name, 'Укажите название курса')
	val.validate(!course.value.description, 'Требуется указать описание курса')
	val.validate(
		Number.isNaN(Number.parseInt(course.value.price.toString())) || course.value.price < 0,
		'Укажите стоимость >= 0',
	)
	return val.returnResult()
}
const saveCourse = async () => {
	if (checkFields()) {
		course.value.price = Math.trunc(course.value.price)
		if (course.value.id == 0) {
			await api
				.post<Course>('courses', course.value)
				.then((res) => {
					hideCourseModal()
					alert('Курс успешно создан')
					emits('saved', res.data)
				})
				.catch((err) => {
					alert('Ошибка, повторите позже')
				})
		} else {
			await api
				.put<Course>('courses', course.value)
				.then((res) => {
					hideCourseModal()
					alert('Курс успешно изменен')
					emits('saved', res.data)
				})
				.catch((err) => {
					alert('Ошибка, повторите позже')
				})
		}
	}
}
const close = () => {
	hideCourseModal()
	emits('closed')
}
defineExpose({ startCreatingCourse, startEditingCourse })
</script>

<template>
	<BModal
		id="course-modal"
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
					v-model.trim="course.name"
					type="text"
					placeholder="Новый курс"
				/>
			</BFormFloatingLabel>

			<BFormFloatingLabel class="my-2" label="Описание" label-for="course-desc">
				<BFormTextarea
					id="course-desc"
					v-model.trim="course.description"
					type="text"
					placeholder="Описание..."
				>
				</BFormTextarea>
			</BFormFloatingLabel>

			<BFormFloatingLabel
				class="my-2"
				label="Стоимость (0, если бесплатно)"
				label-for="course-price"
			>
				<BFormInput
					type="text"
					pattern="\d*"
					v-model.number="course.price"
					id="course-price"
				/>
			</BFormFloatingLabel>

			<div>
				<p>Темы:</p>
				<BFormSelect
					:options="themes"
					select-size="7"
					v-model="course.themesIds"
					value-field="id"
					text-field="name"
					multiple
					id="course-themes"
				/>
			</div>

			<div>
				<p class="mt-2">
					Для организации потоков/групп:<br />
					<span class="small">Привязанная текущая группа:</span>
				</p>
				<select size="4" class="form-select form-control" v-model="course.linkedGroupId">
					<option v-for="gr in groups" :value="gr.id" :key="gr.id">
						{{ gr.name + '\nС ' + gr.dateStart + ' по ' + gr.dateEnd }}
					</option>
				</select>
				<p class="small">
					Важно! Если вы привяжете к курсу группу, то при приобретении курса пользователи
					будут добавляться в группу
				</p>
			</div>
		</BForm>
		<template #footer>
			<BButton variant="primary" @click="saveCourse()">Сохранить</BButton>
			<BButton variant="dark" @click="close">Отмена</BButton>
		</template>
	</BModal>
</template>
