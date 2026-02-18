<script lang="ts">
	import * as Dialog from '$lib/components/ui/dialog';
	import { Button } from '$lib/components/ui/button';
	import { Input } from '$lib/components/ui/input';
	import { Textarea } from '$lib/components/ui/textarea';
	import { Label } from '$lib/components/ui/label';
	import * as Select from '$lib/components/ui/select';
	import { Checkbox } from '$lib/components/ui/checkbox';
	import { Loader2 } from '@lucide/svelte';
	import { browserClient, handleMutationError } from '$lib/api';
	import { toast } from '$lib/components/ui/sonner';
	import { invalidateAll } from '$app/navigation';
	import { createCooldown, createFieldShakes } from '$lib/state';
	import * as m from '$lib/paraglide/messages';
	import type { Note } from '$lib/types';

	interface Props {
		open: boolean;
		note: Note | null;
	}

	let { open = $bindable(), note }: Props = $props();

	const categories = ['Personal', 'Work', 'Ideas'] as const;
	const categoryLabels: Record<string, () => string> = {
		Personal: m.notes_category_Personal,
		Work: m.notes_category_Work,
		Ideas: m.notes_category_Ideas
	};

	let title = $state('');
	let content = $state('');
	let category = $state<string>('Personal');
	let isPinned = $state(false);
	let isSaving = $state(false);
	let fieldErrors = $state<Record<string, string>>({});
	const cooldown = createCooldown();
	const fieldShakes = createFieldShakes();

	$effect(() => {
		if (note) {
			title = note.title ?? '';
			content = note.content ?? '';
			category = note.category ?? 'Personal';
			isPinned = note.isPinned ?? false;
		}
	});

	async function saveNote() {
		if (!note?.id || !title.trim() || !content.trim()) return;
		isSaving = true;
		fieldErrors = {};

		const { response, error } = await browserClient.PUT('/api/v1/notes/{id}', {
			params: { path: { id: note.id } },
			body: {
				title: title.trim(),
				content: content.trim(),
				category: category as 'Personal' | 'Work' | 'Ideas',
				isPinned
			}
		});

		isSaving = false;

		if (response.ok) {
			toast.success(m.notes_edit_success());
			open = false;
			await invalidateAll();
		} else {
			handleMutationError(response, error, {
				cooldown,
				fallback: m.notes_edit_error(),
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
			<Dialog.Title>{m.notes_edit_title()}</Dialog.Title>
			<Dialog.Description>{m.notes_edit_description()}</Dialog.Description>
		</Dialog.Header>
		<div class="space-y-4 py-4">
			<div class="grid gap-2">
				<Label for="edit-note-title">{m.notes_create_titleLabel()}</Label>
				<Input
					id="edit-note-title"
					bind:value={title}
					placeholder={m.notes_create_titlePlaceholder()}
					maxlength={200}
					class={fieldShakes.class('title')}
					aria-invalid={!!fieldErrors.title}
				/>
				{#if fieldErrors.title}
					<p class="text-xs text-destructive">{fieldErrors.title}</p>
				{/if}
			</div>
			<div class="grid gap-2">
				<Label for="edit-note-content">{m.notes_create_contentLabel()}</Label>
				<Textarea
					id="edit-note-content"
					bind:value={content}
					placeholder={m.notes_create_contentPlaceholder()}
					rows={4}
					class={fieldShakes.class('content')}
					aria-invalid={!!fieldErrors.content}
				/>
				{#if fieldErrors.content}
					<p class="text-xs text-destructive">{fieldErrors.content}</p>
				{/if}
			</div>
			<div class="grid gap-2">
				<Label>{m.notes_create_categoryLabel()}</Label>
				<Select.Root type="single" value={category} onValueChange={(v) => (category = v)}>
					<Select.Trigger class="w-full">
						{categoryLabels[category]?.() ?? category}
					</Select.Trigger>
					<Select.Content>
						{#each categories as cat (cat)}
							<Select.Item value={cat}>{categoryLabels[cat]()}</Select.Item>
						{/each}
					</Select.Content>
				</Select.Root>
			</div>
			<div class="flex items-center gap-2">
				<Checkbox id="edit-note-pinned" bind:checked={isPinned} />
				<Label for="edit-note-pinned" class="cursor-pointer">
					{m.notes_edit_pinLabel()}
				</Label>
			</div>
		</div>
		<Dialog.Footer class="flex-col-reverse sm:flex-row">
			<Button variant="outline" onclick={() => (open = false)}>
				{m.common_cancel()}
			</Button>
			<Button
				disabled={!title.trim() || !content.trim() || isSaving || cooldown.active}
				onclick={saveNote}
			>
				{#if cooldown.active}
					{m.common_waitSeconds({ seconds: cooldown.remaining })}
				{:else}
					{#if isSaving}
						<Loader2 class="me-2 h-4 w-4 animate-spin" />
					{/if}
					{m.notes_edit_submit()}
				{/if}
			</Button>
		</Dialog.Footer>
	</Dialog.Content>
</Dialog.Root>
