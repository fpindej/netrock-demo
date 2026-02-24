<script lang="ts">
	import { reveal } from '$lib/actions/reveal';
	import { StatPill } from '$lib/components/common';
	import * as Card from '$lib/components/ui/card';
	import {
		Monitor,
		Server,
		Database,
		ArrowRight,
		ArrowDown,
		Zap,
		Eye,
		HardDrive,
		Clock,
		Bot,
		TestTube,
		FileCode2,
		ShieldCheck,
		Lock,
		UserCog,
		Mail,
		Upload,
		FileText,
		BookOpen,
		Globe,
		HeartPulse,
		Gauge,
		Container,
		type IconProps
	} from '@lucide/svelte';
	import * as m from '$lib/paraglide/messages';
	import type { Component } from 'svelte';

	type FeatureItem = {
		icon: Component<IconProps>;
		title: () => string;
		description: () => string;
		color: string;
	};

	const features: FeatureItem[] = [
		{
			icon: Lock,
			title: m.howItWorks_features_auth,
			description: m.howItWorks_features_authDesc,
			color: 'text-blue-500'
		},
		{
			icon: UserCog,
			title: m.howItWorks_features_permissions,
			description: m.howItWorks_features_permissionsDesc,
			color: 'text-violet-500'
		},
		{
			icon: Mail,
			title: m.howItWorks_features_email,
			description: m.howItWorks_features_emailDesc,
			color: 'text-pink-500'
		},
		{
			icon: Clock,
			title: m.howItWorks_features_jobs,
			description: m.howItWorks_features_jobsDesc,
			color: 'text-amber-500'
		},
		{
			icon: Upload,
			title: m.howItWorks_features_files,
			description: m.howItWorks_features_filesDesc,
			color: 'text-teal-500'
		},
		{
			icon: FileText,
			title: m.howItWorks_features_audit,
			description: m.howItWorks_features_auditDesc,
			color: 'text-orange-500'
		},
		{
			icon: Zap,
			title: m.howItWorks_features_cache,
			description: m.howItWorks_features_cacheDesc,
			color: 'text-red-500'
		},
		{
			icon: BookOpen,
			title: m.howItWorks_features_apiDocs,
			description: m.howItWorks_features_apiDocsDesc,
			color: 'text-emerald-500'
		},
		{
			icon: Globe,
			title: m.howItWorks_features_i18n,
			description: m.howItWorks_features_i18nDesc,
			color: 'text-cyan-500'
		},
		{
			icon: HeartPulse,
			title: m.howItWorks_features_health,
			description: m.howItWorks_features_healthDesc,
			color: 'text-rose-500'
		},
		{
			icon: Gauge,
			title: m.howItWorks_features_rateLimit,
			description: m.howItWorks_features_rateLimitDesc,
			color: 'text-yellow-500'
		},
		{
			icon: Container,
			title: m.howItWorks_features_docker,
			description: m.howItWorks_features_dockerDesc,
			color: 'text-sky-500'
		}
	];
</script>

<svelte:head>
	<title>{m.meta_titleTemplate({ title: m.meta_howItWorks_title() })}</title>
	<meta name="description" content={m.meta_howItWorks_description()} />
</svelte:head>

