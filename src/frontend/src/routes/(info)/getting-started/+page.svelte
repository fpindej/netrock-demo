<script lang="ts">
	import { GettingStartedContent } from '$lib/components/getting-started';
	import * as m from '$lib/paraglide/messages';
</script>

<svelte:head>
	<title>{m.meta_titleTemplate({ title: m.meta_gettingStarted_title() })}</title>
	<meta name="description" content={m.meta_gettingStarted_description()} />
</svelte:head>

<GettingStartedContent />
