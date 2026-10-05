<template>
	<div class="course-stats-chart">
		<h3 class="chart-title">Статистика по курсу</h3>

		<div class="chart-wrapper">
			<Bar :data="chartData" :options="chartOptions" />
		</div>

		<div class="kpi-row">
			<div class="kpi-card">
				<span class="kpi-label">Всего получили доступ</span>
				<span class="kpi-value">{{ totalUsers }}</span>
			</div>
			<div class="kpi-card success">
				<span class="kpi-label">Успешно завершили</span>
				<span class="kpi-value">{{ doneCount }} ({{ completionRate }}%)</span>
			</div>
		</div>
	</div>
</template>

<script setup lang="ts">
import { computed, defineProps } from 'vue'
import { Bar } from 'vue-chartjs'
import {
	Chart as ChartJS,
	Title,
	Tooltip,
	Legend,
	BarElement,
	CategoryScale,
	LinearScale,
} from 'chart.js'

// Регистрация плагинов Chart.js v4+
ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale)

const props = defineProps({
	individualCount: { type: Number, default: 0 },
	fromKitsCount: { type: Number, default: 0 },
	doneCount: { type: Number, default: 0 },
})

const totalUsers = computed(() => props.individualCount + props.fromKitsCount)

const completionRate = computed(() => {
	if (totalUsers.value === 0) return '0.0'
	return ((props.doneCount / totalUsers.value) * 100).toFixed(1)
})

const chartData = computed(() => ({
	labels: ['Пользователи курса'],
	datasets: [
		{
			label: 'Отдельно приобрели',
			data: [props.individualCount],
			backgroundColor: '#3b82f6',
			stack: 'acquired',
			borderRadius: 6,
		},
		{
			label: 'Приобрели из наборов',
			data: [props.fromKitsCount],
			backgroundColor: '#93c5fd',
			stack: 'acquired',
			borderRadius: { topLeft: 0, topRight: 0, bottomLeft: 6, bottomRight: 6 },
		},
		{
			label: 'Завершили курс',
			data: [props.doneCount],
			backgroundColor: 'rgba(34, 197, 94, 0.85)',
			borderColor: '#16a34a',
			borderWidth: 2,
			stack: 'completed',
			borderRadius: 6,
		},
	],
}))

const chartOptions = computed(() => ({
	responsive: true,
	maintainAspectRatio: false,
	interaction: { mode: 'nearest', intersect: false },
	plugins: {
		legend: { position: 'bottom', labels: { usePointStyle: true, padding: 16 } },
		tooltip: {
			backgroundColor: '#1f2937',
			padding: 12,
			cornerRadius: 8,
			callbacks: {
				label: (ctx) => `${ctx.dataset.label}: ${ctx.raw} чел.`,
			},
		},
	},
	scales: {
		x: { stacked: true, grid: { display: false } },
		y: {
			stacked: true,
			beginAtZero: true,
			grid: { color: '#f3f4f6' },
			ticks: { stepSize: 1 },
		},
	},
}))
</script>

<style scoped>
.course-stats-chart {
	background: #ffffff;
	border-radius: 16px;
	box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
	padding: 24px;
	max-width: 600px;
	margin: 0 auto;
}

.chart-title {
	margin: 0 0 16px;
	font-size: 18px;
	font-weight: 600;
	color: #111827;
}

.chart-wrapper {
	height: 280px;
	width: 100%;
}

.kpi-row {
	display: flex;
	gap: 12px;
	margin-top: 20px;
	justify-content: center;
	flex-wrap: wrap;
}

.kpi-card {
	flex: 1;
	min-width: 180px;
	display: flex;
	flex-direction: column;
	align-items: center;
	padding: 14px;
	background: #f9fafb;
	border-radius: 12px;
	border: 1px solid #e5e7eb;
}

.kpi-card.success {
	background: #f0fdf4;
	border-color: #bbf7d0;
}

.kpi-label {
	font-size: 13px;
	color: #6b7280;
	margin-bottom: 4px;
}

.kpi-value {
	font-size: 22px;
	font-weight: 700;
	color: #111827;
}

.kpi-card.success .kpi-value {
	color: #166534;
}
</style>
