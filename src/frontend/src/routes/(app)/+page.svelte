<script lang="ts">
	import * as Card from '$lib/components/ui/card';
	import { Badge } from '$lib/components/ui/badge';
	import { Button } from '$lib/components/ui/button';
	import {
		ShieldCheck,
		UserRound,
		Settings,
		Clock,
		Languages,
		Palette,
		Crown,
		Users,
		Lock,
		BriefcaseBusiness
	} from '@lucide/svelte';
	import { resolve } from '$app/paths';
	import { hasAnyPermission, Permissions } from '$lib/utils';
	import type { PageData } from './$types';
	import * as m from '$lib/paraglide/messages';

	let { data }: { data: PageData } = $props();

	let firstName = $derived(data.user.firstName);
	let isAdmin = $derived(
		hasAnyPermission(data.user, [
			Permissions.Users.View,
			Permissions.Roles.View,
			Permissions.Jobs.View
		])
	);

	const features = [
		{
			key: 'auth',
			icon: ShieldCheck,
			title: () => m.dashboard_feature_auth_title(),
			description: () => m.dashboard_feature_auth_description()
		},
		{
			key: 'profile',
			icon: UserRound,
			title: () => m.dashboard_feature_profile_title(),
			description: () => m.dashboard_feature_profile_description()
		},
		{
			key: 'admin',
			icon: Settings,
			title: () => m.dashboard_feature_admin_title(),
			description: () => m.dashboard_feature_admin_description()
		},
		{
			key: 'jobs',
			icon: Clock,
			title: () => m.dashboard_feature_jobs_title(),
			description: () => m.dashboard_feature_jobs_description()
		},
		{
			key: 'i18n',
			icon: Languages,
			title: () => m.dashboard_feature_i18n_title(),
			description: () => m.dashboard_feature_i18n_description()
		},
		{
			key: 'theme',
			icon: Palette,
			title: () => m.dashboard_feature_theme_title(),
			description: () => m.dashboard_feature_theme_description()
		}
	];
</script>

<svelte:head>
	<title>{m.meta_titleTemplate({ title: m.meta_dashboard_title() })}</title>
	<meta name="description" content={m.meta_dashboard_description()} />
</svelte:head>

<div class="space-y-8">
	<!-- Welcome -->
	<div>
		<h3 class="text-lg font-medium">
			{firstName ? m.dashboard_welcome({ name: firstName }) : m.dashboard_welcomeFallback()}
		</h3>
		<p class="text-sm text-muted-foreground">{m.dashboard_subtitle()}</p>
	</div>
	<div class="h-px w-full bg-border"></div>

	<!-- Feature cards -->
	<div class="grid gap-4 sm:grid-cols-2 xl:grid-cols-3">
		{#each features as feature (feature.key)}
			<Card.Root class="card-hover relative overflow-hidden">
				<div class="glow-xl-top-end"></div>
				<Card.Header class="flex flex-row items-center gap-3 space-y-0 pb-2">
					<div class="flex h-10 w-10 shrink-0 items-center justify-center rounded-lg bg-primary/10">
						<feature.icon class="h-5 w-5 text-primary" />
					</div>
					<Card.Title class="text-base">{feature.title()}</Card.Title>
				</Card.Header>
				<Card.Content>
					<p class="text-sm text-muted-foreground">{feature.description()}</p>
				</Card.Content>
			</Card.Root>
		{/each}
	</div>

	<!-- Admin easter egg -->
	{#if isAdmin}
		<div class="space-y-4">
			<div class="flex items-center gap-2">
				<Crown class="h-5 w-5 text-warning" />
				<h3 class="text-lg font-medium">{m.dashboard_admin_title()}</h3>
				<Badge variant="outline" class="border-warning/50 text-warning">
					{data.user.roles?.[0] ?? 'Admin'}
				</Badge>
			</div>
			<p class="text-sm text-muted-foreground">{m.dashboard_admin_description()}</p>

			<div class="flex flex-wrap gap-3">
				{#if hasAnyPermission(data.user, [Permissions.Users.View])}
					<Button variant="outline" href={resolve('/admin/users')}>
						<Users class="me-2 h-4 w-4" />
						{m.dashboard_admin_users()}
					</Button>
				{/if}
				{#if hasAnyPermission(data.user, [Permissions.Roles.View])}
					<Button variant="outline" href={resolve('/admin/roles')}>
						<Lock class="me-2 h-4 w-4" />
						{m.dashboard_admin_roles()}
					</Button>
				{/if}
				{#if hasAnyPermission(data.user, [Permissions.Jobs.View])}
					<Button variant="outline" href={resolve('/admin/jobs')}>
						<BriefcaseBusiness class="me-2 h-4 w-4" />
						{m.dashboard_admin_jobs()}
					</Button>
				{/if}
			</div>
		</div>
	{/if}
</div>
