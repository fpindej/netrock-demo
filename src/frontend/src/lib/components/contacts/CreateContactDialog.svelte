<script lang="ts">
	import * as Dialog from '$lib/components/ui/dialog';
	import { Button } from '$lib/components/ui/button';
	import { Input } from '$lib/components/ui/input';
	import { Label } from '$lib/components/ui/label';
	import { Textarea } from '$lib/components/ui/textarea';
	import * as Select from '$lib/components/ui/select';
	import { Loader2 } from '@lucide/svelte';
	import { toast } from '$lib/components/ui/sonner';
	import { invalidateAll } from '$app/navigation';
	import * as m from '$lib/paraglide/messages';
	import type { ContactResponse, ContactStatusEnum, ContactSourceEnum } from '$lib/types/contacts';

	interface Props {
		open: boolean;
		contact?: ContactResponse | null;
	}

	let { open = $bindable(), contact = null }: Props = $props();

	let isEditing = $derived(contact !== null);

	let firstName = $state('');
	let lastName = $state('');
	let email = $state('');
	let company = $state('');
	let phone = $state('');
	let status = $state<ContactStatusEnum>('Lead');
	let source = $state<ContactSourceEnum>('Web');
	let value = $state('0');
	let notes = $state('');
	let isSaving = $state(false);
	let fieldErrors = $state<Record<string, string>>({});

	function populateForm() {
		if (contact) {
			firstName = contact.firstName;
			lastName = contact.lastName;
			email = contact.email;
			company = contact.company ?? '';
			phone = contact.phone ?? '';
			status = contact.status as ContactStatusEnum;
			source = contact.source as ContactSourceEnum;
			value = String(contact.value);
			notes = contact.notes ?? '';
		} else {
			resetForm();
		}
	}

	$effect(() => {
		if (open) {
			populateForm();
		}
	});

	function resetForm() {
		firstName = '';
		lastName = '';
		email = '';
		company = '';
		phone = '';
		status = 'Lead';
		source = 'Web';
		value = '0';
		notes = '';
		fieldErrors = {};
	}

	async function handleSubmit(e: Event) {
		e.preventDefault();
		if (!firstName.trim() || !lastName.trim() || !email.trim()) return;
		isSaving = true;
		fieldErrors = {};

		const body = {
			firstName: firstName.trim(),
			lastName: lastName.trim(),
			email: email.trim(),
			company: company.trim() || null,
			phone: phone.trim() || null,
			status,
			source,
			value: parseFloat(value) || 0,
			notes: notes.trim() || null
		};

		let response: Response;

		if (isEditing && contact) {
			response = await fetch(`/api/v1/contacts/${contact.id}`, {
				method: 'PUT',
				headers: { 'Content-Type': 'application/json' },
				body: JSON.stringify(body)
			});
		} else {
			response = await fetch('/api/v1/contacts', {
				method: 'POST',
				headers: { 'Content-Type': 'application/json' },
				body: JSON.stringify(body)
			});
		}

		isSaving = false;

		if (response.ok) {
			toast.success(isEditing ? m.contacts_updated() : m.contacts_created());
			resetForm();
			open = false;
			await invalidateAll();
		} else {
			const errorData = await response.json().catch(() => null);
			if (errorData?.errors && typeof errorData.errors === 'object') {
				const mapped: Record<string, string> = {};
				for (const [key, messages] of Object.entries(errorData.errors)) {
					const fieldName = key.charAt(0).toLowerCase() + key.slice(1);
					mapped[fieldName] = (messages as string[])[0] ?? '';
				}
				fieldErrors = mapped;
			} else {
				toast.error(isEditing ? m.contacts_updateError() : m.contacts_createError());
			}
		}
	}

	const statusOptions: { value: ContactStatusEnum; label: () => string }[] = [
		{ value: 'Lead', label: () => m.contacts_status_lead() },
		{ value: 'Prospect', label: () => m.contacts_status_prospect() },
		{ value: 'Customer', label: () => m.contacts_status_customer() },
		{ value: 'Churning', label: () => m.contacts_status_churning() }
	];

	const sourceOptions: { value: ContactSourceEnum; label: () => string }[] = [
		{ value: 'Web', label: () => m.contacts_source_web() },
		{ value: 'Email', label: () => m.contacts_source_email() },
		{ value: 'Phone', label: () => m.contacts_source_phone() },
		{ value: 'SocialMedia', label: () => m.contacts_source_socialMedia() },
		{ value: 'Referral', label: () => m.contacts_source_referral() },
		{ value: 'Other', label: () => m.contacts_source_other() }
	];
</script>

