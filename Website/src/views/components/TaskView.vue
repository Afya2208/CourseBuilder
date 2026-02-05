<script lang="ts" setup>
import type { Correlation, Task, TaskAnswer, UserAnswer } from '@/models/main'
import api from '@/services/api'
import { useUserStore } from '@/stores/user';
import { indexToChar, randomInt } from '@/util/methods'
import { onMounted, ref } from 'vue'

const props = defineProps<{
    task: Task,
    isEditing: boolean,
    index:number
}>()
const userStore = useUserStore()
const user = ref (userStore.user)
const theTask = ref(props.task)
const answer = ref<TaskAnswer>()
const answerOptions = ref<TaskAnswer[]>([])
const userAnswer = ref<UserAnswer>({
	textValue: '',
	file: undefined,
	fileName: '',
	isRight: undefined,
	taskId: theTask.value.id,
	selectedTaskAnswersId: [],
	selectedTaskAnswerId: 0,
})
const userIndexCorrelations = ref<string[]>([])
const correlationOptionsIndexes = ref<string[]>([])
const originalCorrelations = ref<Correlation[]>([])
const mixedCorrelations = ref<Correlation[]>([])
onMounted(async () => {
    await loadData()
})
const checkTask = async () => {
	switch (theTask.value.taskTypeId) {
		case 1: {
			await api.get<TaskAnswer[]>(`tasks/${theTask.value.id}/answers`).then((res) => {
				let answer = res.data.pop()
				if (answer?.textValue === userAnswer.value.textValue)
					userAnswer.value.isRight = true
				else userAnswer.value.isRight = false
			})
			break
		}
        case 2: {
            if (user.value) {
                await api.post(`tasks/${theTask.value.id}/save-answer`, {...userAnswer.value, userId:user.value?.id}).then((res) => {
                })
            }
            break
		}
        case 3: {
            let isRight = true
            for (let i = 0; i < mixedCorrelations.value.length; i++) {
                let userPut = userIndexCorrelations.value[i] // например, b) = index = 1
                if (userPut) {
                    let index0 = 'a'.charCodeAt(0)
                    let userIndex = userPut.charCodeAt(0) - index0 // b - a == 1
                    let selectedId = mixedCorrelations.value[userIndex].rightId // по индексу ищем ответ
                    let leftId = mixedCorrelations.value[i]?.id
                    if (selectedId != leftId) {
                        isRight = false;
                        break
                    }
                }
                else {
                    isRight = false;
                    break
                }
            }
			userAnswer.value.isRight = isRight
			break
		}
		case 4: {
			let selectedAnswer = answerOptions.value.find(
				(x) => x.id == userAnswer.value.selectedTaskAnswerId,
			)
			if (selectedAnswer) {
				userAnswer.value.isRight = selectedAnswer.isRight
			} else {
				userAnswer.value.isRight = false
			}
			break
		}
		case 5: {
            let areAllRight = true
			for (let answer of answerOptions.value) {
				let didUserSelectIt = userAnswer.value.selectedTaskAnswersId.includes(answer.id)
				if ((didUserSelectIt && !answer.isRight) || (answer.isRight && !didUserSelectIt)) {
					areAllRight = false
					break
				}
			}
			userAnswer.value.isRight = areAllRight
			break
		}
		case 6: {
            if (user.value) {
                
                await api.post(`tasks/${theTask.value.id}/save-answer`, {...userAnswer.value, userId:user.value?.id}).then((res) => {
                })
            }
			break
		}
	}
}
const update = async (task:Task) => {
    theTask.value = task;
    await loadData();
}
defineExpose({ checkTask, update })

