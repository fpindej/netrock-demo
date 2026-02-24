<script lang="ts">
	import { PrivacyContent } from '$lib/components/privacy';
	import * as m from '$lib/paraglide/messages';
</script>

<svelte:head>
	<title>{m.meta_titleTemplate({ title: m.meta_privacy_title() })}</title>
	<meta name="description" content={m.meta_privacy_description()} />
</svelte:head>

<div class="mx-auto w-full max-w-3xl">
	<PrivacyContent />
</div>
