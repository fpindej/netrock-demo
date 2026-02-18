<script lang="ts">
	import * as Dialog from '$lib/components/ui/dialog';
	import { Button } from '$lib/components/ui/button';
	import { Input } from '$lib/components/ui/input';
	import { Textarea } from '$lib/components/ui/textarea';
	import { Label } from '$lib/components/ui/label';
	import * as Select from '$lib/components/ui/select';
	import { Loader2 } from '@lucide/svelte';
	import { browserClient, handleMutationError } from '$lib/api';
	import { toast } from '$lib/components/ui/sonner';
	import { invalidateAll } from '$app/navigation';
	import { createCooldown, createFieldShakes } from '$lib/state';
	import * as m from '$lib/paraglide/messages';

	interface Props {
		open: boolean;
	}

	let { open = $bindable() }: Props = $props();

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
	let isCreating = $state(false);
	let fieldErrors = $state<Record<string, string>>({});
	const cooldown = createCooldown();
	const fieldShakes = createFieldShakes();

	function resetForm() {
		name = '';
		email = '';
		company = '';
		phone = '';
		status = 'Lead';
		source = 'Website';
		value = '';
		notes = '';
		fieldErrors = {};
	}

	async function createContact() {
		if (!name.trim()) return;
		isCreating = true;
		fieldErrors = {};

		const { response, error } = await browserClient.POST('/api/v1/contacts', {
			body: {
				name: name.trim(),
				email: email.trim() || null,
				company: company.trim() || null,
				phone: phone.trim() || null,
				status: status as 'Lead' | 'Prospect' | 'Customer' | 'Churned',
				source: source as 'Website' | 'Referral' | 'Social' | 'Email' | 'Phone' | 'Other',
				value: value ? parseFloat(value) : null,
				notes: notes.trim() || null
			}
		});

		isCreating = false;

		if (response.ok) {
			toast.success(m.contacts_create_success());
			resetForm();
			open = false;
			await invalidateAll();
		} else {
			handleMutationError(response, error, {
				cooldown,
				fallback: m.contacts_create_error(),
				onValidationError(errors) {
					fieldErrors = errors;
					fieldShakes.triggerFields(Object.keys(errors));
				}
			});
		}
	}
</script>

<Dialog.Root bind:open onOpenChange={(isOpen) => !isOpen && resetForm()}>
	<Dialog.Content class="sm:max-w-lg">
		<Dialog.Header>
			<Dialog.Title>{m.contacts_create_title()}</Dialog.Title>
			<Dialog.Description>{m.contacts_create_description()}</Dialog.Description>
		</Dialog.Header>
		<div class="space-y-4 py-4">
			<div class="grid gap-2">
				<Label for="contact-name">{m.contacts_field_name()}</Label>
				<Input
					id="contact-name"
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
					<Label for="contact-email">{m.contacts_field_email()}</Label>
					<Input
						id="contact-email"
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
					<Label for="contact-phone">{m.contacts_field_phone()}</Label>
					<Input
						id="contact-phone"
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
				<Label for="contact-company">{m.contacts_field_company()}</Label>
				<Input
					id="contact-company"
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
				<Label for="contact-value">{m.contacts_field_value()}</Label>
				<Input
					id="contact-value"
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
				<Label for="contact-notes">{m.contacts_field_notes()}</Label>
				<Textarea
					id="contact-notes"
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
		</div>
		<Dialog.Footer class="flex-col-reverse sm:flex-row">
			<Button variant="outline" onclick={() => (open = false)}>
				{m.common_cancel()}
			</Button>
			<Button disabled={!name.trim() || isCreating || cooldown.active} onclick={createContact}>
				{#if cooldown.active}
					{m.common_waitSeconds({ seconds: cooldown.remaining })}
				{:else}
					{#if isCreating}
						<Loader2 class="me-2 h-4 w-4 animate-spin" />
					{/if}
					{m.contacts_create_submit()}
				{/if}
			</Button>
		</Dialog.Footer>
	</Dialog.Content>
</Dialog.Root>