<Dialog.Root bind:open onOpenChange={(isOpen) => !isOpen && resetForm()}>
	<Dialog.Content class="max-h-[90vh] overflow-y-auto sm:max-w-lg">
		<Dialog.Header>
			<Dialog.Title>
				{isEditing ? m.contacts_edit() : m.contacts_create()}
			</Dialog.Title>
			<Dialog.Description>
				{isEditing ? m.contacts_editDescription() : m.contacts_createDescription()}
			</Dialog.Description>
		</Dialog.Header>
		<form onsubmit={handleSubmit}>
			<div class="space-y-4 py-4">
				<div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
					<div>
						<Label for="contact-firstName">{m.contacts_firstName()}</Label>
						<Input
							id="contact-firstName"
							bind:value={firstName}
							maxlength={100}
							aria-invalid={!!fieldErrors.firstName}
							aria-describedby={fieldErrors.firstName ? 'contact-firstName-error' : undefined}
						/>
						{#if fieldErrors.firstName}
							<p id="contact-firstName-error" class="mt-1 text-xs text-destructive">
								{fieldErrors.firstName}
							</p>
						{/if}
					</div>
					<div>
						<Label for="contact-lastName">{m.contacts_lastName()}</Label>
						<Input
							id="contact-lastName"
							bind:value={lastName}
							maxlength={100}
							aria-invalid={!!fieldErrors.lastName}
							aria-describedby={fieldErrors.lastName ? 'contact-lastName-error' : undefined}
						/>
						{#if fieldErrors.lastName}
							<p id="contact-lastName-error" class="mt-1 text-xs text-destructive">
								{fieldErrors.lastName}
							</p>
						{/if}
					</div>
				</div>

				<div>
					<Label for="contact-email">{m.contacts_email()}</Label>
					<Input
						id="contact-email"
						type="email"
						bind:value={email}
						maxlength={256}
						aria-invalid={!!fieldErrors.email}
						aria-describedby={fieldErrors.email ? 'contact-email-error' : undefined}
					/>
					{#if fieldErrors.email}
						<p id="contact-email-error" class="mt-1 text-xs text-destructive">
							{fieldErrors.email}
						</p>
					{/if}
				</div>

				<div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
					<div>
						<Label for="contact-company">{m.contacts_company()}</Label>
						<Input id="contact-company" bind:value={company} maxlength={200} />
					</div>
					<div>
						<Label for="contact-phone">{m.contacts_phone()}</Label>
						<Input id="contact-phone" type="tel" bind:value={phone} maxlength={50} />
					</div>
				</div>

				<div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
					<div>
						<Label>{m.contacts_status()}</Label>
						<Select.Root
							type="single"
							value={status}
							onValueChange={(v) => {
								if (v) status = v as ContactStatusEnum;
							}}
						>
							<Select.Trigger class="w-full">
								{statusOptions.find((o) => o.value === status)?.label() ?? status}
							</Select.Trigger>
							<Select.Content>
								{#each statusOptions as option (option.value)}
									<Select.Item value={option.value} label={option.label()} />
								{/each}
							</Select.Content>
						</Select.Root>
					</div>
					<div>
						<Label>{m.contacts_source()}</Label>
						<Select.Root
							type="single"
							value={source}
							onValueChange={(v) => {
								if (v) source = v as ContactSourceEnum;
							}}
						>
							<Select.Trigger class="w-full">
								{sourceOptions.find((o) => o.value === source)?.label() ?? source}
							</Select.Trigger>
							<Select.Content>
								{#each sourceOptions as option (option.value)}
									<Select.Item value={option.value} label={option.label()} />
								{/each}
							</Select.Content>
						</Select.Root>
					</div>
				</div>

				<div>
					<Label for="contact-value">{m.contacts_value()}</Label>
					<Input id="contact-value" type="number" bind:value min="0" step="100" />
				</div>

				<div>
					<Label for="contact-notes">{m.contacts_notes()}</Label>
					<Textarea id="contact-notes" bind:value={notes} rows={3} maxlength={2000} />
				</div>
			</div>
			<Dialog.Footer class="flex-col-reverse sm:flex-row">
				<Button variant="outline" type="button" onclick={() => (open = false)}>
					{m.common_cancel()}
				</Button>
				<Button
					type="submit"
					disabled={!firstName.trim() || !lastName.trim() || !email.trim() || isSaving}
				>
					{#if isSaving}
						<Loader2 class="me-2 h-4 w-4 animate-spin" />
						{m.contacts_saving()}
					{:else}
						{m.contacts_save()}
					{/if}
				</Button>
			</Dialog.Footer>
		</form>
	</Dialog.Content>
</Dialog.Root>
