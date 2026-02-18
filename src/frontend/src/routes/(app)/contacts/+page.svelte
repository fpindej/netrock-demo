<script lang="ts">
	import { goto } from '$app/navigation';
	import { page } from '$app/state';
	import { SvelteURLSearchParams } from 'svelte/reactivity';
	import * as Card from '$lib/components/ui/card';
	import { Button } from '$lib/components/ui/button';
	import { Input } from '$lib/components/ui/input';
	import { Plus, Search } from '@lucide/svelte';
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

	let searchInput = $state(data.search ?? '');
	let searchTimeout: ReturnType<typeof setTimeout>;

	let contacts = $derived(data.contacts?.items ?? []);
	let totalCount = $derived(data.contacts?.totalCount ?? 0);
	let isSearchActive = $derived(!!(data.search && data.search.length > 0));
	let isEmpty = $derived(totalCount === 0 && !isSearchActive);
	let isEmptySearch = $derived(totalCount === 0 && isSearchActive);

	function handleEdit(contact: Contact) {
		editingContact = contact;
		editDialogOpen = true;
	}

	function handleSearch(value: string) {
		clearTimeout(searchTimeout);
		searchTimeout = setTimeout(() => {
			const params = new SvelteURLSearchParams(page.url.searchParams);
			if (value) {
				params.set('search', value);
			} else {
				params.delete('search');
			}
			params.delete('page');
			const query = params.toString();
			// eslint-disable-next-line svelte/no-navigation-without-resolve -- page.url.pathname is already resolved
			goto(`${page.url.pathname}${query ? `?${query}` : ''}`, {
				replaceState: true,
				keepFocus: true
			});
		}, 300);
	}

	function clearSearch() {
		searchInput = '';
		handleSearch('');
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
	<div class="flex flex-wrap items-center justify-between gap-3">
		<div>
			<h3 class="text-lg font-medium">{m.contacts_title()}</h3>
			<p class="text-sm text-muted-foreground">{m.contacts_description()}</p>
		</div>
		{#if isEmpty}
			<!-- no button when truly empty -->
		{:else}
			<Button onclick={() => (createDialogOpen = true)}>
				<Plus class="me-2 h-4 w-4" />
				{m.contacts_newContact()}
			</Button>
		{/if}
	</div>
	<div class="h-px w-full bg-border"></div>

	{#if isEmpty}
		<ContactEmptyState onCreate={() => (createDialogOpen = true)} />
	{:else}
		<div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
			<div class="relative max-w-sm flex-1">
				<Search class="absolute start-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
				<Input
					type="search"
					placeholder={m.contacts_searchPlaceholder()}
					class="ps-9"
					value={searchInput}
					oninput={(e) => {
						searchInput = e.currentTarget.value;
						handleSearch(searchInput);
					}}
				/>
			</div>
			{#if totalCount > 0}
				<p class="text-sm text-muted-foreground">
					{totalCount} contacts
				</p>
			{/if}
		</div>

		{#if isEmptySearch}
			<div class="flex flex-col items-center justify-center py-12 text-center">
				<p class="text-sm text-muted-foreground">{m.contacts_noSearchResults()}</p>
				<Button variant="link" class="mt-2" onclick={clearSearch}>
					{m.contacts_clearSearch()}
				</Button>
			</div>
		{:else}
			<Card.Root class="overflow-hidden">
				<Card.Content class="p-0">
					<ContactTable {contacts} {totalCount} onEdit={handleEdit} />
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
	{/if}
</div>

<CreateContactDialog bind:open={createDialogOpen} />
<EditContactDialog bind:open={editDialogOpen} contact={editingContact} />
