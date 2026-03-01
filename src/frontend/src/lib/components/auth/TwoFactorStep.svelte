<script lang="ts">
	import { browserClient, getErrorMessage, handleMutationError } from '$lib/api';
	import { cn } from '$lib/utils';
	import { createShake, createCooldown } from '$lib/state';
	import { goto } from '$app/navigation';
	import { resolve } from '$app/paths';
	import { invalidateAll } from '$app/navigation';
	import { Button } from '$lib/components/ui/button';
	import { Input } from '$lib/components/ui/input';
	import { Label } from '$lib/components/ui/label';
	import * as Card from '$lib/components/ui/card';
	import * as m from '$lib/paraglide/messages';
	import { fly } from 'svelte/transition';
	import { Loader2, ArrowLeft } from '@lucide/svelte';
	import { toast } from '$lib/components/ui/sonner';

	interface Props {
		challengeToken: string;
		onBack: () => void;
	}

	let { challengeToken, onBack }: Props = $props();

	let code = $state('');
	let isLoading = $state(false);
	let isRecoveryMode = $state(false);
	const shake = createShake();
	const cooldown = createCooldown();

	const delay = (ms: number) => new Promise((resolve) => setTimeout(resolve, ms));

	async function verify(e: Event) {
		e.preventDefault();
		if (isLoading || cooldown.active) return;

		isLoading = true;

		try {
			const endpoint = isRecoveryMode ? '/api/auth/login/2fa/recovery' : '/api/auth/login/2fa';
			const body = isRecoveryMode
				? { challengeToken, recoveryCode: code }
				: { challengeToken, code };

			const { response, error: apiError } = await browserClient.POST(endpoint, {
				body: body as never
			});

			if (response.ok) {
				await delay(500);
				await invalidateAll();
				await goto(resolve('/'));
			} else {
				handleMutationError(response, apiError, {
					cooldown,
					fallback: m.auth_2fa_error(),
					onRateLimited: () => shake.trigger(),
					onError() {
						toast.error(getErrorMessage(apiError, m.auth_2fa_error()));
						shake.trigger();
					}
				});
			}
		} catch {
			toast.error(m.auth_2fa_error());
			shake.trigger();
		} finally {
			isLoading = false;
		}
	}

	function toggleRecoveryMode() {
		isRecoveryMode = !isRecoveryMode;
		code = '';
	}
</script>

<div class="sm:mx-auto sm:w-full sm:max-w-md" in:fly={{ y: 20, duration: 600, delay: 100 }}>
	<Card.Root
		class={cn(
			'border-muted/60 bg-card/50 shadow-xl backdrop-blur-sm transition-colors duration-300',
			shake.active && 'animate-shake border-destructive'
		)}
	>
		<Card.Header>
			<Card.Title class="text-center text-2xl">{m.auth_2fa_title()}</Card.Title>
			<Card.Description class="text-center">
				{m.auth_2fa_description()}
			</Card.Description>
		</Card.Header>
		<Card.Content>
			<form class="space-y-4" onsubmit={verify}>
				<div class="grid gap-2">
					<Label for="2fa-code">
						{isRecoveryMode ? m.auth_2fa_useRecoveryCode() : m.auth_2fa_code()}
					</Label>
					<Input
						id="2fa-code"
						type="text"
						inputmode={isRecoveryMode ? 'text' : 'numeric'}
						autocomplete="one-time-code"
						required
						bind:value={code}
						placeholder={isRecoveryMode
							? m.auth_2fa_recoveryCodePlaceholder()
							: m.auth_2fa_codePlaceholder()}
						class="bg-background/50"
						aria-invalid={shake.active}
						disabled={isLoading}
						maxlength={isRecoveryMode ? undefined : 6}
					/>
				</div>

				<Button type="submit" class="w-full" disabled={isLoading || cooldown.active || !code}>
					{#if cooldown.active}
						{m.common_waitSeconds({ seconds: cooldown.remaining })}
					{:else}
						{#if isLoading}
							<Loader2 class="me-2 h-4 w-4 animate-spin" />
						{/if}
						{m.auth_2fa_verify()}
					{/if}
				</Button>
			</form>

			<div class="mt-4 flex flex-col items-center gap-2">
				<button
					type="button"
					class="text-sm font-medium text-primary hover:underline"
					onclick={toggleRecoveryMode}
				>
					{isRecoveryMode ? m.auth_2fa_backToCode() : m.auth_2fa_useRecoveryCode()}
				</button>
				<button
					type="button"
					class="inline-flex items-center gap-1 text-sm text-muted-foreground hover:text-foreground"
					onclick={onBack}
				>
					<ArrowLeft class="h-3.5 w-3.5" />
					{m.auth_2fa_backToLogin()}
				</button>
			</div>
		</Card.Content>
	</Card.Root>
</div>
