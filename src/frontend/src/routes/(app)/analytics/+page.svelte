<script lang="ts">
	import * as Card from '$lib/components/ui/card';
	import { Badge } from '$lib/components/ui/badge';
	import { Users, DollarSign, UserCheck, BarChart3, Star } from '@lucide/svelte';
	import { EditContactDialog } from '$lib/components/contacts';
	import * as m from '$lib/paraglide/messages';
	import type { Contact } from '$lib/types';
	import type { PageData } from './$types';

	let { data }: { data: PageData } = $props();

	let editDialogOpen = $state(false);
	let editingContact = $state<Contact | null>(null);

	function handleContactClick(contact: Contact) {
		editingContact = contact;
		editDialogOpen = true;
	}

	const statusLabels: Record<string, () => string> = {
		Lead: m.contacts_status_Lead,
		Prospect: m.contacts_status_Prospect,
		Customer: m.contacts_status_Customer,
		Churned: m.contacts_status_Churned
	};

	const statusColors: Record<string, string> = {
		Lead: 'bg-blue-500',
		Prospect: 'bg-amber-500',
		Customer: 'bg-green-500',
		Churned: 'bg-red-500'
	};

	const sourceLabels: Record<string, () => string> = {
		Website: m.contacts_source_Website,
		Referral: m.contacts_source_Referral,
		Social: m.contacts_source_Social,
		Email: m.contacts_source_Email,
		Phone: m.contacts_source_Phone,
		Other: m.contacts_source_Other
	};

	const sourceColors: Record<string, string> = {
		Website: 'bg-blue-500',
		Referral: 'bg-green-500',
		Social: 'bg-purple-500',
		Email: 'bg-amber-500',
		Phone: 'bg-cyan-500',
		Other: 'bg-gray-500'
	};

	let maxStatusCount = $derived(() => {
		if (!data.stats?.byStatus) return 1;
		return Math.max(1, ...Object.values(data.stats.byStatus));
	});

	let maxSourceCount = $derived(() => {
		if (!data.stats?.bySource) return 1;
		return Math.max(1, ...Object.values(data.stats.bySource));
	});

	function formatValue(value: number | null | undefined) {
		if (value == null) return '$0';
		return new Intl.NumberFormat(undefined, {
			style: 'currency',
			currency: 'USD',
			minimumFractionDigits: 0,
			maximumFractionDigits: 0
		}).format(value);
	}

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
						<Users class="h-5 w-5 text-primary" />
					</div>
					<div>
						<p class="text-sm text-muted-foreground">{m.analytics_totalContacts()}</p>
						<p class="text-2xl font-bold">{data.stats.totalCount}</p>
					</div>
				</Card.Content>
			</Card.Root>

			<Card.Root>
				<Card.Content class="flex items-center gap-4 p-6">
					<div class="rounded-lg bg-primary/10 p-3">
						<DollarSign class="h-5 w-5 text-primary" />
					</div>
					<div>
						<p class="text-sm text-muted-foreground">{m.analytics_pipelineValue()}</p>
						<p class="text-2xl font-bold">{formatValue(data.stats.totalPipelineValue)}</p>
					</div>
				</Card.Content>
			</Card.Root>

			<Card.Root>
				<Card.Content class="flex items-center gap-4 p-6">
					<div class="rounded-lg bg-primary/10 p-3">
						<UserCheck class="h-5 w-5 text-primary" />
					</div>
					<div>
						<p class="text-sm text-muted-foreground">{m.analytics_customers()}</p>
						<p class="text-2xl font-bold">{data.stats.customerCount}</p>
					</div>
				</Card.Content>
			</Card.Root>
		</div>

		<!-- Pipeline by status -->
		<Card.Root>
			<Card.Header>
				<Card.Title>{m.analytics_pipelineByStatus()}</Card.Title>
			</Card.Header>
			<Card.Content>
				<div class="space-y-4">
					{#each Object.entries(data.stats.byStatus ?? {}) as [status, count] (status)}
						<div class="space-y-1.5">
							<div class="flex items-center justify-between text-sm">
								<span class="font-medium">{statusLabels[status]?.() ?? status}</span>
								<div class="flex items-center gap-2">
									<span class="text-muted-foreground">
										{formatValue(data.stats.pipelineValue?.[status] ?? 0)}
									</span>
									<Badge variant="secondary" class="text-xs">{count}</Badge>
								</div>
							</div>
							<div class="h-2.5 overflow-hidden rounded-full bg-muted">
								<div
									class="h-full rounded-full transition-all duration-500 {statusColors[status] ??
										'bg-primary'}"
									style="width: {maxStatusCount() > 0 ? (count / maxStatusCount()) * 100 : 0}%"
								></div>
							</div>
						</div>
					{/each}
				</div>
			</Card.Content>
		</Card.Root>

		<!-- Source breakdown -->
		<Card.Root>
			<Card.Header>
				<Card.Title>{m.analytics_sourceBreakdown()}</Card.Title>
			</Card.Header>
			<Card.Content>
				<div class="space-y-4">
					{#each Object.entries(data.stats.bySource ?? {}).filter(([, count]) => count > 0) as [source, count] (source)}
						<div class="space-y-1.5">
							<div class="flex items-center justify-between text-sm">
								<span class="font-medium">{sourceLabels[source]?.() ?? source}</span>
								<span class="text-muted-foreground">{count}</span>
							</div>
							<div class="h-2.5 overflow-hidden rounded-full bg-muted">
								<div
									class="h-full rounded-full transition-all duration-500 {sourceColors[source] ??
										'bg-primary'}"
									style="width: {maxSourceCount() > 0 ? (count / maxSourceCount()) * 100 : 0}%"
								></div>
							</div>
						</div>
					{/each}
				</div>
			</Card.Content>
		</Card.Root>

		<!-- Recent contacts -->
		{#if data.stats.recentContacts && data.stats.recentContacts.length > 0}
			<Card.Root>
				<Card.Header>
					<Card.Title>{m.analytics_recentContacts()}</Card.Title>
				</Card.Header>
				<Card.Content class="p-0">
					<div class="divide-y">
						{#each data.stats.recentContacts as contact (contact.id)}
							<button
								class="flex w-full cursor-pointer items-center justify-between px-6 py-3 text-start transition-colors hover:bg-muted/50"
								onclick={() => handleContactClick(contact)}
							>
								<div class="min-w-0 flex-1">
									<div class="flex items-center gap-2">
										{#if contact.isFavorite}
											<Star class="h-3 w-3 shrink-0 fill-yellow-400 text-yellow-400" />
										{/if}
										<span class="truncate font-medium">{contact.name}</span>
										{#if contact.company}
											<span class="hidden text-sm text-muted-foreground sm:inline">
												{contact.company}
											</span>
										{/if}
									</div>
								</div>
								<div class="flex items-center gap-3">
									<Badge
										variant="secondary"
										class="shrink-0 {statusColors[contact.status ?? '']
											? statusColors[contact.status ?? ''].replace('bg-', 'bg-') +
												'/20 ' +
												statusColors[contact.status ?? ''].replace('bg-', 'text-')
											: ''}"
									>
										{statusLabels[contact.status ?? '']?.() ?? contact.status}
									</Badge>
									{#if contact.value != null}
										<span class="shrink-0 text-sm font-medium">
											{formatValue(contact.value)}
										</span>
									{/if}
									<span class="shrink-0 text-xs text-muted-foreground">
										{formatDate(contact.createdAt ?? '')}
									</span>
								</div>
							</button>
						{/each}
					</div>
				</Card.Content>
			</Card.Root>
		{/if}
	{/if}
</div>

<EditContactDialog bind:open={editDialogOpen} contact={editingContact} />
