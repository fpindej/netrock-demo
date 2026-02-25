<script lang="ts">
	import * as Dialog from '$lib/components/ui/dialog';
	import { TurnstileWidget } from '$lib/components/auth';
	import { fade } from 'svelte/transition';
	import { Loader2, Rocket } from '@lucide/svelte';
	import * as m from '$lib/paraglide/messages';

	interface Props {
		open?: boolean;
		isLoading?: boolean;
		turnstileSiteKey: string;
		onVerified: (token: string) => void;
	}

	let {
		open = $bindable(false),
		isLoading = false,
		turnstileSiteKey,
		onVerified
	}: Props = $props();

	function handleVerified(token: string) {
		if (!token) return;
		onVerified(token);
	}

	function handleOpenChange(isOpen: boolean) {
		if (!isOpen && !isLoading) {
			open = false;
		}
	}
</script>

<Dialog.Root bind:open onOpenChange={handleOpenChange}>
	<Dialog.Content
		class="max-w-[calc(100vw-2rem)] overflow-hidden sm:max-w-md"
		onInteractOutside={(e) => isLoading && e.preventDefault()}
	>
		<Dialog.Header>
			<Dialog.Title>{m.welcome_cta_tryDemo()}</Dialog.Title>
			{#if !isLoading}
				<Dialog.Description>{m.demo_captcha_description()}</Dialog.Description>
			{/if}
		</Dialog.Header>
		<div class="flex flex-col items-center gap-4 py-4">
			{#if isLoading}
				<div class="flex flex-col items-center gap-4" in:fade={{ duration: 300 }}>
					<div class="flex h-16 w-16 items-center justify-center rounded-full bg-primary/10">
						<Rocket class="h-8 w-8 animate-pulse text-primary" />
					</div>
					<div class="flex items-center gap-2 text-sm text-muted-foreground">
						<Loader2 class="h-4 w-4 animate-spin" />
						{m.demo_captcha_loading()}
					</div>
				</div>
			{:else}
				<div class="w-full overflow-hidden">
					<TurnstileWidget siteKey={turnstileSiteKey} onVerified={handleVerified} />
				</div>
			{/if}
		</div>
	</Dialog.Content>
</Dialog.Root>
