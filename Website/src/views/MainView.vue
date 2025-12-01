<script lang="ts" setup>
import type { Course, Theme } from '@/models/main'
import api from '@/services/api'
import { onMounted, provide, ref } from 'vue'
import ThemeListView from './ThemeListView.vue'
import CourseListView from './CourseListView.vue'

const getData = async () => {
  await api.get<Theme[]>('themes').then((res) => {
    provide('themes', res.data)
  })
  await api.get<Course[]>('courses').then((res) => {
    provide('courses', res.data)
  })
}
onMounted(async () => {
  await getData()
})
</script>

<template>
  <div>
    <h1>Главная</h1>
    <h2>Курсы</h2>
    <CourseListView/>
    <h2>Темы</h2>
    <ThemeListView/>
  </div>
</template>