const emit = defineEmits(['deleteClick', 'startEdit'])
const deleteClick = (task:Task) => {
    emit('deleteClick', task)
}
const loadData = async () => {
    if (theTask.value.id != 0) {
        if (theTask.value.taskTypeId == 1) {
            await api.get<TaskAnswer[]>(`tasks/${theTask.value.id}/answers`).then((res) => {
                answer.value = res.data.pop()
            })
        }
        else if (theTask.value.taskTypeId == 3) {
            await api.get<Correlation[]>(`tasks/${theTask.value.id}/correlations`).then((res) => {
                originalCorrelations.value = res.data
                correlationOptionsIndexes.value = []
                for (let i = 0; i < originalCorrelations.value.length; i++) {
                    originalCorrelations.value[i].rightId = originalCorrelations.value[i].id
                    correlationOptionsIndexes.value?.push(indexToChar(i) + ")")
                }
                mixCorrelations()
            })
        }
        else if (theTask.value.taskTypeId == 4 || theTask.value.taskTypeId == 5) {
            await api.get<TaskAnswer[]>(`tasks/${theTask.value.id}/answers`).then((res) => {
                answerOptions.value = res.data
            })
        }
    }
}

const mixCorrelations = () => {
    const rights = originalCorrelations.value.map(c => ({
        ...c
    }))
    for (let i = rights.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        [rights[i], rights[j]] = [rights[j], rights[i]];
    }
    for (let i = 0; i < rights.length; i++) {
        rights[i].left = originalCorrelations.value[i].left
        rights[i].id = originalCorrelations.value[i].id
    }
    mixedCorrelations.value = rights
}
const startEditingTask = (task:Task) => {
    emit('startEdit', task)
}
</script>

<template>
	<div>
        <div v-if="props.isEditing" class="control_div">
            <span class="m-1 text-muted">Порядок: {{ theTask.order }}</span>
            <button class="btn btn-outline-danger m-1" @click="deleteClick(task)">❌</button>
            <button class="btn btn-outline-warning m-1" @click="startEditingTask(task)">✏️</button>
	    </div>
        <h5 :class="{'question-p-correct': userAnswer.isRight == true, 'question-p-bad': userAnswer.isRight == false
            }">Задание {{ props.index + 1 }}: {{ theTask.question }}</h5>
		<div v-if="theTask.taskTypeId == 1 || theTask.taskTypeId == 2">
			<p>
				<label>
					<span>Ответ:</span>
					<input v-model="userAnswer.textValue" />
				</label>
			</p>
		</div>
		<div v-else-if="theTask.taskTypeId == 3">
			<table >
				<tbody>
					<tr v-for="(correlation, index) in mixedCorrelations">
						<td>
							<p>{{ index + 1 }}) {{ correlation.left }}</p>
						</td>
						<td>
							<p>{{ indexToChar(index) }}) {{ correlation.right }}</p>
						</td>
					</tr>
				</tbody>
			</table>
			<table>
				<tbody>
					<tr v-for="(correlation, index) in mixedCorrelations">
						<td>
							<p>{{ index + 1 }})</p>
						</td>
						<td>
                            <select class="form-control-sm" v-model="userIndexCorrelations[index]">
                                <option :value="opt" v-for="opt in correlationOptionsIndexes">{{ opt }}</option>
                            </select>
						</td>
					</tr>
				</tbody>
			</table>
		</div>
		<div v-else-if="theTask.taskTypeId == 4">
			<select v-model="userAnswer.selectedTaskAnswerId">
				<option v-for="answerOption in answerOptions" :value="answerOption.id">
					{{ answerOption.textValue }}
				</option>
			</select>
		</div>
		<div v-else-if="theTask.taskTypeId == 5">
			<select v-model="userAnswer.selectedTaskAnswersId" size="5" multiple>
				<option v-for="answerOption in answerOptions" :value="answerOption.id">
					{{ answerOption.textValue }}
				</option>
			</select>
		</div>
		<div v-else-if="theTask.taskTypeId == 6">
			<p>
				<label>
					<span>Прикрепите файл:</span>
					<input type="file" />
				</label>
			</p>
		</div>
	</div>
</template>

<style scoped>
.question-p-correct::before {
    content: '✔️';
}
.question-p-bad::before {
    content: '❌';
}
td {
    padding: 5px;
}
</style>
