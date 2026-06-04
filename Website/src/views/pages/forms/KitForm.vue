<script lang="ts" setup>
import type { Kit } from '@/models/kit.ts';
import type { Course } from '@/models/main.ts';
import api from '@/services/api.ts';
import { useUserStore } from '@/stores/user.ts';
import { Validator } from '@/util/validator.ts';
import { BForm, BFormFloatingLabel, BInput, BModal, useToggle, BFormSelect } from 'bootstrap-vue-next';
import { ref } from 'vue';


const { show: showModal, hide: hideModal } = useToggle('kit-modal')
const { user } = useUserStore()
const props = defineProps<{
    courses: Course[]
}>();

const getDefaultKit = () : Kit => {
    return {
        id: 0,
        name: '',
        description: '',
        authorId: user!!.id,
        price: 0,
        coursesInfo: [],
        selectedCoursesId: []
    }
}

const kit = ref<Kit>(getDefaultKit())

const startCreatingKit = () => {
    kit.value = getDefaultKit()
    showModal()
}

const startEditingKit = (k:Kit) => {
    kit.value = {
        id: k.id,
        name: k.name,
        description: k.description,
        authorId: k.authorId,
        price: k.price,
        coursesInfo: k.coursesInfo,
        selectedCoursesId: k.coursesInfo.map(x=>x.id)
    }
    showModal()
}

defineExpose({startCreatingKit, startEditingKit})

const emits = defineEmits<{
    closed: [],
    saved: [k:Kit]
}>()

const close = () => {
    emits('closed')
    hideModal()
}

const save = async () => {
    if (checkFields()) {
        kit.value.coursesInfo = []
        for (let id of kit.value.selectedCoursesId ?? []) {
            kit.value.coursesInfo.push({name:'', id:id})
        }
        if (kit.value.id == 0) {
            await api.post<Kit>('kits', kit.value)
            .then((res) => {
                alert('Набор успешно сохранен')
                emits('saved', res.data)
                hideModal();
            })
            .catch(() => {
                alert('Ошибка, повторите позже')
            })
        } else {
            await api.put<Kit>('kits', kit.value)
            .then((res) => {
                alert('Набор успешно сохранен')
                emits('saved', res.data)
                hideModal();
            })
            .catch(() => {
                alert('Ошибка, повторите позже')
            })
        }
    }
}


const checkFields = () : boolean => {
    const v = new Validator();
    v.validate(!kit.value.name, "Укажите название");
    v.validate(!kit.value.description, "Укажите описание");
    v.validate(!(kit.value.selectedCoursesId?.length ?? 0 > 1), "Укажите как минимум 2 курса в наборе");
    return v.returnResult();
}
</script>


<template>
    <BModal title="Новый набор курсов" id="kit-modal" no-close-on-backdrop centered
        @close="close" header-close-class="bg-white" header-class="bg-dark text-white">
        <BForm>
            <BFormFloatingLabel class="my-2" label="Название" label-for="kit-name">
                <BInput id="kit-name" type="text" v-model="kit.name"/>
            </BFormFloatingLabel>
            <BFormFloatingLabel class="my-2" label="Описание" label-for="kit-desc">
                <BInput id="kit-desc" type="text" v-model="kit.description"/>
            </BFormFloatingLabel>
            <BFormFloatingLabel class="my-2" label="Стоимость" label-for="kit-price">
                <BInput id="kit-price" type="number"  v-model="kit.price"/>
            </BFormFloatingLabel>
            <p>
                Курсы, включенные в набор:
                <BFormSelect
					:options="courses"
					select-size="7"
					v-model="kit.selectedCoursesId"
					value-field="id"
					text-field="name"
					multiple
                    id="courses-select"
				/>
            </p>
        </BForm>
        <template #footer>
            <button class="btn btn-primary" @click="save">Сохранить</button>
			<button class="btn btn-dark" @click="close">Отмена</button>
        </template>
    </BModal>
</template>
