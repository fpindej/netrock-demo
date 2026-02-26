<script lang="ts">
	import * as Dialog from '$lib/components/ui/dialog';
	import { Badge } from '$lib/components/ui/badge';
	import { Timeline, TimelineItem, TimelineContent } from '$lib/components/ui/timeline';
	import { browserClient } from '$lib/api/client';
	import { History } from '@lucide/svelte';
	import * as m from '$lib/paraglide/messages';
	import type { JobExecutionDetail } from '$lib/types';
	import {
		formatJobDate,
		formatJobDuration,
		getJobStatusLabel,
		getJobStatusVariant,
		getLogLevelVariant,
		getLogLevelLabel,
		getTriggeredByLabel
	} from '$lib/utils/jobs';

	interface Props {
		executionId: string | null;
		open: boolean;
		onOpenChange: (open: boolean) => void;
	}

	let { executionId, open, onOpenChange }: Props = $props();

	let detail = $state<JobExecutionDetail | null>(null);
	let loading = $state(false);

	async function loadDetail(id: string): Promise<void> {
		loading = true;
		detail = null;

		const { data } = await browserClient.GET('/api/v1/admin/jobs/executions/{executionId}', {
			params: { path: { executionId: id } }
		});

		if (data) {
			detail = data as JobExecutionDetail;
		}

		loading = false;
	}

	$effect(() => {
		if (open && executionId) {
			loadDetail(executionId);
		}
	});
</script>

<Dialog.Root {open} {onOpenChange}>
	<Dialog.Content class="max-w-lg">
		<Dialog.Header>
			<Dialog.Title>{m.admin_jobDetail_executionDetail()}</Dialog.Title>
		</Dialog.Header>

		{#if loading}
			<div class="flex items-center justify-center py-12">
				<div
					class="h-6 w-6 animate-spin rounded-full border-2 border-primary border-t-transparent"
				></div>
			</div>
		{:else if detail}
			<div class="space-y-4">
				<div class="flex items-center justify-between">
					<span class="text-sm text-muted-foreground">{m.admin_jobDetail_col_status()}</span>
					<Badge variant={getJobStatusVariant(detail.status)}>
						{getJobStatusLabel(detail.status)}
					</Badge>
				</div>

				<div class="flex items-center justify-between">
					<span class="text-sm text-muted-foreground">{m.admin_jobDetail_col_startedAt()}</span>
					<span class="text-sm">{formatJobDate(detail.startedAt)}</span>
				</div>

				{#if detail.completedAt}
					<div class="flex items-center justify-between">
						<span class="text-sm text-muted-foreground">{m.admin_jobDetail_completedAt()}</span>
						<span class="text-sm">{formatJobDate(detail.completedAt)}</span>
					</div>
				{/if}

				{#if detail.duration}
					<div class="flex items-center justify-between">
						<span class="text-sm text-muted-foreground">{m.admin_jobDetail_col_duration()}</span>
						<span class="text-sm tabular-nums">{formatJobDuration(detail.duration)}</span>
					</div>
				{/if}

				<div class="flex items-center justify-between">
					<span class="text-sm text-muted-foreground">{m.admin_jobDetail_triggeredBy()}</span>
					<span class="text-sm">{getTriggeredByLabel(detail.triggeredBy)}</span>
				</div>

				{#if detail.errorMessage}
					<div>
						<span class="text-sm text-muted-foreground">{m.admin_jobDetail_errorMessage()}</span>
						<pre
							class="mt-1 overflow-x-auto rounded-md bg-destructive/10 p-2 text-xs text-destructive">{detail.errorMessage}</pre>
					</div>
				{/if}

				<div class="border-t pt-4">
					<h4 class="mb-3 text-sm font-medium">{m.admin_jobDetail_logEntries()}</h4>

					{#if !detail.logEntries || detail.logEntries.length === 0}
						<div class="flex flex-col items-center justify-center py-6 text-center">
							<div class="mb-2 rounded-full bg-muted p-2">
								<History class="h-4 w-4 text-muted-foreground" />
							</div>
							<p class="text-xs text-muted-foreground">
								{m.admin_jobDetail_noLogEntries()}
							</p>
						</div>
					{:else}
						<Timeline>
							{#each detail.logEntries as entry, i (entry.id)}
								<TimelineItem
									variant={getLogLevelVariant(entry.level)}
									isLast={i === detail.logEntries.length - 1}
								>
									<TimelineContent
										title={entry.message ?? ''}
										timestamp={formatJobDate(entry.timestamp)}
										description={entry.category
											? `${getLogLevelLabel(entry.level)} · ${entry.category}`
											: getLogLevelLabel(entry.level)}
									/>
								</TimelineItem>
							{/each}
						</Timeline>
					{/if}
				</div>
			</div>
		{/if}
	</Dialog.Content>
</Dialog.Root>
