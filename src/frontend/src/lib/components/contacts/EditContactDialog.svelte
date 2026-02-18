<script lang="ts">
	import * as Dialog from '$lib/components/ui/dialog';
	import { Button } from '$lib/components/ui/button';
	import { Input } from '$lib/components/ui/input';
	import { Textarea } from '$lib/components/ui/textarea';
	import { Label } from '$lib/components/ui/label';
	import * as Select from '$lib/components/ui/select';
	import { Checkbox } from '$lib/components/ui/checkbox';
	import { Loader2, ChevronDown, ChevronUp } from '@lucide/svelte';
	import { SvelteSet } from 'svelte/reactivity';
	import { browserClient, handleMutationError } from '$lib/api';
	import { toast } from '$lib/components/ui/sonner';
	import { invalidateAll } from '$app/navigation';
	import { createCooldown, createFieldShakes } from '$lib/state';
	import {
		getAuditActionLabel,
		getAuditActionVariant,
		getAuditDescription,
		getAuditChanges,
		getContactFieldLabel,
		formatChangeValue,
		formatAuditDate
	} from '$lib/utils/audit';
	import { Timeline, TimelineItem, TimelineContent } from '$lib/components/ui/timeline';
	import * as m from '$lib/paraglide/messages';
	import type { Contact, AuditEvent } from '$lib/types';

	interface Props {
		open: boolean;
		contact: Contact | null;
	}

	let { open = $bindable(), contact }: Props = $props();

	const statuses = ['Lead', 'Prospect', 'Customer', 'Churned'] as const;
	const statusLabels: Record<string, () => string> = {
		Lead: m.contacts_status_Lead,
		Prospect: m.contacts_status_Prospect,
		Customer: m.contacts_status_Customer,
		Churned: m.contacts_status_Churned
	};

	const sources = ['Website', 'Referral', 'Social', 'Email', 'Phone', 'Other'] as const;
	const sourceLabels: Record<string, () => string> = {
		Website: m.contacts_source_Website,
		Referral: m.contacts_source_Referral,
		Social: m.contacts_source_Social,
		Email: m.contacts_source_Email,
		Phone: m.contacts_source_Phone,
		Other: m.contacts_source_Other
	};

	let name = $state('');
	let email = $state('');
	let company = $state('');
	let phone = $state('');
	let status = $state<string>('Lead');
	let source = $state<string>('Website');
	let value = $state('');
	let notes = $state('');
	let isFavorite = $state(false);
	let isSaving = $state(false);
	let fieldErrors = $state<Record<string, string>>({});
	let auditEvents = $state<AuditEvent[]>([]);
	let expandedEvents = new SvelteSet<string>();
	const cooldown = createCooldown();
	const fieldShakes = createFieldShakes();

	function toggleExpanded(eventId: string) {
		if (expandedEvents.has(eventId)) {
			expandedEvents.delete(eventId);
		} else {
			expandedEvents.add(eventId);
		}
	}

	$effect(() => {
		if (contact) {
			name = contact.name ?? '';
			email = contact.email ?? '';
			company = contact.company ?? '';
			phone = contact.phone ?? '';
			status = contact.status ?? 'Lead';
			source = contact.source ?? 'Website';
			value = contact.value != null ? String(contact.value) : '';
			notes = contact.notes ?? '';
			isFavorite = contact.isFavorite ?? false;
			expandedEvents.clear();
			loadAudit(contact.id!);
		}
	});

	async function loadAudit(contactId: string) {
		try {
			const { data, response } = await browserClient.GET('/api/v1/contacts/{id}/audit', {
				params: { path: { id: contactId }, query: { page: 1, pageSize: 5 } }
			});
			if (response.ok && data) {
				auditEvents = (data as { events?: AuditEvent[] }).events ?? [];
			}
		} catch {
			// Silently fail — audit is supplementary
		}
	}

	async function saveContact() {
		if (!contact?.id || !name.trim()) return;
		isSaving = true;
		fieldErrors = {};

		const { response, error } = await browserClient.PUT('/api/v1/contacts/{id}', {
			params: { path: { id: contact.id } },
			body: {
				name: name.trim(),
				email: email.trim() || null,
				company: company.trim() || null,
				phone: phone.trim() || null,
				status: status as 'Lead' | 'Prospect' | 'Customer' | 'Churned',
				source: source as 'Website' | 'Referral' | 'Social' | 'Email' | 'Phone' | 'Other',
				value: value ? parseFloat(value) : null,
				notes: notes.trim() || null,
				isFavorite
			}
		});

		isSaving = false;

		if (response.ok) {
			toast.success(m.contacts_edit_success());
			open = false;
			await invalidateAll();
		} else {
			handleMutationError(response, error, {
				cooldown,
				fallback: m.contacts_edit_error(),
				onValidationError(errors) {
					fieldErrors = errors;
					fieldShakes.triggerFields(Object.keys(errors));
				}
			});
		}
	}
