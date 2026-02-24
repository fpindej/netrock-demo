<script lang="ts">
	import * as Card from '$lib/components/ui/card';
	import { Button } from '$lib/components/ui/button';
	import { StatCard, HorizontalBarChart } from '$lib/components/analytics';
	import { Users2, DollarSign, Target } from '@lucide/svelte';
	import * as m from '$lib/paraglide/messages';
	import type { PageData } from './$types';

	let { data }: { data: PageData } = $props();

	let stats = $derived(data.stats);

	const formatCurrency = (value: number) =>
		new Intl.NumberFormat('en-US', {
			style: 'currency',
			currency: 'USD',
			maximumFractionDigits: 0
		}).format(value);

	let totalContacts = $derived(stats?.totalContacts ?? 0);
	let totalValue = $derived(stats?.totalValue ?? 0);
	let customerCount = $derived(stats?.customerCount ?? 0);

	let hasData = $derived(totalContacts > 0);

	let statusItems = $derived.by(() => {
		const items = [
			{
				label: m.contacts_status_lead(),
				count: stats?.leadCount ?? 0,
				colorClass: 'bg-blue-500'
			},
			{
				label: m.contacts_status_prospect(),
				count: stats?.prospectCount ?? 0,
				colorClass: 'bg-orange-500'
			},
			{
				label: m.contacts_status_customer(),
				count: stats?.customerCount ?? 0,
				colorClass: 'bg-green-500'
			},
			{
				label: m.contacts_status_churning(),
				count: stats?.churningCount ?? 0,
				colorClass: 'bg-red-500'
			}
		];

		const maxCount = Math.max(...items.map((i) => i.count), 1);

		return items.map((item) => ({
			...item,
			percentage: (item.count / maxCount) * 100,
			displayValue: formatCurrency(totalValue * (item.count / (totalContacts || 1)))
		}));
	});

	const sourceColorMap: Record<string, string> = {
		Web: 'bg-blue-500',
		Referral: 'bg-green-500',
		SocialMedia: 'bg-purple-500',
		Email: 'bg-orange-500',
		Phone: 'bg-cyan-500',
		Other: 'bg-gray-500'
	};

	const sourceLabelMap: Record<string, () => string> = {
		Web: () => m.contacts_source_web(),
		Referral: () => m.contacts_source_referral(),
		SocialMedia: () => m.contacts_source_socialMedia(),
		Email: () => m.contacts_source_email(),
		Phone: () => m.contacts_source_phone(),
		Other: () => m.contacts_source_other()
	};

	let sourceItems = $derived.by(() => {
		const breakdown = stats?.sourceBreakdown ?? {};
		const entries = Object.entries(breakdown);
		const maxCount = Math.max(...entries.map(([, count]) => count), 1);

		return entries.map(([source, count]) => ({
			label: (sourceLabelMap[source] ?? (() => source))(),
			count,
			percentage: (count / maxCount) * 100,
			displayValue: count.toString(),
			colorClass: sourceColorMap[source] ?? 'bg-gray-500'
		}));
	});
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

	{#if hasData}
		<div class="grid grid-cols-1 gap-4 sm:grid-cols-3">
			<StatCard
				label={m.analytics_totalContacts()}
				value={totalContacts.toString()}
				icon={Users2}
			/>
			<StatCard
				label={m.analytics_pipelineValue()}
				value={formatCurrency(totalValue)}
				icon={DollarSign}
			/>
			<StatCard label={m.analytics_customers()} value={customerCount.toString()} icon={Target} />
		</div>

		<Card.Root>
			<Card.Header>
				<Card.Title>{m.analytics_pipelineByStatus()}</Card.Title>
			</Card.Header>
			<Card.Content>
				<HorizontalBarChart items={statusItems} />
			</Card.Content>
		</Card.Root>

		<Card.Root>
			<Card.Header>
				<Card.Title>{m.analytics_sourceDistribution()}</Card.Title>
			</Card.Header>
			<Card.Content>
				<HorizontalBarChart items={sourceItems} />
			</Card.Content>
		</Card.Root>
	{:else}
		<div class="flex flex-col items-center justify-center py-16 text-center">
			<div class="mb-4 rounded-full bg-muted p-4">
				<Users2 class="h-8 w-8 text-muted-foreground" />
			</div>
			<h3 class="mb-1 text-lg font-medium">{m.analytics_noData()}</h3>
			<p class="mb-6 max-w-sm text-sm text-muted-foreground">
				{m.analytics_noDataDescription()}
			</p>
			<Button href="/contacts">
				{m.analytics_goToContacts()}
			</Button>
		</div>
	{/if}
</div>
