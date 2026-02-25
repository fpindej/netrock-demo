<script lang="ts">
	import { Header, Sidebar, ThemeToggle, LanguageSelector } from '$lib/components/layout';
	import { EmailVerificationBanner } from '$lib/components/auth';
	import { RoleSwitcher } from '$lib/components/demo';
	import { page } from '$app/state';
	import { invalidateAll } from '$app/navigation';
	import { onMount } from 'svelte';
	import { initSidebar, sidebarState, healthState } from '$lib/state';
	import { ArrowLeft } from '@lucide/svelte';
	import * as m from '$lib/paraglide/messages';

	let { children, data } = $props();

	let isLoggedIn = $derived(!!data.user);
	let collapsed = $derived(sidebarState.collapsed);

	onMount(() => {
		if (isLoggedIn) {
			initSidebar();
		}
	});

	let wasOnline = false;
	$effect(() => {
		if (isLoggedIn && healthState.checked && wasOnline && !healthState.online) {
			invalidateAll();
		}
		if (healthState.checked) wasOnline = healthState.online;
	});
</script>

{#if isLoggedIn}
	<!-- App layout (sidebar + header) for logged-in users -->
	<div
		class="grid h-dvh w-full overflow-x-hidden transition-[grid-template-columns] duration-300 md:grid-cols-[var(--sidebar-width)_1fr]"
		style="--sidebar-width: {collapsed
			? 'var(--sidebar-width-collapsed)'
			: 'var(--sidebar-width-md)'};"
	>
		<div class="hidden border-e bg-muted/40 md:block">
			<Sidebar class="h-full" user={data.user} />
		</div>
		<div class="flex flex-col overflow-hidden">
			<Header user={data.user} />
			{#if data.user && !data.user.emailConfirmed}
				<EmailVerificationBanner />
			{/if}
			<main
				class="flex flex-1 flex-col gap-4 overflow-y-auto overscroll-contain p-4 pb-[max(4rem,calc(env(safe-area-inset-bottom,0px)+2rem))] lg:gap-6 lg:p-6 lg:pb-[max(4rem,calc(env(safe-area-inset-bottom,0px)+2rem))]"
			>
				{#key page.url.pathname}
					<div
						class="motion-safe:duration-300 motion-safe:animate-in motion-safe:fade-in motion-safe:slide-in-from-bottom-4"
					>
						{@render children()}
					</div>
				{/key}
			</main>
		</div>
	</div>
	<RoleSwitcher user={data.user} />
{:else}
	<!-- Standalone layout for public visitors -->
	<div class="relative flex min-h-dvh flex-col bg-background">
		<header
			class="flex items-center justify-between border-b px-4 py-3 pt-[max(0.75rem,env(safe-area-inset-top,0px))]"
		>
			<button
				type="button"
				onclick={() => history.back()}
				class="inline-flex items-center gap-1.5 text-sm font-medium text-muted-foreground transition-colors hover:text-foreground"
			>
				<ArrowLeft class="h-4 w-4" />
				{m.common_goBack()}
			</button>
			<div class="flex gap-2">
				<LanguageSelector />
				<ThemeToggle />
			</div>
		</header>
		<main class="mx-auto w-full max-w-4xl flex-1 px-4 py-8">
			{@render children()}
		</main>
	</div>
{/if}
