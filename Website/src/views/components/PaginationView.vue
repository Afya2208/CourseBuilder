<script lang="ts" setup>
import { computed, ref, toRefs } from 'vue'

const props = defineProps<{
	totalCount: number
	searchOptions: {
		pageNumber: number
		pageSize: number
	}
}>()

const { searchOptions, totalCount } = toRefs(props)

const totalPages = computed(() => {
	return Math.ceil(totalCount.value / searchOptions.value.pageSize)
})

const emits = defineEmits<{
	selectedPage: []
}>()

const scrollWindow = () => {
	window.scrollTo(1, 0)
}
const selectPreviousPage = async () => {
	searchOptions.value.pageNumber--
	emits('selectedPage')
}
const selectNextPage = async () => {
	searchOptions.value.pageNumber++
	emits('selectedPage')
}
const selectPage = async (pg: number) => {
	searchOptions.value.pageNumber = pg
	emits('selectedPage')
}

const pageNumbers = computed(() => {
	const maxVisible = 5

	if (totalPages.value <= maxVisible) {
		return Array.from({ length: totalPages.value }, (_, i) => i + 1)
	}

	let start = searchOptions.value.pageNumber - Math.floor(maxVisible / 2)
	let end = searchOptions.value.pageNumber + Math.floor(maxVisible / 2)
	if (start < 1) {
		start = 1
		end = maxVisible
	}

	if (end > totalPages.value) {
		end = totalPages.value
		start = totalPages.value - maxVisible + 1
	}

	return Array.from({ length: end - start + 1 }, (_, i) => start + i)
})
</script>

<template>
	<div>
		<div class="d-flex justify-content-center align-items-center">
			<nav aria-label="Page navigation example">
				<ul class="pagination">
					<li class="page-item">
						<a
							class="page-link"
							@click="selectPreviousPage"
							:class="{ 'a-disabled': searchOptions.pageNumber == 1 }"
							aria-label="Previous"
						>
							<span aria-hidden="true">&laquo;</span>
						</a>
					</li>
					<li class="page-item" v-for="pageNum in pageNumbers" :key="pageNum">
						<a
							class="page-link"
							:class="{ active: pageNum == searchOptions.pageNumber }"
							@click="selectPage(pageNum)"
							>{{ pageNum }}</a
						>
					</li>
					<li class="page-item">
						<a
							class="page-link"
							@click="selectNextPage"
							:class="{ 'a-disabled': searchOptions.pageNumber == totalPages }"
							aria-label="Next"
						>
							<span aria-hidden="true">&raquo;</span>
						</a>
					</li>
				</ul>
			</nav>
		</div>
	</div>
</template>

<style scoped>
.a-disabled {
	pointer-events: none;
}
</style>
