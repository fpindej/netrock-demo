<script lang="ts">
	interface BarItem {
		label: string;
		percentage: number;
		displayValue: string;
		count: number;
		colorClass: string;
	}

	interface Props {
		items: BarItem[];
	}

	let { items }: Props = $props();
</script>

<div class="space-y-3">
	{#each items as item (item.label)}
		<div class="flex items-center gap-3">
			<span class="w-24 shrink-0 truncate text-sm">{item.label}</span>
			<div class="h-3 flex-1 overflow-hidden rounded-full bg-muted">
				<div class="h-full rounded-full {item.colorClass}" style="width: {item.percentage}%"></div>
			</div>
			<span class="w-20 text-end text-sm font-medium">{item.displayValue}</span>
			<span class="w-8 text-end text-sm text-muted-foreground">{item.count}</span>
		</div>
	{/each}
</div>
