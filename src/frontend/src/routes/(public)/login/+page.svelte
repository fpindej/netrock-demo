<script lang="ts">
	import { onMount, tick } from 'svelte';
	import { replaceState } from '$app/navigation';
	import { resolve } from '$app/paths';
	import { LoginForm, WelcomeOverlay } from '$lib/components/auth';
	import { toast } from '$lib/components/ui/sonner';
	import * as m from '$lib/paraglide/messages';

	let { data } = $props();

	let isRegisterOpen = $state(false);
	let showWelcome = $state(false);

	function completeWelcome() {
		showWelcome = false;
		try {
			localStorage.setItem('netrock-welcomed', '1');
		} catch {
			// localStorage unavailable — silently ignore
		}
	}

	function handleRegister() {
		try {
			localStorage.setItem('netrock-welcomed', '1');
		} catch {
			// localStorage unavailable
		}
		showWelcome = false;
		setTimeout(() => (isRegisterOpen = true), 550);
	}

	onMount(async () => {
		try {
			if (!localStorage.getItem('netrock-welcomed')) {
				showWelcome = true;
			}
		} catch {
			// localStorage unavailable — don't show overlay
		}

		if (!data.reason) return;

		await tick();

		if (data.reason === 'session_expired') {
			toast.error(m.auth_sessionExpired_title(), {
				description: m.auth_sessionExpired_description()
			});
		} else if (data.reason === 'password_changed') {
			toast.success(m.auth_passwordChanged_title(), {
				description: m.auth_passwordChanged_description()
			});
		}

		replaceState(resolve('/login'), {});
	});
</script>

<svelte:head>
	<title>{m.meta_titleTemplate({ title: m.meta_login_title() })}</title>
	<meta name="description" content={m.meta_login_description()} />
</svelte:head>

<div inert={showWelcome || undefined}>
	<LoginForm apiUrl={data.apiUrl} turnstileSiteKey={data.turnstileSiteKey} bind:isRegisterOpen />
</div>

{#if showWelcome}
	<WelcomeOverlay onComplete={completeWelcome} onRegister={handleRegister} />
{/if}
