<script setup lang="ts">
import type { Course, Module, Lesson } from '@/models/main'
import api from '@/services/api'
import { onMounted, ref, type Ref } from 'vue'
import { useRoute } from 'vue-router'

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
    <div class="course-main-info-div" v-if="module">
        <h1>{{ module.name }}</h1>
        <label>
            <span>Название:</span>
            <input v-model="module.name" />
        </label>
        <label>
            <span>Описание:</span>
            <input v-model="module.description" />
        </label>
        <RouterLink :to="`/courses/${module.courseId}`">Вернуться к списку модулей</RouterLink>
    </div>
    <div class="module-lessons-div" v-if="lessons">
        <h3>Занятия</h3>
        <div class="block-list-hor">
            <div v-for="lesson in lessons"
                :class="{ is_control: lesson.lessonTypeId == 2, is_study: lesson.lessonTypeId == 1 }"
                :style="{ order: lesson.order }">
                <p>
                    <RouterLink :to="`/courses/modules/lessons/${lesson.id}`">{{ lesson.name }}</RouterLink>
                </p>
                <p>
                    {{ lesson.description }}
                </p>
            </div>
        </div>
    </div>
    <div>
        <button @click="saveChanges">Сохранить</button>
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
