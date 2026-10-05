<script lang="ts" setup>
import type { Module } from '@/models/main.ts'
import api from '@/services/api.ts'
import { Validator } from '@/util/validator.ts';
import {
    BForm, useToggle, BFormFloatingLabel, BFormInput,
    BModal, BButton,
    BFormCheckbox, BFormTextarea
} from 'bootstrap-vue-next';
import { ref } from 'vue'

const props = defineProps<{
    courseId: number,
    modulesHaveOrder: boolean,
    modulesCount: number
}>()
const emits = defineEmits<{
    saved: [m:Module],
    closed: []
}>()

const startEditingModule = (l: Module) => {
	module.value = {
		id: l.id,
		name: l.name,
        description: l.description,
		courseId: props.courseId,
		order: l.order,
	}
	showModal()
}
const getNewModule = (): Module => {
	return {
		id: 0,
        name: '',
		description: '',
		courseId: props.courseId,
		order: props.modulesCount + 1,
	}
}

const { hide: closeModal, show: showModal } = useToggle('module-modal')
const module = ref<Module>(getNewModule())

const startCreatingModule = () => {
    module.value = getNewModule();
	showModal()
}
const close = () => {
    closeModal();
    emits('closed')
}

const checkFields = () : boolean => {
    const v = new Validator();
    v.validate(!module.value.name, "Укажите название");
    v.validate(!module.value.description, "Укажите описание");
    return v.returnResult();
}
const saveModule = async () => {
    if (checkFields()) {
        module.value.order = Math.trunc(module.value.order);
        if (module.value.id == 0) {
            await api.post<Module>('modules', module.value)
            .then((res) => {
                alert('Модуль успешно сохранен')
                emits('saved', res.data)
                closeModal();
            })
            .catch(() => {
                alert('Ошибка, повторите позже')
            })
        } else {
            await api.put<Module>('modules', module.value)
            .then((res) => {
                alert('Модуль успешно сохранен')
                emits('saved', res.data)
                closeModal();
            })
            .catch(() => {
                alert('Ошибка, повторите позже')
            })
        }
    }
}
defineExpose({startCreatingModule, startEditingModule})
</script>
<template>
    <BModal
		id="module-modal"
		centered
		header-class="bg-dark text-white"
		header-close-class="bg-white"
		@close="close"
		title="Новый модуль"
		no-close-on-backdrop>

		<BForm>
			<BFormFloatingLabel class="my-2" label="Название" label-for="module-name">
				<BFormInput
					id="module-name"
					v-model="module.name"
					type="text"
					placeholder="Новый модуль"
				/>
			</BFormFloatingLabel>

			<BFormFloatingLabel class="my-2" label="Описание" label-for="module-desc">
                <BFormTextarea id="module-desc"
                               v-model="module.description"
                               type="text"
                               placeholder="Описание..." />
			</BFormFloatingLabel>

            <BFormCheckbox id="module-lessons-have-order" v-model="module.lessonsHaveOrder">
				У занятий есть порядок
			</BFormCheckbox>

		</BForm>
		<template #footer>
			<BButton variant="primary" @click="saveModule()">Сохранить</BButton>
			<button class="btn btn-dark" @click="close">Отмена</button>
		</template>
	</BModal>
</template>
