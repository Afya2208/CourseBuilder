<script lang="ts" setup>
import { compile, computed, onMounted, onUnmounted, ref, watch } from 'vue'
import type { ContentBlock } from '@/models/main'
import api from '@/services/api'

const props = defineProps<{
	content: ContentBlock,
	isEditing: Boolean
}>()
const content = ref<ContentBlock>(props.content)
onMounted(async () => {
    if (content.value.id != 0) {
        if (content.value.contentBlockTypeId == 2 || content.value.contentBlockTypeId == 3) {
		await api
			.get(`content-blocks/${content.value.id}/file`, {
				responseType: 'blob',
			})
			.then((res) => {
				content.value.fileData = URL.createObjectURL(res.data)
			})
	    }
    }
})
onUnmounted(() => {
	if (content.value.contentBlockTypeId == 2 || content.value.contentBlockTypeId == 3) {
		if (content.value.fileData) {
			URL.revokeObjectURL(content.value.fileData)
		}
    }
})



const loadFile = (event: Event, block: ContentBlock, extensions?: string[]) => {
	const input = event.target as HTMLInputElement
	if (!FileReader || !input.files || !input.files[0]) return
	const file = input.files[0]
	const ext = file.name.split('.').pop() ?? ''
	if (extensions && !extensions.includes(ext.toLowerCase())) {
		event.preventDefault()
		alert(`Неподдерживаемый формат файла. Разрешенные форматы: ${extensions.join(', ')}`)
		return
	}
	const reader = new FileReader()
	reader.onload = (e) => {
        const result = e.target?.result
        block.fileName = file.name
        block.file = file
        if (block.contentBlockTypeId == 2 || block.contentBlockTypeId == 3) {
            if (typeof(result) == "string") {
                block.fileData = result
            }
        }
	}
    reader.readAsDataURL(file)
}
const emit = defineEmits(['delete-click'])
const downloadFile = async () => {
    if (content.value.contentBlockTypeId == 4 && content.value.fileName) {
        await api.get(`content-blocks/${content.value.id}/file-stream`, {responseType: 'blob',})
        .then((res) => {
            const a = document.createElement('a')
            a.href = window.URL.createObjectURL(res.data)
            a.download = content.value.fileName ?? 'file'
            document.body.appendChild(a)
            a.click()
            window.URL.revokeObjectURL(a.href)
            document.body.removeChild(a)
        })
    }
}
const deleteClick = (block:ContentBlock) => {
    emit("delete-click", block)
}
</script>

<template>
    <div class="m-0 p-0">
        <div v-if="props.isEditing" class="control_div">
            <span class="m-1">Тип</span>
            <select v-model="content.contentBlockTypeId">
                <option value="1">Текст</option>
                <option value="2">Изображение</option>
                <option value="3">.pdf</option>
                <option value="4">Файл</option>
            </select>
            <span class="m-1">Порядок:</span>
            <input
                v-model.number="content.order"
                type="number"
                class="form-control d-inline-block w-auto ml-2"
                style="width: 80px"
            />
            <button class="btn btn-outline-danger m-1" @click="deleteClick(content)">❌</button>
	    </div>
    
        <div class="text_div m-0 p-0" v-if="content.contentBlockTypeId == 1">
            <div v-if="props.isEditing">
                <input
                    v-model="content.name"
                    placeholder="Заголовок блока текста"
                    class="form-control mb-2"/>
                <textarea
                    v-model="content.textValue"
                    placeholder="Содержание..."
                    class="form-control"
                    rows="3"/>
            </div>
            <div v-else class="m-0 p-0">
                <h3>{{ content.name }}</h3>
                <p style="white-space: pre-line;" v-html="content.textValue"></p>
            </div>
        </div>
        <div class="image_div m-0 p-1" v-else-if="content.contentBlockTypeId == 2">
            <input
                v-if="props.isEditing"
                type="file"
                @change="loadFile($event, content, ['png', 'jpeg', 'jpg', 'gif', 'webp'])"
                accept=".png,.jpeg,.jpg,.gif,.webp"
                class="form-control mb-2"/>
            <img
                class="img-fluid"
                style="max-height: 500px; max-width: 90%;"
                alt="Изображение"
                :src="content.fileData"/>
            <input
                v-if="props.isEditing"
                v-model="content.name"
                placeholder="Подпись для рисунка"
                class="form-control mb-2"/>
            <p v-else>{{ content.name }}</p>
        </div>
        <div class="pdf_div m-0 p-0" v-else-if="content.contentBlockTypeId == 3">
            <input
                v-if="props.isEditing"
                v-model="content.name"
                placeholder="Заголовок для pdf файла"
                class="form-control mb-2"/>
            <p v-else>{{ content.name }}</p>
            <input
                v-if="props.isEditing"
                v-model="content.textValue"
                placeholder="Описание для pdf файла"
                class="form-control mb-2"/>
            <input
                v-if="props.isEditing"
                type="file"
                @change="loadFile($event, content, ['pdf'])"
                accept=".pdf"
                class="form-control mb-2"/>
            <p v-else class="description_p">{{ content.textValue }}</p>
            <iframe :src="content.fileData" v-if="content.fileData"></iframe>
            <p v-else>Файл не загрузился...</p>
        </div>
        <div class="file_to_download_div m-0 p-0" v-else-if="content.contentBlockTypeId == 4"> <!--file to download-->
            <div v-if="props.isEditing" class="file_to_download_div m-0 p-0">
                <p class="mb-2">Прикрепите файл</p>
                <input type="file" @change="loadFile($event, content)" class="form-control" />
                <p v-if="content.fileName" class="mt-2">Файл: {{ content.fileName }}</p>
                <input
                    v-model="content.name"
                    placeholder="Заголовок для файла"
                    class="form-control mb-2"
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
                    src="../../assets/xlsx.png"
                    title="Скачать файл"
                    @click="downloadFile()"
                    v-if="content.fileName?.split('.').pop() == 'xlsx'"
                />
                <img
                    src="../../assets/doc.png"
                    title="Скачать файл"
                    @click="downloadFile()"
                    v-else-if="content.fileName?.split('.').pop() == 'docx'"
                />
                <img
                    src="../../assets/pdf.png"
                    title="Скачать файл"
                    @click="downloadFile()"
                    v-else-if="content.fileName?.split('.').pop() == 'pdf'"
                />
                <img
                    src="../../assets/file.png"
                    title="Скачать файл"
                    @click="downloadFile()"
                    v-else
                />
                <p>{{ content.textValue }}</p>
                <a download @click="downloadFile()">Скачать файл</a>
            </div>
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
