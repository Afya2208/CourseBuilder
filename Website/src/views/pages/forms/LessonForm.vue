<script lang="ts" setup>
import type { Lesson, LessonType } from '@/models/main.ts'
import api from '@/services/api.ts'
import { Validator } from '@/util/validator.ts'
import {
	BForm,
	useToggle,
	BFormFloatingLabel,
	BFormInput,
	BFormSelect,
	BModal,
	BButton,
	BInputGroup,
	BInputGroupText,
	BFormCheckbox, BFormTextarea,
} from 'bootstrap-vue-next'
import { ref } from 'vue'

const props = defineProps<{
	lessonTypes: LessonType[]
	moduleId: number
	lessonsCount: number
	lessonsHaveOrder: boolean
}>()
const emits = defineEmits<{
	saved: [l: Lesson]
	closed: []
}>()

const startEditingLesson = (l: Lesson) => {
	lesson.value = {
		id: l.id,
		name: l.name,
		description: l.description,
		isRequired: l.isRequired,
		moduleId: l.moduleId,
		lessonTypeId: l.lessonTypeId,
		order: l.order,
	}
	showModal()
}
const getNewLesson = (): Lesson => {
	return {
		id: 0,
		name: '',
		isRequired: false,
		description: '',
		moduleId: props.moduleId,
		lessonTypeId: 1,
		order: props.lessonsCount + 1,
	}
}
const { hide: closeModal, show: showModal } = useToggle('lesson-modal')
const lesson = ref<Lesson>(getNewLesson())
const closedUntilStr = ref(new Date().toISOString().split('T')[0])

const startCreatingLesson = () => {
	closedUntilStr.value = new Date().toISOString().split('T')[0]
	lesson.value = getNewLesson()
	showModal()
}
const close = () => {
	closeModal()
	emits('closed')
}
const checkFields = (): boolean => {
	const v = new Validator()
	v.validate(!lesson.value.name, 'Укажите название')
	v.validate(!lesson.value.description, 'Укажите описание')
	v.validate(
		Number.isNaN(Number.parseInt(lesson.value.order.toString())) || lesson.value.order < 1,
		'Укажите порядок > 0',
	)
	return v.returnResult()
}
const saveLesson = async () => {
	if (checkFields()) {
		lesson.value.order = Math.trunc(lesson.value.order)
		if (lesson.value.id == 0) {
			await api
				.post<Lesson>('lessons', lesson.value)
				.then((res) => {
					alert('Занятие успешно сохранено')
					emits('saved', res.data)
					closeModal()
				})
				.catch(() => {
					alert('Ошибка, повторите позже')
				})
		} else {
			await api
				.put<Lesson>('lessons', lesson.value)
				.then((res) => {
					alert('Занятие успешно сохранено')
					emits('saved', res.data)
					closeModal()
				})
				.catch(() => {
					alert('Ошибка, повторите позже')
				})
		}
	}
}

const onNumberInput = (e) => {
	let val = e.target.value.replace(/\D/g, '')
	val = val.replace(/^0+/, '')
	if (val === '') {
		lesson.value.maxTriesCount = undefined
		e.target.value = ''
		return
	}
	e.target.value = val
	lesson.value.maxTriesCount = val
}

defineExpose({ startCreatingLesson, startEditingLesson })
</script>
<template>
	<BModal
		id="lesson-modal"
		centered
		header-class="bg-dark text-white"
		header-close-class="bg-white"
		@close="close"
		title="Новое занятие"
		no-close-on-backdrop
	>
		<BForm>
			<BFormFloatingLabel class="my-2" label="Название" label-for="lesson-name">
				<BFormInput
					id="lesson-name"
					v-model="lesson.name"
					type="text"
					placeholder="Новое занятие"
				/>
			</BFormFloatingLabel>

			<BFormFloatingLabel class="my-2" label="Описание" label-for="lesson-desc">
                <BFormTextarea id="lesson-desc"
                               v-model="lesson.description"
                               type="text"
                               placeholder="Описание..."/>
			</BFormFloatingLabel>

			<BInputGroup class="m-1">
				<BInputGroupText>Тип занятия:</BInputGroupText>
				<BFormSelect
					:options="lessonTypes"
					text-field="name"
					value-field="id"
					v-model="lesson.lessonTypeId"
				/>
			</BInputGroup>

			<div class="my-2">
				<p>
					Обязательно для зачета:
					<BFormCheckbox
						id="lesson-is-required"
						class="d-inline"
						v-model="lesson.isRequired"
					/>
				</p>
			</div>

            <!---
			<p>Необязательно:</p>

			<BFormFloatingLabel
				class="my-2"
				label="Количество попыток выполнения заданий"
				label-for="lesson-tries-count"
			>
				<BFormInput
					id="lesson-tries-count"
					@input="onNumberInput"
					placeholder="Введите число ≥ 1"
					:model-value="lesson.maxTriesCount"
					type="text"
					inputmode="numeric"
				/>
			</BFormFloatingLabel>
			--->
		</BForm>
		<template #footer>
			<BButton variant="primary" @click="saveLesson()">Сохранить</BButton>
			<button class="btn btn-dark" @click="close">Отмена</button>
		</template>
	</BModal>
</template>
