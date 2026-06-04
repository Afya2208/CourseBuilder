<script lang="ts" setup>
import type { Group } from '@/models/group.ts'
import type { Course } from '@/models/main.ts'
import api from '@/services/api.ts'
import { useUserStore } from '@/stores/user.ts'
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
	BFormCheckbox,
} from 'bootstrap-vue-next'
import { ref } from 'vue'

const { user } = useUserStore()
const props = defineProps<{
	courses: Course[]
}>()
const emits = defineEmits<{
	saved: [g: Group]
	closed: []
}>()

const startEditingGroup = (g: Group) => {
	group.value = {
		id: g.id,
		name: g.name,
		curatorFeedback: g.curatorFeedback,
		curatorId: g.curatorId,
		dateStart: g.dateStart,
		dateEnd: g.dateEnd,
		maxMembersCount: g.maxMembersCount,
		coursesInfo: g.coursesInfo,
		usersInfo: g.usersInfo,
	}
	showModal()
}
const getDefaultGroup = (): Group => {
	return {
		id: 0,
		name: '',
		curatorFeedback: '',
		curatorId: user!!.id,
		dateStart: new Date().toISOString().split('T')[0]!!,
		dateEnd: new Date().toISOString().split('T')[0]!!,
		maxMembersCount: 20,
		coursesInfo: [],
		usersInfo: [],
	}
}
const { hide: closeModal, show: showModal } = useToggle('group-modal')
const group = ref<Group>(getDefaultGroup())

const startCreatingGroup = () => {
	group.value = getDefaultGroup()
	showModal()
}
const close = () => {
	closeModal()
	emits('closed')
}

const checkDates = (): boolean => {
	let de = new Date(Date.parse(group.value.dateEnd))
	let ds = new Date(Date.parse(group.value.dateStart))
	return de > ds
}

const checkFields = (): boolean => {
	const v = new Validator()
	v.validate(!group.value.name, 'Укажите название')
	v.validate(!group.value.dateEnd, 'Укажите дату окончания обучения в группе')
	v.validate(!group.value.dateStart, 'Укажите дату начала обучения в группе')
	v.validate(!checkDates(), 'Дата окончания должна быть позже даты начала хотя бы на 1 день')
	v.validate(
		!group.value.curatorFeedback,
		'Укажите данные для связи студентов с Вами. Это может быть ссылка на группу или контактные данные',
	)
	return v.returnResult()
}
const save = async () => {
	if (checkFields()) {
		if (group.value.id == 0) {
			await api
				.post<Group>('groups', group.value)
				.then((res) => {
					alert('Группа успешно сохранена')
					emits('saved', res.data)
					closeModal()
				})
				.catch(() => {
					alert('Ошибка, повторите позже')
				})
		} else {
			await api
				.put<Group>('groups', group.value)
				.then((res) => {
					alert('Группа успешно сохранена')
					emits('saved', res.data)
					closeModal()
				})
				.catch(() => {
					alert('Ошибка, повторите позже')
				})
		}
	}
}
defineExpose({ startCreatingGroup, startEditingGroup })
</script>

<template>
	<BModal
		id="group-modal"
		centered
		header-class="bg-dark text-white"
		header-close-class="bg-white"
		@close="close"
		title="Новый набор курсов"
		no-close-on-backdrop
	>
		<BForm>
			<BFormFloatingLabel class="my-2" label="Название" label-for="group-name">
				<BFormInput
					id="group-name"
					v-model="group.name"
					type="text"
					placeholder="Набор разработчика..."
				/>
			</BFormFloatingLabel>

			<BFormFloatingLabel
				class="my-2"
				label="Связь с куратором"
				label-for="group-curatorFeedback"
			>
				<BFormInput
					id="group-curatorFeedback"
					v-model="group.curatorFeedback"
					type="text"
					placeholder="Группа в тг: ..."
				/>
			</BFormFloatingLabel>

			<BFormFloatingLabel
				class="my-2"
				label="Дата начала обучения"
				label-for="group-dateStart"
			>
				<BFormInput id="group-dateStart" v-model="group.dateStart" type="date" />
			</BFormFloatingLabel>

			<BFormFloatingLabel
				class="my-2"
				label="Дата окончания обучения"
				label-for="group-dateEnd"
			>
				<BFormInput id="group-dateEnd" v-model="group.dateEnd" type="date" />
			</BFormFloatingLabel>

			<BFormFloatingLabel
				class="my-2"
				label="Максимальное количество студентов в группе"
				label-for="group-max"
			>
				<BFormInput id="group-max" v-model="group.maxMembersCount" type="number" />
			</BFormFloatingLabel>

			<div>
				<p>Курсы для студентов группы:</p>
				<select v-model="group.coursesInfo" multiple select-size="7">
					<option v-for="course in courses" :value="{ id: course.id, name: course.name }">
						{{ course.name }}
					</option>
				</select>
			</div>
		</BForm>
		<template #footer>
			<BButton variant="primary" @click="save">Сохранить</BButton>
			<button class="btn btn-dark" @click="close">Отмена</button>
		</template>
	</BModal>
</template>
