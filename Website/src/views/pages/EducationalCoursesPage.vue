<script lang="ts" setup>
import type { Theme } from '@/models/main'
import api from '@/services/api'
import { onMounted, ref } from 'vue'
import ThemeListView from './ThemeListView.vue'
import CourseListView from './CourseListView.vue'
import { useLoad } from '@/services/useLoad'


const selectedThemeId = ref()
const themeCardClicked = (themeId: number) => {
    selectedThemeId.value = themeId;
}
const themes = ref<Theme[]>()
const loadError = ref(false)
const loadDataAsync = async () => {
    let u = useLoad<Theme[]>()
    await u.execute(api.get<Theme[]>("themes"))
    if (u.state.error) {
        loadError.value = true
    }
    else {
        themes.value = u.state.data
    }
}
onMounted(async () => {
    await loadDataAsync();
})
</script>

<template>
	<div class="container">
        <CourseListView :selected-theme-card-id="selectedThemeId" :themes="themes" />
		<ThemeListView ref="themeListView" :themes="themes" :load-error="loadError" @card-clicked="themeCardClicked"/>
	</div>
</template>
