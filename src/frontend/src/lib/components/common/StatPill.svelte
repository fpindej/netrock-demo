<script lang="ts">
	interface Props {
		label: string;
		description: string;
	}

	let { label, description }: Props = $props();
</script>

<div
	class="flex min-w-[9rem] flex-col items-center rounded-xl border bg-card px-5 py-3 shadow-sm transition-shadow hover:shadow-md"
>
	<span class="text-sm font-bold">{label}</span>
	<span class="text-xs text-muted-foreground">{description}</span>
</div>
