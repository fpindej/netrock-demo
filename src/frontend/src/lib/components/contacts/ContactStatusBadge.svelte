<script lang="ts">
	import * as m from '$lib/paraglide/messages';

	interface Props {
		status: string;
	}

	let { status }: Props = $props();

	const fallback = {
		label: () => m.contacts_status_lead(),
		classes: 'bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400'
	};

	const statusConfig: Record<string, { label: () => string; classes: string }> = {
		Lead: fallback,
		Prospect: {
			label: () => m.contacts_status_prospect(),
			classes: 'bg-orange-100 text-orange-700 dark:bg-orange-900/30 dark:text-orange-400'
		},
		Customer: {
			label: () => m.contacts_status_customer(),
			classes: 'bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400'
		},
		Churning: {
			label: () => m.contacts_status_churning(),
			classes: 'bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400'
		}
	};

	function getConfig(s: string) {
		return statusConfig[s] ?? fallback;
	}

	let config = $derived(getConfig(status));
</script>

<span
	class="inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-semibold {config.classes}"
>
	{config.label()}
</span>
