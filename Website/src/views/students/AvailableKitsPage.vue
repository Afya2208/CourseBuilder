<script lang="ts" setup>
import type { Kit } from '@/models/kit';
import api from '@/services/api';
import { useUserStore } from '@/stores/user';
import { onMounted, ref } from 'vue';

const kits = ref<Kit[]>()
const {user} = useUserStore()
const loadKitsAsync = async () => {
    await api.get('kits/available-for/' + user?.id).then(res => {
        kits.value = res.data
    })
}
onMounted(async () => {
    await loadKitsAsync()
})
</script>

<template>
    <div>
        <div class="d-flex flex-wrap gap-2">
            <div v-for="(kit, index) in kits" class="card m-2" style="width: 300px;">
                <div class="card-body" >
                    <h5 class="card-title">
                        {{ kit.name }}
                    </h5>
                    <p class="card-text">{{ kit.description }}</p>
                    
                    <h6 class="card-subtitle my-2 text-muted">Курсы в наборе:</h6>
                    <ul>
                        <li v-for="course in kit.coursesInfo">
                            <RouterLink :to="`/courses/${course.id}`">{{ course.name }}</RouterLink>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
    </div>
</template>