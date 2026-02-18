<script lang="ts">
	import * as Card from '$lib/components/ui/card';
	import { Badge } from '$lib/components/ui/badge';
	import { StickyNote, Pin, Tag, BarChart3 } from '@lucide/svelte';
	import * as m from '$lib/paraglide/messages';
	import type { PageData } from './$types';

	let { data }: { data: PageData } = $props();

	const categoryLabels: Record<string, () => string> = {
		Personal: m.notes_category_Personal,
		Work: m.notes_category_Work,
		Ideas: m.notes_category_Ideas
	};

	const categoryColors: Record<string, string> = {
		Personal: 'bg-blue-500',
		Work: 'bg-purple-500',
		Ideas: 'bg-amber-500'
	};

	let topCategory = $derived(() => {
		if (!data.stats?.byCategory) return null;
		const entries = Object.entries(data.stats.byCategory);
		if (entries.length === 0) return null;
		const sorted = entries.sort(([, a], [, b]) => b - a);
		return sorted[0][1] > 0 ? sorted[0][0] : null;
	});

	let maxCategoryCount = $derived(() => {
		if (!data.stats?.byCategory) return 1;
		return Math.max(1, ...Object.values(data.stats.byCategory));
	});

	function formatDate(dateStr: string) {
		return new Date(dateStr).toLocaleDateString(undefined, {
			month: 'short',
			day: 'numeric',
			year: 'numeric'
		});
	}
</script>

<svelte:head>
	<title>{m.meta_titleTemplate({ title: m.meta_analytics_title() })}</title>
	<meta name="description" content={m.meta_analytics_description()} />
</svelte:head>

<div class="space-y-6">
	<div>
		<h3 class="text-lg font-medium">{m.analytics_title()}</h3>
		<p class="text-sm text-muted-foreground">{m.analytics_description()}</p>
	</div>
	<div class="h-px w-full bg-border"></div>

	{#if !data.stats || data.stats.totalCount === 0}
		<div class="flex flex-col items-center justify-center py-16 text-center">
			<div class="mb-4 rounded-full bg-muted p-4">
				<BarChart3 class="h-8 w-8 text-muted-foreground" />
			</div>
			<h3 class="mb-1 text-lg font-semibold">{m.analytics_empty_title()}</h3>
			<p class="max-w-sm text-sm text-muted-foreground">
				{m.analytics_empty_description()}
			</p>
		</div>
	{:else}
		<!-- Summary cards -->
		<div class="grid gap-4 sm:grid-cols-3">
			<Card.Root>
				<Card.Content class="flex items-center gap-4 p-6">
					<div class="rounded-lg bg-primary/10 p-3">
						<StickyNote class="h-5 w-5 text-primary" />
					</div>
					<div>
						<p class="text-sm text-muted-foreground">{m.analytics_totalNotes()}</p>
						<p class="text-2xl font-bold">{data.stats.totalCount}</p>
					</div>
				</Card.Content>
			</Card.Root>

			<Card.Root>
				<Card.Content class="flex items-center gap-4 p-6">
					<div class="rounded-lg bg-primary/10 p-3">
						<Pin class="h-5 w-5 text-primary" />
					</div>
					<div>
						<p class="text-sm text-muted-foreground">{m.analytics_pinnedNotes()}</p>
						<p class="text-2xl font-bold">{data.stats.pinnedCount}</p>
					</div>
				</Card.Content>
			</Card.Root>

			<Card.Root>
				<Card.Content class="flex items-center gap-4 p-6">
					<div class="rounded-lg bg-primary/10 p-3">
						<Tag class="h-5 w-5 text-primary" />
					</div>
					<div>
						<p class="text-sm text-muted-foreground">{m.analytics_topCategory()}</p>
						<p class="text-2xl font-bold">
							{#if topCategory()}
								{categoryLabels[topCategory()!]?.() ?? topCategory()}
							{:else}
								{m.analytics_noCategory()}
							{/if}
						</p>
					</div>
				</Card.Content>
			</Card.Root>
		</div>

		<!-- Category breakdown -->
		<Card.Root>
			<Card.Header>
				<Card.Title>{m.analytics_categoryBreakdown()}</Card.Title>
			</Card.Header>
			<Card.Content>
				<div class="space-y-4">
					{#each Object.entries(data.stats.byCategory ?? {}) as [cat, count] (cat)}
						<div class="space-y-1.5">
							<div class="flex items-center justify-between text-sm">
								<span class="font-medium">{categoryLabels[cat]?.() ?? cat}</span>
								<span class="text-muted-foreground">{count}</span>
							</div>
							<div class="h-2.5 overflow-hidden rounded-full bg-muted">
								<div
									class="h-full rounded-full transition-all duration-500 {categoryColors[cat] ??
										'bg-primary'}"
									style="width: {maxCategoryCount() > 0 ? (count / maxCategoryCount()) * 100 : 0}%"
								></div>
							</div>
						</div>
					{/each}
				</div>
			</Card.Content>
		</Card.Root>

		<!-- Recent notes -->
		{#if data.stats.recentNotes && data.stats.recentNotes.length > 0}
			<Card.Root>
				<Card.Header>
					<Card.Title>{m.analytics_recentNotes()}</Card.Title>
				</Card.Header>
				<Card.Content class="p-0">
					<div class="divide-y">
						{#each data.stats.recentNotes as note (note.id)}
							<div class="flex items-center justify-between px-6 py-3">
								<div class="min-w-0 flex-1">
									<div class="flex items-center gap-2">
										{#if note.isPinned}
											<Pin class="h-3 w-3 shrink-0 text-primary" />
										{/if}
										<span class="truncate font-medium">{note.title}</span>
									</div>
								</div>
								<div class="flex items-center gap-3">
									<Badge variant="secondary" class="shrink-0">
										{categoryLabels[note.category ?? '']?.() ?? note.category}
									</Badge>
									<span class="shrink-0 text-xs text-muted-foreground">
										{formatDate(note.createdAt ?? '')}
									</span>
								</div>
							</div>
						{/each}
					</div>
				</Card.Content>
			</Card.Root>
		{/if}
	{/if}
</div>
