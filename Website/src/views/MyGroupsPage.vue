<script lang="ts" setup>
import { useUserStore } from '@/stores/user'
import CreatorGroupsPage from './developers/CreatorGroupsPage.vue'
import StudentGroupsPage from './students/StudentGroupsPage.vue'

const { user } = useUserStore()
</script>
<template>
	<div class="container">
		<h3 class="my-3">Мои учебные группы</h3>

		<CreatorGroupsPage v-if="user?.role.id == 1" />
		<StudentGroupsPage v-else-if="user?.role.id == 3" />
	</div>
</template>
