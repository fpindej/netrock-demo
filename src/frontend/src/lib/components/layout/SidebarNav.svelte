<script lang="ts">
	import { page } from '$app/state';
	import { resolve } from '$app/paths';
	import { cn, hasPermission, Permissions } from '$lib/utils';
	import { buttonVariants } from '$lib/components/ui/button';
	import {
		Sparkles,
		BookOpen,
		Layers,
		Users2,
		BarChart3,
		Mail,
		Users,
		Shield,
		Clock,
		type IconProps
	} from '@lucide/svelte';
	import * as m from '$lib/paraglide/messages';
	import * as Tooltip from '$lib/components/ui/tooltip';
	import type { Component } from 'svelte';
	import type { User } from '$lib/types';

	interface Props {
		collapsed?: boolean;
		onNavigate?: () => void;
		user?: User | null;
	}

	let { collapsed = false, onNavigate, user }: Props = $props();

	type NavItem = { title: () => string; href: string; icon: Component<IconProps>; tour?: string };
	type AdminNavItem = NavItem & { permission: string };

	let showcaseItems: NavItem[] = [
		{
			title: m.nav_guide,
			href: resolve('/guide'),
			icon: BookOpen,
			tour: 'nav-guide'
		},
		{
			title: m.nav_forYou,
			href: resolve('/for-you'),
			icon: Sparkles,
			tour: 'nav-for-you'
		},
		{
			title: m.nav_howItWorks,
			href: resolve('/how-it-works'),
			icon: Layers,
			tour: 'nav-how-it-works'
		}
	];

	let dataItems: NavItem[] = [
		{
			title: m.nav_contacts,
			href: resolve('/contacts'),
			icon: Users2,
			tour: 'nav-contacts'
		},
		{
			title: m.nav_analytics,
			href: resolve('/analytics'),
			icon: BarChart3,
			tour: 'nav-analytics'
		}
	];

	let contactItem: NavItem = {
		title: m.nav_contactMe,
		href: resolve('/contact-us'),
		icon: Mail,
		tour: 'nav-contact-us'
	};

	let adminItems: AdminNavItem[] = [
		{
			title: m.nav_adminUsers,
			href: resolve('/admin/users'),
			icon: Users,
			permission: Permissions.Users.View
		},
		{
			title: m.nav_adminRoles,
			href: resolve('/admin/roles'),
			icon: Shield,
			permission: Permissions.Roles.View
		},
		{
			title: m.nav_adminJobs,
			href: resolve('/admin/jobs'),
			icon: Clock,
			permission: Permissions.Jobs.View
		}
	];

	let visibleAdminItems = $derived(
		adminItems.filter((item) => hasPermission(user, item.permission))
	);

	function isActive(href: string, pathname: string) {
		if (href === resolve('/')) {
			return pathname === href;
		}
		return pathname.startsWith(href);
	}
</script>

<!-- eslint-disable svelte/no-navigation-without-resolve -- hrefs are pre-resolved using resolve() in items array -->
{#snippet navItem(item: NavItem)}
	{@const active = isActive(item.href, page.url.pathname)}
	{#if collapsed}
		<Tooltip.Root>
			<Tooltip.Trigger>
				{#snippet child({ props })}
					<a
						href={item.href}
						data-tour={item.tour}
						class={cn(
							buttonVariants({
								variant: active ? 'default' : 'ghost',
								size: 'icon'
							}),
							'h-9 w-9',
							active &&
								'dark:bg-muted dark:text-foreground dark:hover:bg-muted dark:hover:text-foreground'
						)}
						aria-current={active ? 'page' : undefined}
						aria-label={item.title()}
						onclick={onNavigate}
						{...props}
					>
						<item.icon class="h-4 w-4" />
					</a>
				{/snippet}
			</Tooltip.Trigger>
			<Tooltip.Content side="right">
				{item.title()}
			</Tooltip.Content>
		</Tooltip.Root>
	{:else}
		<a
			href={item.href}
			data-tour={item.tour}
			class={cn(
				buttonVariants({
					variant: active ? 'default' : 'ghost',
					size: 'sm'
				}),
				active &&
					'dark:bg-muted dark:text-foreground dark:hover:bg-muted dark:hover:text-foreground',
				'justify-start'
			)}
			aria-current={active ? 'page' : undefined}
			onclick={onNavigate}
		>
			<item.icon class="me-2 h-4 w-4" />
			{item.title()}
		</a>
	{/if}
{/snippet}

<nav class={cn('grid gap-1', collapsed ? 'justify-center px-2' : 'px-2')}>
	{#each showcaseItems as item (item.href)}
		{@render navItem(item)}
	{/each}

	<div class="my-2 h-px w-full bg-border"></div>
	{#if !collapsed}
		<span class="mb-1 px-3 text-xs font-semibold tracking-wider text-muted-foreground uppercase">
			{m.nav_crmDemo()}
		</span>
	{/if}

	{#each dataItems as item (item.href)}
		{@render navItem(item)}
	{/each}

	{#if visibleAdminItems.length > 0}
		<div data-tour="nav-admin">
			<div class="my-2 h-px w-full bg-border"></div>
			{#if !collapsed}
				<span
					class="mb-1 px-3 text-xs font-semibold tracking-wider text-muted-foreground uppercase"
				>
					{m.nav_admin()}
				</span>
			{/if}
			<div class="grid gap-1">
				{#each visibleAdminItems as item (item.href)}
					{@render navItem(item)}
				{/each}
			</div>
		</div>
	{/if}

	<div class="my-2 h-px w-full bg-border"></div>
	{@render navItem(contactItem)}
</nav>