<div class="space-y-12 pb-8">
	<!-- ── Hero ──────────────────────────────────────────────────────── -->
	<section class="text-center">
		<span
			class="hero-animate hero-delay-0 inline-block rounded-full border border-primary/20 bg-primary/10 px-4 py-1.5 text-xs font-semibold tracking-wide text-primary"
		>
			{m.howItWorks_hero_badge()}
		</span>

		<h1
			class="hero-animate hero-delay-1 mt-4 bg-gradient-to-r from-foreground via-foreground/80 to-foreground/60 bg-clip-text text-4xl font-bold tracking-tight text-transparent sm:text-5xl"
		>
			{m.howItWorks_hero_title()}
		</h1>

		<p
			class="hero-animate hero-delay-2 mx-auto mt-4 max-w-2xl text-base text-muted-foreground sm:text-lg"
		>
			{m.howItWorks_hero_subtitle()}
		</p>

		<!-- Stats pills -->
		<div class="hero-animate hero-delay-3 mt-8 flex flex-wrap justify-center gap-3">
			<StatPill label={m.howItWorks_stats_tests()} description={m.howItWorks_stats_testsDesc()} />
			<StatPill
				label={m.howItWorks_stats_features()}
				description={m.howItWorks_stats_featuresDesc()}
			/>
			<StatPill
				label={m.howItWorks_stats_languages()}
				description={m.howItWorks_stats_languagesDesc()}
			/>
			<StatPill
				label={m.howItWorks_stats_architecture()}
				description={m.howItWorks_stats_architectureDesc()}
			/>
		</div>
	</section>

	<div class="h-px w-full bg-border"></div>

	<!-- ── Architecture ─────────────────────────────────────────────── -->
	<section>
		<div class="mb-8 text-center" use:reveal>
			<h2 class="text-2xl font-bold tracking-tight sm:text-3xl">
				{m.howItWorks_architecture_title()}
			</h2>
			<p class="mt-2 text-sm text-muted-foreground sm:text-base">
				{m.howItWorks_architecture_description()}
			</p>
		</div>

		<!-- Main flow: Frontend → API → Database -->
		<div class="flex flex-col items-center gap-4 sm:flex-row sm:justify-center sm:gap-6">
			<div use:reveal={0}>
				{@render archBox(
					Monitor,
					m.howItWorks_architecture_frontend(),
					m.howItWorks_architecture_frontendDesc(),
					'border-blue-500/30 bg-blue-500/5',
					'text-blue-500'
				)}
			</div>
			<ArrowRight class="arrow-flow hidden h-6 w-6 shrink-0 text-muted-foreground/50 sm:block" />
			<ArrowDown class="arrow-flow-down h-6 w-6 shrink-0 text-muted-foreground/50 sm:hidden" />
			<div use:reveal={150}>
				{@render archBox(
					Server,
					m.howItWorks_architecture_api(),
					m.howItWorks_architecture_apiDesc(),
					'border-green-500/30 bg-green-500/5',
					'text-green-500'
				)}
			</div>
			<ArrowRight class="arrow-flow hidden h-6 w-6 shrink-0 text-muted-foreground/50 sm:block" />
			<ArrowDown class="arrow-flow-down h-6 w-6 shrink-0 text-muted-foreground/50 sm:hidden" />
			<div use:reveal={300}>
				{@render archBox(
					Database,
					m.howItWorks_architecture_db(),
					m.howItWorks_architecture_dbDesc(),
					'border-amber-500/30 bg-amber-500/5',
					'text-amber-500'
				)}
			</div>
		</div>

		<!-- Supporting services -->
		<p
			class="mt-8 text-center text-xs font-medium tracking-wider text-muted-foreground uppercase"
			use:reveal={400}
		>
			{m.howItWorks_architecture_supporting()}
		</p>
		<div class="mt-3 grid grid-cols-2 gap-3 sm:grid-cols-4">
			<div use:reveal={450}>
				{@render supportBox(
					Zap,
					m.howItWorks_architecture_redis(),
					m.howItWorks_architecture_redisDesc(),
					'text-red-500'
				)}
			</div>
			<div use:reveal={525}>
				{@render supportBox(
					Eye,
					m.howItWorks_architecture_seq(),
					m.howItWorks_architecture_seqDesc(),
					'text-purple-500'
				)}
			</div>
			<div use:reveal={600}>
				{@render supportBox(
					HardDrive,
					m.howItWorks_architecture_minio(),
					m.howItWorks_architecture_minioDesc(),
					'text-teal-500'
				)}
			</div>
			<div use:reveal={675}>
				{@render supportBox(
					Clock,
					m.howItWorks_architecture_hangfire(),
					m.howItWorks_architecture_hangfireDesc(),
					'text-orange-500'
				)}
			</div>
		</div>
	</section>

	<div class="h-px w-full bg-border"></div>

	<!-- ── Tech Stack ───────────────────────────────────────────────── -->
	<section>
		<div class="mb-8 text-center" use:reveal>
			<h2 class="text-2xl font-bold tracking-tight sm:text-3xl">
				{m.howItWorks_techStack_title()}
			</h2>
			<p class="mt-2 text-sm text-muted-foreground sm:text-base">
				{m.howItWorks_techStack_description()}
			</p>
		</div>

		<div class="grid gap-6 sm:grid-cols-3">
			<!-- Frontend -->
			<div class="h-full" use:reveal={0}>
				<Card.Root class="relative h-full overflow-hidden">
					<div
						class="absolute inset-x-0 top-0 h-1 bg-gradient-to-r from-blue-500 to-cyan-500"
					></div>
					<Card.Header>
						<Card.Title class="text-blue-500">{m.howItWorks_techStack_frontend()}</Card.Title>
					</Card.Header>
					<Card.Content>
						<ul class="space-y-2.5 text-sm">
							{@render techItem('SvelteKit 2', 'Meta-framework + SSR')}
							{@render techItem('Svelte 5', 'Runes reactivity system')}
							{@render techItem('TypeScript 5', 'Strict mode, no any')}
							{@render techItem('Tailwind CSS 4', 'Utility-first styling')}
							{@render techItem('Paraglide', 'Compile-time i18n')}
							{@render techItem('openapi-fetch', 'Type-safe API client')}
							{@render techItem('Vitest', 'Unit & component tests')}
						</ul>
					</Card.Content>
				</Card.Root>
			</div>

			<!-- Backend -->
			<div class="h-full" use:reveal={120}>
				<Card.Root class="relative h-full overflow-hidden">
					<div
						class="absolute inset-x-0 top-0 h-1 bg-gradient-to-r from-green-500 to-emerald-500"
					></div>
					<Card.Header>
						<Card.Title class="text-green-500">{m.howItWorks_techStack_backend()}</Card.Title>
					</Card.Header>
					<Card.Content>
						<ul class="space-y-2.5 text-sm">
							{@render techItem('.NET 10', 'C# 13, ASP.NET Core')}
							{@render techItem('EF Core 10', 'PostgreSQL + migrations')}
							{@render techItem('FluentValidation', 'Declarative input rules')}
							{@render techItem('Hangfire', 'Background job engine')}
							{@render techItem('Serilog', 'Structured logging → Seq')}
							{@render techItem('SkiaSharp', 'Image processing')}
							{@render techItem('xUnit', '4-tier test suite')}
						</ul>
					</Card.Content>
				</Card.Root>
			</div>

			<!-- Infrastructure -->
			<div class="h-full" use:reveal={240}>
				<Card.Root class="relative h-full overflow-hidden">
					<div
						class="absolute inset-x-0 top-0 h-1 bg-gradient-to-r from-amber-500 to-orange-500"
					></div>
					<Card.Header>
						<Card.Title class="text-amber-500">{m.howItWorks_techStack_infrastructure()}</Card.Title
						>
					</Card.Header>
					<Card.Content>
						<ul class="space-y-2.5 text-sm">
							{@render techItem('PostgreSQL', 'Primary relational DB')}
							{@render techItem('Redis', 'Distributed cache + resilience')}
							{@render techItem('MinIO', 'S3-compatible object store')}
							{@render techItem('Seq', 'Log aggregation & search')}
							{@render techItem('Docker Compose', 'Dev + production overlays')}
							{@render techItem('Resend', 'Email delivery service')}
							{@render techItem('Turnstile', 'Bot protection')}
						</ul>
					</Card.Content>
				</Card.Root>
			</div>
		</div>
	</section>

	<div class="h-px w-full bg-border"></div>

	<!-- ── Quality Assurance ─────────────────────────────────────────── -->
	<section>
		<div class="mb-8 text-center" use:reveal>
			<h2 class="text-2xl font-bold tracking-tight sm:text-3xl">
				{m.howItWorks_quality_title()}
			</h2>
			<p class="mt-2 text-sm text-muted-foreground sm:text-base">
				{m.howItWorks_quality_description()}
			</p>
		</div>

		<div class="grid gap-4 sm:grid-cols-2">
			<div class="h-full" use:reveal={0}>
				{@render qualityCard(
					Bot,
					m.howItWorks_quality_ai_title(),
					m.howItWorks_quality_ai_description(),
					'bg-violet-500/10 text-violet-500'
				)}
			</div>
			<div class="h-full" use:reveal={100}>
				{@render qualityCard(
					TestTube,
					m.howItWorks_quality_testing_title(),
					m.howItWorks_quality_testing_description(),
					'bg-emerald-500/10 text-emerald-500'
				)}
			</div>
			<div class="h-full" use:reveal={200}>
				{@render qualityCard(
					FileCode2,
					m.howItWorks_quality_conventions_title(),
					m.howItWorks_quality_conventions_description(),
					'bg-blue-500/10 text-blue-500'
				)}
			</div>
			<div class="h-full" use:reveal={300}>
				{@render qualityCard(
					ShieldCheck,
					m.howItWorks_quality_security_title(),
					m.howItWorks_quality_security_description(),
					'bg-rose-500/10 text-rose-500'
				)}
			</div>
		</div>
	</section>

	<div class="h-px w-full bg-border"></div>

	<!-- ── Features ──────────────────────────────────────────────────── -->
	<section>
		<div class="mb-8 text-center" use:reveal>
			<h2 class="text-2xl font-bold tracking-tight sm:text-3xl">
				{m.howItWorks_features_title()}
			</h2>
			<p class="mt-2 text-sm text-muted-foreground sm:text-base">
				{m.howItWorks_features_description()}
			</p>
		</div>

		<div class="grid gap-3 sm:grid-cols-2 xl:grid-cols-3">
			{#each features as feat, i (feat.title())}
				<div
					class="group flex gap-3 rounded-lg border bg-card p-4 transition-colors hover:bg-accent/50"
					use:reveal={i * 50}
				>
					<div
						class="flex h-10 w-10 shrink-0 items-center justify-center rounded-lg bg-muted transition-colors group-hover:bg-background"
					>
						<feat.icon class="h-5 w-5 {feat.color}" />
					</div>
					<div class="min-w-0">
						<h4 class="text-sm font-semibold">{feat.title()}</h4>
						<p class="mt-0.5 text-xs text-muted-foreground">{feat.description()}</p>
					</div>
				</div>
			{/each}
		</div>
	</section>
</div>

<!-- ── Snippets ─────────────────────────────────────────────────────── -->

{#snippet archBox(
	Icon: Component<IconProps>,
	name: string,
	desc: string,
	borderClass: string,
	iconColor: string
)}
	<div
		class="flex w-full flex-col items-center rounded-xl border-2 p-5 text-center sm:w-44 {borderClass}"
	>
		<Icon class="h-8 w-8 {iconColor}" />
		<span class="mt-2 text-sm font-bold">{name}</span>
		<span class="text-xs text-muted-foreground">{desc}</span>
	</div>
{/snippet}

