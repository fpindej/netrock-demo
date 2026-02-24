<script lang="ts">
	import { reveal } from '$lib/actions/reveal';
	import { StatPill } from '$lib/components/common';
	import { Button } from '$lib/components/ui/button';
	import {
		Server,
		Monitor,
		Code,
		Smartphone,
		Globe,
		ArrowRight,
		ArrowDown,
		Building2,
		Users,
		UserCircle,
		GraduationCap,
		Handshake,
		Rocket,
		Github,
		ExternalLink,
		BookOpen,
		type IconProps
	} from '@lucide/svelte';
	import * as m from '$lib/paraglide/messages';
	import { onDestroy } from 'svelte';
	import type { Component } from 'svelte';

	type PersonaCard = {
		icon: Component<IconProps>;
		title: () => string;
		pain: () => string;
		value: () => string;
		color: string;
		bgColor: string;
		gradientFrom: string;
	};

	type Consumer = {
		icon: Component<IconProps>;
		name: () => string;
		desc: () => string;
		borderClass: string;
		iconColor: string;
	};

	const consumers: Consumer[] = [
		{
			icon: Monitor,
			name: m.forYou_diagram_spoke_svelte,
			desc: m.forYou_diagram_spoke_svelteDesc,
			borderClass: 'border-blue-500/30 bg-blue-500/5',
			iconColor: 'text-blue-500'
		},
		{
			icon: Code,
			name: m.forYou_diagram_spoke_custom,
			desc: m.forYou_diagram_spoke_customDesc,
			borderClass: 'border-violet-500/30 bg-violet-500/5',
			iconColor: 'text-violet-500'
		},
		{
			icon: Smartphone,
			name: m.forYou_diagram_spoke_mobile,
			desc: m.forYou_diagram_spoke_mobileDesc,
			borderClass: 'border-amber-500/30 bg-amber-500/5',
			iconColor: 'text-amber-500'
		},
		{
			icon: Globe,
			name: m.forYou_diagram_spoke_external,
			desc: m.forYou_diagram_spoke_externalDesc,
			borderClass: 'border-pink-500/30 bg-pink-500/5',
			iconColor: 'text-pink-500'
		}
	];

	let currentIndex = $state(0);
	let paused = $state(false);
	let direction = $state<'next' | 'prev'>('next');

	let intervalId: ReturnType<typeof setInterval> | undefined;

	function startAutoAdvance() {
		stopAutoAdvance();
		intervalId = setInterval(() => {
			if (!paused) {
				direction = 'next';
				currentIndex = (currentIndex + 1) % consumers.length;
			}
		}, 3000);
	}

	function stopAutoAdvance() {
		if (intervalId !== undefined) {
			clearInterval(intervalId);
			intervalId = undefined;
		}
	}

	$effect(() => {
		startAutoAdvance();
		return stopAutoAdvance;
	});

	onDestroy(stopAutoAdvance);

	function goTo(index: number) {
		direction = index > currentIndex ? 'next' : 'prev';
		currentIndex = index;
		startAutoAdvance();
	}

	// Swipe handling
	let pointerStartX = 0;
	let pointerStartY = 0;
	let swiping = false;

	function onPointerDown(e: PointerEvent) {
		pointerStartX = e.clientX;
		pointerStartY = e.clientY;
		swiping = true;
	}

	function onPointerUp(e: PointerEvent) {
		if (!swiping) return;
		swiping = false;
		const dx = e.clientX - pointerStartX;
		const dy = e.clientY - pointerStartY;
		if (Math.abs(dx) < 30 || Math.abs(dy) > Math.abs(dx)) return;
		if (dx < 0) {
			direction = 'next';
			currentIndex = (currentIndex + 1) % consumers.length;
		} else {
			direction = 'prev';
			currentIndex = (currentIndex - 1 + consumers.length) % consumers.length;
		}
		startAutoAdvance();
	}

	const personas: PersonaCard[] = [
		{
			icon: Building2,
			title: m.forYou_personas_business_title,
			pain: m.forYou_personas_business_pain,
			value: m.forYou_personas_business_value,
			color: 'text-blue-500',
			bgColor: 'bg-blue-500/10',
			gradientFrom: 'from-blue-500'
		},
		{
			icon: Users,
			title: m.forYou_personas_teams_title,
			pain: m.forYou_personas_teams_pain,
			value: m.forYou_personas_teams_value,
			color: 'text-green-500',
			bgColor: 'bg-green-500/10',
			gradientFrom: 'from-green-500'
		},
		{
			icon: UserCircle,
			title: m.forYou_personas_solo_title,
			pain: m.forYou_personas_solo_pain,
			value: m.forYou_personas_solo_value,
			color: 'text-violet-500',
			bgColor: 'bg-violet-500/10',
			gradientFrom: 'from-violet-500'
		},
		{
			icon: GraduationCap,
			title: m.forYou_personas_learners_title,
			pain: m.forYou_personas_learners_pain,
			value: m.forYou_personas_learners_value,
			color: 'text-amber-500',
			bgColor: 'bg-amber-500/10',
			gradientFrom: 'from-amber-500'
		},
		{
			icon: Handshake,
			title: m.forYou_personas_customers_title,
			pain: m.forYou_personas_customers_pain,
			value: m.forYou_personas_customers_value,
			color: 'text-pink-500',
			bgColor: 'bg-pink-500/10',
			gradientFrom: 'from-pink-500'
		}
	];
