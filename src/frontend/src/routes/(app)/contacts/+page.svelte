<script lang="ts">
	import { goto, invalidateAll } from '$app/navigation';
	import { page } from '$app/state';
	import { SvelteURLSearchParams } from 'svelte/reactivity';
	import * as Card from '$lib/components/ui/card';
	import { Button } from '$lib/components/ui/button';
	import * as Dialog from '$lib/components/ui/dialog';
	import {
		ContactsEmptyState,
		ContactsTable,
		ContactsToolbar,
		CreateContactDialog
	} from '$lib/components/contacts';
	import { Pagination } from '$lib/components/admin';
	import { UserPlus, Loader2 } from '@lucide/svelte';
	import { toast } from '$lib/components/ui/sonner';
	import * as m from '$lib/paraglide/messages';
	import type { PageData } from './$types';
	import type { ContactResponse } from '$lib/types/contacts';

	let { data }: { data: PageData } = $props();

	let searchInput = $state(data.search ?? '');
	let searchTimeout: ReturnType<typeof setTimeout>;
	let sortValue = $state('newest');

	let createDialogOpen = $state(false);
	let editingContact = $state<ContactResponse | null>(null);
	let editDialogOpen = $state(false);

	let deleteDialogOpen = $state(false);
	let deletingContact = $state<ContactResponse | null>(null);
	let isDeleting = $state(false);
	let isGenerating = $state(false);

	let isEmpty = $derived((data.contacts?.items?.length ?? 0) === 0 && !data.search);

	let sortedContacts = $derived.by(() => {
		const items = [...(data.contacts?.items ?? [])];
		switch (sortValue) {
			case 'oldest':
				return items.sort(
					(a, b) => new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime()
				);
			case 'valueHigh':
				return items.sort((a, b) => b.value - a.value);
			case 'valueLow':
				return items.sort((a, b) => a.value - b.value);
			case 'newest':
			default:
				return items.sort(
					(a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
				);
		}
	});

	function handleSearch(value: string) {
		searchInput = value;
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

	function handlePageChange(newPage: number) {
		const params = new SvelteURLSearchParams(page.url.searchParams);
		params.set('page', String(newPage));
		// eslint-disable-next-line svelte/no-navigation-without-resolve -- page.url.pathname is already resolved
		goto(`${page.url.pathname}?${params.toString()}`, { replaceState: true });
	}

	function handleEdit(contact: ContactResponse) {
		editingContact = contact;
		editDialogOpen = true;
	}

	function handleDeletePrompt(contact: ContactResponse) {
		deletingContact = contact;
		deleteDialogOpen = true;
	}

	async function handleDelete() {
		if (!deletingContact) return;
		isDeleting = true;

		const response = await fetch(`/api/v1/contacts/${deletingContact.id}`, {
			method: 'DELETE'
		});

		isDeleting = false;

		if (response.ok) {
			toast.success(m.contacts_deleted());
			deleteDialogOpen = false;
			deletingContact = null;
			await invalidateAll();
		} else {
			toast.error(m.contacts_deleteError());
		}
	}

	async function handleGenerate() {
		isGenerating = true;

		const response = await fetch('/api/v1/contacts/generate', {
			method: 'POST',
			headers: { 'Content-Type': 'application/json' },
			body: JSON.stringify({ count: 10 })
		});

		isGenerating = false;

		if (response.ok) {
			toast.success(m.contacts_generated());
			await invalidateAll();
		} else {
			toast.error(m.contacts_generateError());
		}
	}
</script>

<svelte:head>
	<title>{m.meta_titleTemplate({ title: m.meta_contacts_title() })}</title>
	<meta name="description" content={m.meta_contacts_description()} />
</svelte:head>

{#if isEmpty}
	<div class="space-y-6">
		<div>
			<h3 class="text-lg font-medium">{m.contacts_title()}</h3>
			<p class="text-sm text-muted-foreground">{m.contacts_description()}</p>
		</div>
		<div class="h-px w-full bg-border"></div>
		<ContactsEmptyState
			onGenerate={handleGenerate}
			onCreate={() => (createDialogOpen = true)}
			{isGenerating}
		/>
	</div>
{:else}
	<div class="space-y-6">
		<div class="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
			<div>
				<h3 class="text-lg font-medium">{m.contacts_title()}</h3>
				<p class="text-sm text-muted-foreground">{m.contacts_description()}</p>
			</div>
			<Button onclick={() => (createDialogOpen = true)}>
				<UserPlus class="me-2 h-4 w-4" />
				{m.contacts_newContact()}
			</Button>
		</div>
		<div class="h-px w-full bg-border"></div>

		<ContactsToolbar
			searchValue={searchInput}
			onSearch={handleSearch}
			{sortValue}
			onSort={(v) => (sortValue = v)}
			totalCount={data.contacts?.totalCount ?? 0}
		/>

		<Card.Root>
			<Card.Content class="p-0">
				<ContactsTable
					contacts={sortedContacts}
					onEdit={handleEdit}
					onDelete={handleDeletePrompt}
				/>
			</Card.Content>
		</Card.Root>

		<Pagination
			pageNumber={data.contacts?.pageNumber ?? 1}
			totalPages={data.contacts?.totalPages ?? 1}
			hasPreviousPage={data.contacts?.hasPreviousPage ?? false}
			hasNextPage={data.contacts?.hasNextPage ?? false}
			onPageChange={handlePageChange}
		/>
	</div>
{/if}

<CreateContactDialog bind:open={createDialogOpen} />
<CreateContactDialog bind:open={editDialogOpen} contact={editingContact} />

<Dialog.Root bind:open={deleteDialogOpen}>
	<Dialog.Content>
		<Dialog.Header>
			<Dialog.Title>{m.contacts_deleteConfirm_title()}</Dialog.Title>
			<Dialog.Description>
				{m.contacts_deleteConfirm_description({
					name: deletingContact ? `${deletingContact.firstName} ${deletingContact.lastName}` : ''
				})}
			</Dialog.Description>
		</Dialog.Header>
		<Dialog.Footer class="flex-col-reverse sm:flex-row">
			<Button variant="outline" onclick={() => (deleteDialogOpen = false)}>
				{m.common_cancel()}
			</Button>
			<Button variant="destructive" onclick={handleDelete} disabled={isDeleting}>
				{#if isDeleting}
					<Loader2 class="me-2 h-4 w-4 animate-spin" />
				{/if}
				{m.common_delete()}
			</Button>
		</Dialog.Footer>
	</Dialog.Content>
</Dialog.Root>
