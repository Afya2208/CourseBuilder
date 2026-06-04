<script lang="ts" setup>
import type { Group } from '@/models/group'
import api from '@/services/api'
import { useUserStore } from '@/stores/user'
import { onMounted, ref, watch } from 'vue'

const currentGroupId = ref<number>()
const groups = ref<Group[]>()
const selectedGroup = ref<Group>()

watch(currentGroupId, (newVal, oldVal) => {
	selectedGroup.value = groups.value?.find((x) => x.id == newVal)
})

const { user } = useUserStore()

const loadGroupsAsync = async () => {
	await api.get<Group[]>('groups/by-user/' + user?.id).then((res) => {
		groups.value = res.data
	})
}

onMounted(async () => {
	await loadGroupsAsync()
})
</script>

<template>
	<div>
		<h5>Общая статистика:</h5>
		<p>Всего групп/потоков: {{ groups?.length }}</p>
		<h5>
			Статистика по группе
			<select v-model="currentGroupId">
				<option v-for="group in groups" :value="group.id">
					<p>
						<span>{{ group.name }}</span>
						<br />
						<span class="text-secondary small"
							>С {{ group.dateStart }} по {{ group.dateEnd }}</span
						>
					</p>
				</option>
			</select>
		</h5>

		<div v-if="currentGroupId && selectedGroup">
			<p>Всего студентов: {{ selectedGroup.usersInfo?.length }}</p>
		</div>
	</div>
</template>
