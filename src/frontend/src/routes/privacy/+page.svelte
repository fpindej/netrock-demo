<script lang="ts">
	import { resolve } from '$app/paths';
	import { LoginBackground } from '$lib/components/auth';
	import { ThemeToggle, LanguageSelector } from '$lib/components/layout';
	import * as Card from '$lib/components/ui/card';
	import * as m from '$lib/paraglide/messages';
	import { ArrowLeft, Database, Target, Lock, Scale, Mail } from '@lucide/svelte';

	const contactEmail = 'contact@mail.pindej.cz';

	const sections = [
		{
			icon: Database,
			title: m.privacy_whatWeCollect_title,
			body: m.privacy_whatWeCollect_body
		},
		{
			icon: Target,
			title: m.privacy_purpose_title,
			body: m.privacy_purpose_body
		},
		{
			icon: Lock,
			title: m.privacy_storage_title,
			body: m.privacy_storage_body
		}
	] as const;

	const linkClose = '</a>';

	function link(href: string): { linkOpen: string; linkClose: string } {
		return {
			linkOpen: `<a href="${href}" class="font-medium text-primary hover:underline">`,
			linkClose
		};
	}
</script>

<svelte:head>
	<title>{m.meta_titleTemplate({ title: m.meta_privacy_title() })}</title>
	<meta name="description" content={m.meta_privacy_description()} />
</svelte:head>

<LoginBackground>
	<div class="absolute end-4 top-4 flex gap-2">
		<LanguageSelector />
		<ThemeToggle />
	</div>

	<div class="sm:mx-auto sm:w-full sm:max-w-2xl">
		<Card.Root class="border-muted/60 bg-card/50 shadow-xl backdrop-blur-sm">
			<Card.Header>
				<Card.Title class="text-center text-2xl">{m.privacy_title()}</Card.Title>
				<Card.Description class="text-center">
					{m.privacy_subtitle()}
				</Card.Description>
			</Card.Header>
			<Card.Content class="space-y-6">
				{#each sections as section (section.title)}
					<div class="flex gap-3">
						<div class="shrink-0 self-start rounded-lg bg-primary/10 p-2">
							<section.icon class="h-5 w-5 text-primary" />
						</div>
						<div>
							<h3 class="font-medium">{section.title()}</h3>
							<p class="mt-1 text-sm text-muted-foreground">{section.body()}</p>
						</div>
					</div>
				{/each}

				<div class="flex gap-3">
					<div class="shrink-0 self-start rounded-lg bg-primary/10 p-2">
						<Scale class="h-5 w-5 text-primary" />
					</div>
					<div>
						<h3 class="font-medium">{m.privacy_rights_title()}</h3>
						<p class="mt-1 text-sm text-muted-foreground">{m.privacy_rights_body()}</p>
						<!-- eslint-disable svelte/no-at-html-tags -- i18n link interpolation, no user input -->
						<ul class="mt-2 space-y-1 text-sm text-muted-foreground">
							<li>
								{@html m.privacy_rights_access(link(resolve('/profile')))}
							</li>
							<li>
								{@html m.privacy_rights_delete(link(resolve('/settings')))}
							</li>
							<li>
								{@html m.privacy_rights_request({
									email: contactEmail,
									...link(`mailto:${contactEmail}`)
								})}
							</li>
						</ul>
						<!-- eslint-enable svelte/no-at-html-tags -->
					</div>
				</div>

				<div class="flex gap-3">
					<div class="shrink-0 self-start rounded-lg bg-primary/10 p-2">
						<Mail class="h-5 w-5 text-primary" />
					</div>
					<div>
						<h3 class="font-medium">{m.privacy_contact_title()}</h3>
						<!-- eslint-disable svelte/no-at-html-tags -- i18n link interpolation, no user input -->
						<p class="mt-1 text-sm text-muted-foreground">
							{@html m.privacy_contact_body({
								email: contactEmail,
								...link(`mailto:${contactEmail}`)
							})}
						</p>
						<!-- eslint-enable svelte/no-at-html-tags -->
					</div>
				</div>

				<div class="border-t pt-4">
					<p class="text-center text-xs text-muted-foreground">
						{m.privacy_lastUpdated({ date: '2026-02-18' })}
					</p>
				</div>

				<div class="text-center text-sm">
					<a
						href={resolve('/login')}
						class="inline-flex items-center gap-1 font-medium text-primary hover:underline"
					>
						<ArrowLeft class="h-4 w-4" />
						{m.privacy_backToLogin()}
					</a>
				</div>
			</Card.Content>
		</Card.Root>
	</div>
</LoginBackground>
