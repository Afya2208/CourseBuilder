<script lang="ts" setup>
import type { Course, Theme } from '@/models/main';
import axios from 'axios';
import { onMounted, ref } from 'vue';


let courses = ref<Course[]>([])
let themes = ref<Theme[]>([])
let getData = async () => {
    await axios.get<Theme[]>("http://localhost:5555/themes").then(res => {
        themes.value = res.data
    })
    await axios.get<Course[]>("http://localhost:5555/courses").then(res => {
        courses.value = res.data
    })
}
onMounted(async ()=>{
    await getData()
})
</script>
<template>
  <div>
    <h1>Главная</h1>
    <h3>Курсы</h3>
    <ul>
        <li v-for="course in courses" v-bind:key="course.id">
            <div>
                <p>
                    {{ course.name }}
                    <br/>
                    {{ course.description }}
                </p>
                <p>
                    Стоимость: {{ course.price }}
                </p>
            </div>
        </li>
    </ul>
    <h3>Темы</h3>
    <ul>
        <li v-for="theme in themes" v-bind:key="theme.id">
            <div>
                <p>
                    {{ theme.name }}
                </p>
            </div>
        </li>
    </ul>
  </div>
</template>

<style></style>
