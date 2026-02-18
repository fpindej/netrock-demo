<script lang="ts">
	import { Users, Loader2, Sparkles, ChevronDown } from '@lucide/svelte';
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
	let selectedCount = $state<number>(10);

	async function seedContacts() {
		isSeeding = true;

		const { response } = await browserClient.POST('/api/v1/contacts/seed', {
			params: { query: { count: selectedCount } }
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
	<div class="mb-4 rounded-full bg-primary/10 p-4">
		<Users class="h-8 w-8 text-primary" />
	</div>
	<h3 class="mb-1 text-lg font-semibold">{m.contacts_empty_title()}</h3>
	<p class="mb-8 max-w-md text-sm text-muted-foreground">
		{m.contacts_empty_description()}
	</p>

	<div class="flex flex-col items-center gap-3">
		<div class="flex items-center">
			<Button class="rounded-e-none" disabled={isSeeding} onclick={seedContacts}>
				{#if isSeeding}
					<Loader2 class="me-2 h-4 w-4 animate-spin" />
				{:else}
					<Sparkles class="me-2 h-4 w-4" />
				{/if}
				{m.contacts_empty_seed({ count: selectedCount })}
			</Button>
			<DropdownMenu.Root>
				<DropdownMenu.Trigger>
					{#snippet child({ props })}
						<Button
							class="rounded-s-none border-s border-s-primary-foreground/20 px-2"
							disabled={isSeeding}
							{...props}
						>
							<ChevronDown class="h-4 w-4" />
							<span class="sr-only">Change count</span>
						</Button>
					{/snippet}
				</DropdownMenu.Trigger>
				<DropdownMenu.Content align="end">
					{#each counts as count (count)}
						<DropdownMenu.Item onclick={() => (selectedCount = count)}>
							{m.contacts_empty_seed({ count })}
							{#if count === selectedCount}
								<span class="ms-auto text-xs text-primary">&#10003;</span>
							{/if}
						</DropdownMenu.Item>
					{/each}
				</DropdownMenu.Content>
			</DropdownMenu.Root>
		</div>

		<button
			class="text-sm text-muted-foreground underline-offset-4 hover:text-foreground hover:underline"
			onclick={onCreate}
		>
			{m.contacts_empty_cta()}
		</button>
	</div>
</div>
