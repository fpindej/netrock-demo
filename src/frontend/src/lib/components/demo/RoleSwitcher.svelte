<script lang="ts">
	import { demoState, type DemoRole } from '$lib/state';
	import * as m from '$lib/paraglide/messages';
	import type { User } from '$lib/types';
	import { Eye, X, Shield, ShieldCheck, User as UserIcon } from '@lucide/svelte';

	interface Props {
		user: User | null | undefined;
	}

	let { user }: Props = $props();

	let expanded = $state(false);

	const roles: { key: DemoRole; label: () => string; icon: typeof Shield }[] = [
		{ key: 'User', label: m.demo_role_user, icon: UserIcon },
		{ key: 'Admin', label: m.demo_role_admin, icon: Shield },
		{ key: 'SuperAdmin', label: m.demo_role_superAdmin, icon: ShieldCheck }
	];

	let availableRoles = $derived(roles);

	function setRole(role: DemoRole) {
		demoState.viewingAs = role;
	}

	function toggle() {
		expanded = !expanded;
	}

	function collapse() {
		expanded = false;
	}

	function handleKeydown(event: KeyboardEvent) {
		if (event.key === 'Escape' && expanded) {
			collapse();
		}
	}

	let currentRoleLabel = $derived(
		roles.find((r) => r.key === demoState.viewingAs)?.label() ?? demoState.viewingAs
	);
</script>

<svelte:window onkeydown={handleKeydown} />

{#if user}
	<!-- Backdrop overlay when expanded -->
	{#if expanded}
		<!-- svelte-ignore a11y_no_static_element_interactions -->
		<div class="fixed inset-0 z-40" onclick={collapse} onkeydown={() => {}}></div>
	{/if}

	<div
		class="fixed z-50"
		style="bottom: max(1.25rem, env(safe-area-inset-bottom)); inset-inline-end: 1.25rem;"
	>
		<!-- Expanded panel -->
		{#if expanded}
			<div
				class="role-switcher-panel flex origin-bottom-right flex-col gap-3 rounded-2xl border border-white/15 bg-background/70 p-4 shadow-2xl backdrop-blur-xl"
				style="
					box-shadow:
						0 0 0 1px rgba(255, 255, 255, 0.05),
						0 8px 40px rgba(0, 0, 0, 0.2),
						0 2px 12px rgba(0, 0, 0, 0.1);
				"
			>
				<!-- Header row -->
				<div class="flex items-center justify-between gap-4">
					<div class="flex items-center gap-2">
						<Eye class="size-3.5 text-muted-foreground" />
						<span class="text-xs font-semibold tracking-wide text-muted-foreground uppercase">
							{m.demo_viewedAs()}
						</span>
					</div>
					<button
						onclick={collapse}
						class="inline-flex size-6 items-center justify-center rounded-full text-muted-foreground transition-colors hover:bg-muted hover:text-foreground"
						aria-label="Close role switcher"
					>
						<X class="size-3.5" />
					</button>
				</div>

				<!-- Role buttons -->
				<div class="flex flex-col gap-1.5">
					{#each availableRoles as role (role.key)}
						{@const isActive = demoState.viewingAs === role.key}
						<button
							onclick={() => {
								setRole(role.key);
								collapse();
							}}
							class="role-option group flex items-center gap-2.5 rounded-xl px-3 py-2.5 text-start text-sm font-medium transition-all duration-200
								{isActive
								? 'bg-primary text-primary-foreground shadow-md'
								: 'text-foreground/80 hover:bg-muted/80 hover:text-foreground'}"
						>
							<role.icon
								class="size-4 shrink-0 {isActive
									? 'opacity-100'
									: 'opacity-50 group-hover:opacity-75'}"
							/>
							<span>{role.label()}</span>
							{#if isActive}
								<span class="ms-auto inline-block size-1.5 rounded-full bg-primary-foreground/60"
								></span>
							{/if}
						</button>
					{/each}
				</div>
			</div>
		{:else}
			<!-- Collapsed pill trigger -->
			<button
				onclick={toggle}
				class="role-switcher-pill group relative flex items-center gap-2 overflow-hidden rounded-full border border-white/15 bg-background/70 py-2.5 ps-3 pe-4 shadow-xl backdrop-blur-xl transition-all duration-300 hover:scale-[1.03] hover:shadow-2xl active:scale-[0.98]"
				style="
					box-shadow:
						0 0 0 1px rgba(255, 255, 255, 0.05),
						0 4px 24px rgba(0, 0, 0, 0.15),
						0 1px 8px rgba(0, 0, 0, 0.08);
				"
				aria-label="Open role switcher"
			>
				<!-- Shimmer overlay -->
				<span class="role-switcher-shimmer" aria-hidden="true"></span>

				<!-- Content -->
				<Eye
					class="relative size-3.5 text-muted-foreground transition-colors group-hover:text-foreground"
				/>
				<span
					class="relative text-sm font-semibold text-foreground/90 transition-colors group-hover:text-foreground"
				>
					{currentRoleLabel}
				</span>
			</button>
		{/if}
	</div>
{/if}

<style>
	/* -------------------------------------------------------------------------
	   Shimmer / Shine animation
	   A diagonal light sweep across the collapsed pill surface.
	   ------------------------------------------------------------------------- */
	.role-switcher-shimmer {
		position: absolute;
		inset: 0;
		pointer-events: none;
		background: linear-gradient(
			120deg,
			transparent 0%,
			transparent 35%,
			rgba(255, 255, 255, 0.12) 42%,
			rgba(255, 255, 255, 0.2) 50%,
			rgba(255, 255, 255, 0.12) 58%,
			transparent 65%,
			transparent 100%
		);
		background-size: 250% 100%;
		animation: shimmer-sweep 4s ease-in-out infinite;
		border-radius: inherit;
	}

	:global(.dark) .role-switcher-shimmer {
		background: linear-gradient(
			120deg,
			transparent 0%,
			transparent 35%,
			rgba(255, 255, 255, 0.06) 42%,
			rgba(255, 255, 255, 0.1) 50%,
			rgba(255, 255, 255, 0.06) 58%,
			transparent 65%,
			transparent 100%
		);
		background-size: 250% 100%;
	}

	@keyframes shimmer-sweep {
		0% {
			background-position: 200% center;
		}
		40% {
			background-position: -50% center;
		}
		100% {
			background-position: -50% center;
		}
	}

	/* -------------------------------------------------------------------------
	   Panel entrance animation
	   ------------------------------------------------------------------------- */
	.role-switcher-panel {
		animation: panel-enter 0.25s cubic-bezier(0.16, 1, 0.3, 1) forwards;
	}

	@keyframes panel-enter {
		from {
			opacity: 0;
			transform: scale(0.92) translateY(8px);
		}
		to {
			opacity: 1;
			transform: scale(1) translateY(0);
		}
	}

	/* -------------------------------------------------------------------------
	   Role option hover micro-interaction
	   ------------------------------------------------------------------------- */
	.role-option {
		transition:
			background-color 0.2s ease,
			color 0.2s ease,
			transform 0.15s ease;
	}

	.role-option:active {
		transform: scale(0.98);
	}

	/* -------------------------------------------------------------------------
	   Respect reduced motion preferences
	   ------------------------------------------------------------------------- */
	@media (prefers-reduced-motion: reduce) {
		.role-switcher-shimmer {
			animation: none;
		}

		.role-switcher-panel {
			animation: none;
		}

		.role-switcher-pill {
			transition: none;
		}

		.role-option {
			transition: none;
		}
	}
</style>
