<script lang="ts">
	import { goto, invalidateAll } from '$app/navigation';
	import { page } from '$app/state';
	import { SvelteSet, SvelteURLSearchParams } from 'svelte/reactivity';
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
	import { UserPlus, Loader2, Trash2 } from '@lucide/svelte';
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

	let selectedIds = new SvelteSet<string>();
	let selectAllAcrossPages = $state(false);
	let bulkDeleteDialogOpen = $state(false);
	let isBulkDeleting = $state(false);

	let isEmpty = $derived((data.contacts?.items?.length ?? 0) === 0 && !data.search);

	let sortedContacts = $derived.by(() => {
		const items = [...(data.contacts?.items ?? [])];
		switch (sortValue) {
			case 'oldest':
				return items.sort(
					(a, b) => new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime()
				);
			case 'nameAsc':
				return items.sort(
					(a, b) => a.lastName.localeCompare(b.lastName) || a.firstName.localeCompare(b.firstName)
				);
			case 'nameDesc':
				return items.sort(
					(a, b) => b.lastName.localeCompare(a.lastName) || b.firstName.localeCompare(a.firstName)
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

	let pageSize = $derived(data.contacts?.pageSize ?? 10);
	let totalCount = $derived(data.contacts?.totalCount ?? 0);
	let allOnPageSelected = $derived(
		sortedContacts.length > 0 && sortedContacts.every((c) => selectedIds.has(c.id))
	);
	let selectionCount = $derived(selectAllAcrossPages ? totalCount : selectedIds.size);

	function handleToggle(id: string) {
		if (selectedIds.has(id)) {
			selectedIds.delete(id);
		} else {
			selectedIds.add(id);
		}
		selectAllAcrossPages = false;
	}

	function handleToggleAll() {
		if (allOnPageSelected) {
			for (const c of sortedContacts) {
				selectedIds.delete(c.id);
			}
			selectAllAcrossPages = false;
		} else {
			for (const c of sortedContacts) {
				selectedIds.add(c.id);
			}
		}
	}

	function handleSelectAllAcrossPages() {
		for (const c of sortedContacts) {
			selectedIds.add(c.id);
		}
		selectAllAcrossPages = true;
	}

	function clearSelection() {
		selectedIds.clear();
		selectAllAcrossPages = false;
	}

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

	async function handleBulkDelete() {
		isBulkDeleting = true;

		if (selectAllAcrossPages) {
			let currentPage = 1;
			let hasMore = true;

			while (hasMore) {
				const listResponse = await fetch(`/api/v1/contacts?page=${currentPage}&pageSize=50`);
				if (!listResponse.ok) {
					toast.error(m.contacts_deleteError());
					isBulkDeleting = false;
					return;
				}
				const listData = await listResponse.json();
				const items: ContactResponse[] = listData.items ?? [];

				const results = await Promise.all(
					items.map((c) => fetch(`/api/v1/contacts/${c.id}`, { method: 'DELETE' }))
				);

				if (results.some((r) => !r.ok)) {
					toast.error(m.contacts_deleteError());
					isBulkDeleting = false;
					bulkDeleteDialogOpen = false;
					clearSelection();
					await invalidateAll();
					return;
				}

				hasMore = listData.hasNextPage;
				currentPage++;
			}
		} else {
			const ids = Array.from(selectedIds);
			const results = await Promise.all(
				ids.map((id) => fetch(`/api/v1/contacts/${id}`, { method: 'DELETE' }))
			);

			if (results.some((r) => !r.ok)) {
				toast.error(m.contacts_deleteError());
				isBulkDeleting = false;
				bulkDeleteDialogOpen = false;
				clearSelection();
				await invalidateAll();
				return;
			}
		}

		isBulkDeleting = false;
		toast.success(m.contacts_bulkDeleted());
		bulkDeleteDialogOpen = false;
		clearSelection();
		await invalidateAll();
	}

	async function handleGenerate(count: number = 10) {
		isGenerating = true;

		const response = await fetch('/api/v1/contacts/generate', {
			method: 'POST',
			headers: { 'Content-Type': 'application/json' },
			body: JSON.stringify({ count })
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
	<div data-tour="contacts-content" class="space-y-6">
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
	<div data-tour="contacts-content" class="space-y-6">
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

		{#if selectedIds.size > 0}
			<div class="flex flex-col gap-2">
				<div class="flex items-center justify-between rounded-lg border bg-muted/50 px-4 py-2">
					<span class="text-sm font-medium">
						{m.contacts_selected({ count: selectionCount })}
					</span>
					<Button variant="destructive" size="sm" onclick={() => (bulkDeleteDialogOpen = true)}>
						<Trash2 class="me-2 h-4 w-4" />
						{m.contacts_deleteSelected()}
					</Button>
				</div>
				{#if allOnPageSelected && !selectAllAcrossPages && totalCount > pageSize}
					<div class="rounded-lg border bg-muted/30 px-4 py-2 text-center text-sm">
						{m.contacts_selectAllOnPage({ count: sortedContacts.length })}
						<button
							type="button"
							class="ms-1 font-medium text-primary underline-offset-4 hover:underline"
							onclick={handleSelectAllAcrossPages}
						>
							{m.contacts_selectAll({ count: totalCount })}
						</button>
					</div>
				{/if}
			</div>
		{/if}

		<Card.Root>
			<Card.Content class="p-0">
				<ContactsTable
					contacts={sortedContacts}
					onEdit={handleEdit}
					onDelete={handleDeletePrompt}
					{selectedIds}
					onToggle={handleToggle}
					onToggleAll={handleToggleAll}
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

<Dialog.Root bind:open={bulkDeleteDialogOpen}>
	<Dialog.Content>
		<Dialog.Header>
			<Dialog.Title>{m.contacts_bulkDeleteConfirm_title({ count: selectionCount })}</Dialog.Title>
			<Dialog.Description>
				{m.contacts_bulkDeleteConfirm_description({ count: selectionCount })}
			</Dialog.Description>
		</Dialog.Header>
		<Dialog.Footer class="flex-col-reverse sm:flex-row">
			<Button variant="outline" onclick={() => (bulkDeleteDialogOpen = false)}>
				{m.common_cancel()}
			</Button>
			<Button variant="destructive" onclick={handleBulkDelete} disabled={isBulkDeleting}>
				{#if isBulkDeleting}
					<Loader2 class="me-2 h-4 w-4 animate-spin" />
				{/if}
				{m.contacts_deleteSelected()}
			</Button>
		</Dialog.Footer>
	</Dialog.Content>
</Dialog.Root>
