<script setup lang="ts">
import type { Course, Module } from '@/models/main'
import api from '@/services/api'
import { BCard, BCardText } from 'bootstrap-vue-next'
import { onMounted, ref, type Ref } from 'vue'
import { useRoute } from 'vue-router'

const course = ref<Course>()
const modules = ref<Module[]>()
const courseId = useRoute().params.courseId
const getData = async () => {
    await api.get<Course>(`courses/${courseId}`).then((res) => {
        course.value = res.data
    })
    await api.get<Module[]>(`courses/${courseId}/modules`).then((res) => {
        modules.value = res.data
    })
}
const saveChanges = async () => {}
onMounted(async () => {
    await getData()
})
</script>

<template>
    <div class="container">
        <div v-if="course">
            <h1>{{ course.name }}</h1>
            <p v-if="course.description">{{ course.description }}</p>
            <p v-if="course.price && course.price > 0">{{ course.price }} руб.</p>
        </div>
        <h3>Темы</h3>
        <div v-if="course" class="d-flex flex-wrap">
            <ul>
                <li v-for="theme in course.themes">
                    <p>{{ theme.name }}</p>
                </li>
            </ul>
        </div>
        <h3>Модули</h3>
        <div class="d-flex flex-wrap" v-if="modules" >
            <BCard :title="module.name" class="m-2" style="max-width: 20rem;"
            v-for="module in modules" :style="{ order: module.order }">
                <BCardText>    
                    <p>{{ module.description }}</p>
                    <p>Количество занятий: {{ module.lessonsCount }}</p>
                    <p><RouterLink :to="`/courses/modules/${module.id}`">Перейти к занятиям</RouterLink></p>
                </BCardText>
            </BCard>
        </div>
    </div>
    
</template>

<style scoped>

</style>
