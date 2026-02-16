<script lang="ts">
	import { Button } from '$lib/components/ui/button';
	import { ChevronLeft, ChevronRight } from '@lucide/svelte';

	interface Props {
		pageNumber: number;
		totalPages: number;
		hasPreviousPage: boolean;
		hasNextPage: boolean;
		onPageChange: (page: number) => void;
	}

	let { pageNumber, totalPages, hasPreviousPage, hasNextPage, onPageChange }: Props = $props();
</script>

{#if totalPages > 1}
	<div class="flex items-center justify-center gap-3">
		<Button
			variant="outline"
			size="icon"
			class="h-10 w-10"
			disabled={!hasPreviousPage}
			onclick={() => onPageChange(pageNumber - 1)}
		>
			<ChevronLeft class="h-4 w-4" />
		</Button>
		<span class="min-w-12 text-center text-sm text-muted-foreground tabular-nums">
			{pageNumber} / {totalPages}
		</span>
		<Button
			variant="outline"
			size="icon"
			class="h-10 w-10"
			disabled={!hasNextPage}
			onclick={() => onPageChange(pageNumber + 1)}
		>
			<ChevronRight class="h-4 w-4" />
		</Button>
	</div>
{/if}
