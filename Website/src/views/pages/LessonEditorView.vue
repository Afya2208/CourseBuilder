<script setup lang="ts">
import { ref } from 'vue';

const items = ref([])
const createItem = () => {
    items.value.push("item-" + items.value.length+1)
}
</script>
<template>
    <div>
        <div>
            <button @click="createItem">Добавить блок содержания</button>
        </div>
        <div>
            <div class="item" v-for="item in items">
                <p>{{ item }}</p>
            </div>
        </div>
    </div>
</template>
<style scoped>
    .item {
        margin: 15px;
        background-color: green;
        border: solid slateblue 2px;
    }
</style>