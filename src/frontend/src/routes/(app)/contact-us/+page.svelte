<script lang="ts">
	import { reveal } from '$lib/actions/reveal';
	import { StatPill } from '$lib/components/common';
	import { Button } from '$lib/components/ui/button';
	import {
		Star,
		GitPullRequest,
		Coffee,
		Code,
		Mail,
		Linkedin,
		Globe,
		ArrowRight,
		ExternalLink,
		Sparkles,
		type IconProps
	} from '@lucide/svelte';
	import * as m from '$lib/paraglide/messages';
	import type { Component } from 'svelte';

	type EngageCard = {
		icon: Component<IconProps>;
		title: () => string;
		description: () => string;
		button: () => string;
		href: string;
		color: string;
		bgColor: string;
		gradientFrom: string;
		variant: 'default' | 'outline';
	};

	type ConnectChannel = {
		icon: Component<IconProps>;
		title: () => string;
		description: () => string;
		button: () => string;
		href: string;
		color: string;
		bgColor: string;
	};

	const engageCards: EngageCard[] = [
		{
			icon: Star,
			title: m.contact_star_title,
			description: m.contact_star_description,
			button: m.contact_star_button,
			href: 'https://github.com/fpindej/netrock',
			color: 'text-amber-500',
			bgColor: 'bg-amber-500/10',
			gradientFrom: 'from-amber-500',
			variant: 'default'
		},
		{
			icon: GitPullRequest,
			title: m.contact_contribute_title,
			description: m.contact_contribute_description,
			button: m.contact_contribute_button,
			href: 'https://github.com/fpindej/netrock',
			color: 'text-green-500',
			bgColor: 'bg-green-500/10',
			gradientFrom: 'from-green-500',
			variant: 'outline'
		},
		{
			icon: Coffee,
			title: m.contact_coffee_title,
			description: m.contact_coffee_description,
			button: m.contact_coffee_button,
			href: 'https://buymeacoffee.com/fpindej',
			color: 'text-pink-500',
			bgColor: 'bg-pink-500/10',
			gradientFrom: 'from-pink-500',
			variant: 'outline'
		},
		{
			icon: Code,
			title: m.contact_explore_title,
			description: m.contact_explore_description,
			button: m.contact_explore_button,
			href: 'https://github.com/fpindej/netrock',
			color: 'text-blue-500',
			bgColor: 'bg-blue-500/10',
			gradientFrom: 'from-blue-500',
			variant: 'outline'
		}
	];

	const connectChannels: ConnectChannel[] = [
		{
			icon: Mail,
			title: m.contact_email_title,
			description: m.contact_email_description,
			button: m.contact_email_button,
			href: 'mailto:contact@mail.pindej.cz',
			color: 'text-blue-500',
			bgColor: 'bg-blue-500/10'
		},
		{
			icon: Linkedin,
			title: m.contact_linkedin_title,
			description: m.contact_linkedin_description,
			button: m.contact_linkedin_button,
			href: 'https://www.linkedin.com/in/filip-dorian-pindej/',
			color: 'text-sky-500',
			bgColor: 'bg-sky-500/10'
		},
		{
			icon: Globe,
			title: m.contact_web_title,
			description: m.contact_web_description,
			button: m.contact_web_button,
			href: 'https://pindej.cz',
			color: 'text-violet-500',
			bgColor: 'bg-violet-500/10'
		}
	];
</script>

<svelte:head>
	<title>{m.meta_titleTemplate({ title: m.meta_contactMe_title() })}</title>
	<meta name="description" content={m.meta_contactMe_description()} />
</svelte:head>

