<script lang="ts">
	import { onMount } from 'svelte';
	import { fade, fly } from 'svelte/transition';
	import { resolve } from '$app/paths';
	import { Button } from '$lib/components/ui/button';
	import { LanguageSelector } from '$lib/components/layout';
	import * as m from '$lib/paraglide/messages';
	import {
		ShieldCheck,
		Users,
		BarChart3,
		LineChart,
		EyeOff,
		Trash2,
		ChevronLeft,
		ChevronRight
	} from '@lucide/svelte';

	interface Props {
		onComplete: () => void;
		onRegister: () => void;
	}

	let { onComplete, onRegister }: Props = $props();

	const TOTAL_SLIDES = 5;
	const SWIPE_THRESHOLD = 50;

	let currentSlide = $state(0);
	let visible = $state(true);
	let skipVisible = $state(false);
	let reducedMotion = $state(false);

	// Swipe / drag tracking
	let pointerStartX = $state(0);
	let pointerStartY = $state(0);
	let isDragging = $state(false);

	const techPills = ['.NET 10', 'SvelteKit', 'TypeScript', 'PostgreSQL', 'Redis', 'Docker'];

	const pillars = [
		{
			icon: ShieldCheck,
			label: () => m.welcome_pillar_auth(),
			desc: () => m.welcome_pillar_auth_desc(),
			color: 'text-green-500'
		},
		{
			icon: Users,
			label: () => m.welcome_pillar_admin(),
			desc: () => m.welcome_pillar_admin_desc(),
			color: 'text-blue-500'
		},
		{
			icon: BarChart3,
			label: () => m.welcome_pillar_pipeline(),
			desc: () => m.welcome_pillar_pipeline_desc(),
			color: 'text-violet-500'
		},
		{
			icon: LineChart,
			label: () => m.welcome_pillar_analytics(),
			desc: () => m.welcome_pillar_analytics_desc(),
			color: 'text-amber-500'
		}
	];

	const privacyItems = [
		{ icon: ShieldCheck, msg: () => m.welcome_privacy_noCollection() },
		{ icon: EyeOff, msg: () => m.welcome_privacy_anonymized() },
		{ icon: Trash2, msg: () => m.welcome_privacy_cleared() }
	];

	function fadeDuration(ms: number) {
		return reducedMotion ? 0 : ms;
	}

	function nextSlide() {
		if (currentSlide < TOTAL_SLIDES - 1) currentSlide++;
	}

	function prevSlide() {
		if (currentSlide > 0) currentSlide--;
	}

	function dismiss() {
		visible = false;
		setTimeout(() => onComplete(), fadeDuration(500) + 50);
	}

	function handleRegister() {
		visible = false;
		setTimeout(() => onRegister(), fadeDuration(500) + 50);
	}

	function handleKeydown(e: KeyboardEvent) {
		if (e.key === 'ArrowRight') nextSlide();
		else if (e.key === 'ArrowLeft') prevSlide();
		else if (e.key === 'Escape') dismiss();
	}

	function handlePointerDown(e: PointerEvent) {
		// Only handle primary button (left click / single touch)
		if (e.button !== 0) return;
		pointerStartX = e.clientX;
		pointerStartY = e.clientY;
		isDragging = true;
	}

	function handlePointerUp(e: PointerEvent) {
		if (!isDragging) return;
		isDragging = false;

		const dx = e.clientX - pointerStartX;
		const dy = e.clientY - pointerStartY;

		// Only register horizontal swipe if it's more horizontal than vertical
		if (Math.abs(dx) > SWIPE_THRESHOLD && Math.abs(dx) > Math.abs(dy)) {
			if (dx < 0) nextSlide();
			else prevSlide();
		}
	}

	function handlePointerCancel() {
		isDragging = false;
	}

	onMount(() => {
		const mq = window.matchMedia('(prefers-reduced-motion: reduce)');
		reducedMotion = mq.matches;

		const onChange = (e: MediaQueryListEvent) => (reducedMotion = e.matches);
		mq.addEventListener('change', onChange);

		const skipTimer = setTimeout(() => (skipVisible = true), 1500);

		return () => {
			mq.removeEventListener('change', onChange);
			clearTimeout(skipTimer);
		};
	});
</script>

<svelte:window onkeydown={handleKeydown} />

