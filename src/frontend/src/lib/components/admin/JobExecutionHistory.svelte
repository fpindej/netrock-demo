<script lang="ts">
	import { untrack } from 'svelte';
	import * as Card from '$lib/components/ui/card';
	import * as Select from '$lib/components/ui/select';
	import { Badge } from '$lib/components/ui/badge';
	import { Pagination, JobExecutionDetailDialog } from '$lib/components/admin';
	import { browserClient } from '$lib/api/client';
	import { History, TriangleAlert } from '@lucide/svelte';
	import * as m from '$lib/paraglide/messages';
	import type { JobExecutionSummary } from '$lib/types';
	import {
		formatJobDate,
		formatJobDuration,
		getJobStatusLabel,
		getJobStatusVariant,
		getTriggeredByLabel
	} from '$lib/utils/jobs';

	interface Props {
		jobId: string;
	}

	let { jobId }: Props = $props();

	let executions = $state<JobExecutionSummary[]>([]);
	let loading = $state(true);
	let error = $state(false);
	let pageNumber = $state(1);
	let totalPages = $state(0);
	let hasPreviousPage = $state(false);
	let hasNextPage = $state(false);
	let statusFilter = $state<string>('');

	let selectedExecutionId = $state<string | null>(null);
	let dialogOpen = $state(false);

	const pageSize = 10;

	const statusOptions = [
		{ value: '', label: m.admin_jobDetail_allStatuses() },
		{ value: 'Succeeded', label: m.admin_jobs_status_succeeded() },
		{ value: 'Failed', label: m.admin_jobs_status_failed() },
		{ value: 'Running', label: m.admin_jobDetail_status_running() }
	];

	async function loadExecutions(page: number): Promise<void> {
		loading = true;
		error = false;

		try {
			const { data } = await browserClient.GET('/api/v1/admin/jobs/{jobId}/executions', {
				params: {
					path: { jobId },
					query: {
						pageNumber: page,
						pageSize,
						status: statusFilter || undefined
					}
				}
			});

			if (data) {
				executions = (data.items as JobExecutionSummary[]) ?? [];
				pageNumber = data.pageNumber ?? 1;
				totalPages = data.totalPages ?? 0;
				hasPreviousPage = data.hasPreviousPage ?? false;
				hasNextPage = data.hasNextPage ?? false;
			} else {
				error = true;
			}
		} catch {
			error = true;
		}

		loading = false;
	}

	function onPageChange(page: number): void {
		loadExecutions(page);
	}

	function onStatusChange(value: string): void {
		statusFilter = value;
		loadExecutions(1);
	}

	function openDetail(id: string | undefined): void {
		if (!id) return;
		selectedExecutionId = id;
		dialogOpen = true;
	}

	$effect(() => {
		// Only re-run when jobId changes; untrack to avoid re-running on statusFilter changes
		void jobId;
		untrack(() => {
			statusFilter = '';
			loadExecutions(1);
		});
	});
</script>

