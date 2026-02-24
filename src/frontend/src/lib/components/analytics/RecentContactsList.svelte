<script lang="ts">
	import * as m from '$lib/paraglide/messages';
	import { ContactStatusBadge } from '$lib/components/contacts';

	interface RecentContact {
		name: string;
		company: string | null;
		status: string;
		value: number;
		date: string;
	}

	interface Props {
		contacts: RecentContact[];
	}

	let { contacts }: Props = $props();

	const formatCurrency = (value: number) =>
		new Intl.NumberFormat('en-US', {
			style: 'currency',
			currency: 'USD',
			maximumFractionDigits: 0
		}).format(value);

	const formatDate = (dateStr: string) => {
		const date = new Date(dateStr);
		return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
	};
</script>

{#if contacts.length === 0}
	<p class="py-8 text-center text-sm text-muted-foreground">{m.analytics_noData()}</p>
{:else}
	<div class="divide-y">
		{#each contacts as contact (contact.name + contact.date)}
			<div class="flex items-center justify-between px-4 py-3">
				<div class="min-w-0 flex-1">
					<p class="truncate text-sm font-medium">{contact.name}</p>
					<p class="hidden truncate text-xs text-muted-foreground sm:block">
						{contact.company ?? ''}
					</p>
				</div>
				<div class="flex items-center gap-3">
					<ContactStatusBadge status={contact.status} />
					<span class="hidden w-20 text-end text-sm sm:inline">{formatCurrency(contact.value)}</span
					>
					<span class="w-16 text-end text-xs text-muted-foreground">{formatDate(contact.date)}</span
					>
				</div>
			</div>
		{/each}
	</div>
{/if}
