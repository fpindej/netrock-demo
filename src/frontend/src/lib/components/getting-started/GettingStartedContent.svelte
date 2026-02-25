<script lang="ts">
	import { resolve } from '$app/paths';
	import { reveal } from '$lib/actions/reveal';
	import { Button } from '$lib/components/ui/button';
	import {
		Github,
		GitFork,
		Terminal,
		Container,
		Rocket,
		ExternalLink,
		BookOpen,
		CircleCheck,
		Info
	} from '@lucide/svelte';
	import * as m from '$lib/paraglide/messages';
	import CodeBlock from './CodeBlock.svelte';
	import ChecklistItem from './ChecklistItem.svelte';
	import StepCard from './StepCard.svelte';
</script>

<div class="space-y-12 pb-8">
	<!-- ── Hero ──────────────────────────────────────────────────────── -->
	<section class="text-center">
		<span
			class="hero-animate hero-delay-0 inline-block rounded-full border border-primary/20 bg-primary/10 px-4 py-1.5 text-xs font-semibold tracking-wide text-primary"
		>
			{m.gettingStarted_hero_badge()}
		</span>

		<h1
			class="hero-animate hero-delay-1 mt-4 bg-gradient-to-r from-foreground via-foreground/80 to-foreground/60 bg-clip-text text-4xl font-bold tracking-tight text-transparent sm:text-5xl"
		>
			{m.gettingStarted_hero_title()}
		</h1>

		<p
			class="hero-animate hero-delay-2 mx-auto mt-4 max-w-2xl text-base text-muted-foreground sm:text-lg"
		>
			{m.gettingStarted_hero_subtitle()}
		</p>
	</section>

	<div class="h-px w-full bg-border"></div>

	<!-- ── Prerequisites ────────────────────────────────────────────── -->
	<section use:reveal>
		<div class="rounded-xl border border-blue-500/20 bg-blue-500/5 p-5 sm:p-6">
			<div class="mb-4 flex items-center gap-3">
				<div class="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-blue-500/10">
					<Info class="h-5 w-5 text-blue-500" />
				</div>
				<h2 class="text-lg font-semibold">{m.gettingStarted_prereq_title()}</h2>
			</div>
			<p class="mb-4 text-sm text-muted-foreground">
				{m.gettingStarted_prereq_description()}
			</p>
			<div class="grid gap-3 sm:grid-cols-2">
				<div class="flex items-start gap-3 rounded-lg border bg-card p-3">
					<CircleCheck class="mt-0.5 h-4 w-4 shrink-0 text-green-500" />
					<div>
						<span class="text-sm font-medium">{m.gettingStarted_prereq_git()}</span>
						<p class="text-xs text-muted-foreground">
							{m.gettingStarted_prereq_gitDesc()}
						</p>
					</div>
				</div>
				<div class="flex items-start gap-3 rounded-lg border bg-card p-3">
					<CircleCheck class="mt-0.5 h-4 w-4 shrink-0 text-green-500" />
					<div>
						<span class="text-sm font-medium">{m.gettingStarted_prereq_docker()}</span>
						<p class="text-xs text-muted-foreground">
							{m.gettingStarted_prereq_dockerDesc()}
						</p>
					</div>
				</div>
				<div class="flex items-start gap-3 rounded-lg border bg-card p-3">
					<CircleCheck class="mt-0.5 h-4 w-4 shrink-0 text-green-500" />
					<div>
						<span class="text-sm font-medium">{m.gettingStarted_prereq_dotnet()}</span>
						<p class="text-xs text-muted-foreground">
							{m.gettingStarted_prereq_dotnetDesc()}
						</p>
					</div>
				</div>
				<div class="flex items-start gap-3 rounded-lg border bg-card p-3">
					<CircleCheck class="mt-0.5 h-4 w-4 shrink-0 text-green-500" />
					<div>
						<span class="text-sm font-medium">{m.gettingStarted_prereq_node()}</span>
						<p class="text-xs text-muted-foreground">
							{m.gettingStarted_prereq_nodeDesc()}
						</p>
					</div>
				</div>
			</div>
		</div>
	</section>

	<div class="h-px w-full bg-border"></div>

	<!-- ── Steps Flow ───────────────────────────────────────────────── -->
	<section data-tour="getting-started-steps">
		<div class="mb-8 text-center" use:reveal>
			<h2 class="text-2xl font-bold tracking-tight sm:text-3xl">
				{m.gettingStarted_steps_title()}
			</h2>
			<p class="mt-2 text-sm text-muted-foreground sm:text-base">
				{m.gettingStarted_steps_description()}
			</p>
		</div>

		<div class="mx-auto max-w-2xl">
			<!-- Step 1: Visit the repo -->
			<StepCard
				step={1}
				icon={Github}
				color="blue"
				title={m.gettingStarted_step1_title}
				description={m.gettingStarted_step1_description}
			>
				<Button
					variant="outline"
					size="sm"
					href="https://github.com/fpindej/netrock"
					target="_blank"
					rel="noopener noreferrer"
				>
					<Github class="me-2 h-4 w-4" />
					{m.gettingStarted_step1_linkText()}
					<ExternalLink class="ms-2 h-3.5 w-3.5" />
				</Button>
				<p class="mt-3 text-xs text-muted-foreground">
					{m.gettingStarted_step1_note()}
				</p>
			</StepCard>

			<!-- Step 2: Create from template -->
			<StepCard
				step={2}
				icon={GitFork}
				color="green"
				title={m.gettingStarted_step2_title}
				description={m.gettingStarted_step2_description}
			>
				<p class="text-xs text-muted-foreground">
					{m.gettingStarted_step2_note()}
				</p>
			</StepCard>

			<!-- Step 3: Clone & initialize -->
			<StepCard
				step={3}
				icon={Terminal}
				color="violet"
				title={m.gettingStarted_step3_title}
				description={m.gettingStarted_step3_description}
			>
				<div class="space-y-3">
					<CodeBlock code={m.gettingStarted_step3_code1()} lang="bash" />
					<CodeBlock
						variants={{
							unix: m.gettingStarted_step3_code2(),
							powershell: m.gettingStarted_step3_code2Ps()
						}}
					/>
				</div>
				<p class="mt-3 text-xs text-muted-foreground">
					{m.gettingStarted_step3_note()}
				</p>
			</StepCard>

			<!-- Step 4: Start Docker -->
			<StepCard
				step={4}
				icon={Container}
				color="amber"
				title={m.gettingStarted_step4_title}
				description={m.gettingStarted_step4_description}
			>
				<CodeBlock
					variants={{
						unix: m.gettingStarted_step4_code(),
						powershell: m.gettingStarted_step4_codePs()
					}}
				/>
				<p class="mt-3 text-xs text-muted-foreground">
					{m.gettingStarted_step4_note()}
				</p>
			</StepCard>

			<!-- Step 5: Start coding -->
			<StepCard
				step={5}
				icon={Rocket}
				color="pink"
				title={m.gettingStarted_step5_title}
				description={m.gettingStarted_step5_description}
				last
			>
				<div class="space-y-4">
					<!-- Docker ports -->
					<div>
						<span
							class="mb-2 inline-block rounded-full border border-pink-500/20 bg-pink-500/10 px-2.5 py-0.5 text-[10px] font-semibold tracking-wider text-pink-500 uppercase"
						>
							{m.gettingStarted_step5_docker_label()}
						</span>
						<div class="grid gap-2 sm:grid-cols-2">
							<div class="rounded-lg border bg-muted/50 p-3 text-center">
								<span class="text-xs font-medium text-muted-foreground">
									{m.gettingStarted_step5_docker_frontend()}
								</span>
								<p class="mt-1 font-mono text-sm font-semibold">
									{m.gettingStarted_step5_docker_frontendUrl()}
								</p>
							</div>
							<div class="rounded-lg border bg-muted/50 p-3 text-center">
								<span class="text-xs font-medium text-muted-foreground">
									{m.gettingStarted_step5_docker_backend()}
								</span>
								<p class="mt-1 font-mono text-sm font-semibold">
									{m.gettingStarted_step5_docker_backendUrl()}
								</p>
							</div>
						</div>
					</div>

					<!-- Local dev -->
					<div class="rounded-lg border border-dashed border-muted-foreground/20 bg-muted/20 p-3">
						<span
							class="mb-2 inline-block rounded-full border border-violet-500/20 bg-violet-500/10 px-2.5 py-0.5 text-[10px] font-semibold tracking-wider text-violet-500 uppercase"
						>
							{m.gettingStarted_step5_local_label()}
						</span>
						<p class="mb-2 text-xs text-muted-foreground">
							{m.gettingStarted_step5_local_description()}
						</p>
						<div class="grid gap-2 sm:grid-cols-2">
							<div class="rounded-lg border bg-card p-2.5 text-center">
								<span class="text-xs font-medium text-muted-foreground">
									{m.gettingStarted_step5_local_frontend()}
								</span>
								<p class="mt-0.5 font-mono text-sm font-semibold">
									{m.gettingStarted_step5_local_frontendUrl()}
								</p>
							</div>
							<div class="rounded-lg border bg-card p-2.5 text-center">
								<span class="text-xs font-medium text-muted-foreground">
									{m.gettingStarted_step5_local_backend()}
								</span>
								<p class="mt-0.5 font-mono text-sm font-semibold">
									{m.gettingStarted_step5_local_backendUrl()}
								</p>
							</div>
						</div>
					</div>
				</div>
				<p class="mt-3 text-xs text-muted-foreground">
					{m.gettingStarted_step5_note()}
				</p>
			</StepCard>
		</div>
	</section>

	<div class="h-px w-full bg-border"></div>

	<!-- ── Checklist ────────────────────────────────────────────────── -->
	<section use:reveal>
		<div class="mb-6 text-center">
			<h2 class="text-2xl font-bold tracking-tight sm:text-3xl">
				{m.gettingStarted_checklist_title()}
			</h2>
			<p class="mt-2 text-sm text-muted-foreground sm:text-base">
				{m.gettingStarted_checklist_description()}
			</p>
		</div>

		<div class="mx-auto max-w-lg rounded-xl border bg-card p-4 shadow-sm sm:p-5">
			<ChecklistItem label={m.gettingStarted_checklist_item1} />
			<ChecklistItem label={m.gettingStarted_checklist_item2} />
			<ChecklistItem label={m.gettingStarted_checklist_item3} />
			<ChecklistItem label={m.gettingStarted_checklist_item4} />
			<ChecklistItem label={m.gettingStarted_checklist_item5} />
		</div>
	</section>

	<div class="h-px w-full bg-border"></div>

	<!-- ── Resources ────────────────────────────────────────────────── -->
	<section use:reveal>
		<div class="mb-6 text-center">
			<h2 class="text-2xl font-bold tracking-tight sm:text-3xl">
				{m.gettingStarted_resources_title()}
			</h2>
			<p class="mt-2 text-sm text-muted-foreground sm:text-base">
				{m.gettingStarted_resources_description()}
			</p>
		</div>

		<div class="grid gap-4 sm:grid-cols-2">
			<a
				href="https://github.com/fpindej/netrock"
				target="_blank"
				rel="noopener noreferrer"
				class="group rounded-xl border bg-card p-4 shadow-sm transition-all hover:border-primary/30 hover:shadow-md"
			>
				<div class="mb-2 flex items-center gap-3">
					<div class="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-foreground/5">
						<Github class="h-5 w-5" />
					</div>
					<h3 class="text-sm font-semibold">{m.gettingStarted_resources_github_title()}</h3>
					<ExternalLink
						class="ms-auto h-3.5 w-3.5 text-muted-foreground opacity-0 transition-opacity group-hover:opacity-100"
					/>
				</div>
				<p class="text-xs text-muted-foreground">
					{m.gettingStarted_resources_github_description()}
				</p>
			</a>

			<a
				href="https://github.com/fpindej/netrock#readme"
				target="_blank"
				rel="noopener noreferrer"
				class="group rounded-xl border bg-card p-4 shadow-sm transition-all hover:border-primary/30 hover:shadow-md"
			>
				<div class="mb-2 flex items-center gap-3">
					<div class="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-foreground/5">
						<BookOpen class="h-5 w-5" />
					</div>
					<h3 class="text-sm font-semibold">
						{m.gettingStarted_resources_readme_title()}
					</h3>
					<ExternalLink
						class="ms-auto h-3.5 w-3.5 text-muted-foreground opacity-0 transition-opacity group-hover:opacity-100"
					/>
				</div>
				<p class="text-xs text-muted-foreground">
					{m.gettingStarted_resources_readme_description()}
				</p>
			</a>

			<a
				href="https://deepwiki.org/fpindej/netrock"
				target="_blank"
				rel="noopener noreferrer"
				class="group rounded-xl border bg-card p-4 shadow-sm transition-all hover:border-primary/30 hover:shadow-md"
			>
				<div class="mb-2 flex items-center gap-3">
					<div class="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-foreground/5">
						<BookOpen class="h-5 w-5" />
					</div>
					<h3 class="text-sm font-semibold">
						{m.gettingStarted_resources_deepwiki_title()}
					</h3>
					<ExternalLink
						class="ms-auto h-3.5 w-3.5 text-muted-foreground opacity-0 transition-opacity group-hover:opacity-100"
					/>
				</div>
				<p class="text-xs text-muted-foreground">
					{m.gettingStarted_resources_deepwiki_description()}
				</p>
			</a>

			<a
				href={resolve('/guide')}
				class="group rounded-xl border bg-card p-4 shadow-sm transition-all hover:border-primary/30 hover:shadow-md"
			>
				<div class="mb-2 flex items-center gap-3">
					<div class="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-foreground/5">
						<Rocket class="h-5 w-5" />
					</div>
					<h3 class="text-sm font-semibold">
						{m.gettingStarted_resources_demo_title()}
					</h3>
				</div>
				<p class="text-xs text-muted-foreground">
					{m.gettingStarted_resources_demo_description()}
				</p>
			</a>
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
				{m.gettingStarted_cta_title()}
			</h2>
			<p class="mt-2 text-sm text-muted-foreground">
				{m.gettingStarted_cta_subtitle()}
			</p>
			<div class="mt-5 flex flex-col items-center gap-3">
				<Button href="https://github.com/fpindej/netrock" target="_blank" rel="noopener noreferrer">
					<Github class="me-2 h-4 w-4" />
					{m.gettingStarted_cta_github()}
					<ExternalLink class="ms-2 h-3.5 w-3.5" />
				</Button>
				<Button variant="outline" size="sm" href={resolve('/guide')}>
					{m.gettingStarted_cta_explore()}
				</Button>
			</div>
		</div>
	</section>
</div>
