<script lang="ts" setup>
import type { Course, Theme } from '@/models/main'
import api from '@/services/api'
import { onMounted, provide, ref } from 'vue'
import ThemeListView from '../components/ThemeListView.vue'
import CourseListView from '../components/CourseListView.vue'
import LessonEditorView from './LessonEditorView.vue'

const courses = ref<Course[]>()
const themes = ref<Theme[]>()
provide('themes', themes)
const getData = async () => {
    await api.get<Theme[]>('themes').then((res) => {
        themes.value = res.data
    })
    await api.get<Course[]>('courses').then((res) => {
        courses.value = res.data
    })
}
onMounted(async () => {
    await getData()
})
</script>

<template>
    <div class="container">
        <h1>Курсы</h1>
        <p v-if="!courses || !themes">Загрузка...</p>
        <div v-if="courses && themes">
            <CourseListView :courses="courses" :themes="themes"/>
            <ThemeListView />
        </div>
        <LessonEditorView />
    </div>
</template>
