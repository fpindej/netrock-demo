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
		ArrowUp,
		ArrowLeft as ArrowLeftIcon,
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

		<div class="hero-animate hero-delay-3 mt-8 flex flex-wrap justify-center gap-3">
			<StatPill label={m.forYou_hero_stat1()} description={m.forYou_hero_stat1Desc()} />
			<StatPill label={m.forYou_hero_stat2()} description={m.forYou_hero_stat2Desc()} />
			<StatPill label={m.forYou_hero_stat3()} description={m.forYou_hero_stat3Desc()} />
			<StatPill label={m.forYou_hero_stat4()} description={m.forYou_hero_stat4Desc()} />
		</div>
	</section>

	<div class="h-px w-full bg-border"></div>

	<!-- ── Persona Cards ────────────────────────────────────────────── -->
	<section>
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
	<section>
		<div class="mb-8 text-center" use:reveal>
			<h2 class="text-2xl font-bold tracking-tight sm:text-3xl">
				{m.forYou_diagram_title()}
			</h2>
			<p class="mt-2 text-sm text-muted-foreground sm:text-base">
				{m.forYou_diagram_description()}
			</p>
		</div>

		<!-- Desktop: CSS Grid cross layout (5 rows: spoke / arrow / hub row / arrow / spoke) -->
		<div
			class="hidden sm:grid sm:grid-cols-[1fr_auto_1fr] sm:grid-rows-[auto_auto_auto_auto_auto] sm:items-center sm:justify-items-center sm:gap-x-4 sm:gap-y-1"
		>
			<!-- Row 1: Top spoke (SvelteKit) -->
			<div class="col-start-2 row-start-1" use:reveal={150}>
				{@render spokeBox(
					Monitor,
					m.forYou_diagram_spoke_svelte(),
					m.forYou_diagram_spoke_svelteDesc(),
					'border-blue-500/30 bg-blue-500/5',
					'text-blue-500'
				)}
			</div>

			<!-- Row 2: Arrow between hub and top spoke -->
			<div class="col-start-2 row-start-2">
				<ArrowUp class="arrow-flow-up h-6 w-6 text-muted-foreground/50" />
			</div>

			<!-- Row 3: Left spoke + hub + right spoke -->
			<div class="col-start-1 row-start-3 justify-self-end" use:reveal={300}>
				{@render spokeBox(
					Globe,
					m.forYou_diagram_spoke_external(),
					m.forYou_diagram_spoke_externalDesc(),
					'border-pink-500/30 bg-pink-500/5',
					'text-pink-500'
				)}
			</div>

			<div class="col-start-2 row-start-3 flex items-center gap-4">
				<ArrowLeftIcon class="arrow-flow-left h-6 w-6 text-muted-foreground/50" />
				<div use:reveal={0}>
					{@render hubBox()}
				</div>
				<ArrowRight class="arrow-flow h-6 w-6 text-muted-foreground/50" />
			</div>

			<div class="col-start-3 row-start-3 justify-self-start" use:reveal={300}>
				{@render spokeBox(
					Code,
					m.forYou_diagram_spoke_custom(),
					m.forYou_diagram_spoke_customDesc(),
					'border-violet-500/30 bg-violet-500/5',
					'text-violet-500'
				)}
			</div>

			<!-- Row 4: Arrow between hub and bottom spoke -->
			<div class="col-start-2 row-start-4">
				<ArrowDown class="arrow-flow-down h-6 w-6 text-muted-foreground/50" />
			</div>

			<!-- Row 5: Bottom spoke (Mobile) -->
			<div class="col-start-2 row-start-5" use:reveal={150}>
				{@render spokeBox(
					Smartphone,
					m.forYou_diagram_spoke_mobile(),
					m.forYou_diagram_spoke_mobileDesc(),
					'border-amber-500/30 bg-amber-500/5',
					'text-amber-500'
				)}
			</div>
		</div>

		<!-- Mobile: Vertical stack -->
		<div class="flex flex-col items-center gap-3 sm:hidden">
			<div use:reveal={0}>
				{@render hubBox()}
			</div>
			<ArrowDown class="arrow-flow-down h-6 w-6 text-muted-foreground/50" />
			<div use:reveal={100}>
				{@render spokeBox(
					Monitor,
					m.forYou_diagram_spoke_svelte(),
					m.forYou_diagram_spoke_svelteDesc(),
					'border-blue-500/30 bg-blue-500/5',
					'text-blue-500'
				)}
			</div>
			<ArrowDown class="arrow-flow-down h-6 w-6 text-muted-foreground/50" />
			<div use:reveal={200}>
				{@render spokeBox(
					Code,
					m.forYou_diagram_spoke_custom(),
					m.forYou_diagram_spoke_customDesc(),
					'border-violet-500/30 bg-violet-500/5',
					'text-violet-500'
				)}
			</div>
			<ArrowDown class="arrow-flow-down h-6 w-6 text-muted-foreground/50" />
			<div use:reveal={300}>
				{@render spokeBox(
					Smartphone,
					m.forYou_diagram_spoke_mobile(),
					m.forYou_diagram_spoke_mobileDesc(),
					'border-amber-500/30 bg-amber-500/5',
					'text-amber-500'
				)}
			</div>
			<ArrowDown class="arrow-flow-down h-6 w-6 text-muted-foreground/50" />
			<div use:reveal={400}>
				{@render spokeBox(
					Globe,
					m.forYou_diagram_spoke_external(),
					m.forYou_diagram_spoke_externalDesc(),
					'border-pink-500/30 bg-pink-500/5',
					'text-pink-500'
				)}
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

{#snippet hubBox()}
	<div
		class="flex w-full flex-col items-center rounded-xl border-2 border-green-500/40 bg-green-500/5 p-6 text-center shadow-sm shadow-green-500/10 sm:w-48"
	>
		<Server class="h-10 w-10 text-green-500" />
		<span class="mt-2 text-sm font-bold">{m.forYou_diagram_hub()}</span>
		<span class="text-xs text-muted-foreground">{m.forYou_diagram_hubDesc()}</span>
	</div>
{/snippet}

{#snippet spokeBox(
	Icon: Component<IconProps>,
	name: string,
	desc: string,
	borderClass: string,
	iconColor: string
)}
	<div
		class="flex w-full flex-col items-center justify-center rounded-xl border-2 p-5 text-center sm:min-h-[9rem] sm:w-48 {borderClass}"
	>
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

	@keyframes arrow-nudge-up {
		0%,
		100% {
			transform: translateY(0);
			opacity: 0.4;
		}
		50% {
			transform: translateY(-3px);
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

	@keyframes arrow-nudge-left {
		0%,
		100% {
			transform: translateX(0);
			opacity: 0.4;
		}
		50% {
			transform: translateX(-3px);
			opacity: 0.9;
		}
	}

	:global(.arrow-flow) {
		animation: arrow-nudge 2s ease-in-out infinite;
	}

	:global(.arrow-flow-up) {
		animation: arrow-nudge-up 2s ease-in-out infinite;
	}

	:global(.arrow-flow-down) {
		animation: arrow-nudge-down 2s ease-in-out infinite;
	}

	:global(.arrow-flow-left) {
		animation: arrow-nudge-left 2s ease-in-out infinite;
	}

	/* Respect reduced motion */
	@media (prefers-reduced-motion: reduce) {
		:global(.hero-animate) {
			animation: none;
		}
		:global(.arrow-flow),
		:global(.arrow-flow-up),
		:global(.arrow-flow-down),
		:global(.arrow-flow-left) {
			animation: none;
		}
	}
</style>
