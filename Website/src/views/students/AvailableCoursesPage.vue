<script lang="ts" setup>
import type { Course, Theme } from '@/models/main'
import api from '@/services/api'
import { useUserStore } from '@/stores/user'
import { onMounted, ref } from 'vue'
import type { ComponentExposed } from 'vue-component-type-helpers'

const courses = ref<Course[]>()
const themes = ref<Theme[]>()
const {user} = useUserStore()
onMounted(async () => {
    await getDataAsync()
})
const getDataAsync= async () => { 
    await api.get<Course[]>(`courses/available-for/${user?.id}`)
    .then(res => {
        courses.value = res.data
    })
    await api.get<Theme[]>(`themes`)
    .then(res => {
        themes.value = res.data
    })
}
</script>

<template>
    <div>
        <div class="d-flex flex-wrap gap-2">
            <div v-for="(course, index) in courses" class="card m-2" style="width: 300px;">
                <div class="card-body" >
                    <h5 class="card-title">
                        {{ course.name }}
                    </h5>
                    <p class="card-text">{{ course.description }}</p>
                    <p class="card-text">
                        Количество модулей: {{ course.modulesCount }}
                        <br />
                        Количество занятий: {{ course.lessonsCount }}
                    </p>
                    <RouterLink class=" stretched-link" :to="`courses/${course.id}`"></RouterLink>
                    <h6 class="card-subtitle my-2 text-muted">Темы курса:</h6>
                    <ul>
                        <li v-for="theme in course.themes">
                            <p>{{ theme.name }}</p>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
    </div>
</template>