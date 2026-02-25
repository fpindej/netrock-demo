<script lang="ts">
	import { Check, Copy } from '@lucide/svelte';
	import * as m from '$lib/paraglide/messages';
	import { shellState, setShell, type Shell } from '$lib/state';

	interface Props {
		code?: string;
		lang?: string;
		variants?: { unix: string; powershell: string };
	}

	let { code, lang, variants }: Props = $props();

	let activeCode = $derived(variants ? variants[shellState.shell] : (code ?? ''));
	let activeLang = $derived(
		variants ? (shellState.shell === 'unix' ? 'bash' : 'powershell') : lang
	);

	let copied = $state(false);
	let timeout: ReturnType<typeof setTimeout> | undefined;

	function copyToClipboard() {
		navigator.clipboard.writeText(activeCode);
		copied = true;
		clearTimeout(timeout);
		timeout = setTimeout(() => (copied = false), 1500);
	}

	const shells: { key: Shell; label: () => string }[] = [
		{ key: 'unix', label: m.gettingStarted_codeBlock_shellUnix },
		{ key: 'powershell', label: m.gettingStarted_codeBlock_shellPowershell }
	];
</script>

<div class="group relative rounded-lg bg-zinc-900 dark:bg-zinc-950">
	<div class="flex items-center justify-between px-3 pt-2">
		{#if variants}
			<div class="flex gap-0.5 rounded-md bg-zinc-800 p-0.5">
				{#each shells as shell (shell.key)}
					<button
						type="button"
						class="rounded px-2 py-0.5 text-[10px] font-medium tracking-wider uppercase transition-colors {shellState.shell ===
						shell.key
							? 'bg-zinc-700 text-zinc-200'
							: 'text-zinc-500 hover:text-zinc-400'}"
						onclick={() => setShell(shell.key)}
					>
						{shell.label()}
					</button>
				{/each}
			</div>
		{:else if activeLang}
			<span class="text-[10px] font-medium tracking-wider text-zinc-500 uppercase">
				{activeLang}
			</span>
		{:else}
			<span></span>
		{/if}
		<button
			type="button"
			class="flex items-center gap-1 rounded-md px-2 py-1 text-xs text-zinc-400 transition-colors hover:bg-zinc-800 hover:text-zinc-200"
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
	</div>
	<pre class="overflow-x-auto px-4 pt-2 pb-4 font-mono text-sm text-zinc-100"><code
			>{activeCode}</code
		></pre>
</div>
