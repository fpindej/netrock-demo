<script lang="ts">
	import { cn } from '$lib/utils';
	import { SidebarNav, ThemeToggle, LanguageSelector, UserNav } from '$lib/components/layout';
	import { Package2, PanelLeftClose, PanelLeft, Github, BookOpen } from '@lucide/svelte';
	import { resolve } from '$app/paths';
	import type { User } from '$lib/types';
	import * as m from '$lib/paraglide/messages';
	import { sidebarState, toggleSidebar } from '$lib/state';
	import * as Tooltip from '$lib/components/ui/tooltip';
	import { Button } from '$lib/components/ui/button';

	interface Props {
		class?: string;
		user: User | null | undefined;
	}

	let { class: className, user }: Props = $props();

	let collapsed = $derived(sidebarState.collapsed);
</script>

<div
	class={cn('flex h-full flex-col gap-2 transition-all duration-300', className)}
	style="padding-inline-start: env(safe-area-inset-left, 0px);"
	data-collapsed={collapsed}
>
	<div class="flex-1 overflow-auto overscroll-contain py-4">
		<div class="px-3 py-2">
			<div
				data-tour="sidebar-brand"
				class={cn('mb-4 flex items-center', collapsed ? 'justify-center px-0' : 'px-2')}
			>
				<a
					href={resolve('/')}
					class={cn('flex items-center gap-2 font-semibold', collapsed ? 'text-base' : 'text-lg')}
				>
					<Package2 class="h-6 w-6 shrink-0" />
					{#if !collapsed}
						<span>{m.app_name()}</span>
					{/if}
				</a>
			</div>
			<div class="space-y-1">
				<SidebarNav {collapsed} {user} />
			</div>
		</div>
	</div>
	<div data-tour="sidebar-tools" class="border-t p-3">
		<div class={cn('flex items-center gap-1', collapsed ? 'flex-col' : 'flex-row justify-between')}>
			{#if collapsed}
				<!-- Collapsed: stack vertically -->
				<LanguageSelector />
				<ThemeToggle collapsed />
				<Tooltip.Root>
					<Tooltip.Trigger>
						{#snippet child({ props })}
							<Button
								variant="ghost"
								size="icon"
								class="h-9 w-9"
								href="https://github.com/fpindej/netrock"
								target="_blank"
								rel="noopener noreferrer"
								aria-label={m.nav_github()}
								{...props}
							>
								<Github class="h-4 w-4" />
							</Button>
						{/snippet}
					</Tooltip.Trigger>
					<Tooltip.Content side="right">
						{m.nav_github()}
					</Tooltip.Content>
				</Tooltip.Root>
				<Tooltip.Root>
					<Tooltip.Trigger>
						{#snippet child({ props })}
							<Button
								variant="ghost"
								size="icon"
								class="h-9 w-9"
								href="https://deepwiki.org/fpindej/netrock"
								target="_blank"
								rel="noopener noreferrer"
								aria-label={m.nav_deepwiki()}
								{...props}
							>
								<BookOpen class="h-4 w-4" />
							</Button>
						{/snippet}
					</Tooltip.Trigger>
					<Tooltip.Content side="right">
						{m.nav_deepwiki()}
					</Tooltip.Content>
				</Tooltip.Root>
				{#if user}
					<UserNav {user} />
				{/if}
				<Tooltip.Root>
					<Tooltip.Trigger>
						{#snippet child({ props })}
							<Button
								variant="ghost"
								size="icon"
								class="mt-2 h-9 w-9"
								aria-label={m.nav_expand()}
								{...props}
								onclick={toggleSidebar}
							>
								<PanelLeft class="h-4 w-4" />
							</Button>
						{/snippet}
					</Tooltip.Trigger>
					<Tooltip.Content side="right">
						{m.nav_expand()}
					</Tooltip.Content>
				</Tooltip.Root>
			{:else}
				<!-- Expanded: row layout -->
				<div class="flex items-center gap-1">
					<LanguageSelector />
					<ThemeToggle />
					<Tooltip.Root>
						<Tooltip.Trigger>
							{#snippet child({ props })}
								<Button
									variant="ghost"
									size="icon"
									class="h-9 w-9"
									href="https://github.com/fpindej/netrock"
									target="_blank"
									rel="noopener noreferrer"
									aria-label={m.nav_github()}
									{...props}
								>
									<Github class="h-4 w-4" />
								</Button>
							{/snippet}
						</Tooltip.Trigger>
						<Tooltip.Content side="top">
							{m.nav_github()}
						</Tooltip.Content>
					</Tooltip.Root>
					<Tooltip.Root>
						<Tooltip.Trigger>
							{#snippet child({ props })}
								<Button
									variant="ghost"
									size="icon"
									class="h-9 w-9"
									href="https://deepwiki.org/fpindej/netrock"
									target="_blank"
									rel="noopener noreferrer"
									aria-label={m.nav_deepwiki()}
									{...props}
								>
									<BookOpen class="h-4 w-4" />
								</Button>
							{/snippet}
						</Tooltip.Trigger>
						<Tooltip.Content side="top">
							{m.nav_deepwiki()}
						</Tooltip.Content>
					</Tooltip.Root>
				</div>
				<div class="flex items-center gap-1">
					{#if user}
						<UserNav {user} />
					{/if}
					<Tooltip.Root>
						<Tooltip.Trigger>
							{#snippet child({ props })}
								<Button
									variant="ghost"
									size="icon"
									class="h-9 w-9"
									aria-label={m.nav_collapse()}
									{...props}
									onclick={toggleSidebar}
								>
									<PanelLeftClose class="h-4 w-4" />
								</Button>
							{/snippet}
						</Tooltip.Trigger>
						<Tooltip.Content side="right">
							{m.nav_collapse()}
						</Tooltip.Content>
					</Tooltip.Root>
				</div>
			{/if}
		</div>
	</div>
</div>