<div class="space-y-12 pb-8">
	<!-- ── Hero ──────────────────────────────────────────────────────── -->
	<section class="text-center">
		<span
			class="hero-animate hero-delay-0 inline-block rounded-full border border-primary/20 bg-primary/10 px-4 py-1.5 text-xs font-semibold tracking-wide text-primary"
		>
			{m.contact_hero_badge()}
		</span>

		<h1
			class="hero-animate hero-delay-1 mt-4 bg-gradient-to-r from-foreground via-foreground/80 to-foreground/60 bg-clip-text text-4xl font-bold tracking-tight text-transparent sm:text-5xl"
		>
			{m.contact_hero_title()}
		</h1>

		<p
			class="hero-animate hero-delay-2 mx-auto mt-4 max-w-2xl text-base text-muted-foreground sm:text-lg"
		>
			{m.contact_hero_subtitle()}
		</p>

		<div class="hero-animate hero-delay-3 mt-8 grid grid-cols-2 gap-3 sm:grid-cols-4">
			<StatPill label={m.contact_stats_tests()} description={m.contact_stats_testsDesc()} />
			<StatPill label={m.contact_stats_license()} description={m.contact_stats_licenseDesc()} />
			<StatPill label={m.contact_stats_stack()} description={m.contact_stats_stackDesc()} />
			<StatPill label={m.contact_stats_languages()} description={m.contact_stats_languagesDesc()} />
		</div>
	</section>

	<div class="h-px w-full bg-border"></div>

	<!-- ── Get Involved ─────────────────────────────────────────────── -->
	<section>
		<div class="mb-8 text-center" use:reveal>
			<h2 class="text-2xl font-bold tracking-tight sm:text-3xl">
				{m.contact_engage_title()}
			</h2>
			<p class="mt-2 text-sm text-muted-foreground sm:text-base">
				{m.contact_engage_description()}
			</p>
		</div>

		<div class="grid gap-4 sm:grid-cols-2">
			{#each engageCards as card, i (card.title())}
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
						<div class="mt-auto pt-1">
							<Button
								variant={card.variant}
								size="sm"
								href={card.href}
								target="_blank"
								rel="noopener noreferrer"
								class="gap-1.5"
							>
								{card.button()}
								<ExternalLink class="h-3.5 w-3.5" />
							</Button>
						</div>
					</div>
				</div>
			{/each}
		</div>
	</section>

	<div class="h-px w-full bg-border"></div>

	<!-- ── Get in Touch ─────────────────────────────────────────────── -->
	<section>
		<div class="mb-8 text-center" use:reveal>
			<h2 class="text-2xl font-bold tracking-tight sm:text-3xl">
				{m.contact_connect_title()}
			</h2>
			<p class="mt-2 text-sm text-muted-foreground sm:text-base">
				{m.contact_connect_description()}
			</p>
		</div>

		<div class="grid gap-3 sm:grid-cols-3">
			{#each connectChannels as channel, i (channel.href)}
				<div
					class="group flex items-center gap-4 rounded-xl border bg-card p-4 transition-colors hover:bg-accent/50"
					use:reveal={i * 80}
				>
					<div
						class="flex h-10 w-10 shrink-0 items-center justify-center rounded-lg {channel.bgColor}"
					>
						<channel.icon class="h-5 w-5 {channel.color}" />
					</div>
					<div class="min-w-0 flex-1">
						<h4 class="text-sm font-semibold">{channel.title()}</h4>
						<p class="text-xs text-muted-foreground">{channel.description()}</p>
					</div>
					<Button
						variant="ghost"
						size="sm"
						href={channel.href}
						target="_blank"
						rel="noopener noreferrer"
						class="shrink-0 gap-1 px-2"
					>
						{channel.button()}
						<ArrowRight class="h-3.5 w-3.5 transition-transform group-hover:translate-x-0.5" />
					</Button>
				</div>
			{/each}
		</div>
	</section>

	<div class="h-px w-full bg-border"></div>

	<!-- ── Final CTA ────────────────────────────────────────────────── -->
	<section use:reveal>
		<div
			class="mx-auto max-w-xl rounded-xl border border-primary/20 bg-gradient-to-br from-primary/5 to-transparent p-6 text-center sm:p-8"
		>
			<div class="mx-auto mb-4 flex h-12 w-12 items-center justify-center rounded-xl bg-primary/10">
				<Sparkles class="h-6 w-6 text-primary" />
			</div>
			<h2 class="text-xl font-bold tracking-tight sm:text-2xl">
				{m.contact_cta_title()}
			</h2>
			<p class="mt-2 text-sm text-muted-foreground">
				{m.contact_cta_description()}
			</p>
			<div class="mt-5 flex flex-col items-center gap-3 sm:flex-row sm:justify-center">
				<Button href="https://github.com/fpindej/netrock" target="_blank" rel="noopener noreferrer">
					<Star class="me-2 h-4 w-4" />
					{m.contact_cta_starButton()}
					<ExternalLink class="ms-2 h-3.5 w-3.5" />
				</Button>
				<Button
					variant="outline"
					href="https://github.com/fpindej/netrock"
					target="_blank"
					rel="noopener noreferrer"
				>
					{m.contact_cta_repoButton()}
					<ExternalLink class="ms-2 h-3.5 w-3.5" />
				</Button>
			</div>
		</div>
	</section>
</div>
