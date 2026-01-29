<script setup lang="ts">
import type { Theme } from '@/models/main';
import api from '@/services/api';
import { ref } from 'vue';

const theme = ref<Theme>({
    id: 0,
    name: ''
})
const saveTheme = async () => {
    await api.post("")
    .then()
}
</script>

<template>
    <div class="modal" id="addThemeForm" tabindex="-1">
        <div class="modal-dialog">
            <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Создание новой темы</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <form>
                    <div class="form-floating">
                        <input id="themeName" class="form-control" v-model="theme.name" type="text"/>
                        <label for="themeName"></label>
                    </div>
                </form>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Отмена</button>
                <button type="button" class="btn btn-primary" @click="">Сохранить</button>
            </div>
            </div>
        </div>
    </div>
</template>

<style scoped>
</style>