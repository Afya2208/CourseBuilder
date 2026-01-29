<script lang="ts" setup>
import type { Course, Theme } from '@/models/main'
import api from '@/services/api'
import { onMounted, provide, ref } from 'vue'
import ThemeListView from '../components/ThemeListView.vue'
import CourseListView from '../components/CourseListView.vue'
import LessonEditorView from './LessonEditorView.vue'
import ThemeFormView from '../components/ThemeFormView.vue'

const courses = ref<Course[]>()
const themes = ref<Theme[]>()
provide('themes', themes)
provide('courses', courses)
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
    <h1>Главная</h1>
    <button data-bs-toggle="modal" data-bs-target="#addThemeForm">
        Добавить тему
    </button>
    <ThemeFormView id="addThemeForm"/>
    <b-modal></b-modal>
    <p v-if="!courses || !themes">Загрузка...</p>
    <div v-if="courses && themes">
      <h2>Темы</h2>
      <ThemeListView />
      <h2>Курсы</h2>
      <CourseListView />
    </div>
    <LessonEditorView />
  </div>
</template>
