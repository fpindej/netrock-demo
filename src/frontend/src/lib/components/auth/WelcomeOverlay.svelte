<script lang="ts">
	import { onMount } from 'svelte';
	import { fade } from 'svelte/transition';
	import { resolve } from '$app/paths';
	import { Button } from '$lib/components/ui/button';
	import { LanguageSelector, ThemeToggle } from '$lib/components/layout';
	import * as m from '$lib/paraglide/messages';
	import {
		ShieldCheck,
		Users,
		Clock,
		Rocket,
		EyeOff,
		Trash2,
		ChevronLeft,
		ChevronRight,
		Star,
		MessageCircle,
		Linkedin
	} from '@lucide/svelte';

	interface Props {
		onComplete: () => void;
		onRegister: () => void;
		onTryDemo?: () => void;
		initialSlide?: number;
	}

	let { onComplete, onRegister, onTryDemo, initialSlide = 0 }: Props = $props();

	const TOTAL_SLIDES = 5;
	const SWIPE_THRESHOLD = 50;
	const SLIDE_KEY = 'netrock-welcome-slide';

	let currentSlide = $state(initialSlide);
	let visible = $state(true);
	let skipVisible = $state(false);
	let reducedMotion = $state(false);

	// Swipe / drag tracking
	let pointerStartX = $state(0);
	let pointerStartY = $state(0);
	let isDragging = $state(false);

	const techPills = ['.NET 10', 'SvelteKit', 'TypeScript', 'PostgreSQL', 'Aspire', 'Docker'];

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
			icon: Clock,
			label: () => m.welcome_pillar_jobs(),
			desc: () => m.welcome_pillar_jobs_desc(),
			color: 'text-violet-500'
		},
		{
			icon: Rocket,
			label: () => m.welcome_pillar_infra(),
			desc: () => m.welcome_pillar_infra_desc(),
			color: 'text-amber-500'
		}
	];

	const privacyItems = [
		{ icon: ShieldCheck, msg: () => m.welcome_privacy_noCollection() },
		{ icon: EyeOff, msg: () => m.welcome_privacy_anonymized() },
		{ icon: Trash2, msg: () => m.welcome_privacy_cleared() }
	];

	// Persist current slide so language-change reloads can resume
	$effect(() => {
		if (visible) {
			try {
				sessionStorage.setItem(SLIDE_KEY, String(currentSlide));
			} catch {
				// sessionStorage unavailable
			}
		}
	});

	function fadeDuration(ms: number) {
		return reducedMotion ? 0 : ms;
	}

	function nextSlide() {
		if (currentSlide < TOTAL_SLIDES - 1) currentSlide++;
	}

	function prevSlide() {
		if (currentSlide > 0) currentSlide--;
	}

	function clearSlideState() {
		try {
			sessionStorage.removeItem(SLIDE_KEY);
		} catch {
			// sessionStorage unavailable
		}
	}

	function dismiss() {
		visible = false;
		clearSlideState();
		setTimeout(() => onComplete(), fadeDuration(500) + 50);
	}

	function handleRegister() {
		visible = false;
		clearSlideState();
		setTimeout(() => onRegister(), fadeDuration(500) + 50);
	}

	function handleTryDemo() {
		visible = false;
		clearSlideState();
		setTimeout(() => onTryDemo?.(), fadeDuration(500) + 50);
	}

	function handleKeydown(e: KeyboardEvent) {
		if (e.key === 'ArrowRight') nextSlide();
		else if (e.key === 'ArrowLeft') prevSlide();
		else if (e.key === 'Escape') dismiss();
	}

	function handlePointerDown(e: PointerEvent) {
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

		// Show skip immediately if resuming mid-tour (after language change)
		const skipDelay = initialSlide > 0 ? 0 : 1500;
		const skipTimer = setTimeout(() => (skipVisible = true), skipDelay);

		return () => {
			mq.removeEventListener('change', onChange);
			clearTimeout(skipTimer);
		};
	});
</script>

<svelte:window onkeydown={handleKeydown} />

