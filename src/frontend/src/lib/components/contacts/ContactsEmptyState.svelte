<script lang="ts">
	import { Button } from '$lib/components/ui/button';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
	import { Users, Loader2, ChevronDown, Check } from '@lucide/svelte';
	import * as m from '$lib/paraglide/messages';

	interface Props {
		onGenerate: (count: number) => void;
		onCreate: () => void;
		isGenerating: boolean;
	}

	let { onGenerate, onCreate, isGenerating }: Props = $props();

	let selectedCount = $state(10);

	const countOptions: { value: number; label: () => string }[] = [
		{ value: 5, label: () => m.contacts_generate_5() },
		{ value: 10, label: () => m.contacts_generate_10() },
		{ value: 20, label: () => m.contacts_generate_20() },
		{ value: 50, label: () => m.contacts_generate_50() }
	];

	function getGenerateLabel(count: number): string {
		return m.contacts_generate({ count });
	}
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
		<div class="inline-flex rounded-md shadow-sm">
			<Button
				class="rounded-e-none"
				onclick={() => onGenerate(selectedCount)}
				disabled={isGenerating}
			>
				{#if isGenerating}
					<Loader2 class="me-2 h-4 w-4 animate-spin" />
				{/if}
				{getGenerateLabel(selectedCount)}
			</Button>
			<DropdownMenu.Root>
				<DropdownMenu.Trigger>
					<Button
						variant="default"
						size="icon"
						class="rounded-s-none border-s border-s-primary-foreground/20"
						disabled={isGenerating}
						aria-label="Choose generate count"
					>
						<ChevronDown class="h-4 w-4" />
					</Button>
				</DropdownMenu.Trigger>
				<DropdownMenu.Content align="end">
					{#each countOptions as option (option.value)}
						<DropdownMenu.Item
							onclick={() => {
								selectedCount = option.value;
							}}
						>
							<div class="flex w-full items-center gap-2">
								<span class="w-4">
									{#if selectedCount === option.value}
										<Check class="h-4 w-4" />
									{/if}
								</span>
								{option.label()}
							</div>
						</DropdownMenu.Item>
					{/each}
				</DropdownMenu.Content>
			</DropdownMenu.Root>
		</div>
		<button
			type="button"
			class="text-sm text-muted-foreground underline-offset-4 hover:underline"
			onclick={onCreate}
		>
			{m.contacts_createManually()}
		</button>
	</div>
</div>
