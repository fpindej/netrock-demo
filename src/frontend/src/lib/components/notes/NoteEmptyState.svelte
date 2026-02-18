<script lang="ts">
	import { StickyNote } from '@lucide/svelte';
	import { Button } from '$lib/components/ui/button';
	import * as m from '$lib/paraglide/messages';

	interface Props {
		onCreate: () => void;
	}

	let { onCreate }: Props = $props();
</script>

<div class="flex flex-col items-center justify-center py-16 text-center">
	<div class="mb-4 rounded-full bg-muted p-4">
		<StickyNote class="h-8 w-8 text-muted-foreground" />
	</div>
	<h3 class="mb-1 text-lg font-semibold">{m.notes_empty_title()}</h3>
	<p class="mb-6 max-w-sm text-sm text-muted-foreground">
		{m.notes_empty_description()}
	</p>
	<Button onclick={onCreate}>{m.notes_empty_cta()}</Button>
</div>
