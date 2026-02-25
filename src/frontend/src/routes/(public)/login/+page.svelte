<script lang="ts">
	import { onMount, tick } from 'svelte';
	import { goto, invalidateAll, replaceState } from '$app/navigation';
	import { resolve } from '$app/paths';
	import { browserClient } from '$lib/api';
	import { DemoCaptchaDialog, LoginForm, WelcomeOverlay } from '$lib/components/auth';
	import { toast } from '$lib/components/ui/sonner';
	import * as m from '$lib/paraglide/messages';

	let { data } = $props();

	let isRegisterOpen = $state(false);
	let showWelcome = $state(false);
	let welcomeSlide = $state(0);
	let checked = $state(false);

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

	function replayWelcome() {
		welcomeSlide = 0;
		showWelcome = true;
	}

	let showDemoCaptcha = $state(false);
	let isDemoLoading = $state(false);

	const delay = (ms: number) => new Promise((r) => setTimeout(r, ms));

	function handleTryDemo() {
		showDemoCaptcha = true;
	}

	async function handleDemoCaptchaVerified(captchaToken: string) {
		isDemoLoading = true;

		try {
			const [{ response }] = await Promise.all([
				browserClient.POST('/api/v1/demo/try', { body: { captchaToken } }),
				delay(1500)
			]);

			if (response.ok) {
				await delay(500);
				showDemoCaptcha = false;
				await invalidateAll();
				await goto(resolve('/'));
			} else {
				showDemoCaptcha = false;
				toast.error(m.demo_tryDemo_failed());
			}
		} catch {
			showDemoCaptcha = false;
			toast.error(m.demo_tryDemo_failed());
		} finally {
			isDemoLoading = false;
		}
	}

	onMount(async () => {
		try {
			// Resume mid-tour if language was changed (sessionStorage survives reloads)
			const savedSlide = sessionStorage.getItem('netrock-welcome-slide');
			if (savedSlide !== null) {
				welcomeSlide = Number(savedSlide);
				showWelcome = true;
			} else if (!localStorage.getItem('netrock-welcomed')) {
				showWelcome = true;
			}
		} catch {
			// storage unavailable — don't show overlay
		}
		checked = true;

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

<!-- Hide until localStorage check completes to prevent login-form flash -->
<div class:invisible={!checked}>
	<div inert={showWelcome || undefined}>
		<LoginForm
			apiUrl={data.apiUrl}
			turnstileSiteKey={data.turnstileSiteKey}
			bind:isRegisterOpen
			onReplayWelcome={replayWelcome}
			onTryDemo={handleTryDemo}
		/>
	</div>

	{#if showWelcome}
		<WelcomeOverlay
			onComplete={completeWelcome}
			onRegister={handleRegister}
			onTryDemo={handleTryDemo}
			initialSlide={welcomeSlide}
		/>
	{/if}
</div>

<DemoCaptchaDialog
	bind:open={showDemoCaptcha}
	isLoading={isDemoLoading}
	turnstileSiteKey={data.turnstileSiteKey}
	onVerified={handleDemoCaptchaVerified}
/>