{#if visible}
	<div
		class="welcome-overlay fixed inset-0 z-50 flex touch-pan-y flex-col items-center justify-center overflow-hidden bg-background"
		transition:fade={{ duration: fadeDuration(500) }}
		role="dialog"
		tabindex="-1"
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

		<!-- Language + theme selectors (top-right, matching login page) -->
		<div
			class="welcome-safe-top absolute end-[max(1rem,env(safe-area-inset-right,0px))] top-4 z-10 flex gap-2"
		>
			<LanguageSelector />
			<ThemeToggle />
		</div>

		<!-- Slide content — absolute positioning lets old/new slides crossfade on top of each other -->
		<div class="relative z-10 flex w-full max-w-lg flex-1 items-center justify-center px-6">
			{#key currentSlide}
				<div
					class="absolute inset-0 flex flex-col items-center justify-center px-6 text-center"
					in:fade={{ duration: fadeDuration(450), delay: reducedMotion ? 0 : 200 }}
					out:fade={{ duration: fadeDuration(350) }}
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
							<p class="text-sm text-muted-foreground">{m.welcome_tech_tests()}</p>
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
							{#if onTryDemo}
								<Button variant="secondary" size="lg" onclick={handleTryDemo}>
									{m.welcome_cta_tryDemo()}
								</Button>
							{/if}
							<Button variant="outline" size="lg" onclick={dismiss}>
								{m.welcome_cta_signIn()}
							</Button>
						</div>
						<p class="welcome-stagger-3 mt-4 text-xs text-muted-foreground">
							{m.welcome_cta_note()}
						</p>
						<div class="welcome-stagger-3 mt-6 flex flex-col items-center gap-2">
							<span class="text-xs font-medium text-muted-foreground/70">
								{m.welcome_cta_community()}
							</span>
							<div class="flex gap-3">
								<a
									href="https://github.com/fpindej/netrock"
									target="_blank"
									rel="noopener noreferrer"
									class="flex items-center gap-1.5 rounded-md px-2.5 py-1.5 text-xs text-muted-foreground transition-colors hover:bg-muted/50 hover:text-foreground"
								>
									<Star class="h-3.5 w-3.5" />
									GitHub
								</a>
								<a
									href="https://discord.gg/5rHquRptSh"
									target="_blank"
									rel="noopener noreferrer"
									class="flex items-center gap-1.5 rounded-md px-2.5 py-1.5 text-xs text-muted-foreground transition-colors hover:bg-muted/50 hover:text-foreground"
								>
									<MessageCircle class="h-3.5 w-3.5" />
									Discord
								</a>
								<a
									href="https://www.linkedin.com/in/filip-dorian-pindej/"
									target="_blank"
									rel="noopener noreferrer"
									class="flex items-center gap-1.5 rounded-md px-2.5 py-1.5 text-xs text-muted-foreground transition-colors hover:bg-muted/50 hover:text-foreground"
								>
									<Linkedin class="h-3.5 w-3.5" />
									LinkedIn
								</a>
							</div>
						</div>
					{/if}
				</div>
			{/key}
		</div>

		<!-- Bottom navigation -->
		<div class="welcome-safe-bottom relative z-10 flex items-center gap-4 pb-8">
			<button
				type="button"
				class="flex h-10 w-10 items-center justify-center rounded-full text-muted-foreground transition-colors hover:bg-muted/50 hover:text-foreground disabled:pointer-events-none disabled:opacity-0"
				onclick={prevSlide}
				disabled={currentSlide === 0}
				aria-label="Previous slide"
			>
				<ChevronLeft class="h-5 w-5" />
			</button>

			<div class="flex items-center gap-1">
				{#each Array.from({ length: TOTAL_SLIDES }, (__, k) => k) as i (i)}
					<button
						type="button"
						class="flex h-8 items-center justify-center rounded-full px-1"
						onclick={() => (currentSlide = i)}
						aria-label="Go to slide {i + 1}"
					>
						<span
							class="block h-2 rounded-full transition-all duration-300 {i === currentSlide
								? 'w-6 bg-primary'
								: 'w-2 bg-muted-foreground/30 hover:bg-muted-foreground/50'}"
						></span>
					</button>
				{/each}
			</div>

			{#if currentSlide < TOTAL_SLIDES - 1}
				<button
					type="button"
					class="flex h-10 w-10 items-center justify-center rounded-full text-muted-foreground transition-colors hover:bg-muted/50 hover:text-foreground"
					onclick={nextSlide}
					aria-label="Next slide"
				>
					<ChevronRight class="h-5 w-5" />
				</button>
			{:else}
				<!-- Invisible spacer to keep dots centered on last slide -->
				<div class="w-10"></div>
			{/if}
		</div>

		<!-- Skip button (bottom-right, above nav) -->
		{#if skipVisible && currentSlide < TOTAL_SLIDES - 1}
			<button
				type="button"
				class="welcome-safe-skip absolute end-[max(1rem,env(safe-area-inset-right,0px))] bottom-6 z-10 rounded-lg px-3 py-1.5 text-sm text-muted-foreground transition-colors hover:bg-muted/50 hover:text-foreground"
				onclick={dismiss}
				in:fade={{ duration: fadeDuration(300) }}
			>
				{m.welcome_skip()}
			</button>
		{/if}
	</div>
{/if}

<style>
	/* Use dvh for the overlay so it respects Safari's dynamic toolbar */
	.welcome-overlay {
		height: 100vh;
	}

	@supports (height: 100dvh) {
		.welcome-overlay {
			height: 100dvh;
		}
	}

	/* Safe-area offsets for positioned elements on notched devices */
	.welcome-safe-top {
		top: max(1rem, env(safe-area-inset-top, 0px));
	}

	.welcome-safe-bottom {
		padding-bottom: max(2rem, calc(env(safe-area-inset-bottom, 0px) + 0.5rem));
	}

	.welcome-safe-skip {
		bottom: max(1.5rem, calc(env(safe-area-inset-bottom, 0px) + 0.25rem));
	}

	.welcome-glow-center {
		position: absolute;
		top: 50%;
		inset-inline-start: 50%;
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
