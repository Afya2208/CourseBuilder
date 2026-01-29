<script setup lang="ts">
import type { ContentBlock, ContentBlockType } from '@/models/main';
import api from '@/services/api';
import { onMounted, ref } from 'vue';

const blocks = ref<ContentBlock[]>([])
const contentTypes = ref<ContentBlockType[]>([])
const createItem = () => {
    blocks.value.push({
        contentBlockTypeId: 4,
        order: blocks.value.length + 1,
        id: 0,
        name: ''
    })
}
const deleteItem = (index:number) => {
    blocks.value.splice(index, 1)
}
const fetch = async () => {
    await api.get<ContentBlockType[]>("content-block-types")
    .then(res=>{
        contentTypes.value = res.data
    })
}
const loadImage = (event:Event, block:ContentBlock) => {
    let input = event.target as HTMLInputElement
    if (FileReader && input.files && input.files[0]) {
        let file = input.files[0]
        if (file?.name.endsWith(".png") || file?.name.endsWith(".jpeg") || file?.name.endsWith(".jpg")) {
            let reader = new FileReader()
            reader.onload = () => {
                block.fileUrl = reader.result
                block.fileName = file.name
            }
            reader.readAsDataURL(file)
        }
        else {
            // alert
        }
    }
}
const loadFile = (event:Event, block:ContentBlock) => {
    let input = event.target as HTMLInputElement
    if (FileReader && input.files && input.files[0]) {
        let file = input.files[0]
        let reader = new FileReader()
        reader.onload = () => {
            block.fileUrl = reader.result
            block.fileName = file.name
        }
        reader.readAsDataURL(file)
        alert(JSON.stringify(block))
    }
}
const loadPdf = (event:Event, block:ContentBlock) => {
    let input = event.target as HTMLInputElement
    if (FileReader && input.files && input.files[0]) {
        let file = input.files[0]
        if (file?.name.endsWith(".pdf")) {
            let reader = new FileReader()
            reader.onload = () => {
                block.fileUrl = reader.result
                block.fileName = file.name
            }
            reader.readAsDataURL(file)
        }
        else {
            // alert
        }
    }
}
onMounted(async() => {
    await fetch();
})
</script>
<template>
    <div>
        <div>
            <button @click="createItem">Добавить блок содержания</button>
        </div>
        <div>
            <div class="item" v-for="block, index in blocks" :style="{order: block.order}">
                <div>
                    <button @click="deleteItem(index)">Удалить</button>
                    <span>Тип</span>
                    <select v-model="block.contentBlockTypeId">
                        <option v-for="type in contentTypes" :value="type.id">{{ type.name }}</option>
                    </select>
                </div>
                <div v-if="block.contentBlockTypeId == 1">
                    <input v-model="block.name" placeholder="Заголовок блока текста"/>
                    <input v-model="block.textValue" placeholder="Содержание..."/>
                </div>
                <div v-else-if="block.contentBlockTypeId == 2">
                    <input type="file" @change="loadImage($event, block)"/>
                    <img :src="block.fileUrl">
                </div>
                <div v-else-if="block.contentBlockTypeId == 3">
                    <input type="file" @change="loadPdf($event, block)"/>
                    <iframe :src="block.fileUrl"></iframe>
                </div>
                <div v-else-if="block.contentBlockTypeId == 4">
                    <p>Прикрепите файл</p>
                    <input type="file" @change="loadFile($event, block)"/>
                </div>
            </div>
        </div>
    </div>
</template>
<style scoped>
    .item {
        margin: 15px;
        background-color: green;
        border: solid slateblue 2px;
    }
</style>