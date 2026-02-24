<script lang="ts">
	import {
		Database,
		Cookie,
		Eye,
		Server,
		Trash2,
		Mail,
		ShieldCheck,
		ExternalLink
	} from '@lucide/svelte';
	import * as Card from '$lib/components/ui/card';
	import * as m from '$lib/paraglide/messages';

	const sections = [
		{
			icon: Eye,
			title: m.privacy_section_dataCollected_title,
			description: m.privacy_section_dataCollected_description,
			items: [
				m.privacy_section_dataCollected_item_email,
				m.privacy_section_dataCollected_item_name,
				m.privacy_section_dataCollected_item_phone,
				m.privacy_section_dataCollected_item_avatar,
				m.privacy_section_dataCollected_item_bio
			]
		},
		{
			icon: ShieldCheck,
			title: m.privacy_section_dataUsage_title,
			description: m.privacy_section_dataUsage_description,
			items: [
				m.privacy_section_dataUsage_item_auth,
				m.privacy_section_dataUsage_item_profile,
				m.privacy_section_dataUsage_item_demo,
				m.privacy_section_dataUsage_item_noSale
			]
		},
		{
			icon: Database,
			title: m.privacy_section_storage_title,
			description: m.privacy_section_storage_description,
			items: [
				m.privacy_section_storage_item_database,
				m.privacy_section_storage_item_passwords,
				m.privacy_section_storage_item_hosting
			]
		},
		{
			icon: Cookie,
			title: m.privacy_section_cookies_title,
			description: m.privacy_section_cookies_description,
			items: [
				m.privacy_section_cookies_item_jwt,
				m.privacy_section_cookies_item_refresh,
				m.privacy_section_cookies_item_preferences,
				m.privacy_section_cookies_item_noTracking
			]
		},
		{
			icon: Server,
			title: m.privacy_section_thirdParty_title,
			description: m.privacy_section_thirdParty_description,
			items: [m.privacy_section_thirdParty_item_resend, m.privacy_section_thirdParty_item_turnstile]
		},
		{
			icon: Trash2,
			title: m.privacy_section_retention_title,
			description: m.privacy_section_retention_description,
			items: [
				m.privacy_section_retention_item_periodic,
				m.privacy_section_retention_item_delete,
				m.privacy_section_retention_item_noPermanent
			]
		}
	];
</script>

<div class="space-y-6">
	<div>
		<h1 class="text-lg font-medium">{m.privacy_title()}</h1>
		<p class="text-sm text-muted-foreground">{m.privacy_description()}</p>
	</div>
	<div class="h-px w-full bg-border"></div>

	<p class="text-xs text-muted-foreground">
		{m.privacy_lastUpdated({ date: '2026-02-24' })}
	</p>

	<div class="space-y-6">
		{#each sections as section (section.title)}
			<Card.Root>
				<Card.Header>
					<div class="flex items-center gap-3">
						<section.icon class="h-5 w-5 text-muted-foreground" />
						<Card.Title>{section.title()}</Card.Title>
					</div>
					<Card.Description>{section.description()}</Card.Description>
				</Card.Header>
				<Card.Content>
					<ul class="space-y-2 text-sm text-muted-foreground">
						{#each section.items as item, i (i)}
							<li class="flex items-start gap-2">
								<span class="mt-1.5 h-1.5 w-1.5 shrink-0 rounded-full bg-primary"></span>
								<span>{item()}</span>
							</li>
						{/each}
					</ul>
				</Card.Content>
			</Card.Root>
		{/each}

		<Card.Root>
			<Card.Header>
				<div class="flex items-center gap-3">
					<Mail class="h-5 w-5 text-muted-foreground" />
					<Card.Title>{m.privacy_section_contact_title()}</Card.Title>
				</div>
				<Card.Description>{m.privacy_section_contact_description()}</Card.Description>
			</Card.Header>
			<Card.Content>
				<a
					href="mailto:contact@mail.pindej.cz"
					class="inline-flex items-center gap-1.5 text-sm font-medium text-primary hover:underline"
				>
					{m.privacy_section_contact_email({ email: 'contact@mail.pindej.cz' })}
					<ExternalLink class="h-3.5 w-3.5" />
				</a>
			</Card.Content>
		</Card.Root>
	</div>
</div>
