<script lang="ts">
	import { Pin, Pencil, Trash2 } from '@lucide/svelte';
	import { Button } from '$lib/components/ui/button';
	import { Badge } from '$lib/components/ui/badge';
	import * as Dialog from '$lib/components/ui/dialog';
	import { browserClient } from '$lib/api';
	import { toast } from '$lib/components/ui/sonner';
	import { invalidateAll } from '$app/navigation';
	import * as m from '$lib/paraglide/messages';
	import type { Note } from '$lib/types';

	interface Props {
		notes: Note[];
		onEdit: (note: Note) => void;
	}

	let { notes, onEdit }: Props = $props();

	const categoryLabels: Record<string, () => string> = {
		Personal: m.notes_category_Personal,
		Work: m.notes_category_Work,
		Ideas: m.notes_category_Ideas
	};

	const categoryColors: Record<string, string> = {
		Personal: 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-300',
		Work: 'bg-purple-100 text-purple-800 dark:bg-purple-900/30 dark:text-purple-300',
		Ideas: 'bg-amber-100 text-amber-800 dark:bg-amber-900/30 dark:text-amber-300'
	};

	let deleteConfirmOpen = $state(false);
	let noteToDelete = $state<Note | null>(null);
	let isDeleting = $state(false);

	function confirmDelete(note: Note) {
		noteToDelete = note;
		deleteConfirmOpen = true;
	}

	async function deleteNote() {
		if (!noteToDelete) return;
		isDeleting = true;

		const { response } = await browserClient.DELETE('/api/v1/notes/{id}', {
			params: { path: { id: noteToDelete.id! } }
		});

		isDeleting = false;

		if (response.ok) {
			toast.success(m.notes_delete_success());
			deleteConfirmOpen = false;
			noteToDelete = null;
			await invalidateAll();
		} else {
			toast.error(m.notes_delete_error());
		}
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
				<th class="px-4 py-3 text-start font-medium">{m.notes_table_title()}</th>
				<th class="px-4 py-3 text-start font-medium">{m.notes_table_category()}</th>
				<th class="px-4 py-3 text-start font-medium">{m.notes_table_date()}</th>
				<th class="px-4 py-3 text-end font-medium"></th>
			</tr>
		</thead>
		<tbody>
			{#each notes as note (note.id)}
				<tr class="border-b transition-colors hover:bg-muted/50">
					<td class="px-4 py-3">
						<div class="flex items-center gap-2">
							{#if note.isPinned}
								<Pin class="h-3.5 w-3.5 shrink-0 text-primary" />
							{/if}
							<span class="font-medium">{note.title}</span>
						</div>
						<p class="mt-0.5 line-clamp-1 text-sm text-muted-foreground">
							{note.content}
						</p>
					</td>
					<td class="px-4 py-3">
						<Badge variant="secondary" class={categoryColors[note.category ?? ''] ?? ''}>
							{categoryLabels[note.category ?? '']?.() ?? note.category}
						</Badge>
					</td>
					<td class="px-4 py-3 text-sm text-muted-foreground">
						{formatDate(note.createdAt ?? '')}
					</td>
					<td class="px-4 py-3 text-end">
						<div class="flex items-center justify-end gap-1">
							<Button variant="ghost" size="icon" class="h-8 w-8" onclick={() => onEdit(note)}>
								<Pencil class="h-4 w-4" />
							</Button>
							<Button
								variant="ghost"
								size="icon"
								class="h-8 w-8 text-destructive hover:text-destructive"
								onclick={() => confirmDelete(note)}
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
	{#each notes as note (note.id)}
		<div class="rounded-lg border p-4 transition-colors hover:bg-muted/50">
			<div class="flex items-start justify-between gap-2">
				<div class="min-w-0 flex-1">
					<div class="flex items-center gap-2">
						{#if note.isPinned}
							<Pin class="h-3.5 w-3.5 shrink-0 text-primary" />
						{/if}
						<h4 class="truncate font-medium">{note.title}</h4>
					</div>
					<p class="mt-1 line-clamp-2 text-sm text-muted-foreground">{note.content}</p>
				</div>
				<div class="flex shrink-0 items-center gap-1">
					<Button variant="ghost" size="icon" class="h-8 w-8" onclick={() => onEdit(note)}>
						<Pencil class="h-4 w-4" />
					</Button>
					<Button
						variant="ghost"
						size="icon"
						class="h-8 w-8 text-destructive hover:text-destructive"
						onclick={() => confirmDelete(note)}
					>
						<Trash2 class="h-4 w-4" />
					</Button>
				</div>
			</div>
			<div class="mt-2 flex items-center gap-2">
				<Badge variant="secondary" class={categoryColors[note.category ?? ''] ?? ''}>
					{categoryLabels[note.category ?? '']?.() ?? note.category}
				</Badge>
				<span class="text-xs text-muted-foreground">{formatDate(note.createdAt ?? '')}</span>
			</div>
		</div>
	{/each}
</div>

<!-- Delete confirmation dialog -->
<Dialog.Root bind:open={deleteConfirmOpen}>
	<Dialog.Content>
		<Dialog.Header>
			<Dialog.Title>{m.notes_delete_title()}</Dialog.Title>
			<Dialog.Description>{m.notes_delete_description()}</Dialog.Description>
		</Dialog.Header>
		<Dialog.Footer class="flex-col-reverse sm:flex-row">
			<Button variant="outline" onclick={() => (deleteConfirmOpen = false)}>
				{m.common_cancel()}
			</Button>
			<Button variant="destructive" disabled={isDeleting} onclick={deleteNote}>
				{m.notes_delete_confirm()}
			</Button>
		</Dialog.Footer>
	</Dialog.Content>
</Dialog.Root>
