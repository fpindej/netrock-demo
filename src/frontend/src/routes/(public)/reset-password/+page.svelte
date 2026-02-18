<script lang="ts">
	import { ResetPasswordForm } from '$lib/components/auth';
	import * as m from '$lib/paraglide/messages';

	let { data } = $props();

	let pageTitle = $derived(
		data.invited ? m.meta_setPassword_title() : m.meta_resetPassword_title()
	);
	let pageDescription = $derived(
		data.invited ? m.meta_setPassword_description() : m.meta_resetPassword_description()
	);
</script>

<svelte:head>
	<title>{m.meta_titleTemplate({ title: pageTitle })}</title>
	<meta name="description" content={pageDescription} />
</svelte:head>

<ResetPasswordForm token={data.token} invited={data.invited} />
