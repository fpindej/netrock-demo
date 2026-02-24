import { createApiClient, getErrorMessage } from '$lib/api';
import { error } from '@sveltejs/kit';
import * as m from '$lib/paraglide/messages';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async ({ fetch, url, parent }) => {
	await parent();
	const client = createApiClient(fetch, url.origin);
	const { data, response, error: apiError } = await client.GET('/api/v1/contacts/stats');

	if (!response.ok) {
		throw error(response.status, getErrorMessage(apiError, m.serverError_failedToLoadAnalytics()));
	}

	return { stats: data };
};
