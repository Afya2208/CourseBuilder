<script setup lang="ts">
import type { Lesson, LessonContent, Task, ContentBlock } from '@/models/main'
import api from '@/services/api'
import { onMounted, ref, type Ref } from 'vue'
import { useRoute } from 'vue-router'
import ContentBlockView from '../components/ContentBlockView.vue'

const lesson = ref<Lesson>()
const contentBlocks = ref<ContentBlock[]>()
const tasks = ref<Task[]>()
const lessonContent = ref<LessonContent[]>()

const lessonId = useRoute().params.lessonId

const getData = async () => {
  await api.get<Lesson>(`lessons/${lessonId}`).then((res) => {
    lesson.value = res.data
  })
  await api.get<Task[]>(`lessons/${lessonId}/tasks`).then(res=>{
    tasks.value = res.data
  })
  await api.get<ContentBlock[]>(`lessons/${lessonId}/content-blocks`).then(res=>{
    contentBlocks.value = res.data
  })
  if (tasks.value && contentBlocks.value) {
    let lessonContentTasksAndBlocks: LessonContent[] = [...tasks.value, ...contentBlocks.value]
    lessonContent.value = lessonContentTasksAndBlocks.sort((a, b) => a.order - b.order)
  }
}

onMounted(async () => {
    await getData()
})
</script>

<template>
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
  </div>
  <div class="lesson-content-div" v-if="lessonContent">
    <h2>Содержание занятия</h2>
    <ContentBlockView v-for="contentBlock in contentBlocks" :content="contentBlock"/>
  </div>
</template>

<style scoped>
 
</style>