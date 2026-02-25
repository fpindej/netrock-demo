<script lang="ts">
	interface Props {
		label: () => string;
	}

	let { label }: Props = $props();

	let checked = $state(false);
</script>

<label
	class="flex cursor-pointer items-center gap-3 rounded-lg p-2 transition-colors hover:bg-muted/50"
>
	<input type="checkbox" bind:checked class="h-4 w-4 rounded border-border accent-primary" />
	<span
		class="text-sm transition-all"
		class:text-muted-foreground={checked}
		class:line-through={checked}
		class:text-foreground={!checked}
	>
		{label()}
	</span>
</label>
