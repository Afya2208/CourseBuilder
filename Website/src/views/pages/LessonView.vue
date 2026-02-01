<script setup lang="ts">
import type { Lesson, Task, ContentBlock } from '@/models/main'
import api from '@/services/api'
import { onMounted, ref, type Ref } from 'vue'
import { useRoute } from 'vue-router'
import ContentBlockView from '../components/ContentBlockView.vue'
import TaskView from '../components/TaskView.vue'

const lessonId = useRoute().params.lessonId
const lesson = ref<Lesson>()
const contentBlocks = ref<ContentBlock[]>([])
const tasks = ref<Task[]>([])
const refsToTaskViews = ref([])
const getData = async () => {
    await api.get<Lesson>(`lessons/${lessonId}`).then((res) => {
        lesson.value = res.data
    })
    await api.get<Task[]>(`lessons/${lessonId}/tasks`).then((res) => {
        tasks.value = res.data
        console.log(JSON.stringify(tasks.value))
    })
    await api.get<ContentBlock[]>(`lessons/${lessonId}/content-blocks`).then((res) => {
        contentBlocks.value = res.data
    })
}

const checkTasks = async () => {
    refsToTaskViews.value.forEach((x) => x.checkTask())
}

onMounted(async () => {
    await getData()
})
</script>

<template>
    <div class="container">
        <div class="lesson-main-info-div" v-if="lesson">
            <h1>{{ lesson.name }}</h1>
            <label>
                <span>Название:</span>
                <input v-model="lesson.name" />
            </label>
            <label>
                <span>Описание:</span>
                <input v-model="lesson.description" />
            </label>
            <RouterLink :to="`/courses/modules/${lesson.moduleId}`"
                >Вернуться к списку занятий модуля</RouterLink
            >
        </div>
        <div class="lesson-content-div" v-if="lesson?.lessonTypeId == 1 && contentBlocks.length > 0">
            <h2>Содержание занятия</h2>
            <ContentBlockView v-for="contentBlock in contentBlocks" :content="contentBlock" />
        </div>
        <div class="lesson-tasks-div" v-if="tasks.length > 0">
            <h2>Задания занятия</h2>
            <TaskView
                v-for="(task, index) in tasks"
                :task="task"
                :ref="
                    (ref) => {
                        if (ref) refsToTaskViews[index] = ref
                    }
                "
            />
            <button @click="checkTasks">Проверить задания</button>
        </div>
    </div>
</template>

<style scoped></style>
