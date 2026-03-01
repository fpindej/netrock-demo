<script lang="ts">
	import * as Dialog from '$lib/components/ui/dialog';
	import { Button } from '$lib/components/ui/button';
	import { Input } from '$lib/components/ui/input';
	import { Label } from '$lib/components/ui/label';
	import * as m from '$lib/paraglide/messages';
	import { browserClient, getErrorMessage, handleMutationError } from '$lib/api';
	import { toast } from '$lib/components/ui/sonner';
	import { invalidateAll } from '$app/navigation';
	import { createCooldown } from '$lib/state';
	import { Loader2, Copy, Check } from '@lucide/svelte';

	interface Props {
		open: boolean;
	}

	let { open = $bindable() }: Props = $props();

	let step: 'setup' | 'verify' | 'recovery' = $state('setup');
	let sharedKey = $state('');
	let authenticatorUri = $state('');
	let qrDataUrl = $state('');
	let verifyCode = $state('');
	let recoveryCodes = $state<string[]>([]);
	let isLoading = $state(false);
	let codeCopied = $state(false);
	const cooldown = createCooldown();

	$effect(() => {
		if (open) {
			step = 'setup';
			sharedKey = '';
			authenticatorUri = '';
			qrDataUrl = '';
			verifyCode = '';
			recoveryCodes = [];
			codeCopied = false;
			startSetup();
		}
	});

	async function startSetup() {
		isLoading = true;
		try {
			const { response, data, error: apiError } = await browserClient.POST('/api/auth/2fa/setup');

			if (response.ok && data) {
				sharedKey = data.sharedKey ?? '';
				authenticatorUri = data.authenticatorUri ?? '';
				if (authenticatorUri) {
					const QRCode = (await import('qrcode')).default;
					qrDataUrl = await QRCode.toDataURL(authenticatorUri, {
						width: 200,
						margin: 2,
						color: { dark: '#000000', light: '#ffffff' }
					});
				}
				step = 'verify';
			} else {
				toast.error(getErrorMessage(apiError, m.settings_2fa_setup_error()));
				open = false;
			}
		} catch {
			toast.error(m.settings_2fa_setup_error());
			open = false;
		} finally {
			isLoading = false;
		}
	}

	async function verifySetup(e: Event) {
		e.preventDefault();
		isLoading = true;

		try {
			const {
				response,
				data,
				error: apiError
			} = await browserClient.POST('/api/auth/2fa/verify-setup', { body: { code: verifyCode } });

			if (response.ok && data) {
				recoveryCodes = data.recoveryCodes ?? [];
				step = 'recovery';
				toast.success(m.settings_2fa_setup_success());
				await invalidateAll();
			} else {
				handleMutationError(response, apiError, {
					cooldown,
					fallback: m.settings_2fa_setup_error(),
					onError() {
						toast.error(getErrorMessage(apiError, m.settings_2fa_setup_error()));
					}
				});
			}
		} catch {
			toast.error(m.settings_2fa_setup_error());
		} finally {
			isLoading = false;
		}
	}

	async function copyRecoveryCodes() {
		try {
			await navigator.clipboard.writeText(recoveryCodes.join('\n'));
			codeCopied = true;
			setTimeout(() => (codeCopied = false), 2000);
		} catch {
			// Clipboard API not available
		}
	}
</script>

<Dialog.Root bind:open>
	<Dialog.Content
		class="max-h-[85vh] overflow-y-auto sm:max-w-md"
		interactOutsideBehavior="ignore"
		showCloseButton={false}
	>
		<Dialog.Header>
			<Dialog.Title>
				{step === 'recovery' ? m.settings_2fa_recovery_title() : m.settings_2fa_setup_title()}
			</Dialog.Title>
			<Dialog.Description>
				{step === 'recovery'
					? m.settings_2fa_recovery_description()
					: m.settings_2fa_setup_description()}
			</Dialog.Description>
		</Dialog.Header>

		{#if step === 'verify'}
			<div class="space-y-4 py-4">
				{#if qrDataUrl}
					<div class="flex justify-center">
						<div class="rounded-lg bg-white p-2">
							<img
								src={qrDataUrl}
								alt={m.settings_2fa_setup_scanQr()}
								class="h-[200px] w-[200px]"
							/>
						</div>
					</div>
				{/if}

				{#if sharedKey}
					<div class="space-y-1">
						<p class="text-center text-xs text-muted-foreground">
							{m.settings_2fa_setup_manualEntry()}
						</p>
						<p class="text-center font-mono text-sm tracking-wider select-all">{sharedKey}</p>
					</div>
				{/if}

				<form onsubmit={verifySetup}>
					<div class="grid gap-3">
						<Label for="verify-code">{m.settings_2fa_setup_verificationCode()}</Label>
						<Input
							id="verify-code"
							type="text"
							inputmode="numeric"
							autocomplete="one-time-code"
							bind:value={verifyCode}
							placeholder={m.settings_2fa_setup_verificationCodePlaceholder()}
							maxlength={6}
							disabled={isLoading}
						/>
						<Button
							type="submit"
							disabled={isLoading || verifyCode.length !== 6 || cooldown.active}
						>
							{#if isLoading}
								<Loader2 class="me-2 h-4 w-4 animate-spin" />
							{/if}
							{cooldown.active
								? m.common_waitSeconds({ seconds: cooldown.remaining })
								: m.settings_2fa_setup_confirm()}
						</Button>
					</div>
				</form>

				<Button
					variant="outline"
					class="w-full"
					onclick={() => (open = false)}
					disabled={isLoading}
				>
					{m.common_cancel()}
				</Button>
			</div>
		{:else if step === 'recovery'}
			<div class="space-y-4 py-4">
				<div class="grid grid-cols-2 gap-2 rounded-lg border bg-muted/50 p-4 font-mono text-sm">
					{#each recoveryCodes as code (code)}
						<span class="text-center">{code}</span>
					{/each}
				</div>
				<Button variant="outline" class="w-full" onclick={copyRecoveryCodes}>
					{#if codeCopied}
						<Check class="me-2 h-4 w-4" />
						{m.settings_2fa_recovery_copied()}
					{:else}
						<Copy class="me-2 h-4 w-4" />
						{m.settings_2fa_recovery_copy()}
					{/if}
				</Button>
				<p class="text-xs text-muted-foreground">
					{m.settings_2fa_recovery_warning()}
				</p>
				<Button class="w-full" onclick={() => (open = false)}>
					{m.common_done()}
				</Button>
			</div>
		{:else}
			<div class="flex items-center justify-center py-8">
				<Loader2 class="h-8 w-8 animate-spin text-muted-foreground" />
			</div>
		{/if}
	</Dialog.Content>
</Dialog.Root>
