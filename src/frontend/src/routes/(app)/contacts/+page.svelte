<script lang="ts">
	import { goto } from '$app/navigation';
	import { page } from '$app/state';
	import { SvelteURLSearchParams } from 'svelte/reactivity';
	import * as Card from '$lib/components/ui/card';
	import { Button } from '$lib/components/ui/button';
	import { Plus } from '@lucide/svelte';
	import {
		ContactTable,
		CreateContactDialog,
		EditContactDialog,
		ContactEmptyState
	} from '$lib/components/contacts';
	import { Pagination } from '$lib/components/admin';
	import * as m from '$lib/paraglide/messages';
	import type { Contact } from '$lib/types';
	import type { PageData } from './$types';

	let { data }: { data: PageData } = $props();

	let createDialogOpen = $state(false);
	let editDialogOpen = $state(false);
	let editingContact = $state<Contact | null>(null);

	let contacts = $derived(data.contacts?.items ?? []);
	let isEmpty = $derived((data.contacts?.totalCount ?? 0) === 0);

	function handleEdit(contact: Contact) {
		editingContact = contact;
		editDialogOpen = true;
	}

	function handlePageChange(newPage: number) {
		const params = new SvelteURLSearchParams(page.url.searchParams);
		params.set('page', String(newPage));
		// eslint-disable-next-line svelte/no-navigation-without-resolve -- page.url.pathname is already resolved
		goto(`${page.url.pathname}?${params.toString()}`, { replaceState: true });
	}
</script>

<svelte:head>
	<title>{m.meta_titleTemplate({ title: m.meta_contacts_title() })}</title>
	<meta name="description" content={m.meta_contacts_description()} />
</svelte:head>

<div class="space-y-6">
	<div class="flex items-center justify-between">
		<div>
			<h3 class="text-lg font-medium">{m.contacts_title()}</h3>
			<p class="text-sm text-muted-foreground">{m.contacts_description()}</p>
		</div>
		{#if !isEmpty}
			<div class="flex items-center gap-3">
				{#if data.contacts?.totalCount != null}
					<p class="text-sm text-muted-foreground">
						{data.contacts.totalCount} contacts
					</p>
				{/if}
				<Button onclick={() => (createDialogOpen = true)}>
					<Plus class="me-2 h-4 w-4" />
					{m.contacts_newContact()}
				</Button>
			</div>
		{/if}
	</div>
	<div class="h-px w-full bg-border"></div>

	{#if isEmpty}
		<ContactEmptyState onCreate={() => (createDialogOpen = true)} />
	{:else}
		<Card.Root>
			<Card.Content class="p-0">
				<ContactTable {contacts} onEdit={handleEdit} />
			</Card.Content>
		</Card.Root>

		<Pagination
			pageNumber={data.contacts?.pageNumber ?? 1}
			totalPages={data.contacts?.totalPages ?? 1}
			hasPreviousPage={data.contacts?.hasPreviousPage ?? false}
			hasNextPage={data.contacts?.hasNextPage ?? false}
			onPageChange={handlePageChange}
		/>
	{/if}
</div>

<CreateContactDialog bind:open={createDialogOpen} />
<EditContactDialog bind:open={editDialogOpen} contact={editingContact} />
