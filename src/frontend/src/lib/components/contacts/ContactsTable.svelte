<script lang="ts">
	import { ContactStatusBadge } from '$lib/components/contacts';
	import { Checkbox } from '$lib/components/ui/checkbox';
	import { Button } from '$lib/components/ui/button';
	import { Pencil, Trash2, Users } from '@lucide/svelte';
	import * as m from '$lib/paraglide/messages';
	import type { ContactResponse } from '$lib/types/contacts';

	interface Props {
		contacts: ContactResponse[];
		onEdit: (contact: ContactResponse) => void;
		onDelete: (contact: ContactResponse) => void;
		selectedIds: Set<string>;
		onToggle: (id: string) => void;
		onToggleAll: () => void;
	}

	let { contacts, onEdit, onDelete, selectedIds, onToggle, onToggleAll }: Props = $props();

	let allChecked = $derived(contacts.length > 0 && contacts.every((c) => selectedIds.has(c.id)));
	let someChecked = $derived(contacts.some((c) => selectedIds.has(c.id)) && !allChecked);

	const sourceLabels: Record<string, () => string> = {
		Web: () => m.contacts_source_web(),
		Email: () => m.contacts_source_email(),
		Phone: () => m.contacts_source_phone(),
		SocialMedia: () => m.contacts_source_socialMedia(),
		Referral: () => m.contacts_source_referral(),
		Other: () => m.contacts_source_other()
	};

	function formatValue(value: number): string {
		return new Intl.NumberFormat('en-US', {
			style: 'currency',
			currency: 'USD',
			minimumFractionDigits: 0,
			maximumFractionDigits: 0
		}).format(value);
	}

	function getSourceLabel(source: string): string {
		return sourceLabels[source]?.() ?? source;
	}
</script>

{#if contacts.length === 0}
	<div class="flex flex-col items-center justify-center py-12 text-center">
		<div class="mb-3 rounded-full bg-muted p-3">
			<Users class="h-6 w-6 text-muted-foreground" />
		</div>
		<p class="text-sm text-muted-foreground">{m.contacts_empty_title()}</p>
	</div>
{:else}
	<!-- Mobile: card list -->
	<div class="divide-y md:hidden">
		{#each contacts as contact (contact.id)}
			<div class="flex items-center gap-3 p-4">
				<Checkbox
					checked={selectedIds.has(contact.id)}
					onCheckedChange={() => onToggle(contact.id)}
					aria-label="Select {contact.firstName} {contact.lastName}"
				/>
				<div class="min-w-0 flex-1">
					<div class="flex items-center gap-2">
						<p class="truncate text-sm font-medium">
							{contact.firstName}
							{contact.lastName}
						</p>
						<ContactStatusBadge status={contact.status} />
					</div>
					<p class="mt-0.5 truncate text-xs text-muted-foreground">
						{contact.email}
					</p>
					<p class="mt-0.5 text-xs font-medium text-muted-foreground">
						{formatValue(contact.value)}
					</p>
				</div>
				<div class="flex shrink-0 items-center gap-1">
					<Button
						variant="ghost"
						size="icon"
						class="h-9 w-9"
						onclick={() => onEdit(contact)}
						aria-label={m.contacts_edit()}
					>
						<Pencil class="h-4 w-4" />
					</Button>
					<Button
						variant="ghost"
						size="icon"
						class="h-9 w-9 text-destructive hover:text-destructive"
						onclick={() => onDelete(contact)}
						aria-label={m.contacts_deleteConfirm_title()}
					>
						<Trash2 class="h-4 w-4" />
					</Button>
				</div>
			</div>
		{/each}
	</div>

	<!-- Desktop: table -->
	<div class="hidden overflow-x-auto md:block">
		<table class="w-full text-sm">
			<thead>
				<tr class="border-b bg-muted/50 text-start">
					<th class="w-12 px-4 py-3">
						<Checkbox
							checked={allChecked}
							indeterminate={someChecked}
							onCheckedChange={onToggleAll}
							aria-label="Select all"
						/>
					</th>
					<th class="px-4 py-3 text-start text-xs font-medium tracking-wide text-muted-foreground">
						{m.contacts_column_name()}
					</th>
					<th
						class="hidden px-4 py-3 text-start text-xs font-medium tracking-wide text-muted-foreground lg:table-cell"
					>
						{m.contacts_column_company()}
					</th>
					<th class="px-4 py-3 text-start text-xs font-medium tracking-wide text-muted-foreground">
						{m.contacts_column_status()}
					</th>
					<th
						class="hidden px-4 py-3 text-start text-xs font-medium tracking-wide text-muted-foreground lg:table-cell"
					>
						{m.contacts_column_source()}
					</th>
					<th class="px-4 py-3 text-end text-xs font-medium tracking-wide text-muted-foreground">
						{m.contacts_column_value()}
					</th>
					<th class="w-20 px-4 py-3">
						<span class="sr-only">{m.contacts_edit()}</span>
					</th>
				</tr>
			</thead>
			<tbody>
				{#each contacts as contact (contact.id)}
					<tr class="border-b transition-colors hover:bg-muted/50">
						<td class="px-4 py-3">
							<Checkbox
								checked={selectedIds.has(contact.id)}
								onCheckedChange={() => onToggle(contact.id)}
								aria-label="Select {contact.firstName} {contact.lastName}"
							/>
						</td>
						<td class="max-w-48 px-4 py-3">
							<div class="min-w-0">
								<p class="truncate font-medium">
									{contact.firstName}
									{contact.lastName}
								</p>
								<p class="mt-0.5 truncate text-xs text-muted-foreground">
									{contact.email}
								</p>
							</div>
						</td>
						<td class="hidden max-w-32 px-4 py-3 text-muted-foreground lg:table-cell">
							<span class="block truncate">{contact.company ?? '\u2014'}</span>
						</td>
						<td class="px-4 py-3">
							<ContactStatusBadge status={contact.status} />
						</td>
						<td class="hidden px-4 py-3 text-muted-foreground lg:table-cell">
							{getSourceLabel(contact.source)}
						</td>
						<td class="px-4 py-3 text-end font-medium tabular-nums">
							{formatValue(contact.value)}
						</td>
						<td class="px-4 py-3 text-end">
							<div class="flex items-center justify-end gap-1">
								<Button
									variant="ghost"
									size="icon"
									class="h-9 w-9"
									onclick={() => onEdit(contact)}
									aria-label={m.contacts_edit()}
								>
									<Pencil class="h-4 w-4" />
								</Button>
								<Button
									variant="ghost"
									size="icon"
									class="h-9 w-9 text-destructive hover:text-destructive"
									onclick={() => onDelete(contact)}
									aria-label={m.contacts_deleteConfirm_title()}
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
{/if}
