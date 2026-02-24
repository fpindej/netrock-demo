<script lang="ts">
	import { GuideTour } from '$lib/components/guide';
	import { Button } from '$lib/components/ui/button';
	import {
		Rocket,
		ShieldCheck,
		CircleUser,
		Users,
		BarChart3,
		Settings,
		ArrowRight,
		Lightbulb,
		type IconProps
	} from '@lucide/svelte';
	import * as m from '$lib/paraglide/messages';
	import type { Component } from 'svelte';
	import type { Action } from 'svelte/action';

	/** Scroll-triggered reveal animation. Applied via JS to avoid SSR flash. */
	const reveal: Action<HTMLElement, number | undefined> = (node, delay = 0) => {
		if (
			typeof window !== 'undefined' &&
			window.matchMedia('(prefers-reduced-motion: reduce)').matches
		)
			return;

		node.style.opacity = '0';
		node.style.transform = 'translateY(20px)';
		node.style.transition = `opacity 0.6s ease-out ${delay}ms, transform 0.6s ease-out ${delay}ms`;

		const observer = new IntersectionObserver(
			(entries) => {
				const entry = entries[0];
				if (entry?.isIntersecting) {
					node.style.opacity = '1';
					node.style.transform = 'none';
					observer.disconnect();
				}
			},
			{ threshold: 0.1 }
		);
		observer.observe(node);
		return { destroy: () => observer.disconnect() };
	};

	type FeatureCard = {
		icon: Component<IconProps>;
		title: () => string;
		description: () => string;
		color: string;
		bgColor: string;
		gradientFrom: string;
		action?: { label: () => string; href: string };
	};

	const featureCards: FeatureCard[] = [
		{
			icon: Rocket,
			title: m.guide_step1_title,
			description: m.guide_step1_description,
			color: 'text-blue-500',
			bgColor: 'bg-blue-500/10',
			gradientFrom: 'from-blue-500'
		},
		{
			icon: ShieldCheck,
			title: m.guide_step2_title,
			description: m.guide_step2_description,
			color: 'text-green-500',
			bgColor: 'bg-green-500/10',
			gradientFrom: 'from-green-500'
		},
		{
			icon: CircleUser,
			title: m.guide_step3_title,
			description: m.guide_step3_description,
			color: 'text-violet-500',
			bgColor: 'bg-violet-500/10',
			gradientFrom: 'from-violet-500',
			action: { label: m.guide_goToProfile, href: '/profile' }
		},
		{
			icon: Users,
			title: m.guide_step4_title,
			description: m.guide_step4_description,
			color: 'text-amber-500',
			bgColor: 'bg-amber-500/10',
			gradientFrom: 'from-amber-500',
			action: { label: m.guide_goToContacts, href: '/contacts' }
		},
		{
			icon: BarChart3,
			title: m.guide_step5_title,
			description: m.guide_step5_description,
			color: 'text-pink-500',
			bgColor: 'bg-pink-500/10',
			gradientFrom: 'from-pink-500',
			action: { label: m.guide_goToAnalytics, href: '/analytics' }
		},
		{
			icon: Settings,
			title: m.guide_step6_title,
			description: m.guide_step6_description,
			color: 'text-red-500',
			bgColor: 'bg-red-500/10',
			gradientFrom: 'from-red-500',
			action: { label: m.guide_viewAsAdmin, href: '/admin/users' }
		}
	];
</script>

<svelte:head>
	<title>{m.meta_titleTemplate({ title: m.meta_guide_title() })}</title>
	<meta name="description" content={m.meta_guide_description()} />
</svelte:head>

