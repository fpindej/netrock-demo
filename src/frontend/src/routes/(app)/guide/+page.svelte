<script lang="ts">
	import { GuideStep } from '$lib/components/guide';
	import { Rocket, ShieldCheck, CircleUser, Users, BarChart3, Settings } from '@lucide/svelte';
	import * as m from '$lib/paraglide/messages';
</script>

<svelte:head>
	<title>{m.meta_titleTemplate({ title: m.meta_guide_title() })}</title>
	<meta name="description" content={m.meta_guide_description()} />
</svelte:head>

<div class="space-y-6">
	<div>
		<h3 class="text-lg font-medium">{m.guide_title()}</h3>
		<p class="text-sm text-muted-foreground">{m.guide_description()}</p>
	</div>
	<div class="h-px w-full bg-border"></div>
	<div class="space-y-4">
		<GuideStep
			step={1}
			icon={Rocket}
			title={m.guide_step1_title()}
			description={m.guide_step1_description()}
		/>
		<GuideStep
			step={2}
			icon={ShieldCheck}
			title={m.guide_step2_title()}
			description={m.guide_step2_description()}
		/>
		<GuideStep
			step={3}
			icon={CircleUser}
			title={m.guide_step3_title()}
			description={m.guide_step3_description()}
			action={{ label: m.guide_goToProfile(), href: '/profile' }}
		/>
		<GuideStep
			step={4}
			icon={Users}
			title={m.guide_step4_title()}
			description={m.guide_step4_description()}
			action={{ label: m.guide_goToContacts(), href: '/contacts' }}
		/>
		<GuideStep
			step={5}
			icon={BarChart3}
			title={m.guide_step5_title()}
			description={m.guide_step5_description()}
			action={{ label: m.guide_goToAnalytics(), href: '/analytics' }}
		/>
		<GuideStep
			step={6}
			icon={Settings}
			title={m.guide_step6_title()}
			description={m.guide_step6_description()}
			action={{ label: m.guide_viewAsAdmin(), href: '/admin/users' }}
		/>
	</div>
</div>
