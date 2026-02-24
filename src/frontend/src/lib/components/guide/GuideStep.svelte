<script lang="ts">
	import type { Component } from 'svelte';
	import type { IconProps } from '@lucide/svelte';
	import * as Card from '$lib/components/ui/card';
	import { Button } from '$lib/components/ui/button';

	interface Props {
		step: number;
		icon: Component<IconProps>;
		title: string;
		description: string;
		action?: { label: string; href: string };
	}

	let { step, icon: Icon, title, description, action }: Props = $props();
</script>

<Card.Root>
	<Card.Content class="flex items-start gap-4 p-4 sm:p-6">
		<div
			class="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-primary text-sm font-semibold text-primary-foreground"
		>
			{step}
		</div>
		<div class="flex min-w-0 flex-1 flex-col gap-3">
			<div class="flex items-center gap-2">
				<Icon class="h-5 w-5 shrink-0 text-muted-foreground" />
				<h4 class="text-sm font-medium">{title}</h4>
			</div>
			<p class="text-sm text-muted-foreground">{description}</p>
			{#if action}
				<div>
					<Button variant="outline" size="sm" href={action.href} class="w-full sm:w-auto">
						{action.label}
					</Button>
				</div>
			{/if}
		</div>
	</Card.Content>
</Card.Root>