</script>

<div class="space-y-12 pb-8">
	<!-- ── Hero ──────────────────────────────────────────────────────── -->
	<section class="text-center">
		<span
			class="hero-animate hero-delay-0 inline-block rounded-full border border-primary/20 bg-primary/10 px-4 py-1.5 text-xs font-semibold tracking-wide text-primary"
		>
			{m.forYou_hero_badge()}
		</span>

		<h1
			class="hero-animate hero-delay-1 mt-4 bg-gradient-to-r from-foreground via-foreground/80 to-foreground/60 bg-clip-text text-4xl font-bold tracking-tight text-transparent sm:text-5xl"
		>
			{m.forYou_hero_title()}
		</h1>

		<p
			class="hero-animate hero-delay-2 mx-auto mt-4 max-w-2xl text-base text-muted-foreground sm:text-lg"
		>
			{m.forYou_hero_subtitle()}
		</p>

		<div
			class="hero-animate hero-delay-3 mt-8 grid grid-cols-2 gap-3 sm:flex sm:flex-wrap sm:justify-center"
		>
			<StatPill label={m.forYou_hero_stat1()} description={m.forYou_hero_stat1Desc()} />
			<StatPill label={m.forYou_hero_stat2()} description={m.forYou_hero_stat2Desc()} />
			<StatPill label={m.forYou_hero_stat3()} description={m.forYou_hero_stat3Desc()} />
			<StatPill label={m.forYou_hero_stat4()} description={m.forYou_hero_stat4Desc()} />
		</div>
	</section>

	<div class="h-px w-full bg-border"></div>

	<!-- ── Persona Cards ────────────────────────────────────────────── -->
	<section data-tour="for-you-personas">
		<div class="mb-8 text-center" use:reveal>
			<h2 class="text-2xl font-bold tracking-tight sm:text-3xl">
				{m.forYou_personas_title()}
			</h2>
			<p class="mt-2 text-sm text-muted-foreground sm:text-base">
				{m.forYou_personas_description()}
			</p>
		</div>

		<div class="flex flex-wrap justify-center gap-4">
			{#each personas as persona, i (persona.title())}
				<div
					class="group relative flex flex-[1_1_100%] flex-col overflow-hidden rounded-xl border bg-card transition-colors hover:bg-accent/50 sm:max-w-[calc(50%-0.5rem)] sm:flex-[1_1_calc(50%-0.5rem)] xl:max-w-[calc(33.333%-0.667rem)] xl:flex-[1_1_calc(33.333%-0.667rem)]"
					use:reveal={i * 80}
				>
					<div
						class="absolute inset-x-0 top-0 h-1 bg-gradient-to-r {persona.gradientFrom} to-transparent"
					></div>
					<div class="flex flex-1 flex-col gap-4 p-5">
						<div class="flex items-center gap-3">
							<div
								class="flex h-10 w-10 shrink-0 items-center justify-center rounded-lg {persona.bgColor}"
							>
								<persona.icon class="h-5 w-5 {persona.color}" />
							</div>
							<h3 class="text-sm font-semibold">{persona.title()}</h3>
						</div>

						<div>
							<p class="text-xs font-semibold tracking-wider text-muted-foreground/70 uppercase">
								{m.forYou_personas_painPointLabel()}
							</p>
							<p class="mt-1 text-sm text-muted-foreground">{persona.pain()}</p>
						</div>

						<div class="h-px w-full bg-border"></div>

						<div>
							<p class="text-xs font-semibold tracking-wider text-muted-foreground/70 uppercase">
								{m.forYou_personas_solutionLabel()}
							</p>
							<p class="mt-1 text-sm text-muted-foreground">{persona.value()}</p>
						</div>
					</div>
				</div>
			{/each}
		</div>
	</section>

	<div class="h-px w-full bg-border"></div>

	<!-- ── Hub-and-Spoke Diagram ────────────────────────────────────── -->
	<section data-tour="for-you-diagram">
		<div class="mb-8 text-center" use:reveal>
			<h2 class="text-2xl font-bold tracking-tight sm:text-3xl">
				{m.forYou_diagram_title()}
			</h2>
			<p class="mt-2 text-sm text-muted-foreground sm:text-base">
				{m.forYou_diagram_description()}
			</p>
		</div>

		<!-- API → Consumer carousel -->
		<div
			class="flex flex-col items-center gap-4 sm:flex-row sm:justify-center sm:gap-6"
			use:reveal={0}
		>
			<!-- .NET API box -->
			<div
				class="flex w-full max-w-64 flex-col items-center rounded-xl border-2 border-green-500/40 bg-green-500/5 p-6 text-center shadow-sm shadow-green-500/10 sm:w-48 sm:max-w-none"
			>
				<Server class="h-10 w-10 text-green-500" />
				<span class="mt-2 text-sm font-bold">{m.forYou_diagram_hub()}</span>
				<span class="text-xs text-muted-foreground">{m.forYou_diagram_hubDesc()}</span>
			</div>

			<!-- Arrow -->
			<ArrowRight class="arrow-flow hidden h-6 w-6 shrink-0 text-muted-foreground/50 sm:block" />
			<ArrowDown class="arrow-flow-down h-6 w-6 shrink-0 text-muted-foreground/50 sm:hidden" />

			<!-- Consumer carousel -->
			<div
				class="relative w-full max-w-64 sm:w-48 sm:max-w-none"
				role="region"
				aria-roledescription="carousel"
				aria-label={m.forYou_diagram_title()}
				onpointerdown={onPointerDown}
				onpointerup={onPointerUp}
				onpointerenter={() => (paused = true)}
				onpointerleave={() => {
					paused = false;
					swiping = false;
				}}
			>
				<div class="overflow-hidden">
					{#each consumers as consumer, i (consumer.name())}
						{#if i === currentIndex}
							<div
								class="carousel-slide"
								class:carousel-enter-next={direction === 'next'}
								class:carousel-enter-prev={direction === 'prev'}
							>
								{@render consumerBox(
									consumer.icon,
									consumer.name(),
									consumer.desc(),
									consumer.borderClass,
									consumer.iconColor
								)}
							</div>
						{/if}
					{/each}
				</div>

				<!-- Dot indicators -->
				<div class="mt-3 flex justify-center gap-1.5">
					{#each consumers as consumer, i (consumer.name())}
						<button
							type="button"
							class="h-2 w-2 rounded-full transition-colors {i === currentIndex
								? 'bg-foreground'
								: 'bg-foreground/20 hover:bg-foreground/40'}"
							aria-label="Go to slide {i + 1}"
							onclick={() => goTo(i)}
						></button>
					{/each}
				</div>
			</div>
		</div>
	</section>

	<div class="h-px w-full bg-border"></div>

	<!-- ── CTA ──────────────────────────────────────────────────────── -->
	<section use:reveal>
		<div
			class="mx-auto max-w-xl rounded-xl border border-primary/20 bg-gradient-to-br from-primary/5 to-transparent p-6 text-center sm:p-8"
		>
			<div class="mx-auto mb-4 flex h-12 w-12 items-center justify-center rounded-xl bg-primary/10">
				<Rocket class="h-6 w-6 text-primary" />
			</div>
			<h2 class="text-xl font-bold tracking-tight sm:text-2xl">
				{m.forYou_cta_title()}
			</h2>
			<p class="mt-2 text-sm text-muted-foreground">
				{m.forYou_cta_subtitle()}
			</p>
			<div class="mt-5 flex flex-col items-center gap-3">
				<Button href="https://github.com/fpindej/netrock" target="_blank" rel="noopener noreferrer">
					<Github class="me-2 h-4 w-4" />
					{m.forYou_cta_button()}
					<ExternalLink class="ms-2 h-3.5 w-3.5" />
				</Button>
				<Button variant="outline" size="sm" href="/login">
					{m.forYou_cta_explore()}
				</Button>
				<Button
					variant="link"
					size="sm"
					href="https://deepwiki.org/fpindej/netrock"
					target="_blank"
					rel="noopener noreferrer"
					class="h-auto gap-1 p-0 text-xs"
				>
					<BookOpen class="h-3.5 w-3.5" />
					{m.forYou_cta_docs()}
					<ExternalLink class="h-3 w-3" />
				</Button>
			</div>
		</div>
	</section>
</div>

<!-- ── Snippets ─────────────────────────────────────────────────────── -->

{#snippet consumerBox(
	Icon: Component<IconProps>,
	name: string,
	desc: string,
	borderClass: string,
	iconColor: string
)}
	<div class="flex w-full flex-col items-center rounded-xl border-2 p-5 text-center {borderClass}">
		<Icon class="h-8 w-8 {iconColor}" />
		<span class="mt-2 text-sm font-bold">{name}</span>
		<span class="text-xs text-muted-foreground">{desc}</span>
	</div>
{/snippet}

<style>
	/* Hero staggered fade-in on page load */
	@keyframes hero-fade-in {
		from {
			opacity: 0;
			transform: translateY(20px);
		}
	}

	:global(.hero-animate) {
		animation: hero-fade-in 0.6s ease-out both;
	}
	:global(.hero-delay-0) {
		animation-delay: 0ms;
	}
	:global(.hero-delay-1) {
		animation-delay: 100ms;
	}
	:global(.hero-delay-2) {
		animation-delay: 200ms;
	}
	:global(.hero-delay-3) {
		animation-delay: 350ms;
	}

	/* Directional nudge for architecture flow arrows */
	@keyframes arrow-nudge {
		0%,
		100% {
			transform: translateX(0);
			opacity: 0.4;
		}
		50% {
			transform: translateX(3px);
			opacity: 0.9;
		}
	}

	@keyframes arrow-nudge-down {
		0%,
		100% {
			transform: translateY(0);
			opacity: 0.4;
		}
		50% {
			transform: translateY(3px);
			opacity: 0.9;
		}
	}

	:global(.arrow-flow) {
		animation: arrow-nudge 2s ease-in-out infinite;
	}

	:global(.arrow-flow-down) {
		animation: arrow-nudge-down 2s ease-in-out infinite;
	}

	/* Carousel slide transitions */
	@keyframes carousel-in-next {
		from {
			opacity: 0;
			transform: translateX(20px);
		}
		to {
			opacity: 1;
			transform: translateX(0);
		}
	}

	@keyframes carousel-in-prev {
		from {
			opacity: 0;
			transform: translateX(-20px);
		}
		to {
			opacity: 1;
			transform: translateX(0);
		}
	}

	.carousel-slide {
		animation-duration: 0.3s;
		animation-timing-function: ease-out;
		animation-fill-mode: both;
	}

	.carousel-enter-next {
		animation-name: carousel-in-next;
	}

	.carousel-enter-prev {
		animation-name: carousel-in-prev;
	}

	/* Respect reduced motion */
	@media (prefers-reduced-motion: reduce) {
		:global(.hero-animate) {
			animation: none;
		}
		:global(.arrow-flow),
		:global(.arrow-flow-down) {
			animation: none;
		}
		.carousel-slide {
			animation: none;
		}
	}
</style>
