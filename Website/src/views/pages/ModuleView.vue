<script setup lang="ts">
import type { Course, Module, Lesson } from '@/models/main'
import api from '@/services/api'
import { onMounted, ref, type Ref } from 'vue'
import { useRoute } from 'vue-router'
import { BCard, BCardText } from 'bootstrap-vue-next'
const module = ref<Module>()
const lessons = ref<Lesson[]>()
const courseId = useRoute().params.courseId
const moduleId = useRoute().params.moduleId
const getData = async () => {
    await api.get<Module>(`modules/${moduleId}`).then((res) => {
        module.value = res.data
    })
    await api.get<Lesson[]>(`modules/${moduleId}/lessons`).then((res) => {
        lessons.value = res.data
    })
}
const saveChanges = async () => {}
onMounted(async () => {
    await getData()
})
</script>

<template>
    <div class="container">
        <div v-if="module">
            <h1>{{ module.name }}</h1>
            <p v-if="module.description">{{ module.description }}</p>
            <p v-if="module.lessonsCount">{{ module.lessonsCount }} занятий</p>
            <RouterLink :to="`/courses/${module.courseId}`">Вернуться к списку модулей</RouterLink>
        </div>
        <h3>Занятия</h3>
        <div class="d-flex flex-wrap" v-if="lessons" >
            <BCard :title="lesson.name" class="m-2" style="max-width: 20rem;"
            :class="{ is_control: lesson.lessonTypeId == 2, is_study: lesson.lessonTypeId == 1 }"
            v-for="lesson in lessons" :style="{ order: lesson.order }">
                <BCardText>    
                    <p>{{ lesson.description }}</p>
                    <p><RouterLink :to="`/courses/modules/lessons/${lesson.id}`">Перейти к занятию</RouterLink></p>
                </BCardText>
            </BCard>
        </div>
    </div>
</template>

<style scoped>
.is_control {
    background-color: red;
}
.is_study {
    background-color: blue;
}
</style>
