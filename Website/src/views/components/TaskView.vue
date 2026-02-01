<script lang="ts" setup>
import type { Correlation, Task, TaskAnswer, UserAnswer } from '@/models/main'
import api from '@/services/api'
import { indexToChar, mixCorrelations, randomInt } from '@/util/methods'
import { onMounted, ref } from 'vue'

const props = defineProps<{
    task: Task
}>()
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
const userCorrelations = ref<Correlation[]>([])
const correlations = ref<Correlation[]>([])
onMounted(async () => {
    await loadData()
})
const checkTask = async () => {
    switch (theTask.value.taskTypeId) {
        case 1: {
            await api.get<TaskAnswer[]>(`tasks/${theTask.value.id}/answers`).then((res) => {
                let answer = res.data.pop()
                if (answer?.textValue === userAnswer.value.textValue) userAnswer.value.isRight = true
                else userAnswer.value.isRight = false
            })
            break
        }
        case 2: {
            await api.post('task-answers', userAnswer.value).then((res) => {
                // ааа
            })
            break
        }
        case 3: {
            let i = 0
            let areAllRight = true
            for (; i < correlations.value.length; i++) {
                let indexOfLeft = i
                let userInput = userCorrelations.value[i]
                if (userInput) {
                    let startCode = 'a'.charCodeAt(0)
                    let userChar = userInput.right
                    let index = userChar.charCodeAt(0) - startCode
                    areAllRight = indexOfLeft == index
                    if (!areAllRight) break
                } else {
                    areAllRight = false
                    break
                }
            }
            userAnswer.value.isRight = areAllRight
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
        // сохранение файла в базе
        case 6: {
            await api.post('task-answers', userAnswer.value).then((res) => {})
            break
        }
    }
}
defineExpose({ checkTask })
const loadData = async () => {
    // 1 - простая задача        ответ-готовый точный 1 ответ => проверка может быть на месте
    if (theTask.value.taskTypeId == 1) {
        await api.get<TaskAnswer[]>(`tasks/${theTask.value.id}/answers`).then((res) => {
            answer.value = res.data.pop()
        })
    }
    // 2 - задача с текстовым ответом, без автопроверки => при проверке сохранение введенного ответа
    else if (theTask.value.taskTypeId == 2) {
    }
    // 3 - задача соотношения
    else if (theTask.value.taskTypeId == 3) {
        await api.get<Correlation[]>(`tasks/${theTask.value.id}/correlations`).then((res) => {
            let correctList: Correlation[] = res.data
            /*
                                                let mixed: Correlation[] = []
                                                let rightNumbers: number[] = []
                                                while(rightNumbers.length != correctList.length) {
                                                                let newRandom = randomInt(0, correctList.length-1);
                                                                if (!rightNumbers.includes(newRandom)) {
                                                                                rightNumbers.push(newRandom)
                                                                }
                                                }
                                                let i = 0;
                                                for (let num of rightNumbers) {
                                                                let correlation = correctList[num];
                                                                let leftCorrelation = correctList[i];
                                                                if (correlation && leftCorrelation) {
                                                                                mixed.push({
                                                                                                left: leftCorrelation.left,
                                                                                                right: correlation.right,
                                                                                                id: 0
                                                                                })
                                                                                i++;
                                                                }
                                                }
                                                */
            correlations.value = correctList
            for (let correlation of correctList) {
                userCorrelations.value.push({
                    left: '',
                    right: '',
                    id: correlation.id,
                })
            }
        })
    }
    // 4, 5 задача с 1/несколькими ответом из нескольких
    else if (theTask.value.taskTypeId == 4 || theTask.value.taskTypeId == 5) {
        await api.get<TaskAnswer[]>(`tasks/${theTask.value.id}/answers`).then((res) => {
            answerOptions.value = res.data
        })
    }
}
</script>

<template>
    <div :class="{ is_right: userAnswer.isRight == true, is_false: userAnswer.isRight == false }">
        <div v-if="theTask.taskTypeId == 1 || theTask.taskTypeId == 2">
            <p>{{ theTask.question }}</p>
            <p>
                <label>
                    <span>Ответ:</span>
                    <input v-model="userAnswer.textValue" />
                </label>
            </p>
        </div>
        <div v-else-if="theTask.taskTypeId == 3">
            <p>{{ theTask.question }}</p>
            <table>
                <tbody>
                    <tr v-for="(correlation, index) in correlations">
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
                    <tr v-for="(correlation, index) in userCorrelations">
                        <td>
                            <p>{{ index + 1 }})</p>
                        </td>
                        <td>
                            <input v-model="correlation.right" />
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div v-else-if="theTask.taskTypeId == 4">
            <p>{{ theTask.question }}</p>
            <select v-model="userAnswer.selectedTaskAnswerId" size="5">
                <option v-for="answerOption in answerOptions" :value="answerOption.id">
                    {{ answerOption.textValue }}
                </option>
            </select>
        </div>
        <div v-else-if="theTask.taskTypeId == 5">
            <p>{{ theTask.question }}</p>
            <select v-model="userAnswer.selectedTaskAnswersId" size="5" multiple>
                <option v-for="answerOption in answerOptions" :value="answerOption.id">
                    {{ answerOption.textValue }}
                </option>
            </select>
        </div>
        <div v-else-if="theTask.taskTypeId == 6">
            <p>{{ theTask.question }}</p>
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
.is_right {
    background-color: green;
}
.is_false {
    background-color: red;
}
</style>
