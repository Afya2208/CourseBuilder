<script lang="ts" setup >
import { compile, computed, onMounted, onUnmounted, ref } from 'vue';
import type { ContentBlock } from '@/models/main';
import api from '@/services/api';


const props = defineProps<{
    content:ContentBlock
}>()
const content = ref<ContentBlock>(props.content)


onMounted(async ()=> {
    if (content.value.contentBlockTypeId == 2 || content.value.contentBlockTypeId == 3) {
        await api.get(`content-blocks/${content.value.id}/file/${content.value.fileName}`, {
            responseType: 'blob'
        }).then(res=> {
            content.value.fileUrl = URL.createObjectURL(res.data)
        })
    }
})
onUnmounted(() => {
    if (content.value.contentBlockTypeId == 2 || content.value.contentBlockTypeId == 3) {
        if (content.value.fileUrl) {
            URL.revokeObjectURL(content.value.fileUrl)
        }
    }
})

const downloadFile = async () => {
    if (content.value.contentBlockTypeId == 4 && content.value.fileName) {
        await api.get(`content-blocks/${content.value.id}/file-stream/${content.value.fileName}`, {
            responseType: 'blob'
        }).then(res=> {
            const a = document.createElement('a');
            a.href = window.URL.createObjectURL(res.data);;
            a.download = content.value.fileName ?? ""; 
            document.body.appendChild(a);
            a.click(); 
            window.URL.revokeObjectURL(a.href);
            document.body.removeChild(a);
        })
    }
}

</script>

<template >
    <div class="text_div" v-if="content.contentBlockTypeId == 1" > <!--text-->
        <h3>{{ content.name }}</h3>
        <p v-html="content.textValue"></p>
    </div>
     <div class="image_div" v-else-if="content.contentBlockTypeId == 2"> <!--image file-->
        <img  alt="Изображение" :src="content.fileUrl"></img>
        <p>{{ content.name }}</p>
    </div>
    <div class="pdf_div" v-else-if="content.contentBlockTypeId == 3"> <!--pdf file-->
        <p>{{ content.name }}</p>
        <p class="description_p">{{ content.textValue }}</p>
        <iframe width="1200px" height="600px" :src="content.fileUrl" v-if="content.fileUrl"></iframe>
        <p v-else>Файл не загрузился...</p>
    </div>
    <div class="file_to_download_div" v-else-if="content.contentBlockTypeId == 4"> <!--file to download-->
        <p>{{ content.name }}</p>
        <img src="../../assets/excel-icon.jpg" title="Скачать файл" @click="downloadFile()" v-if="content.fileName?.split('.').pop() == 'xlsx'">
        <img src="../../assets/docx-icon.png" title="Скачать файл" @click="downloadFile()" v-else-if="content.fileName?.split('.').pop() == 'docx'">
        <img src="../../assets/pdf-icon.png" title="Скачать файл" @click="downloadFile()" v-else-if="content.fileName?.split('.').pop() == 'pdf'">
        <p>{{ content.textValue }}</p>
        <a download @click="downloadFile()">Скачать файл</a>
    </div>
     
</template>
<style scoped>
    div {
        margin: 10px;
        border: 2px solid red;
    }
    .text_div h3 {
        font-weight: 600;
    }
    .file_to_download_div, .image_div, .pdf_div {
        display: flex;
        align-items: center;
        flex-direction: column;
        justify-content: start;
    }
    .file_to_download_div a {
        cursor: pointer;
    }
    .pdf_div iframe {
        width: 90%;
        height: 95%;
    }
    .pdf_div .description_p{
        color: rebeccapurple;
        
    }
    .pdf_div {
        height: 50vh;
    }
    .image_div {
        margin-inline: 10px;
    }
    .file_to_download_div img {
        width: 150px;
        height: 150px;
        cursor: pointer;
    }
</style>