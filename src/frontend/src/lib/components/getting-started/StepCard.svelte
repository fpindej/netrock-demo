<script lang="ts">
	import { reveal } from '$lib/actions/reveal';
	import type { Component, Snippet } from 'svelte';
	import type { IconProps } from '@lucide/svelte';

	interface Props {
		step: number;
		icon: Component<IconProps>;
		color: string;
		title: () => string;
		description: () => string;
		last?: boolean;
		children?: Snippet;
	}

	let { step, icon: Icon, color, title, description, last = false, children }: Props = $props();

	const colorMap: Record<string, { bg: string; text: string; border: string }> = {
		blue: { bg: 'bg-blue-500/10', text: 'text-blue-500', border: 'border-blue-500/30' },
		green: { bg: 'bg-green-500/10', text: 'text-green-500', border: 'border-green-500/30' },
		violet: { bg: 'bg-violet-500/10', text: 'text-violet-500', border: 'border-violet-500/30' },
		amber: { bg: 'bg-amber-500/10', text: 'text-amber-500', border: 'border-amber-500/30' },
		pink: { bg: 'bg-pink-500/10', text: 'text-pink-500', border: 'border-pink-500/30' }
	};

	const defaultColor = { bg: 'bg-blue-500/10', text: 'text-blue-500', border: 'border-blue-500/30' };

	let colors = $derived(colorMap[color] ?? defaultColor);
</script>

<div class="relative flex gap-4 sm:gap-6" use:reveal={(step - 1) * 120}>
	<!-- Timeline column -->
	<div class="flex flex-col items-center">
		<div
			class="flex h-10 w-10 shrink-0 items-center justify-center rounded-full border-2 text-sm font-bold {colors.border} {colors.bg} {colors.text}"
		>
			{step}
		</div>
		{#if !last}
			<div class="step-line mt-2 w-0.5 flex-1 {colors.bg}"></div>
		{/if}
	</div>

	<!-- Card content -->
	<div class="flex-1 pb-8">
		<div class="rounded-xl border bg-card p-4 shadow-sm transition-shadow hover:shadow-md sm:p-5">
			<div class="mb-3 flex items-center gap-3">
				<div class="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg {colors.bg}">
					<Icon class="h-5 w-5 {colors.text}" />
				</div>
				<h3 class="text-base font-semibold sm:text-lg">{title()}</h3>
			</div>
			<p class="text-sm text-muted-foreground">{description()}</p>
			{#if children}
				<div class="mt-4">
					{@render children()}
				</div>
			{/if}
		</div>
	</div>
</div>