<Card.Root>
	<Card.Header>
		<div class="flex items-center justify-between">
			<Card.Title>{m.admin_jobDetail_executionHistory()}</Card.Title>
			<Select.Root type="single" value={statusFilter} onValueChange={onStatusChange}>
				<Select.Trigger class="w-40">
					{statusOptions.find((o) => o.value === statusFilter)?.label ??
						m.admin_jobDetail_allStatuses()}
				</Select.Trigger>
				<Select.Content>
					{#each statusOptions as option (option.value)}
						<Select.Item value={option.value}>{option.label}</Select.Item>
					{/each}
				</Select.Content>
			</Select.Root>
		</div>
	</Card.Header>
	<Card.Content class="p-0">
		{#if loading}
			<div class="flex items-center justify-center py-12">
				<div
					class="h-6 w-6 animate-spin rounded-full border-2 border-primary border-t-transparent"
				></div>
			</div>
		{:else if error}
			<div class="flex flex-col items-center justify-center py-12 text-center">
				<div class="mb-3 rounded-full bg-destructive/10 p-3">
					<TriangleAlert class="h-6 w-6 text-destructive" />
				</div>
				<p class="text-sm text-muted-foreground">{m.serverError_failedToLoadExecutions()}</p>
			</div>
		{:else if executions.length === 0}
			<div class="flex flex-col items-center justify-center py-12 text-center">
				<div class="mb-3 rounded-full bg-muted p-3">
					<History class="h-6 w-6 text-muted-foreground" />
				</div>
				<p class="text-sm text-muted-foreground">{m.admin_jobDetail_noHistory()}</p>
			</div>
		{:else}
			<!-- Mobile: card list -->
			<div class="divide-y md:hidden">
				{#each executions as execution (execution.id)}
					<button
						type="button"
						class="w-full space-y-1 p-4 text-start transition-colors hover:bg-muted/50"
						onclick={() => openDetail(execution.id)}
					>
						<div class="flex items-center justify-between">
							<span class="text-xs text-muted-foreground">
								{formatJobDate(execution.startedAt)}
							</span>
							<Badge variant={getJobStatusVariant(execution.status)}>
								{getJobStatusLabel(execution.status)}
							</Badge>
						</div>
						<div class="flex items-center gap-2">
							{#if execution.duration}
								<span class="text-xs text-muted-foreground">
									{formatJobDuration(execution.duration)}
								</span>
							{/if}
							{#if execution.triggeredBy}
								<span class="text-xs text-muted-foreground">
									· {getTriggeredByLabel(execution.triggeredBy)}
								</span>
							{/if}
						</div>
						{#if execution.errorMessage}
							<p class="truncate text-xs text-destructive">{execution.errorMessage}</p>
						{/if}
					</button>
				{/each}
			</div>

			<!-- Desktop: table -->
			<div class="hidden overflow-x-auto md:block">
				<table class="w-full text-sm">
					<thead>
						<tr class="border-b bg-muted/50 text-start">
							<th
								class="px-4 py-3 text-start text-xs font-medium tracking-wide text-muted-foreground"
							>
								{m.admin_jobDetail_col_startedAt()}
							</th>
							<th
								class="px-4 py-3 text-start text-xs font-medium tracking-wide text-muted-foreground"
							>
								{m.admin_jobDetail_col_duration()}
							</th>
							<th
								class="px-4 py-3 text-start text-xs font-medium tracking-wide text-muted-foreground"
							>
								{m.admin_jobDetail_col_status()}
							</th>
							<th
								class="px-4 py-3 text-start text-xs font-medium tracking-wide text-muted-foreground"
							>
								{m.admin_jobDetail_triggeredBy()}
							</th>
							<th
								class="px-4 py-3 text-start text-xs font-medium tracking-wide text-muted-foreground"
							>
								{m.admin_jobDetail_col_error()}
							</th>
						</tr>
					</thead>
					<tbody>
						{#each executions as execution (execution.id)}
							<tr
								class="cursor-pointer border-b transition-colors hover:bg-muted/50"
								onclick={() => openDetail(execution.id)}
								role="button"
								tabindex="0"
								onkeydown={(e) => {
									if (e.key === 'Enter' || e.key === ' ') {
										e.preventDefault();
										openDetail(execution.id);
									}
								}}
							>
								<td class="px-4 py-3 text-muted-foreground">
									{formatJobDate(execution.startedAt)}
								</td>
								<td class="px-4 py-3 text-muted-foreground tabular-nums">
									{formatJobDuration(execution.duration)}
								</td>
								<td class="px-4 py-3">
									<Badge variant={getJobStatusVariant(execution.status)}>
										{getJobStatusLabel(execution.status)}
									</Badge>
								</td>
								<td class="px-4 py-3 text-muted-foreground">
									{getTriggeredByLabel(execution.triggeredBy)}
								</td>
								<td class="max-w-xs truncate px-4 py-3 text-muted-foreground">
									{execution.errorMessage ?? '-'}
								</td>
							</tr>
						{/each}
					</tbody>
				</table>
			</div>

			<div class="p-4">
				<Pagination {pageNumber} {totalPages} {hasPreviousPage} {hasNextPage} {onPageChange} />
			</div>
		{/if}
	</Card.Content>
</Card.Root>

<JobExecutionDetailDialog
	executionId={selectedExecutionId}
	open={dialogOpen}
	onOpenChange={(open) => (dialogOpen = open)}
/>
