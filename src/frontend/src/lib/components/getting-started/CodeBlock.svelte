<script lang="ts">
	import { Check, Copy } from '@lucide/svelte';
	import * as m from '$lib/paraglide/messages';

	interface Props {
		code: string;
		lang?: string;
	}

	let { code, lang }: Props = $props();

	let copied = $state(false);
	let timeout: ReturnType<typeof setTimeout> | undefined;

	function copyToClipboard() {
		navigator.clipboard.writeText(code);
		copied = true;
		clearTimeout(timeout);
		timeout = setTimeout(() => (copied = false), 1500);
	}
</script>

<div class="group relative rounded-lg bg-zinc-900 dark:bg-zinc-950">
	{#if lang}
		<span
			class="absolute start-3 top-2 text-[10px] font-medium tracking-wider text-zinc-500 uppercase"
		>
			{lang}
		</span>
	{/if}
	<button
		type="button"
		class="absolute end-2 top-2 flex items-center gap-1 rounded-md px-2 py-1 text-xs text-zinc-400 transition-colors hover:bg-zinc-800 hover:text-zinc-200"
		onclick={copyToClipboard}
	>
		{#if copied}
			<Check class="h-3.5 w-3.5" />
			{m.gettingStarted_codeBlock_copied()}
		{:else}
			<Copy class="h-3.5 w-3.5" />
			{m.gettingStarted_codeBlock_copy()}
		{/if}
	</button>
	<pre class="overflow-x-auto p-4 font-mono text-sm text-zinc-100" class:pt-8={lang}><code
			>{code}</code
		></pre>
</div>
