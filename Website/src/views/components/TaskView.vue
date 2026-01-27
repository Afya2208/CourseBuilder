<script lang="ts" setup>
import type { Correlation, Task, TaskAnswer } from '@/models/main';
import api from '@/services/api';
import { indexToChar, mixCorrelations, randomInt } from '@/util/methods';
import { onMounted, ref } from 'vue';

const props = defineProps<{
    task:Task
}>() 
const theTask = ref(props.task)
const answer = ref<TaskAnswer>()
const answerOptions = ref<TaskAnswer[]>([])
const userAnswer = ref<TaskAnswer>({
    textValue: '',
    file: undefined,
    fileName: '',
    isRight: false,
    taskId: theTask.value.id
})
const userCorrelations = ref<Correlation[]>([])
const correlations = ref<Correlation[]>()
onMounted(async()=>{
    await loadData();
})
const loadData = async () => {
    // 1 - простая задача  ответ-готовый точный 1 ответ => проверка может быть на месте
    if (theTask.value.taskTypeId == 1) {
        await api.get<TaskAnswer[]>(`tasks/${theTask.value.id}/answers`).then(res=>{
            answer.value = res.data.pop()
        })
    }
    // 2 - задача с текстовым ответом, без автопроверки => при проверке сохранение введенного ответа
    else if (theTask.value.taskTypeId == 2) {
        
    }
    // 3 - задача соотношения
    else if (theTask.value.taskTypeId == 3) {
        await api.get<Correlation[]>(`tasks/${theTask.value.id}/correlations`).then(res=>{
            let correctList:Correlation[] = res.data;

            console.log("TEST")
            console.log(JSON.stringify(correctList))
            let mixed = mixCorrelations(correctList)
            console.log(JSON.stringify(mixed))
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
                    id: correlation.id
                })
            }
        })
    }
    // 4, 5 задача с 1/несколькими ответом из нескольких
    else if (theTask.value.taskTypeId == 4 || theTask.value.taskTypeId == 5) {
        await api.get<TaskAnswer[]>(`tasks/${theTask.value.id}/answers`).then(res=>{
            answerOptions.value = res.data
        })
    }
}
</script>
<template>
    <div>
        <div v-if="theTask.taskTypeId == 1 || theTask.taskTypeId == 2">
            <p>{{ theTask.question }}</p>
            <p>
                <label>
                    <span>Ответ:</span>
                    <input v-model="userAnswer.textValue"/>
                </label>
            </p>
        </div>
        <div v-else-if="theTask.taskTypeId == 3">
            <p>{{ theTask.question }}</p>
            <table>
                <tbody>
                    <tr v-for="correlation, index in correlations">
                        <td>
                            <p>{{ index+1 }})  {{ correlation.left }}</p>
                        </td>
                        <td>
                            <p>{{ indexToChar(index) }})  {{ correlation.right }}</p>
                        </td>
                    </tr>
                </tbody>
            </table>
            <table>
                <tbody>
                    <tr v-for="correlation in userCorrelations">
                        <td>
                            <input v-model="correlation.left"/>
                        </td>
                        <td>
                            <input v-model="correlation.right"/>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div v-else-if="theTask.taskTypeId == 4 || theTask.taskTypeId == 5">
            <p>{{ theTask.question }}</p>
            <select size="5" :multiple="theTask.taskTypeId == 5">
                <option v-for="answerOption in answerOptions" :value="answerOption.textValue">{{ answerOption.textValue }}</option>
            </select>
        </div>
    </div>
</template>

<style scoped>

</style>