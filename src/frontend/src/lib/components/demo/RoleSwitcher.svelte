<script lang="ts">
	import { Button } from '$lib/components/ui/button';
	import { demoState, type DemoRole } from '$lib/state';
	import * as m from '$lib/paraglide/messages';
	import type { User } from '$lib/types';

	interface Props {
		user: User | null | undefined;
	}

	let { user }: Props = $props();

	const roles: { key: DemoRole; label: () => string }[] = [
		{ key: 'User', label: m.demo_role_user },
		{ key: 'Admin', label: m.demo_role_admin },
		{ key: 'SuperAdmin', label: m.demo_role_superAdmin }
	];

	let availableRoles = $derived(roles);

	function setRole(role: DemoRole) {
		demoState.viewingAs = role;
	}
</script>

{#if user}
	<div
		class="fixed start-1/2 z-50 flex -translate-x-1/2 items-center gap-2 rounded-full border bg-background/80 px-4 py-2 shadow-lg backdrop-blur-sm"
		style="bottom: max(1rem, env(safe-area-inset-bottom))"
	>
		<span class="text-sm font-medium text-muted-foreground">{m.demo_viewedAs()}</span>
		{#each availableRoles as role (role.key)}
			<Button
				variant={demoState.viewingAs === role.key ? 'default' : 'ghost'}
				size="sm"
				onclick={() => setRole(role.key)}
			>
				{role.label()}
			</Button>
		{/each}
	</div>
{/if}
