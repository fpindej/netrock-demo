<script lang="ts">
	import { Star, Pencil, Trash2, Loader2 } from '@lucide/svelte';
	import { Button } from '$lib/components/ui/button';
	import { Badge } from '$lib/components/ui/badge';
	import { Checkbox } from '$lib/components/ui/checkbox';
	import * as Dialog from '$lib/components/ui/dialog';
	import { browserClient } from '$lib/api';
	import { toast } from '$lib/components/ui/sonner';
	import { invalidateAll } from '$app/navigation';
	import { SvelteSet } from 'svelte/reactivity';
	import * as m from '$lib/paraglide/messages';
	import type { Contact } from '$lib/types';

	interface Props {
		contacts: Contact[];
		totalCount: number;
		onEdit: (contact: Contact) => void;
	}

	let { contacts, totalCount, onEdit }: Props = $props();

	const statusLabels: Record<string, () => string> = {
		Lead: m.contacts_status_Lead,
		Prospect: m.contacts_status_Prospect,
		Customer: m.contacts_status_Customer,
		Churned: m.contacts_status_Churned
	};

	const statusColors: Record<string, string> = {
		Lead: 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-300',
		Prospect: 'bg-amber-100 text-amber-800 dark:bg-amber-900/30 dark:text-amber-300',
		Customer: 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-300',
		Churned: 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-300'
	};

	const sourceLabels: Record<string, () => string> = {
		Website: m.contacts_source_Website,
		Referral: m.contacts_source_Referral,
		Social: m.contacts_source_Social,
		Email: m.contacts_source_Email,
		Phone: m.contacts_source_Phone,
		Other: m.contacts_source_Other
	};

	// Selection state
	let selectedIds = new SvelteSet<string>();
	let allAcrossPagesSelected = $state(false);

	let allSelected = $derived(contacts.length > 0 && selectedIds.size === contacts.length);
	let someSelected = $derived(selectedIds.size > 0 && selectedIds.size < contacts.length);
	let effectiveSelectionCount = $derived(allAcrossPagesSelected ? totalCount : selectedIds.size);

	// Reset selection when page changes (contacts array reference changes)
	$effect(() => {
		// Subscribe to contacts array identity
		// eslint-disable-next-line @typescript-eslint/no-unused-expressions
		contacts;
		selectedIds.clear();
		allAcrossPagesSelected = false;
	});

	function toggleSelectAll() {
		if (allSelected) {
			selectedIds.clear();
			allAcrossPagesSelected = false;
		} else {
			for (const c of contacts) {
				selectedIds.add(c.id!);
			}
		}
	}

	function toggleSelect(id: string) {
		allAcrossPagesSelected = false;
		if (selectedIds.has(id)) {
			selectedIds.delete(id);
		} else {
			selectedIds.add(id);
		}
	}

	function selectAllAcrossPages() {
		for (const c of contacts) {
			selectedIds.add(c.id!);
		}
		allAcrossPagesSelected = true;
	}

	function clearSelection() {
		selectedIds.clear();
		allAcrossPagesSelected = false;
	}

	// Single delete state
	let deleteConfirmOpen = $state(false);
	let contactToDelete = $state<Contact | null>(null);
	let isDeleting = $state(false);

	function confirmDelete(contact: Contact) {
		contactToDelete = contact;
		deleteConfirmOpen = true;
	}

	async function deleteContact() {
		if (!contactToDelete) return;
		isDeleting = true;

		const { response } = await browserClient.DELETE('/api/v1/contacts/{id}', {
			params: { path: { id: contactToDelete.id! } }
		});

		isDeleting = false;

		if (response.ok) {
			toast.success(m.contacts_delete_success());
			deleteConfirmOpen = false;
			contactToDelete = null;
			await invalidateAll();
		} else {
			toast.error(m.contacts_delete_error());
		}
	}

	// Bulk delete state
	let bulkDeleteConfirmOpen = $state(false);
	let isBulkDeleting = $state(false);

	async function bulkDelete() {
		isBulkDeleting = true;
		const count = effectiveSelectionCount;

		const { response } = allAcrossPagesSelected
			? await browserClient.DELETE('/api/v1/contacts/bulk', {
					body: { ids: [], all: true }
				})
			: await browserClient.DELETE('/api/v1/contacts/bulk', {
					body: { ids: [...selectedIds] }
				});

		isBulkDeleting = false;

		if (response.ok) {
			toast.success(m.contacts_bulkDelete_success({ count }));
			bulkDeleteConfirmOpen = false;
			selectedIds.clear();
			allAcrossPagesSelected = false;
			await invalidateAll();
		} else {
			toast.error(m.contacts_bulkDelete_error());
		}
	}

	// Favorite toggle
	let togglingFavoriteId = $state<string | null>(null);

	async function toggleFavorite(contact: Contact) {
		togglingFavoriteId = contact.id!;

		const { response } = await browserClient.PATCH('/api/v1/contacts/{id}/favorite', {
			params: { path: { id: contact.id! } }
		});

		togglingFavoriteId = null;

		if (response.ok) {
			await invalidateAll();
		} else {
			toast.error(m.contacts_favorite_error());
		}
	}

	function formatValue(value: number | null | undefined) {
		if (value == null) return '';
		return new Intl.NumberFormat('en-US', {
			style: 'currency',
			currency: 'USD',
			minimumFractionDigits: 0,
			maximumFractionDigits: 0
		}).format(value);
	}
