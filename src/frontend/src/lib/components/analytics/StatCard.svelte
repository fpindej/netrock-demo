<script lang="ts">
	import type { Component } from 'svelte';
	import * as Card from '$lib/components/ui/card';

	interface Props {
		label: string;
		value: string;
		icon: Component<{ class?: string }>;
	}

	let { label, value, icon: Icon }: Props = $props();
</script>

<Card.Root>
	<Card.Content class="flex items-center gap-4 p-6">
		<div class="rounded-full bg-muted p-3">
			<Icon class="h-5 w-5 text-muted-foreground" />
		</div>
		<div>
			<p class="text-sm text-muted-foreground">{label}</p>
			<p class="text-2xl font-bold">{value}</p>
		</div>
	</Card.Content>
</Card.Root>
