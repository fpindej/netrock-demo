<script lang="ts">
	import { invalidateAll } from '$app/navigation';
	import { Button } from '$lib/components/ui/button';
	import { Loader2 } from '@lucide/svelte';
	import { browserClient } from '$lib/api';
	import { toast } from '$lib/components/ui/sonner';
	import * as m from '$lib/paraglide/messages';
	import type { User } from '$lib/types';
	import { fly } from 'svelte/transition';

	interface Props {
		user: User | null;
	}

	let { user }: Props = $props();

	const roles = ['User', 'Admin', 'SuperAdmin'] as const;
	const roleLetters: Record<string, string> = {
		User: 'U',
		Admin: 'A',
		SuperAdmin: 'S'
	};

	let isSwitching = $state(false);
	let switchingTo = $state<string | null>(null);
	let expanded = $state(false);

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

	function handleWindowClick(e: MouseEvent) {
		const target = e.target as HTMLElement;
		if (expanded && !target.closest('[data-demo-panel]')) {
			expanded = false;
		}
	}
</script>

<svelte:window onclick={handleWindowClick} />

<div class="fixed end-4 bottom-4 z-50 flex flex-col items-end gap-2" data-demo-panel>
	{#if expanded}
		<div
			class="flex items-center gap-3 rounded-2xl border bg-background/80 px-4 py-2.5 shadow-lg backdrop-blur-sm"
			transition:fly={{ y: 8, duration: 200 }}
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
	{/if}

	<button
		class="relative flex h-12 w-12 items-center justify-center rounded-full border bg-primary text-primary-foreground shadow-lg transition-transform hover:scale-105 active:scale-95"
		onclick={() => (expanded = !expanded)}
		aria-label={m.demo_viewingAs()}
	>
		<span class="relative z-10 text-sm font-bold">{roleLetters[currentRole()] ?? 'U'}</span>
		<span
			class="absolute inset-0 animate-ping rounded-full bg-primary opacity-20"
			aria-hidden="true"
		></span>
	</button>
</div>
