<script setup lang="ts">
import type { Course, Module } from '@/models/main'
import api from '@/services/api'
import { onMounted, ref, type Ref } from 'vue'
import { useRoute } from 'vue-router'

const course = ref<Course>()
const modules = ref<Module[]>()
const courseId = useRoute().params.courseId
const getData = async () => {
  await api.get<Course>(`courses/${courseId}`).then((res) => {
    course.value = res.data
  })
  await api.get<Module[]>(`courses/${courseId}/modules`).then(res=>{
        modules.value = res.data
  })
}
const saveChanges = async () => {}
onMounted(async () => {
    await getData()
})
</script>

<template>
  <div class="course-main-info-div" v-if="course">
    <h1>{{ course.name }}</h1>
    <label>
      <span>Название:</span>
      <input v-model="course.name" />
    </label>
    <label>
      <span>Стоимость:</span>
      <input v-model="course.price" type="number" />
    </label>
    <label>
      <span>Описание:</span>
      <input v-model="course.description" />
    </label>
  </div>
  <div v-if="course" class="course-themes-div">
    <h3>Темы</h3>
    <span>+</span>
    <ul>
        <li v-for="theme in course.themes">
            <p>{{ theme.name }}</p>
        </li>
    </ul>
  </div>
  <div class="course-modules-div" v-if="modules">
    <h3>Модули</h3>
    <div class="block-list-hor">
      <div v-for="module in modules" :style="{order: module.order}">
        <p>
          <RouterLink :to="`/courses/modules/${module.id}`">{{ module.name }}</RouterLink>
          <span class="li-block-title"></span>
          <br/>
          {{ module.description }}
          <br/>
          Количество занятий: {{ module.lessonsCount }}
        </p>
      </div>
    </div>
  </div>
  <div>
    <button @click="saveChanges">Сохранить</button>
  </div>
</template>

<style scoped >
    
</style>