{#if visible}
	<div
		class="fixed inset-0 z-50 flex touch-pan-y flex-col items-center justify-center overflow-hidden bg-background"
		transition:fade={{ duration: fadeDuration(500) }}
		role="dialog"
		aria-modal="true"
		aria-label={m.welcome_splash_title()}
		onpointerdown={handlePointerDown}
		onpointerup={handlePointerUp}
		onpointercancel={handlePointerCancel}
	>
		<!-- Background glows -->
		<div class="pointer-events-none absolute inset-0 overflow-hidden" aria-hidden="true">
			<div class="glow-xl-top-end animate-glow-pulse"></div>
			<div class="glow-xl-bottom-start animate-glow-pulse animation-delay-2000"></div>
			<div class="welcome-glow-center"></div>
		</div>

		<!-- Language selector (top-right, consistent with login page) -->
		<div class="absolute end-4 top-4 z-10">
			<LanguageSelector />
		</div>

		<!-- Slide content -->
		<div class="relative z-10 flex w-full max-w-lg flex-1 items-center justify-center px-6">
			{#key currentSlide}
				<div
					class="flex w-full flex-col items-center text-center"
					in:fly={{ y: 30, duration: fadeDuration(400), delay: reducedMotion ? 0 : 100 }}
					out:fade={{ duration: fadeDuration(250) }}
				>
					{#if currentSlide === 0}
						<!-- Slide 0: Welcome Splash -->
						<span
							class="welcome-stagger-0 mb-4 text-xs font-semibold tracking-widest text-muted-foreground uppercase"
						>
							NETROCK
						</span>
						<h1
							class="welcome-stagger-1 mb-3 bg-gradient-to-r from-foreground via-primary to-foreground bg-clip-text text-4xl font-bold tracking-tight text-transparent sm:text-5xl"
						>
							{m.welcome_splash_title()}
						</h1>
						<p class="welcome-stagger-2 max-w-md text-base text-muted-foreground sm:text-lg">
							{m.welcome_splash_subtitle()}
						</p>
					{:else if currentSlide === 1}
						<!-- Slide 1: What's Inside -->
						<h2 class="welcome-stagger-0 mb-6 text-2xl font-bold tracking-tight sm:text-3xl">
							{m.welcome_features_title()}
						</h2>
						<div class="grid w-full max-w-md grid-cols-2 gap-3">
							{#each pillars as pillar, i (pillar.color)}
								<div
									class="welcome-stagger-{i} flex flex-col items-center gap-2 rounded-xl border border-border/50 bg-card/50 p-4 backdrop-blur-sm"
								>
									<pillar.icon class="h-6 w-6 {pillar.color}" />
									<span class="text-sm font-semibold">{pillar.label()}</span>
									<span class="text-xs text-muted-foreground">{pillar.desc()}</span>
								</div>
							{/each}
						</div>
					{:else if currentSlide === 2}
						<!-- Slide 2: Tech & Quality -->
						<h2 class="welcome-stagger-0 mb-6 text-2xl font-bold tracking-tight sm:text-3xl">
							{m.welcome_tech_title()}
						</h2>
						<div class="welcome-stagger-1 mb-6 flex flex-wrap justify-center gap-2">
							{#each techPills as pill (pill)}
								<span
									class="rounded-full border border-border/60 bg-muted/50 px-3 py-1 text-xs font-medium text-foreground"
								>
									{pill}
								</span>
							{/each}
						</div>
						<div
							class="welcome-stagger-2 rounded-xl border border-primary/20 bg-primary/5 px-6 py-4"
						>
							<span class="text-2xl font-bold text-primary">650+</span>
							<p class="mt-1 text-sm text-muted-foreground">{m.welcome_tech_tests()}</p>
						</div>
					{:else if currentSlide === 3}
						<!-- Slide 3: Privacy & Safety -->
						<h2 class="welcome-stagger-0 mb-6 text-2xl font-bold tracking-tight sm:text-3xl">
							{m.welcome_privacy_title()}
						</h2>
						<div class="w-full max-w-md space-y-3">
							{#each privacyItems as item, i (item.icon)}
								<div
									class="welcome-stagger-{i} flex items-start gap-3 rounded-lg border border-border/50 bg-card/50 p-3 text-start backdrop-blur-sm"
								>
									<item.icon class="mt-0.5 h-5 w-5 shrink-0 text-primary" />
									<span class="text-sm text-muted-foreground">{item.msg()}</span>
								</div>
							{/each}
						</div>
						<a
							href={resolve('/privacy')}
							class="welcome-stagger-3 mt-4 text-sm text-primary hover:underline"
							onclick={dismiss}
						>
							{m.welcome_privacy_learnMore()}
						</a>
					{:else if currentSlide === 4}
						<!-- Slide 4: Get Started -->
						<h2 class="welcome-stagger-0 mb-2 text-2xl font-bold tracking-tight sm:text-3xl">
							{m.welcome_cta_title()}
						</h2>
						<p class="welcome-stagger-1 mb-6 text-base text-muted-foreground">
							{m.welcome_cta_subtitle()}
						</p>
						<div class="welcome-stagger-2 flex flex-col items-center gap-3 sm:flex-row">
							<Button size="lg" onclick={handleRegister}>
								{m.welcome_cta_register()}
							</Button>
							<Button variant="outline" size="lg" onclick={dismiss}>
								{m.welcome_cta_signIn()}
							</Button>
						</div>
						<p class="welcome-stagger-3 mt-4 text-xs text-muted-foreground">
							{m.welcome_cta_note()}
						</p>
					{/if}
				</div>
			{/key}
		</div>

		<!-- Bottom navigation -->
		<div class="relative z-10 flex items-center gap-4 pb-8">
			<button
				type="button"
				class="rounded-full p-1.5 text-muted-foreground transition-colors hover:bg-muted/50 hover:text-foreground disabled:pointer-events-none disabled:opacity-0"
				onclick={prevSlide}
				disabled={currentSlide === 0}
				aria-label="Previous slide"
			>
				<ChevronLeft class="h-5 w-5" />
			</button>

			<div class="flex items-center gap-2">
				{#each Array.from({ length: TOTAL_SLIDES }, (__, k) => k) as i (i)}
					<button
						type="button"
						class="h-2 rounded-full transition-all duration-300 {i === currentSlide
							? 'w-6 bg-primary'
							: 'w-2 bg-muted-foreground/30 hover:bg-muted-foreground/50'}"
						onclick={() => (currentSlide = i)}
						aria-label="Go to slide {i + 1}"
					></button>
				{/each}
			</div>

			{#if currentSlide < TOTAL_SLIDES - 1}
				<button
					type="button"
					class="rounded-full p-1.5 text-muted-foreground transition-colors hover:bg-muted/50 hover:text-foreground"
					onclick={nextSlide}
					aria-label="Next slide"
				>
					<ChevronRight class="h-5 w-5" />
				</button>
			{:else}
				<!-- Invisible spacer to keep dots centered on last slide -->
				<div class="w-8"></div>
			{/if}
		</div>

		<!-- Skip button (bottom-right, above nav) -->
		{#if skipVisible && currentSlide < TOTAL_SLIDES - 1}
			<button
				type="button"
				class="absolute end-4 bottom-6 z-10 rounded-lg px-3 py-1.5 text-sm text-muted-foreground transition-colors hover:bg-muted/50 hover:text-foreground"
				onclick={dismiss}
				in:fade={{ duration: fadeDuration(300) }}
			>
				{m.welcome_skip()}
			</button>
		{/if}
	</div>
{/if}

<style>
	.welcome-glow-center {
		position: absolute;
		top: 50%;
		left: 50%;
		width: 24rem;
		height: 24rem;
		transform: translate(-50%, -50%);
		border-radius: 9999px;
		background: radial-gradient(
			circle,
			hsl(var(--primary) / 0.08) 0%,
			hsl(var(--accent) / 0.04) 50%,
			transparent 70%
		);
		filter: blur(64px);
		pointer-events: none;
	}

	@keyframes welcome-fade-in {
		from {
			opacity: 0;
			transform: translateY(16px);
		}
	}

	:global(.welcome-stagger-0) {
		animation: welcome-fade-in 0.6s ease-out both;
		animation-delay: 0ms;
	}
	:global(.welcome-stagger-1) {
		animation: welcome-fade-in 0.6s ease-out both;
		animation-delay: 120ms;
	}
	:global(.welcome-stagger-2) {
		animation: welcome-fade-in 0.6s ease-out both;
		animation-delay: 250ms;
	}
	:global(.welcome-stagger-3) {
		animation: welcome-fade-in 0.6s ease-out both;
		animation-delay: 400ms;
	}

	@media (prefers-reduced-motion: reduce) {
		:global(.welcome-stagger-0),
		:global(.welcome-stagger-1),
		:global(.welcome-stagger-2),
		:global(.welcome-stagger-3) {
			animation: none;
		}
	}
</style>
