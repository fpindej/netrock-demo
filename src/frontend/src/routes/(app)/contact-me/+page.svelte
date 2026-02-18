<script lang="ts">
	import * as Card from '$lib/components/ui/card';
	import { Button } from '$lib/components/ui/button';
	import { Mail, Linkedin, Github, Globe } from '@lucide/svelte';
	import * as m from '$lib/paraglide/messages';

	const channels = [
		{
			title: m.contact_email_title,
			description: m.contact_email_description,
			icon: Mail,
			href: 'mailto:contact@mail.pindej.cz',
			external: false
		},
		{
			title: m.contact_linkedin_title,
			description: m.contact_linkedin_description,
			icon: Linkedin,
			href: 'https://www.linkedin.com/in/filip-dorian-pindej/',
			external: true
		},
		{
			title: m.contact_github_title,
			description: m.contact_github_description,
			icon: Github,
			href: 'https://github.com/fpindej/netrock',
			external: true
		},
		{
			title: m.contact_website_title,
			description: m.contact_website_description,
			icon: Globe,
			href: 'https://pindej.cz',
			external: true
		}
	] as const;
</script>

<svelte:head>
	<title>{m.meta_titleTemplate({ title: m.meta_contact_title() })}</title>
	<meta name="description" content={m.meta_contact_description()} />
</svelte:head>

<div class="space-y-6">
	<div>
		<h3 class="text-lg font-medium">{m.contact_title()}</h3>
		<p class="text-sm text-muted-foreground">{m.contact_description()}</p>
	</div>
	<div class="h-px w-full bg-border"></div>

	<!-- eslint-disable svelte/no-navigation-without-resolve -- all hrefs are external links -->
	<div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
		{#each channels as channel (channel.href)}
			<Card.Root>
				<Card.Header>
					<div class="flex items-center gap-3">
						<div class="rounded-lg bg-primary/10 p-2">
							<channel.icon class="h-5 w-5 text-primary" />
						</div>
						<div>
							<Card.Title class="text-base">{channel.title()}</Card.Title>
						</div>
					</div>
				</Card.Header>
				<Card.Content>
					<p class="mb-4 text-sm text-muted-foreground">{channel.description()}</p>
					<a
						href={channel.href}
						target={channel.external ? '_blank' : undefined}
						rel={channel.external ? 'noopener noreferrer' : undefined}
					>
						<Button variant="outline" size="sm">
							<channel.icon class="me-2 h-4 w-4" />
							{channel.title()}
						</Button>
					</a>
				</Card.Content>
			</Card.Root>
		{/each}
	</div>
</div>
