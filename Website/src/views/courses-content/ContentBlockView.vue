<script lang="ts" setup>
import { onMounted, onUnmounted, ref, toRaw, toRefs } from 'vue'
import type { ContentBlock } from '@/models/main.ts'
import xlsxImg from '@/assets/xlsx.png'
import docImg from '@/assets/doc.png'
import pdfImg from '@/assets/pdf.png'
import fileImg from '@/assets/file.png'
import api from '@/services/api.ts'

const props = defineProps<{
	content: ContentBlock
	isEditing: boolean
	index: number
	blocksCount: number
	lessonId: number
}>()
const { content, lessonId } = toRefs(props)
const selectedFile = ref()

const loadImageOrPdfAsync = async () => {
	await api
		.get(`content-blocks/${content.value.id}/file`, {
			responseType: 'blob',
		})
		.then((res) => {
			content.value.fileData = URL.createObjectURL(res.data)
		})
}
const loadVideoAsync = async () => {
	api.get(`token-for-video/${content.value.id}`).then(async (res) => {
		videoUrl.value = res.data.url
	})
}
const videoUrl = ref()

onMounted(async () => {
	if (content.value.id != 0) {
		if (content.value.contentBlockTypeId == 2 || content.value.contentBlockTypeId == 3) {
			await loadImageOrPdfAsync()
		} else if (content.value.contentBlockTypeId == 5) {
			await loadVideoAsync()
		}
	}
})
onUnmounted(() => {
	if (content.value.fileData) {
		URL.revokeObjectURL(content.value.fileData)
	}
	if (videoUrl.value) {
		URL.revokeObjectURL(videoUrl.value)
	}
	console.log('unmounted:', content.value.id, content.value.tempId)
})

const uploadFile = async (p: Event, extensions?: string[]) => {
	const inp = p.target as HTMLInputElement
	if (inp.files && inp.files[0]) {
		const file = inp.files[0]
		const ext = file.name.split('.').pop() ?? ''
		if (extensions && !extensions.includes(ext.toLowerCase())) {
			alert(`Неподдерживаемый формат файла. Разрешенные форматы: ${extensions.join(', ')}`)
			inp.value = ''
			return
		}
		if (file.size / 1024 / 1024 > 20) {
			alert(`Слишком большой размер файла. Загрузите файл не более 20Мб`)
			inp.value = ''
			return
		}
		content.value.fileNameView = file.name
		selectedFile.value = file
		console.log('Файл взят: ' + selectedFile.value.fileName + ' ' + selectedFile.value.size)
		if (content.value.contentBlockTypeId == 2 || content.value.contentBlockTypeId == 3) {
			const fileReader = new FileReader()
			fileReader.onload = (ev) => {
				const result = ev.target?.result
				content.value.fileData = result as string
			}
			fileReader.readAsDataURL(file)
		}
	}
}

const emits = defineEmits<{
	deleteBlock: [b: ContentBlock]
	upClicked: []
	downClicked: []
}>()

const downloadFile = async () => {
	if (content.value.contentBlockTypeId == 4 && content.value.id != 0) {
		const a = document.createElement('a')
		await api
			.get(`content-blocks/${content.value.id}/file-download`, { responseType: 'blob' })
			.then((res) => {
				a.href = window.URL.createObjectURL(res.data)
			})
		a.download = content.value.fileNameView ?? 'file'
		document.body.appendChild(a)
		a.click()
		window.URL.revokeObjectURL(a.href)
		document.body.removeChild(a)
	}
}
const getTitleForExtension = (ext: string): string => {
	let result = 'Скачать '
	switch (ext) {
		case 'xlsx':
			result += 'таблицы'
			break
		case 'docx':
			result += 'документ .docx'
			break
		case 'pdf':
			result += 'документ .pdf'
			break
		default:
			result += 'файл'
	}
	return result
}
const getImagePathForExtension = (ext: string): string => {
	let result = fileImg
	switch (ext) {
		case 'xlsx':
		case 'xls':
			result = xlsxImg
			break
		case 'docx':
			result = docImg
			break
		case 'pdf':
			result = pdfImg
			break
	}
	return result
}
const deleteClick = (block: ContentBlock) => emits('deleteBlock', block)

