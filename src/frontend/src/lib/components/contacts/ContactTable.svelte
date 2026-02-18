<script lang="ts">
	import { Star, Pencil, Trash2 } from '@lucide/svelte';
	import { Button } from '$lib/components/ui/button';
	import { Badge } from '$lib/components/ui/badge';
	import * as Dialog from '$lib/components/ui/dialog';
	import { browserClient } from '$lib/api';
	import { toast } from '$lib/components/ui/sonner';
	import { invalidateAll } from '$app/navigation';
	import * as m from '$lib/paraglide/messages';
	import type { Contact } from '$lib/types';

	interface Props {
		contacts: Contact[];
		onEdit: (contact: Contact) => void;
	}

	let { contacts, onEdit }: Props = $props();

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

	function formatValue(value: number | null | undefined) {
		if (value == null) return '';
		return new Intl.NumberFormat(undefined, {
			style: 'currency',
			currency: 'USD',
			minimumFractionDigits: 0,
			maximumFractionDigits: 0
		}).format(value);
	}

	function formatDate(dateStr: string) {
		return new Date(dateStr).toLocaleDateString(undefined, {
			month: 'short',
			day: 'numeric',
			year: 'numeric'
		});
	}
</script>

<!-- Desktop table -->
<div class="hidden md:block">
	<table class="w-full">
		<thead>
			<tr class="border-b text-sm text-muted-foreground">
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
				<tr class="border-b transition-colors hover:bg-muted/50">
					<td class="px-4 py-3">
						<div class="flex items-center gap-2">
							{#if contact.isFavorite}
								<Star class="h-3.5 w-3.5 shrink-0 fill-yellow-400 text-yellow-400" />
							{/if}
							<div>
								<span class="font-medium">{contact.name}</span>
								{#if contact.email}
									<p class="mt-0.5 text-sm text-muted-foreground">{contact.email}</p>
								{/if}
							</div>
						</div>
					</td>
					<td class="px-4 py-3 text-sm text-muted-foreground">
						{contact.company ?? ''}
					</td>
					<td class="px-4 py-3">
						<Badge variant="secondary" class={statusColors[contact.status ?? ''] ?? ''}>
							{statusLabels[contact.status ?? '']?.() ?? contact.status}
						</Badge>
					</td>
					<td class="px-4 py-3 text-sm text-muted-foreground">
						{sourceLabels[contact.source ?? '']?.() ?? contact.source}
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

<!-- Mobile cards -->
<div class="space-y-3 md:hidden">
	{#each contacts as contact (contact.id)}
		<div class="rounded-lg border p-4 transition-colors hover:bg-muted/50">
			<div class="flex items-start justify-between gap-2">
				<div class="min-w-0 flex-1">
					<div class="flex items-center gap-2">
						{#if contact.isFavorite}
							<Star class="h-3.5 w-3.5 shrink-0 fill-yellow-400 text-yellow-400" />
						{/if}
						<h4 class="truncate font-medium">{contact.name}</h4>
					</div>
					{#if contact.company}
						<p class="mt-0.5 text-sm text-muted-foreground">{contact.company}</p>
					{/if}
					{#if contact.email}
						<p class="mt-0.5 text-sm text-muted-foreground">{contact.email}</p>
					{/if}
				</div>
				<div class="flex shrink-0 items-center gap-1">
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
			</div>
			<div class="mt-2 flex flex-wrap items-center gap-2">
				<Badge variant="secondary" class={statusColors[contact.status ?? ''] ?? ''}>
					{statusLabels[contact.status ?? '']?.() ?? contact.status}
				</Badge>
				{#if contact.value != null}
					<span class="text-sm font-medium">{formatValue(contact.value)}</span>
				{/if}
				<span class="text-xs text-muted-foreground">{formatDate(contact.createdAt ?? '')}</span>
			</div>
		</div>
	{/each}
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
