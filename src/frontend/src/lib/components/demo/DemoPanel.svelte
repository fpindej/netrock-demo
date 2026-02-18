<script lang="ts">
	import { invalidateAll } from '$app/navigation';
	import { Button } from '$lib/components/ui/button';
	import { Loader2 } from '@lucide/svelte';
	import { browserClient } from '$lib/api';
	import { toast } from '$lib/components/ui/sonner';
	import * as m from '$lib/paraglide/messages';
	import type { User } from '$lib/types';

	interface Props {
		user: User | null;
	}

	let { user }: Props = $props();

	const roles = ['User', 'Admin', 'SuperAdmin'] as const;

	let isSwitching = $state(false);
	let switchingTo = $state<string | null>(null);

	let currentRole = $derived(() => {
		if (!user?.roles) return 'User';
		const r = [...user.roles];
		if (r.includes('SuperAdmin')) return 'SuperAdmin';
		if (r.includes('Admin')) return 'Admin';
		return 'User';
	});

	async function switchRole(role: string) {
		if (isSwitching || currentRole() === role) return;
		isSwitching = true;
		switchingTo = role;

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
			switchingTo = null;
		}
	}
</script>

<div
	class="fixed inset-x-0 bottom-4 z-50 mx-auto flex w-fit items-center gap-3 rounded-full border bg-background/80 px-4 py-2 shadow-lg backdrop-blur-sm"
>
	<span class="text-xs font-medium text-muted-foreground">{m.demo_viewingAs()}:</span>
	{#each roles as role (role)}
		{@const isActive = currentRole() === role}
		<Button
			variant={isActive ? 'default' : 'ghost'}
			size="sm"
			class="h-7 rounded-full px-3 text-xs"
			disabled={isSwitching}
			onclick={() => switchRole(role)}
		>
			{#if isSwitching && switchingTo === role}
				<Loader2 class="me-1 h-3 w-3 animate-spin" />
			{/if}
			{role}
		</Button>
	{/each}
</div>
