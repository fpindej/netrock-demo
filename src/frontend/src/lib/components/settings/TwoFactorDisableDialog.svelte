<script lang="ts">
	import * as Dialog from '$lib/components/ui/dialog';
	import { Button } from '$lib/components/ui/button';
	import { Input } from '$lib/components/ui/input';
	import { Label } from '$lib/components/ui/label';
	import * as m from '$lib/paraglide/messages';
	import { browserClient, getErrorMessage, handleMutationError } from '$lib/api';
	import { toast } from '$lib/components/ui/sonner';
	import { invalidateAll } from '$app/navigation';
	import { createFieldShakes, createCooldown } from '$lib/state';

	interface Props {
		open: boolean;
	}

	let { open = $bindable() }: Props = $props();

	let password = $state('');
	let isLoading = $state(false);
	let generalError = $state('');
	const fieldShakes = createFieldShakes();
	const cooldown = createCooldown();

	$effect(() => {
		if (open) {
			password = '';
			generalError = '';
		}
	});

	async function handleSubmit(e: Event) {
		e.preventDefault();
		generalError = '';
		isLoading = true;

		try {
			const { response, error: apiError } = await browserClient.POST('/api/auth/2fa/disable', {
				body: { password }
			});

			if (response.ok) {
				open = false;
				toast.success(m.settings_2fa_disable_success());
				await invalidateAll();
			} else {
				handleMutationError(response, apiError, {
					cooldown,
					fallback: m.settings_2fa_disable_error(),
					onError() {
						generalError = getErrorMessage(apiError, m.settings_2fa_disable_error());
						fieldShakes.trigger('password');
					}
				});
			}
		} catch {
			toast.error(m.settings_2fa_disable_error());
		} finally {
			isLoading = false;
		}
	}
</script>

<Dialog.Root bind:open>
	<Dialog.Content class="sm:max-w-md">
		<Dialog.Header>
			<Dialog.Title>{m.settings_2fa_disable_title()}</Dialog.Title>
			<Dialog.Description>
				{m.settings_2fa_disable_description()}
			</Dialog.Description>
		</Dialog.Header>
		<form onsubmit={handleSubmit}>
			<div class="grid gap-4 py-4">
				<div class="grid gap-2">
					<Label for="disable2faPassword">{m.settings_2fa_disable_password()}</Label>
					<Input
						id="disable2faPassword"
						type="password"
						autocomplete="current-password"
						bind:value={password}
						placeholder={m.settings_2fa_disable_passwordPlaceholder()}
						aria-invalid={!!generalError}
						aria-describedby={generalError ? 'disable2faError' : undefined}
						class={fieldShakes.class('password')}
						disabled={isLoading}
					/>
					{#if generalError}
						<p id="disable2faError" class="text-xs text-destructive">
							{generalError}
						</p>
					{/if}
				</div>
			</div>
			<Dialog.Footer class="flex-col-reverse gap-2 sm:flex-row">
				<Dialog.Close>
					{#snippet child({ props })}
						<Button {...props} variant="outline" disabled={isLoading}>
							{m.common_cancel()}
						</Button>
					{/snippet}
				</Dialog.Close>
				<Button
					type="submit"
					variant="destructive"
					disabled={isLoading || !password || cooldown.active}
				>
					{cooldown.active
						? m.common_waitSeconds({ seconds: cooldown.remaining })
						: m.settings_2fa_disable_confirm()}
				</Button>
			</Dialog.Footer>
		</form>
	</Dialog.Content>
</Dialog.Root>
