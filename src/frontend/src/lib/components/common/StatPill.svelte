<script lang="ts">
	interface Props {
		label: string;
		description: string;
	}

	let { label, description }: Props = $props();
</script>

<div
	class="flex flex-col items-center rounded-xl border bg-card px-5 py-3 shadow-sm transition-shadow hover:shadow-md sm:min-w-[9rem]"
>
	<span class="text-sm font-bold">{label}</span>
	<span class="text-xs text-muted-foreground">{description}</span>
</div>
