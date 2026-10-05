<script lang="ts" setup>
import type { Group } from '@/models/group';
import api from '@/services/api';
import { useUserStore } from '@/stores/user';
import { onMounted, ref, watch } from 'vue';


const currentTab = ref<'Past' | 'PresentAndFuture'>('PresentAndFuture')
const groups = ref<Group[]>()
const {user} = useUserStore()

const loadGroupsAsync = async () => {
    if (currentTab.value == 'PresentAndFuture') {
        await api.get<Group[]>("groups/user-in/" + user?.id).then(res => {
            groups.value = res.data
        })
    }
    else if (currentTab.value == 'Past') {
        await api.get<Group[]>("groups/user-in/" + user?.id + "/past").then(res => {
            groups.value = res.data
        })
    }
}


const selectedGroup = ref<Group>()


watch(() => currentTab.value,
    async (newVal, oldVal) => {
        if (newVal) {
           await loadGroupsAsync()
        }
    }
)

const selectGroup = async (id: number) => {
    console.log(JSON.stringify(groups.value))
    await api.get<Group>('groups/' + id).then(res => {
        selectedGroup.value = res.data
    })
}




onMounted(async () => {
    await loadGroupsAsync()
})
</script>
<template>
    <div class="my-3">
        <ul class="nav nav-tabs">
            <li class="nav-item mx-2" @click="currentTab = 'PresentAndFuture'">
                <a class="nav-link" :class="{'active': currentTab == 'PresentAndFuture'}" >
                    Текущие и будущие
                </a>
            </li>
            <li class="nav-item" @click="currentTab = 'Past'">
                <a class="nav-link" :class="{'active': currentTab == 'Past'}">
                    Прошлые
                </a>
            </li>
        </ul>
        <div class="d-flex flex-wrap my-3 gap-2">
            <div @click="selectGroup(group.id)" class="card p-2 card-clickable" v-for="group in groups">
                <div class="card-title">
                    <span>
                        {{ group.name }}
                    </span>
                    <br>
                    <span class="text-secondary small">
                        С {{ group.dateStart }} по {{ group.dateEnd }}
                    </span>
                </div>
            </div>
            <div v-if="groups?.length == 0">
                <p>Пока нет групп/потоков</p>
            </div>
        </div>

        <div v-if="selectedGroup">
            <hr>
            <h5>Группа {{ selectedGroup.name }}</h5>
            <p>
                Куратор: {{ selectedGroup.curatorName }}<br>
                Связь с куратором: {{ selectedGroup.curatorFeedback }}<br>
                Дата начала: {{ selectedGroup.dateStart }}<br>
                Дата окончания: {{ selectedGroup.dateEnd }}<br>
            </p>
            <p>Курсы, прикреплённые к группе:</p>

            <div class="d-flex flex-wrap gap-2">
                <div class="card p-2" v-for="course in selectedGroup.coursesInfo">
                    <h6 class="card-title">{{ course.name }}</h6>
                    <RouterLink class="stretched-link" :to="`/courses/${course.id}`"/>
                </div>
            </div>
        </div>
    </div>
</template>
