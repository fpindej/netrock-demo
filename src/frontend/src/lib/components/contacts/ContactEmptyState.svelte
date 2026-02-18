<script lang="ts">
	import { Users, Loader2, DatabaseZap, ChevronDown } from '@lucide/svelte';
	import { Button } from '$lib/components/ui/button';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
	import { browserClient } from '$lib/api';
	import { toast } from '$lib/components/ui/sonner';
	import { invalidateAll } from '$app/navigation';
	import * as m from '$lib/paraglide/messages';

	interface Props {
		onCreate: () => void;
	}

	let { onCreate }: Props = $props();

	let isSeeding = $state(false);

	const counts = [5, 10, 20, 50] as const;

	async function seedContacts(count: number) {
		isSeeding = true;

		const { response } = await browserClient.POST('/api/v1/contacts/seed', {
			params: { query: { count } }
		});

		isSeeding = false;

		if (response.ok) {
			toast.success(m.contacts_seed_success());
			await invalidateAll();
		} else {
			toast.error(m.contacts_seed_error());
		}
	}
</script>

<div class="flex flex-col items-center justify-center py-16 text-center">
	<div class="mb-4 rounded-full bg-muted p-4">
		<Users class="h-8 w-8 text-muted-foreground" />
	</div>
	<h3 class="mb-1 text-lg font-semibold">{m.contacts_empty_title()}</h3>
	<p class="mb-6 max-w-sm text-sm text-muted-foreground">
		{m.contacts_empty_description()}
	</p>
	<div class="flex gap-3">
		<DropdownMenu.Root>
			<DropdownMenu.Trigger>
				{#snippet child({ props })}
					<Button variant="outline" disabled={isSeeding} {...props}>
						{#if isSeeding}
							<Loader2 class="me-2 h-4 w-4 animate-spin" />
						{:else}
							<DatabaseZap class="me-2 h-4 w-4" />
						{/if}
						{m.contacts_empty_seed({ count: 10 })}
						<ChevronDown class="ms-1 h-3 w-3" />
					</Button>
				{/snippet}
			</DropdownMenu.Trigger>
			<DropdownMenu.Content>
				{#each counts as count (count)}
					<DropdownMenu.Item onclick={() => seedContacts(count)}>
						{m.contacts_empty_seed({ count })}
					</DropdownMenu.Item>
				{/each}
			</DropdownMenu.Content>
		</DropdownMenu.Root>
		<Button onclick={onCreate}>{m.contacts_empty_cta()}</Button>
	</div>
</div>
