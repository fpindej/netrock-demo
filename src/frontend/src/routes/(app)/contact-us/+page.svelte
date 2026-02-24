<script lang="ts">
	import { Mail, Linkedin, Github, Globe } from '@lucide/svelte';
	import * as Card from '$lib/components/ui/card';
	import { Button } from '$lib/components/ui/button';
	import * as m from '$lib/paraglide/messages';

	const channels = [
		{
			icon: Mail,
			title: m.contact_email_title,
			description: m.contact_email_description,
			button: m.contact_email_button,
			href: 'mailto:contact@mail.pindej.cz'
		},
		{
			icon: Linkedin,
			title: m.contact_linkedin_title,
			description: m.contact_linkedin_description,
			button: m.contact_linkedin_button,
			href: 'https://www.linkedin.com/in/filip-dorian-pindej/'
		},
		{
			icon: Github,
			title: m.contact_github_title,
			description: m.contact_github_description,
			button: m.contact_github_button,
			href: 'https://github.com/fpindej'
		},
		{
			icon: Globe,
			title: m.contact_web_title,
			description: m.contact_web_description,
			button: m.contact_web_button,
			href: 'https://pindej.cz'
		}
	];
</script>

<svelte:head>
	<title>{m.meta_titleTemplate({ title: m.meta_contactMe_title() })}</title>
	<meta name="description" content={m.meta_contactMe_description()} />
</svelte:head>

<div class="space-y-6">
	<div>
		<h3 class="text-lg font-medium">{m.contact_title()}</h3>
		<p class="text-sm text-muted-foreground">{m.contact_description()}</p>
	</div>
	<div class="h-px w-full bg-border"></div>
	<div class="grid gap-6 sm:grid-cols-2">
		{#each channels as channel (channel.href)}
			<Card.Root class="transition-colors hover:border-primary/50">
				<Card.Header>
					<div class="flex items-center gap-3">
						<channel.icon class="h-5 w-5 text-muted-foreground" />
						<Card.Title>{channel.title()}</Card.Title>
					</div>
				</Card.Header>
				<Card.Content>
					<p class="text-sm text-muted-foreground">{channel.description()}</p>
				</Card.Content>
				<Card.Footer>
					<!-- eslint-disable-next-line svelte/no-navigation-without-resolve -- external URLs, not SvelteKit routes -->
					<a href={channel.href} target="_blank" rel="noopener noreferrer">
						<Button variant="outline" size="sm">
							<channel.icon class="me-2 h-4 w-4" />
							{channel.button()}
						</Button>
					</a>
				</Card.Footer>
			</Card.Root>
		{/each}
	</div>
</div>
