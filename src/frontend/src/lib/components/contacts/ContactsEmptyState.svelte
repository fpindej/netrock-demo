<script lang="ts">
	import { Button } from '$lib/components/ui/button';
	import { Users, Loader2 } from '@lucide/svelte';
	import * as m from '$lib/paraglide/messages';

	interface Props {
		onGenerate: () => void;
		onCreate: () => void;
		isGenerating: boolean;
	}

	let { onGenerate, onCreate, isGenerating }: Props = $props();
</script>

<div class="flex flex-col items-center justify-center py-16 text-center">
	<div class="mb-4 rounded-full bg-muted p-4">
		<Users class="h-8 w-8 text-muted-foreground" />
	</div>
	<h3 class="mb-1 text-lg font-medium">{m.contacts_empty_title()}</h3>
	<p class="mb-6 max-w-sm text-sm text-muted-foreground">
		{m.contacts_empty_description()}
	</p>
	<div class="flex flex-col items-center gap-3">
		<Button onclick={onGenerate} disabled={isGenerating}>
			{#if isGenerating}
				<Loader2 class="me-2 h-4 w-4 animate-spin" />
			{/if}
			{m.contacts_generate({ count: 10 })}
		</Button>
		<button
			type="button"
			class="text-sm text-muted-foreground underline-offset-4 hover:underline"
			onclick={onCreate}
		>
			{m.contacts_createManually()}
		</button>
	</div>
</div>