{#snippet supportBox(Icon: Component<IconProps>, name: string, desc: string, iconColor: string)}
	<div class="flex flex-col items-center rounded-lg border bg-card p-3 text-center">
		<Icon class="h-5 w-5 {iconColor}" />
		<span class="mt-1 text-sm font-medium">{name}</span>
		<span class="text-xs text-muted-foreground">{desc}</span>
	</div>
{/snippet}

{#snippet techItem(name: string, desc: string)}
	<li class="flex items-baseline gap-2">
		<span class="h-1.5 w-1.5 shrink-0 rounded-full bg-foreground/20"></span>
		<span>
			<span class="font-medium">{name}</span>
			<span class="text-muted-foreground"> — {desc}</span>
		</span>
	</li>
{/snippet}

{#snippet qualityCard(
	Icon: Component<IconProps>,
	title: string,
	description: string,
	iconClass: string
)}
	<Card.Root class="h-full">
		<Card.Header>
			<div class="flex items-center gap-3">
				<div class="flex h-10 w-10 items-center justify-center rounded-lg {iconClass}">
					<Icon class="h-5 w-5" />
				</div>
				<Card.Title class="text-base">{title}</Card.Title>
			</div>
		</Card.Header>
		<Card.Content>
			<p class="text-sm text-muted-foreground">{description}</p>
		</Card.Content>
	</Card.Root>
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

	/* Respect reduced motion */
	@media (prefers-reduced-motion: reduce) {
		:global(.hero-animate) {
			animation: none;
		}
		:global(.arrow-flow),
		:global(.arrow-flow-down) {
			animation: none;
		}
	}
</style>