const onLoaded = () => console.log('Видео готово к воспроизведению')
const onError = (e) => console.error('Ошибка загрузки видео:', e)
const uploadVideo = (e: Event) => {
	const inp = e.target as HTMLInputElement
	if (inp.files && inp.files[0]) {
		const file = inp.files[0]
		const ext = file.name.split('.').pop() ?? ''
		if (ext != 'mp4') {
			alert(`Неподдерживаемый формат файла. Разрешенные форматы: mp4`)
			inp.value = ''
			return
		}
		content.value.fileNameView = file.name
		selectedFile.value = file
		console.log('Файл взят: ' + selectedFile.value.name + ' ' + selectedFile.value.size)
		const fileReader = new FileReader()
		fileReader.onload = (ev) => {
			const result = ev.target?.result
			videoUrl.value = result as string
		}
		fileReader.readAsDataURL(file)
		console.log('Файл показан: ' + selectedFile.value.name + ' ' + selectedFile.value.size)
	}
}
const saveBlockAsync = async () => {
	const formData = new FormData()
	formData.append('Id', content.value.id.toString())
	formData.append('Name', content.value.name || '')
	formData.append('LessonId', lessonId.value.toString())
	formData.append('Order', content.value.order.toString())
	formData.append('ContentBlockTypeId', content.value.contentBlockTypeId.toString())

	const blockTypeNum = Number(content.value.contentBlockTypeId)
	console.log('1. Тип блока после Number():', typeof blockTypeNum, 'Значение:', blockTypeNum)


	let fileToUpload = selectedFile.value
	if (fileToUpload) {
		fileToUpload = toRaw(fileToUpload)
	
		if (fileToUpload instanceof FileList || Array.isArray(fileToUpload)) {
			fileToUpload = fileToUpload[0]
		}
	}

	console.log('2. Это настоящий объект File?', fileToUpload instanceof File, fileToUpload)


	switch (blockTypeNum) {
		case 1:
			console.log('✅ Выполнен case 1')
			formData.append('TextValue', content.value.textValue || '')
			break
		case 2:
			console.log('✅ Выполнен case 2')
			if (fileToUpload instanceof File) formData.append('FormFile', fileToUpload)
			break
		case 3:
		case 4:
		case 5:
			console.log('Выполнен case 3, 4 или 5! (Тип:', blockTypeNum, ')') // <--- ЕСЛИ ЭТОГО НЕТ, ПРОБЛЕМА ТУТ
			if (fileToUpload instanceof File) {
				formData.append('FormFile', fileToUpload)
				console.log('Файл успешно добавлен в FormData под ключом "FormFile"')
			} else {
				console.error(
					'ОШИБКА: fileToUpload не является File!',
					typeof fileToUpload,
					fileToUpload,
				)
			}
			formData.append('TextValue', content.value.textValue || '')
			break
		default:
			console.error('switch не прошел! Значение switch:', blockTypeNum)
	}

	console.log('3. formData.has("FormFile"):', formData.has('FormFile'))
	console.log('4. Реальное содержимое FormData:', Object.fromEntries(formData.entries()))

	try {
		const t = localStorage.getItem('token')
		if (content.value.id == 0) {
			const response = await fetch(`http://localhost:5555/content-blocks`, {
				method: 'POST',
				headers: {
					Authorization: `Bearer ${t}`,
				},
				body: formData,
			})
			if (response.ok) {
				content.value.id = await response.json()
				console.log('Блок сохранен - ' + content.value.id)
			}
		} else {
			const response = await fetch(`http://localhost:5555/content-blocks`, {
				method: 'PUT',
				headers: {
					Authorization: `Bearer ${t}`,
				},
				body: formData,
			})
			if (response.ok) {
				console.log('Блок сохранен - ' + content.value.id)
			}
		}
	} catch (error) {
		console.error('Ошибка сохранения блока:', error)
	}
}
defineExpose({ selectedFile, content, saveBlockAsync })
</script>