</script>

<Dialog.Root bind:open>
	<Dialog.Content class="sm:max-w-lg">
		<Dialog.Header>
			<Dialog.Title>{m.contacts_edit_title()}</Dialog.Title>
			<Dialog.Description>{m.contacts_edit_description()}</Dialog.Description>
		</Dialog.Header>
		<div class="scrollbar-thin max-h-[60vh] space-y-4 overflow-y-auto py-4 pe-1">
			<div class="grid gap-2">
				<Label for="edit-contact-name">{m.contacts_field_name()}</Label>
				<Input
					id="edit-contact-name"
					bind:value={name}
					placeholder={m.contacts_field_namePlaceholder()}
					maxlength={200}
					class={fieldShakes.class('name')}
					aria-invalid={!!fieldErrors.name}
				/>
				{#if fieldErrors.name}
					<p class="text-xs text-destructive">{fieldErrors.name}</p>
				{/if}
			</div>
			<div class="grid gap-4 sm:grid-cols-2">
				<div class="grid gap-2">
					<Label for="edit-contact-email">{m.contacts_field_email()}</Label>
					<Input
						id="edit-contact-email"
						type="email"
						bind:value={email}
						placeholder={m.contacts_field_emailPlaceholder()}
						maxlength={256}
						class={fieldShakes.class('email')}
						aria-invalid={!!fieldErrors.email}
					/>
					{#if fieldErrors.email}
						<p class="text-xs text-destructive">{fieldErrors.email}</p>
					{/if}
				</div>
				<div class="grid gap-2">
					<Label for="edit-contact-phone">{m.contacts_field_phone()}</Label>
					<Input
						id="edit-contact-phone"
						bind:value={phone}
						placeholder={m.contacts_field_phonePlaceholder()}
						maxlength={50}
						class={fieldShakes.class('phone')}
						aria-invalid={!!fieldErrors.phone}
					/>
					{#if fieldErrors.phone}
						<p class="text-xs text-destructive">{fieldErrors.phone}</p>
					{/if}
				</div>
			</div>
			<div class="grid gap-2">
				<Label for="edit-contact-company">{m.contacts_field_company()}</Label>
				<Input
					id="edit-contact-company"
					bind:value={company}
					placeholder={m.contacts_field_companyPlaceholder()}
					maxlength={200}
					class={fieldShakes.class('company')}
					aria-invalid={!!fieldErrors.company}
				/>
				{#if fieldErrors.company}
					<p class="text-xs text-destructive">{fieldErrors.company}</p>
				{/if}
			</div>
			<div class="grid gap-4 sm:grid-cols-2">
				<div class="grid gap-2">
					<Label>{m.contacts_field_status()}</Label>
					<Select.Root type="single" value={status} onValueChange={(v) => (status = v)}>
						<Select.Trigger class="w-full">
							{statusLabels[status]?.() ?? status}
						</Select.Trigger>
						<Select.Content>
							{#each statuses as s (s)}
								<Select.Item value={s}>{statusLabels[s]()}</Select.Item>
							{/each}
						</Select.Content>
					</Select.Root>
				</div>
				<div class="grid gap-2">
					<Label>{m.contacts_field_source()}</Label>
					<Select.Root type="single" value={source} onValueChange={(v) => (source = v)}>
						<Select.Trigger class="w-full">
							{sourceLabels[source]?.() ?? source}
						</Select.Trigger>
						<Select.Content>
							{#each sources as s (s)}
								<Select.Item value={s}>{sourceLabels[s]()}</Select.Item>
							{/each}
						</Select.Content>
					</Select.Root>
				</div>
			</div>
			<div class="grid gap-2">
				<Label for="edit-contact-value">{m.contacts_field_value()}</Label>
				<Input
					id="edit-contact-value"
					type="number"
					min="0"
					step="0.01"
					bind:value
					placeholder={m.contacts_field_valuePlaceholder()}
					class={fieldShakes.class('value')}
					aria-invalid={!!fieldErrors.value}
				/>
				{#if fieldErrors.value}
					<p class="text-xs text-destructive">{fieldErrors.value}</p>
				{/if}
			</div>
			<div class="grid gap-2">
				<Label for="edit-contact-notes">{m.contacts_field_notes()}</Label>
				<Textarea
					id="edit-contact-notes"
					bind:value={notes}
					placeholder={m.contacts_field_notesPlaceholder()}
					rows={3}
					class={fieldShakes.class('notes')}
					aria-invalid={!!fieldErrors.notes}
				/>
				{#if fieldErrors.notes}
					<p class="text-xs text-destructive">{fieldErrors.notes}</p>
				{/if}
			</div>
			<div class="flex items-center gap-2">
				<Checkbox id="edit-contact-favorite" bind:checked={isFavorite} />
				<Label for="edit-contact-favorite" class="cursor-pointer">
					{m.contacts_edit_favoriteLabel()}
				</Label>
			</div>

			<!-- Mini audit timeline -->
			{#if auditEvents.length > 0}
				<div class="border-t pt-4">
					<p class="mb-2 text-xs font-semibold tracking-wider text-muted-foreground uppercase">
						{m.contacts_edit_activity()}
					</p>
					<Timeline>
						{#each auditEvents as event, i (event.id)}
							{@const changes = getAuditChanges(event.metadata)}
							{@const isExpanded = event.id ? expandedEvents.has(event.id) : false}
							<TimelineItem
								variant={getAuditActionVariant(event.action)}
								isLast={i === auditEvents.length - 1}
							>
								<TimelineContent
									title={getAuditActionLabel(event.action)}
									timestamp={formatAuditDate(event.createdAt)}
									description={getAuditDescription(event.action, event.metadata)}
								>
									{#if changes}
										<button
											type="button"
											class="mt-1 flex items-center gap-1 text-xs text-muted-foreground hover:text-foreground"
											onclick={() => event.id && toggleExpanded(event.id)}
										>
											{#if isExpanded}
												<ChevronUp class="h-3 w-3" />
												{m.contacts_audit_hideDetails()}
											{:else}
												<ChevronDown class="h-3 w-3" />
												{m.contacts_audit_showDetails()}
											{/if}
										</button>
										{#if isExpanded}
											<ul class="mt-1 space-y-0.5 text-xs text-muted-foreground">
												{#each Object.entries(changes) as [field, change] (field)}
													<li>
														<span class="font-medium">{getContactFieldLabel(field)}:</span>
														{formatChangeValue(field, change.from) || '–'}
														<span class="mx-0.5">&rarr;</span>
														{formatChangeValue(field, change.to) || '–'}
													</li>
												{/each}
											</ul>
										{/if}
									{/if}
								</TimelineContent>
							</TimelineItem>
						{/each}
					</Timeline>
				</div>
			{/if}
		</div>
		<Dialog.Footer class="flex-col-reverse sm:flex-row">
			<Button variant="outline" onclick={() => (open = false)}>
				{m.common_cancel()}
			</Button>
			<Button disabled={!name.trim() || isSaving || cooldown.active} onclick={saveContact}>
				{#if cooldown.active}
					{m.common_waitSeconds({ seconds: cooldown.remaining })}
				{:else}
					{#if isSaving}
						<Loader2 class="me-2 h-4 w-4 animate-spin" />
					{/if}
					{m.contacts_edit_submit()}
				{/if}
			</Button>
		</Dialog.Footer>
	</Dialog.Content>
</Dialog.Root>
