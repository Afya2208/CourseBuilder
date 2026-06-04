<script setup lang="ts">
import type { Course, Module } from '@/models/main'
import api from '@/services/api'
import { useUserStore } from '@/stores/user'
import { BButton } from 'bootstrap-vue-next'
import { onMounted, ref, type Ref } from 'vue'
import { RouterLink, useRoute, useRouter } from 'vue-router'
import ModuleForm from '@/views/pages/forms/ModuleForm.vue'
import type { ComponentExposed } from 'vue-component-type-helpers'
import { pluralizeRu } from '@/util/methods.ts'

const course = ref<Course>()
const router = useRouter()
const moduleForm = ref<ComponentExposed<typeof ModuleForm>>()
const modules = ref<Module[]>([])
const { user, availableCoursesIds, addAvailableCourse } = useUserStore()
const itIsOwner = ref(false)
const courseId = Number.parseInt(useRoute().params.courseId as string)
const getData = async () => {
	await api.get<Course>(`courses/${courseId}`).then((res) => {
		course.value = res.data
		itIsOwner.value = res.data.authorId == user?.id
	})
	await loadModulesAsync()
}

const loadModulesAsync = async () => {
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

const addModule = () => moduleForm.value?.startCreatingModule()

const startEditingModule = async (module: Module) => {
	moduleForm.value?.startEditingModule(module)
}
const showFileInput = ref(false)
const showImport = () => {
	showFileInput.value = true
}
const importModules = async () => {
	const inp = document.createElement('input')
	inp.type = 'file'
	inp.accept = '.xlsx'
	document.body.appendChild(inp)
	inp.click()
	inp.onchange = async (e) => {
		if (inp.files && inp.files[0]) {
			const file = inp.files[0]
			const ext = file.name.split('.').pop() ?? ''
			if (ext != 'xlsx') {
				alert('Неправильный формат файла импорта - требуется файл с расширением .xlsx')
				return
			}
			const formData = new FormData()
			formData.append('File', file)
			formData.append('FileName', file.name || '')
			await api
				.post(`course/${courseId}/import-modules`, formData, {
					headers: {
						'Content-Type': 'multipart/form-data',
					},
				})
				.then((res) => {
					alert('Модули успешно импортированы')
				})
				.catch((err) => {
					if (err.status == 400) alert('Ошибка, неправильный формат данных в файле')
				})
			await getData()
		}
	}
	document.body.removeChild(inp)
}
const uploadFile = async (p) => {
	const inp = p.target
	if (inp.files && inp.files[0]) {
		const file = inp.files[0]
		const ext = file.name.split('.').pop() ?? ''
		if (ext != 'xlsx') {
			alert('Неправильный формат файла импорта - требуется файл с расширением .xlsx')
			inp.value = null
			fileInfo.value = null
			return
		}
		const formData = new FormData()
		formData.append('File', file)
		formData.append('FileName', file.name || '')
		fileInfo.value = formData
	}
}
const userHasCourse = ref(false)
if (availableCoursesIds) {
	if (availableCoursesIds.includes(courseId)) {
		userHasCourse.value = true
	}
}
const fileInfo = ref()
const sendFile = async () => {
	if (fileInfo.value) {
		await api
			.post(`course/${courseId}/import-modules`, fileInfo.value, {
				headers: {
					'Content-Type': 'multipart/form-data',
				},
			})
			.then(async (res) => {
				alert('Модули успешно импортированы')
				await getData()
			})
			.catch((err) => {
				if (err.status == 400)
					alert(
						'Ошибка, неправильный формат данных в файле. У всех модулей должны быть указаны название и описание' +
							', порядок должен быть указан в виде целого числа',
					)
			})
	} else {
		alert('Загрузите файл')
	}
}
const addCourseToUser = async () => {
	if (!user) {
		alert("Для приобретения курса нужно авторизоваться")
		return
	}
	if (course.value?.price && course.value?.price > 0) {
		await router.push('/payment/course/' + courseId)
	} else {
		await api.post(`users/${user?.id}/add-course/${courseId}`).then((res) => {
			alert('Курс успешно добавлен в список доступных курсов')
			addAvailableCourse(courseId)
			userHasCourse.value = true
		})
	}
}
const onSaved = async () => {
	orderChanged.value = false
	await loadModulesAsync()
}
const formsLessons = ['занятие', 'занятия', 'занятий']

const counts = (m: Module): string => {
	return `${m.lessonsCount ?? 0} ${pluralizeRu(m.lessonsCount ?? 0, formsLessons)}`
}

const syncOrders = () => {
	modules.value.forEach((card, i) => {
		card.order = i + 1
	})
}

function move(fromIndex: number, toIndex: number) {
	if (toIndex < 0 || toIndex >= modules.value.length || fromIndex === toIndex) return
	const arr = modules.value
	const [item] = arr.splice(fromIndex, 1)
	arr.splice(toIndex, 0, item)
	orderChanged.value = true
}

const saveOrder = async () => {
	syncOrders()
	await api
		.post(
			'/modules/order',
			modules.value.map((x) => {
				return { id: x.id, order: x.order }
			}),
		)
		.then((res) => {
			alert('Порядок модулей сохранен')
			orderChanged.value = false
		})
}

const orderChanged = ref(false)
const loadError = ref(false)
</script>

<template>
	<div class="container" v-if="course">
		<h3 class="my-3">
			{{ course.name }}
		</h3>
		<p>{{ course.description }}</p>
		<span class="text-primary" v-if="course.linkedGroupId">Курс изучается в группе</span>
		<p>
			<span class="text-primary" v-if="course.price > 0">{{ course.price }} руб.</span>
			<span class="text-success" v-else>Курс бесплатный</span>
			<button
				class="btn btn-outline-primary m-2"
				@click="addCourseToUser"
				v-if="user?.role.id == 3 && !userHasCourse && course.isPublic || !user && course.isPublic"
			>
				Приобрести
			</button>
		</p>

		<h4>
			Модули
			<button v-if="itIsOwner" class="btn btn-primary m-1" @click="addModule">
				Добавить модуль
			</button>
			<button v-if="itIsOwner" class="btn btn-outline-info m-1" @click="showImport">
				Импортировать модули
			</button>
			<input
				v-if="showFileInput"
				accept=".xlsx"
				class="form-control w-25 d-inline m-2"
				type="file"
				id="moduleImportFile"
				@change="uploadFile"
			/>
			<button v-if="showFileInput" class="btn btn-outline-info m-1" @click="sendFile">
				Отправить файл для импорта
			</button>
			<button v-if="orderChanged" class="btn btn-outline-info m-1" @click="saveOrder()">
				Сохранить порядок модулей
			</button>
		</h4>

		<div v-if="!modules && !loadError">
			<p>Загрузка данных, подождите, пожалуйста</p>
		</div>

		<div v-else-if="!modules && loadError">
			<p>Ошибка загрузки данных, попробуйте позже</p>
		</div>

		<div v-else-if="modules && modules.length > 0" class="row row-cols-1 row-cols-md-4 g-4 p-2">
			<div class="col" v-for="(module, index) in modules" :key="module.id">
				<div class="card h-100" style="min-height: 200px">
					<div
						class="card-body d-flex flex-column card-clickable position-relative flex-grow-1"
					>
						<h5 class="card-title">{{ module.name }}</h5>
						<p class="card-text my-1">{{ module.description }}</p>
						<RouterLink
							class="stretched-link"
							:to="`/courses/${courseId}/modules/${module.id}`"
						/>
						<p class="card-subtitle small text-primary-emphasis mt-auto">
							{{ counts(module) }}
						</p>
					</div>

					<div class="card-footer" v-if="itIsOwner">
						<div class="d-flex justify-content-between gap-2">
							<div class="d-flex gap-2">
								<BButton
									variant="outline-danger"
									@click="deleteModule(module, index)"
									>❌</BButton
								>
								<BButton
									variant="outline-warning"
									@click="startEditingModule(module)"
									>✏️</BButton
								>
							</div>
							<div class="d-flex gap-2">
								<BButton
									variant="outline-dark"
									:disabled="index === 0"
									@click="move(index, index - 1)"
									>⬅️</BButton
								>
								<BButton
									variant="outline-dark"
									:disabled="index === modules.length - 1"
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

		<ModuleForm
			ref="moduleForm"
			:course-id="courseId"
			@saved="onSaved"
			:modules-count="modules.length"
			:modules-have-order="course?.modulesHaveOrder == true"
		/>
	</div>
</template>

<style scoped></style>
