<script lang="ts">
	import * as Card from '$lib/components/ui/card';
	import { Button } from '$lib/components/ui/button';
	import { Plus } from '@lucide/svelte';
	import {
		ContactTable,
		CreateContactDialog,
		EditContactDialog,
		ContactEmptyState
	} from '$lib/components/contacts';
	import * as m from '$lib/paraglide/messages';
	import type { Contact } from '$lib/types';
	import type { PageData } from './$types';

	let { data }: { data: PageData } = $props();

	let createDialogOpen = $state(false);
	let editDialogOpen = $state(false);
	let editingContact = $state<Contact | null>(null);

	function handleEdit(contact: Contact) {
		editingContact = contact;
		editDialogOpen = true;
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
		{#if data.contacts.length > 0}
			<Button onclick={() => (createDialogOpen = true)}>
				<Plus class="me-2 h-4 w-4" />
				{m.contacts_newContact()}
			</Button>
		{/if}
	</div>
	<div class="h-px w-full bg-border"></div>

	{#if data.contacts.length === 0}
		<ContactEmptyState onCreate={() => (createDialogOpen = true)} />
	{:else}
		<Card.Root>
			<Card.Content class="p-0">
				<ContactTable contacts={data.contacts} onEdit={handleEdit} />
			</Card.Content>
		</Card.Root>
	{/if}
</div>

<CreateContactDialog bind:open={createDialogOpen} />
<EditContactDialog bind:open={editDialogOpen} contact={editingContact} />
