<script lang="ts">
	import { ForYouContent } from '$lib/components/for-you';
	import * as m from '$lib/paraglide/messages';
</script>

<svelte:head>
	<title>{m.meta_titleTemplate({ title: m.meta_forYou_title() })}</title>
	<meta name="description" content={m.meta_forYou_description()} />
</svelte:head>

<ForYouContent />
