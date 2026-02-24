import { createApiClient } from '$lib/api';
import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async ({ fetch, url, parent }) => {
	await parent();
	const client = createApiClient(fetch, url.origin);
	const { data } = await client.GET('/api/v1/contacts/stats');
	return { stats: data };
};
