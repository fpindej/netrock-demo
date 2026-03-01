<script lang="ts">
	import * as Card from '$lib/components/ui/card';
	import { Badge } from '$lib/components/ui/badge';
	import { Button } from '$lib/components/ui/button';
	import { ShieldCheck, ShieldOff } from '@lucide/svelte';
	import * as m from '$lib/paraglide/messages';
	import { TwoFactorSetupDialog, TwoFactorDisableDialog } from '$lib/components/settings';
	import type { User } from '$lib/types';

	interface Props {
		user: User;
	}

	let { user }: Props = $props();

	let setupDialogOpen = $state(false);
	let disableDialogOpen = $state(false);

	let isEnabled = $derived(user.twoFactorEnabled ?? false);
</script>

<Card.Root class="card-hover" data-tour="settings-2fa">
	<Card.Header>
		<Card.Title class="flex items-center gap-2">
			{#if isEnabled}
				<ShieldCheck class="h-5 w-5 text-success" />
			{:else}
				<ShieldOff class="h-5 w-5 text-muted-foreground" />
			{/if}
			{m.settings_2fa_title()}
		</Card.Title>
		<Card.Description>{m.settings_2fa_description()}</Card.Description>
	</Card.Header>
	<Card.Content>
		<div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
			<div class="space-y-1">
				{#if isEnabled}
					<div class="flex items-center gap-2">
						<Badge
							variant="outline"
							class="border-success/30 bg-success/10 text-success dark:border-success/30 dark:bg-success/10 dark:text-success-foreground"
						>
							{m.settings_2fa_enabled()}
						</Badge>
					</div>
					<p class="text-sm text-muted-foreground">
						{m.settings_2fa_enabledDescription()}
					</p>
				{:else}
					<div class="flex items-center gap-2">
						<Badge variant="outline" class="text-muted-foreground">
							{m.settings_2fa_disabled()}
						</Badge>
					</div>
					<p class="text-sm text-muted-foreground">
						{m.settings_2fa_disabledDescription()}
					</p>
				{/if}
			</div>
			<div class="flex shrink-0 gap-2">
				{#if isEnabled}
					<Button variant="destructive" onclick={() => (disableDialogOpen = true)}>
						{m.settings_2fa_disable()}
					</Button>
				{:else}
					<Button onclick={() => (setupDialogOpen = true)}>
						{m.settings_2fa_enable()}
					</Button>
				{/if}
			</div>
		</div>
		<p class="mt-3 text-xs text-muted-foreground/70 italic">
			{m.settings_2fa_demoNote()}
		</p>
	</Card.Content>
</Card.Root>

<TwoFactorSetupDialog bind:open={setupDialogOpen} />
<TwoFactorDisableDialog bind:open={disableDialogOpen} />
