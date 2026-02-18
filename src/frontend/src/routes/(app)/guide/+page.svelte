<script lang="ts">
	import { resolve } from '$app/paths';
	import { invalidateAll } from '$app/navigation';
	import { page } from '$app/state';
	import * as Card from '$lib/components/ui/card';
	import { Button } from '$lib/components/ui/button';
	import { Badge } from '$lib/components/ui/badge';
	import {
		Rocket,
		ShieldCheck,
		User,
		Users as UsersIcon,
		BarChart3,
		Users,
		Shield,
		Clock,
		Languages,
		Github,
		Linkedin,
		Mail,
		Globe,
		AlertTriangle,
		Loader2
	} from '@lucide/svelte';
	import { browserClient, handleMutationError } from '$lib/api';
	import { toast } from '$lib/components/ui/sonner';
	import { createCooldown } from '$lib/state';
	import { LanguageSelector, ThemeToggle } from '$lib/components/layout';
	import * as m from '$lib/paraglide/messages';
	import type { LayoutData } from '../../$types';

	let layoutData: LayoutData = $derived(page.data as LayoutData);
	let user = $derived(layoutData.user);
	let emailConfirmed = $derived(user?.emailConfirmed ?? false);

	let isResending = $state(false);
	let isSwitching = $state(false);
	const cooldown = createCooldown();

	async function resendVerification() {
		if (isResending || cooldown.active) return;
		isResending = true;

		try {
			const { response } = await browserClient.POST('/api/auth/resend-verification');
			if (response.ok) {
				toast.success(m.auth_emailBanner_resendSuccess());
				cooldown.start(60);
			} else {
				handleMutationError(response, undefined, {
					cooldown,
					fallback: m.auth_emailBanner_resendError()
				});
			}
		} catch {
			toast.error(m.auth_emailBanner_resendError());
		} finally {
			isResending = false;
		}
	}

	async function switchRole(role: string) {
		if (isSwitching) return;
		isSwitching = true;

		try {
			const { response } = await browserClient.POST('/api/v1/demo/switch-role', {
				body: { role }
			});

			if (response.ok) {
				await invalidateAll();
			} else {
				toast.error(m.demo_switchError());
			}
		} catch {
			toast.error(m.demo_switchError());
		} finally {
			isSwitching = false;
		}
	}

	type Step = {
		number: number;
		title: () => string;
		description: () => string;
		icon: typeof Rocket;
		content?: 'verify' | 'link' | 'admin' | 'superadmin' | 'jobs' | 'i18n' | 'github';
		href?: string;
		linkText?: () => string;
	};

	const steps: Step[] = [
		{
			number: 1,
			title: m.guide_step1_title,
			description: m.guide_step1_description,
			icon: Rocket,
			content: 'verify'
		},
		{
			number: 2,
			title: m.guide_step2_title,
			description: m.guide_step2_description,
			icon: ShieldCheck
		},
		{
			number: 3,
			title: m.guide_step3_title,
			description: m.guide_step3_description,
			icon: User,
			content: 'link',
			href: resolve('/profile'),
			linkText: m.guide_step3_link
		},
		{
			number: 4,
			title: m.guide_step4_title,
			description: m.guide_step4_description,
			icon: UsersIcon,
			content: 'link',
			href: resolve('/contacts'),
			linkText: m.guide_step4_link
		},
		{
			number: 5,
			title: m.guide_step5_title,
			description: m.guide_step5_description,
			icon: BarChart3,
			content: 'link',
			href: resolve('/analytics'),
			linkText: m.guide_step5_link
		},
		{
			number: 6,
			title: m.guide_step6_title,
			description: m.guide_step6_description,
			icon: Users,
			content: 'admin'
		},
		{
			number: 7,
			title: m.guide_step7_title,
			description: m.guide_step7_description,
			icon: Shield,
			content: 'superadmin'
		},
		{
			number: 8,
			title: m.guide_step8_title,
			description: m.guide_step8_description,
			icon: Clock,
			content: 'jobs',
			href: resolve('/admin/jobs'),
			linkText: m.guide_step8_link
		},
		{
			number: 9,
			title: m.guide_step9_title,
			description: m.guide_step9_description,
			icon: Languages,
			content: 'i18n'
		},
		{
			number: 10,
			title: m.guide_step10_title,
			description: m.guide_step10_description,
			icon: Github,
			content: 'github'
		}
	];
</script>

<svelte:head>
	<title>{m.meta_titleTemplate({ title: m.meta_guide_title() })}</title>
	<meta name="description" content={m.meta_guide_description()} />
</svelte:head>

