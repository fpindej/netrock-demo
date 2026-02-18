<script lang="ts">
	import * as Card from '$lib/components/ui/card';
	import { Button } from '$lib/components/ui/button';
	import { Plus } from '@lucide/svelte';
	import {
		NoteTable,
		CreateNoteDialog,
		EditNoteDialog,
		NoteEmptyState
	} from '$lib/components/notes';
	import * as m from '$lib/paraglide/messages';
	import type { Note } from '$lib/types';
	import type { PageData } from './$types';

	let { data }: { data: PageData } = $props();

	let createDialogOpen = $state(false);
	let editDialogOpen = $state(false);
	let editingNote = $state<Note | null>(null);

	function handleEdit(note: Note) {
		editingNote = note;
		editDialogOpen = true;
	}
</script>

<svelte:head>
	<title>{m.meta_titleTemplate({ title: m.meta_notes_title() })}</title>
	<meta name="description" content={m.meta_notes_description()} />
</svelte:head>

<div class="space-y-6">
	<div class="flex items-center justify-between">
		<div>
			<h3 class="text-lg font-medium">{m.notes_title()}</h3>
			<p class="text-sm text-muted-foreground">{m.notes_description()}</p>
		</div>
		{#if data.notes.length > 0}
			<Button onclick={() => (createDialogOpen = true)}>
				<Plus class="me-2 h-4 w-4" />
				{m.notes_newNote()}
			</Button>
		{/if}
	</div>
	<div class="h-px w-full bg-border"></div>

	{#if data.notes.length === 0}
		<NoteEmptyState onCreate={() => (createDialogOpen = true)} />
	{:else}
		<Card.Root>
			<Card.Content class="p-0">
				<NoteTable notes={data.notes} onEdit={handleEdit} />
			</Card.Content>
		</Card.Root>
	{/if}
</div>

<CreateNoteDialog bind:open={createDialogOpen} />
<EditNoteDialog bind:open={editDialogOpen} note={editingNote} />
