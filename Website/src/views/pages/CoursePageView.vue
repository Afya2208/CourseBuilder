<script setup lang="ts">
import type { Course, Module } from '@/models/main'
import api from '@/services/api'
import { useUserStore } from '@/stores/user'
import {
	BButton,
	BModal,
	useToggle,
	BForm,
	BFormFloatingLabel,
	BFormInput,
	BFormCheckbox,
	BCard,
	BCardFooter,
	BCardText,
} from 'bootstrap-vue-next'
import { storeToRefs } from 'pinia'
import { onMounted, ref, type Ref } from 'vue'
import { useRoute } from 'vue-router'

const course = ref<Course>()
const modules = ref<Module[]>([])
const { user } = storeToRefs(useUserStore())
const courseId = useRoute().params.courseId
const getData = async () => {
	await api.get<Course>(`courses/${courseId}`).then((res) => {
		course.value = res.data
	})
	await api.get<Module[]>(`courses/${courseId}/modules`).then((res) => {
		modules.value = res.data
	})
}
onMounted(async () => {
	await getData()
})
const deleteModule = async (module: Module, index: number) => {
	if (confirm(`Вы уверены, что хотите удалить модуль ${module.name}?`)) {
		await api
			.delete(`modules/${module.id}`)
			.then((res) => {
				modules.value.splice(index, 1)
			})
			.catch((err) => {
				alert('Ошибка, попробуйте позже')
			})
	}
}
const { hide: hideModuleModal, show: showModuleModal } = useToggle('module-modal')
const addModule = () => showModuleModal()
const getNullModule = (): Module => {
	return {
		id: 0,
		name: '',
		description: '',
		lessonsHaveOrder: true,
		courseId: parseInt(courseId as string),
		order: modules.value.length + 1,
	}
}
const close = () => {
	if (editableModule.value.id != 0) {
		selectedModuleIndex.value = undefined
		editableModule.value = getNullModule()
	}
	hideModuleModal()
}
const editableModule = ref<Module>(getNullModule())
const selectedModuleIndex = ref<number>()
const saveModule = async () => {
	if (editableModule.value.id == 0) {
		await api
			.post<Module>('modules', editableModule.value)
			.then((res) => {
				modules.value.push({ ...res.data, lessonsCount: 0 })
				editableModule.value = getNullModule()
				hideModuleModal()
				alert('Модуль успешно создан')
			})
			.catch((err) => {
				hideModuleModal()
				alert('Ошибка, повторите позже')
			})
	} else {
		await api
			.put<Module>('modules', editableModule.value)
			.then((res) => {
				let oldModule = modules.value[selectedModuleIndex.value!!]
				let newModule = res.data
				newModule.lessonsCount = oldModule?.lessonsCount
				modules.value[selectedModuleIndex.value!!] = newModule
				alert('Модуль успешно изменен')
			})
			.catch((err) => {
				alert('Ошибка, повторите позже')
			})
		hideModuleModal()
		editableModule.value = getNullModule()
		selectedModuleIndex.value = undefined
	}
}
const startEditingModule = async (module: Module, index: number) => {
	selectedModuleIndex.value = index
	editableModule.value = {
		id: module.id,
		courseId: module.courseId,
		order: module.order,
		name: module.name,
		description: module.description,
		lessonsHaveOrder: module.lessonsHaveOrder ?? true,
	}
	showModuleModal()
}
</script>

<template>
	<div class="container">
		<div v-if="course">
			<h1>{{ course.name }}</h1>
			<p v-if="course.description">{{ course.description }}</p>
			<p v-if="course.price && course.price > 0">{{ course.price }} руб.</p>
		</div>
		<h3>Темы</h3>
		<div v-if="course" class="d-flex flex-wrap">
			<ul>
				<li v-for="theme in course.themes">
					<p>{{ theme.name }}</p>
				</li>
			</ul>
		</div>
		<h3>
			Модули
			<BButton v-if="user?.role.id == 1" @click="addModule" variant="primary" class="m-2"
				>Добавить модуль</BButton
			>
		</h3>

		<div class="d-flex flex-wrap" v-if="modules">
			<BCard
				:title="module.name"
				class="m-2"
				style="max-width: 20rem"
				v-for="(module, index) in modules"
				:style="{ order: module.order }"
			>
				<BCardText>
					<p>{{ module.description }}</p>
					<p>Количество занятий: {{ module.lessonsCount }}</p>
					<p>
						<RouterLink
							class="btn btn-outline-primary"
							:to="`/courses/modules/${module.id}`"
							>Перейти к занятиям</RouterLink
						>
					</p>
				</BCardText>
				<template #footer v-if="user?.role.id == 1">
					<BButton
						variant="outline-danger"
						class="me-2"
						@click="deleteModule(module, index)"
						>❌</BButton
					>
					<BButton variant="outline-warning" @click="startEditingModule(module, index)"
						>✏️</BButton
					>
				</template>
			</BCard>
		</div>
	</div>

	<BModal
		id="module-modal"
		centered
		header-class="bg-dark text-white"
		header-close-class="bg-white"
		@close="close"
		title="Новый модуль"
		no-close-on-backdrop
	>
		<BForm>
			<BFormFloatingLabel class="my-2" label="Название" label-for="module-name">
				<BFormInput
					id="module-name"
					v-model="editableModule.name"
					type="text"
					placeholder="Новый модуль"
				/>
			</BFormFloatingLabel>

			<BFormFloatingLabel class="my-2" label="Описание" label-for="module-desc">
				<BFormInput
					id="module-desc"
					v-model="editableModule.description"
					type="text"
					placeholder="Описание..."
				/>
			</BFormFloatingLabel>

			<BFormCheckbox id="module-lessons-have-order" v-model="editableModule.lessonsHaveOrder">
				У занятий есть порядок
			</BFormCheckbox>

			<BFormFloatingLabel class="my-2" label="Порядок" label-for="module-order">
				<BFormInput
					id="module-order"
					v-model="editableModule.order"
					:disabled="!course?.modulesHaveOrder"
					type="number"
					min="1"
					max="100"
					placeholder="1"
				/>
			</BFormFloatingLabel>
		</BForm>
		<template #footer>
			<BButton variant="primary" @click="saveModule()">Сохранить</BButton>
			<button class="btn btn-dark" @click="close">Отмена</button>
		</template>
	</BModal>
</template>

<style scoped></style>