<div class="space-y-6">
	<div>
		<h3 class="text-lg font-medium">{m.guide_title()}</h3>
		<p class="text-sm text-muted-foreground">{m.guide_subtitle()}</p>
	</div>
	<div class="h-px w-full bg-border"></div>

	<!-- Timeline -->
	<div class="relative ms-4 border-s-2 border-border ps-8">
		{#each steps as step (step.number)}
			{@const locked = !emailConfirmed && step.number > 1}
			<div class="relative pb-10 last:pb-0 {locked ? 'opacity-50' : ''}">
				<!-- Timeline dot -->
				<div
					class="absolute -start-[calc(2rem+1.25rem/2+1px)] flex h-5 w-5 items-center justify-center rounded-full border-2 border-border bg-background text-xs font-bold {locked
						? 'text-muted-foreground'
						: 'border-primary text-primary'}"
				>
					{step.number}
				</div>

				<Card.Root>
					<Card.Header>
						<div class="flex items-center gap-3">
							<div class="rounded-lg p-2 {locked ? 'bg-muted' : 'bg-primary/10'}">
								<step.icon class="h-5 w-5 {locked ? 'text-muted-foreground' : 'text-primary'}" />
							</div>
							<div>
								<Card.Title class="text-base">{step.title()}</Card.Title>
							</div>
						</div>
					</Card.Header>
					<Card.Content>
						<p class="text-sm text-muted-foreground">{step.description()}</p>

						{#if step.content === 'verify' && !emailConfirmed}
							<div
								class="mt-4 flex items-start gap-3 rounded-lg border border-warning bg-warning/10 p-3"
							>
								<AlertTriangle class="mt-0.5 h-4 w-4 shrink-0 text-warning-foreground" />
								<div class="flex-1">
									<p class="text-sm font-medium text-warning-foreground">
										{m.guide_step1_verifyWarning()}
									</p>
									<Button
										variant="outline"
										size="sm"
										class="mt-2"
										disabled={isResending || cooldown.active}
										onclick={resendVerification}
									>
										{#if cooldown.active}
											{m.common_waitSeconds({ seconds: cooldown.remaining })}
										{:else if isResending}
											<Loader2 class="me-2 h-3 w-3 animate-spin" />
											{m.auth_emailBanner_resending()}
										{:else}
											{m.guide_step1_resendVerification()}
										{/if}
									</Button>
								</div>
							</div>
						{/if}

						{#if !locked}
							{#if step.content === 'link' && step.href}
								<!-- eslint-disable-next-line svelte/no-navigation-without-resolve -- href is pre-resolved -->
								<a href={step.href}>
									<Button variant="outline" size="sm" class="mt-4">
										{step.linkText?.() ?? ''}
									</Button>
								</a>
							{/if}

							{#if step.content === 'admin'}
								<Button
									variant="outline"
									size="sm"
									class="mt-4"
									disabled={isSwitching}
									onclick={() => switchRole('Admin')}
								>
									{#if isSwitching}
										<Loader2 class="me-2 h-3 w-3 animate-spin" />
									{/if}
									{m.guide_step6_viewAsAdmin()}
								</Button>
							{/if}

							{#if step.content === 'superadmin'}
								<Button
									variant="outline"
									size="sm"
									class="mt-4"
									disabled={isSwitching}
									onclick={() => switchRole('SuperAdmin')}
								>
									{#if isSwitching}
										<Loader2 class="me-2 h-3 w-3 animate-spin" />
									{/if}
									{m.guide_step7_viewAsSuperAdmin()}
								</Button>
							{/if}

							{#if step.content === 'jobs' && step.href}
								<!-- eslint-disable-next-line svelte/no-navigation-without-resolve -- href is pre-resolved -->
								<a href={step.href}>
									<Button variant="outline" size="sm" class="mt-4">
										{step.linkText?.() ?? ''}
									</Button>
								</a>
							{/if}

							{#if step.content === 'i18n'}
								<div class="mt-4 flex flex-wrap items-center gap-3">
									<LanguageSelector />
									<ThemeToggle />
								</div>
							{/if}

							{#if step.content === 'github'}
								<div class="mt-4 flex flex-wrap gap-2">
									<a
										href="https://github.com/netrock-consulting"
										target="_blank"
										rel="noopener noreferrer"
									>
										<Button variant="outline" size="sm">
											<Github class="me-2 h-4 w-4" />
											{m.guide_step10_github()}
										</Button>
									</a>
									<a
										href="https://www.linkedin.com/in/filip-dorian-pindej/"
										target="_blank"
										rel="noopener noreferrer"
									>
										<Button variant="outline" size="sm">
											<Linkedin class="me-2 h-4 w-4" />
											{m.guide_step10_linkedin()}
										</Button>
									</a>
									<a href="mailto:contact@mail.pindej.cz">
										<Button variant="outline" size="sm">
											<Mail class="me-2 h-4 w-4" />
											{m.guide_step10_email()}
										</Button>
									</a>
									<a href="https://pindej.cz" target="_blank" rel="noopener noreferrer">
										<Button variant="outline" size="sm">
											<Globe class="me-2 h-4 w-4" />
											{m.guide_step10_website()}
										</Button>
									</a>
								</div>
							{/if}
						{:else}
							<Badge variant="secondary" class="mt-4">{m.guide_locked()}</Badge>
						{/if}
					</Card.Content>
				</Card.Root>
			</div>
		{/each}
	</div>
</div>