</script>

<!-- Bulk actions bar -->
{#if selectedIds.size > 0}
	<div class="flex flex-wrap items-center gap-3 border-b bg-muted/50 px-4 py-2">
		<span class="text-sm font-medium">
			{m.contacts_selected({ count: effectiveSelectionCount })}
		</span>
		<Button variant="destructive" size="sm" onclick={() => (bulkDeleteConfirmOpen = true)}>
			<Trash2 class="me-2 h-3.5 w-3.5" />
			{m.contacts_bulkDelete()}
		</Button>
	</div>
{/if}

<!-- Select all across pages banner -->
{#if allSelected && !allAcrossPagesSelected && totalCount > contacts.length}
	<div class="border-b bg-blue-50 px-4 py-2 text-center text-sm dark:bg-blue-950/30">
		{m.contacts_allOnPageSelected({ count: contacts.length })}
		<button class="ms-1 font-medium text-primary underline" onclick={selectAllAcrossPages}>
			{m.contacts_selectAllAcrossPages({ count: totalCount })}
		</button>
	</div>
{:else if allAcrossPagesSelected}
	<div class="border-b bg-blue-50 px-4 py-2 text-center text-sm dark:bg-blue-950/30">
		{m.contacts_allContactsSelected({ count: totalCount })}
		<button class="ms-1 font-medium text-primary underline" onclick={clearSelection}>
			{m.contacts_clearSelection()}
		</button>
	</div>
{/if}

<!-- Desktop table -->
<div class="hidden lg:block">
	<table class="w-full table-fixed">
		<colgroup>
			<col class="w-10" />
			<col class="w-[30%]" />
			<col class="w-[18%]" />
			<col class="w-[14%]" />
			<col class="w-[13%]" />
			<col class="w-[13%]" />
			<col class="w-20" />
		</colgroup>
		<thead>
			<tr class="border-b text-sm text-muted-foreground">
				<th class="px-4 py-3 text-start">
					<Checkbox
						checked={allSelected}
						indeterminate={someSelected}
						onCheckedChange={toggleSelectAll}
					/>
				</th>
				<th class="px-4 py-3 text-start font-medium">{m.contacts_table_name()}</th>
				<th class="px-4 py-3 text-start font-medium">{m.contacts_table_company()}</th>
				<th class="px-4 py-3 text-start font-medium">{m.contacts_table_status()}</th>
				<th class="px-4 py-3 text-start font-medium">{m.contacts_table_source()}</th>
				<th class="px-4 py-3 text-end font-medium">{m.contacts_table_value()}</th>
				<th class="px-4 py-3 text-end font-medium"></th>
			</tr>
		</thead>
		<tbody>
			{#each contacts as contact (contact.id)}
				<tr
					class="h-[65px] border-b transition-colors hover:bg-muted/50 {selectedIds.has(contact.id!)
						? 'bg-muted/30'
						: ''}"
				>
					<td class="px-4 py-3">
						<Checkbox
							checked={selectedIds.has(contact.id!)}
							onCheckedChange={() => toggleSelect(contact.id!)}
						/>
					</td>
					<td class="px-4 py-3">
						<div class="flex items-center gap-2 overflow-hidden">
							<button
								class="shrink-0 rounded p-0.5 transition-colors hover:bg-muted"
								disabled={togglingFavoriteId === contact.id}
								onclick={() => toggleFavorite(contact)}
							>
								{#if togglingFavoriteId === contact.id}
									<Loader2 class="h-3.5 w-3.5 animate-spin text-muted-foreground" />
								{:else if contact.isFavorite}
									<Star class="h-3.5 w-3.5 fill-yellow-400 text-yellow-400" />
								{:else}
									<Star class="h-3.5 w-3.5 text-muted-foreground/40" />
								{/if}
							</button>
							<div class="min-w-0">
								<span class="block truncate font-medium">{contact.name}</span>
								{#if contact.email}
									<p class="mt-0.5 truncate text-sm text-muted-foreground">
										{contact.email}
									</p>
								{/if}
							</div>
						</div>
					</td>
					<td class="px-4 py-3 text-sm text-muted-foreground">
						<span class="block truncate">{contact.company ?? ''}</span>
					</td>
					<td class="px-4 py-3">
						<Badge
							variant="secondary"
							class="max-w-full truncate {statusColors[contact.status ?? ''] ?? ''}"
						>
							{statusLabels[contact.status ?? '']?.() ?? contact.status}
						</Badge>
					</td>
					<td class="px-4 py-3 text-sm text-muted-foreground">
						<span class="block truncate"
							>{sourceLabels[contact.source ?? '']?.() ?? contact.source}</span
						>
					</td>
					<td class="px-4 py-3 text-end text-sm font-medium">
						{formatValue(contact.value)}
					</td>
					<td class="px-4 py-3 text-end">
						<div class="flex items-center justify-end gap-1">
							<Button variant="ghost" size="icon" class="h-8 w-8" onclick={() => onEdit(contact)}>
								<Pencil class="h-4 w-4" />
							</Button>
							<Button
								variant="ghost"
								size="icon"
								class="h-8 w-8 text-destructive hover:text-destructive"
								onclick={() => confirmDelete(contact)}
							>
								<Trash2 class="h-4 w-4" />
							</Button>
						</div>
					</td>
				</tr>
			{/each}
		</tbody>
	</table>
</div>

<!-- Mobile/tablet cards -->
<div class="lg:hidden">
	<!-- Mobile select-all header -->
	<div class="flex items-center gap-2 border-b px-4 py-2">
		<Checkbox
			checked={allSelected}
			indeterminate={someSelected}
			onCheckedChange={toggleSelectAll}
		/>
		<span class="text-sm text-muted-foreground">{m.contacts_selectAll()}</span>
	</div>

	<div class="space-y-2 p-2">
		{#each contacts as contact (contact.id)}
			<div
				class="rounded-lg border px-4 py-3 transition-colors hover:bg-muted/50 {selectedIds.has(
					contact.id!
				)
					? 'bg-muted/30'
					: ''}"
			>
				<div class="flex items-start gap-3">
					<div class="pt-0.5">
						<Checkbox
							checked={selectedIds.has(contact.id!)}
							onCheckedChange={() => toggleSelect(contact.id!)}
						/>
					</div>
					<div class="min-w-0 flex-1">
						<div class="flex items-center gap-2">
							<button
								class="shrink-0 rounded p-0.5 transition-colors hover:bg-muted"
								disabled={togglingFavoriteId === contact.id}
								onclick={() => toggleFavorite(contact)}
							>
								{#if togglingFavoriteId === contact.id}
									<Loader2 class="h-3.5 w-3.5 animate-spin text-muted-foreground" />
								{:else if contact.isFavorite}
									<Star class="h-3.5 w-3.5 fill-yellow-400 text-yellow-400" />
								{:else}
									<Star class="h-3.5 w-3.5 text-muted-foreground/40" />
								{/if}
							</button>
							<h4 class="truncate font-medium">{contact.name}</h4>
						</div>
						{#if contact.company}
							<p class="mt-0.5 truncate text-sm text-muted-foreground">{contact.company}</p>
						{/if}
						{#if contact.email}
							<p class="mt-0.5 truncate text-sm text-muted-foreground">{contact.email}</p>
						{/if}
					</div>
				</div>
				<div class="ms-7 mt-2 flex flex-wrap items-center gap-2">
					<Badge
						variant="secondary"
						class="max-w-full truncate {statusColors[contact.status ?? ''] ?? ''}"
					>
						{statusLabels[contact.status ?? '']?.() ?? contact.status}
					</Badge>
					{#if contact.value != null}
						<span class="text-sm font-medium">{formatValue(contact.value)}</span>
					{/if}
					<div class="ms-auto flex items-center gap-1">
						<Button variant="ghost" size="icon" class="h-7 w-7" onclick={() => onEdit(contact)}>
							<Pencil class="h-3.5 w-3.5" />
						</Button>
						<Button
							variant="ghost"
							size="icon"
							class="h-7 w-7 text-destructive hover:text-destructive"
							onclick={() => confirmDelete(contact)}
						>
							<Trash2 class="h-3.5 w-3.5" />
						</Button>
					</div>
				</div>
			</div>
		{/each}
	</div>
</div>

<!-- Delete confirmation dialog -->
<Dialog.Root bind:open={deleteConfirmOpen}>
	<Dialog.Content>
		<Dialog.Header>
			<Dialog.Title>{m.contacts_delete_title()}</Dialog.Title>
			<Dialog.Description>{m.contacts_delete_description()}</Dialog.Description>
		</Dialog.Header>
		<Dialog.Footer class="flex-col-reverse sm:flex-row">
			<Button variant="outline" onclick={() => (deleteConfirmOpen = false)}>
				{m.common_cancel()}
			</Button>
			<Button variant="destructive" disabled={isDeleting} onclick={deleteContact}>
				{m.contacts_delete_confirm()}
			</Button>
		</Dialog.Footer>
	</Dialog.Content>
</Dialog.Root>

<!-- Bulk delete confirmation dialog -->
<Dialog.Root bind:open={bulkDeleteConfirmOpen}>
	<Dialog.Content>
		<Dialog.Header>
			<Dialog.Title>{m.contacts_bulkDelete_title()}</Dialog.Title>
			<Dialog.Description>
				{m.contacts_bulkDelete_description({ count: effectiveSelectionCount })}
			</Dialog.Description>
		</Dialog.Header>
		<Dialog.Footer class="flex-col-reverse sm:flex-row">
			<Button variant="outline" onclick={() => (bulkDeleteConfirmOpen = false)}>
				{m.common_cancel()}
			</Button>
			<Button variant="destructive" disabled={isBulkDeleting} onclick={bulkDelete}>
				{#if isBulkDeleting}
					<Loader2 class="me-2 h-4 w-4 animate-spin" />
				{/if}
				{m.contacts_bulkDelete_confirm({ count: effectiveSelectionCount })}
			</Button>
		</Dialog.Footer>
	</Dialog.Content>
</Dialog.Root>
