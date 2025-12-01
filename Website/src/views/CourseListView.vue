<script setup lang="ts">
import type { Course } from '@/models/main'
import api from '@/services/api'
import { useUserStore } from '@/stores/user'
import { storeToRefs } from 'pinia'
import { inject, ref } from 'vue'
import { RouterLink } from 'vue-router'

const courses = ref<Course[]>(inject<Course[]>('courses') ?? [])
const { user } = storeToRefs(useUserStore())
const roleId = user.value?.roleId ?? 0

const addCourseClick = async () => {
    const newCourseName = prompt('Напишите название нового курса', 'Новый курс')
    if (newCourseName) {
        const defaultNewCourse: Course = {
            id: 0,
            name: newCourseName,
            price: 0,
            authorId: user.value?.userId
        }
        await api.post<Course>('courses', defaultNewCourse).then((res) => {
            alert(`Новый курс ${res.data.name} успешно создан`)
        })
    }
    
}
const deleteCourseClick = async (courseId:number) => {
    await api.delete(`courses/${courseId}`).then(res=> {

    })
}
</script>

<template>
  <div v-if="roleId == 1">
    <span @click="addCourseClick"></span>
  </div>
  <ul>
    <li v-for="course in courses" :key="course.id">
        <p>
            <RouterLink :to="`courses/${course.id}`">{{ course.name }}</RouterLink>
            <span v-if="roleId == 1" @click="deleteCourseClick(course.id)">Удалить</span>
            <br/>
            {{ course.description }}
        </p>
        <p>
            Стоимость: {{ course.price }} руб.
        </p>
        <p>
            Темы курса:
            <br/>
            <ul>
                <li v-for="theme in course.themes">
                    <p>{{ theme.name }}</p>
                </li>
            </ul>
        </p>
    </li>
  </ul>
</template>