<div class="space-y-12 pb-8">
	<!-- Hero -->
	<section class="text-center">
		<span
			class="hero-animate hero-delay-0 inline-block rounded-full border border-primary/20 bg-primary/10 px-4 py-1.5 text-xs font-semibold tracking-wide text-primary"
		>
			{m.guide_hero_badge()}
		</span>

		<h1
			class="hero-animate hero-delay-1 mt-4 bg-gradient-to-r from-foreground via-foreground/80 to-foreground/60 bg-clip-text text-4xl font-bold tracking-tight text-transparent sm:text-5xl"
		>
			{m.guide_hero_title()}
		</h1>

		<p
			class="hero-animate hero-delay-2 mx-auto mt-4 max-w-2xl text-base text-muted-foreground sm:text-lg"
		>
			{m.guide_hero_subtitle()}
		</p>

		<div class="hero-animate hero-delay-3 mt-8 flex flex-wrap justify-center gap-3">
			{@render statPill(m.guide_stats_stack(), m.guide_stats_stackDesc())}
			{@render statPill(m.guide_stats_auth(), m.guide_stats_authDesc())}
			{@render statPill(m.guide_stats_admin(), m.guide_stats_adminDesc())}
			{@render statPill(m.guide_stats_pipeline(), m.guide_stats_pipelineDesc())}
		</div>

		<div class="hero-animate hero-delay-3 mt-6 flex justify-center">
			<GuideTour />
		</div>
	</section>

	<div class="h-px w-full bg-border"></div>

	<!-- Feature Cards -->
	<section>
		<div class="mb-8 text-center" use:reveal>
			<h2 class="text-2xl font-bold tracking-tight sm:text-3xl">
				{m.guide_features_title()}
			</h2>
			<p class="mt-2 text-sm text-muted-foreground sm:text-base">
				{m.guide_features_description()}
			</p>
		</div>

		<div class="grid gap-4 sm:grid-cols-2 xl:grid-cols-3">
			{#each featureCards as card, i (card.title())}
				<div
					class="group relative flex flex-col overflow-hidden rounded-xl border bg-card transition-colors hover:bg-accent/50"
					use:reveal={i * 80}
				>
					<div
						class="absolute inset-x-0 top-0 h-1 bg-gradient-to-r {card.gradientFrom} to-transparent"
					></div>
					<div class="flex flex-1 flex-col gap-3 p-5">
						<div class="flex items-center gap-3">
							<div
								class="flex h-10 w-10 shrink-0 items-center justify-center rounded-lg {card.bgColor}"
							>
								<card.icon class="h-5 w-5 {card.color}" />
							</div>
							<h3 class="text-sm font-semibold">{card.title()}</h3>
						</div>
						<p class="text-sm text-muted-foreground">{card.description()}</p>
						{#if card.action}
							<div class="mt-auto pt-1">
								<Button
									variant="ghost"
									size="sm"
									href={card.action.href}
									class="group/btn gap-1.5 px-0 text-xs font-medium"
								>
									{card.action.label()}
									<ArrowRight
										class="h-3.5 w-3.5 transition-transform group-hover/btn:translate-x-0.5"
									/>
								</Button>
							</div>
						{/if}
					</div>
				</div>
			{/each}
		</div>
	</section>

	<div class="h-px w-full bg-border"></div>

	<!-- Template Disclaimer -->
	<section use:reveal>
		<div
			class="rounded-xl border-2 border-dashed border-muted-foreground/20 bg-muted/30 p-6 sm:p-8"
		>
			<div class="flex gap-4">
				<div class="flex h-10 w-10 shrink-0 items-center justify-center rounded-lg bg-amber-500/10">
					<Lightbulb class="h-5 w-5 text-amber-500" />
				</div>
				<div>
					<h3 class="text-base font-semibold">{m.guide_disclaimer_title()}</h3>
					<p class="mt-1 text-sm text-muted-foreground">
						{m.guide_disclaimer_description()}
					</p>
					<Button
						variant="link"
						size="sm"
						href="/how-it-works"
						class="mt-2 h-auto gap-1 p-0 text-xs"
					>
						{m.guide_disclaimer_learnMore()}
						<ArrowRight class="h-3.5 w-3.5" />
					</Button>
				</div>
			</div>
		</div>
	</section>
</div>

{#snippet statPill(label: string, description: string)}
	<div
		class="flex min-w-[9rem] flex-col items-center rounded-xl border bg-card px-5 py-3 shadow-sm transition-shadow hover:shadow-md"
	>
		<span class="text-sm font-bold">{label}</span>
		<span class="text-xs text-muted-foreground">{description}</span>
	</div>
{/snippet}

<style>
	@keyframes hero-fade-in {
		from {
			opacity: 0;
			transform: translateY(20px);
		}
	}

	:global(.hero-animate) {
		animation: hero-fade-in 0.7s ease-out both;
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

	@media (prefers-reduced-motion: reduce) {
		:global(.hero-animate) {
			animation: none;
		}
	}
</style>