<template>
	<div class="m-0 p-0">
		<div v-if="isEditing" class="control_div">
			<span class="m-1">Тип</span>
			<select class="form-control" style="width: 150px" v-model="content.contentBlockTypeId">
				<option value="1">Текст</option>
				<option value="2">Изображение</option>
				<option value="3">.pdf</option>
				<option value="4">Файл</option>
				<option value="5">Видео</option>
			</select>
			<button class="btn btn-outline-danger m-2" @click="deleteClick(content)">❌</button>
			<button
				class="btn btn-outline-dark btn-sm m-1"
				:disabled="index === 0"
				@click="emits('upClicked')"
			>
				⬆️
			</button>
			<button
				class="btn btn-outline-dark btn-sm m-1"
				:disabled="index === blocksCount - 1"
				@click="emits('downClicked')"
			>
				⬇️
			</button>
		</div>

		<div class="text_div m-0 p-0" v-if="content.contentBlockTypeId == 1">
			<div v-if="isEditing">
				<input
					v-model="content.name"
					placeholder="Заголовок блока текста"
					class="form-control mb-2"
				/>
				<textarea
					v-model="content.textValue"
					placeholder="Содержание..."
					class="form-control"
					rows="3"
				/>
			</div>
			<div v-else class="m-0 p-0">
				<h4>{{ content.name }}</h4>
				<p style="white-space: pre-line" v-html="content.textValue"></p>
			</div>
		</div>
		<div class="image_div m-0 p-1" v-else-if="content.contentBlockTypeId == 2">
			<input
				v-if="isEditing"
				type="file"
				@change="uploadFile($event, ['png', 'jpeg', 'jpg', 'gif', 'webp'])"
				accept=".png,.jpeg,.jpg,.gif,.webp"
				class="form-control mb-2"
			/>
			<img
				class="img-fluid"
				style="max-height: 500px; max-width: 90%"
				alt="Изображение"
				:src="content.fileData"
			/>
			<input
				v-if="isEditing"
				v-model="content.name"
				placeholder="Подпись для рисунка"
				class="form-control my-2 text-center"
			/>
			<p v-else>{{ content.name }}</p>
		</div>
		<div class="pdf_div m-0 p-0" v-else-if="content.contentBlockTypeId == 3">
			<input
				v-if="isEditing"
				v-model="content.name"
				placeholder="Заголовок для pdf файла"
				class="form-control mb-2"
			/>
			<p v-else>{{ content.name }}</p>
			<input
				v-if="isEditing"
				v-model="content.textValue"
				placeholder="Описание для pdf файла"
				class="form-control mb-2"
			/>
			<input
				v-if="isEditing"
				type="file"
				@change="uploadFile($event, ['pdf'])"
				accept=".pdf"
				class="form-control mb-2"
			/>
			<p v-else class="description_p">{{ content.textValue }}</p>
			<iframe :src="content.fileData" v-if="content.fileData"></iframe>
			<p v-else>Файл не загрузился...</p>
		</div>
		<div class="file_to_download_div m-0 p-0" v-else-if="content.contentBlockTypeId == 4">
			<div v-if="isEditing" class="file_to_download_div m-0 p-0">
				<p class="mb-2">Прикрепите файл</p>
				<input type="file" @change="uploadFile($event)" class="form-control" />
				<p v-if="content.fileNameView" class="my-2">Файл: {{ content.fileNameView }}</p>
				<input
					v-model="content.name"
					placeholder="Заголовок для файла"
					class="form-control my-2"
				/>
				<input
					v-model="content.textValue"
					placeholder="Описание для файла"
					class="form-control mb-2"
				/>
			</div>
			<div v-else class="file_to_download_div m-0 p-0">
				<p>{{ content.name }}</p>
				<img
					:title="getTitleForExtension(content.fileNameView?.split('.').pop() ?? '')"
					:src="getImagePathForExtension(content.fileNameView?.split('.').pop() ?? '')"
					@click="downloadFile()"
				/>
				<p class="m-1">{{ content.textValue }}</p>
				<a class="btn btn-outline-info" download @click="downloadFile">{{
					getTitleForExtension(content.fileNameView?.split('.').pop() ?? '')
				}}</a>
			</div>
		</div>
		<div v-else-if="content.contentBlockTypeId == 5">
			<input
				accept=".mp4"
				v-show="isEditing"
				type="file"
				class="form-control my-2"
				@change="uploadVideo"
			/>
			<video
				:src="videoUrl"
				v-if="videoUrl"
				width="100%"
				height="auto"
				controls
				crossorigin="anonymous"
				@error="onError"
				@loadeddata="onLoaded"
			/>
		</div>
	</div>
</template>
<style scoped>
.file_to_download_div,
.image_div,
.pdf_div {
	display: flex;
	align-items: center;
	flex-direction: column;
	justify-content: start;
}
.control_div {
	display: flex;
	align-items: center;
	justify-content: center;
	margin-block: 10px;
}
.file_to_download_div a {
	cursor: pointer;
}
.pdf_div iframe {
	width: 90%;
	height: 95%;
}
.pdf_div {
	height: 90vh;
}
.file_to_download_div img {
	width: 150px;
	height: 150px;
	cursor: pointer;
}
</style>
