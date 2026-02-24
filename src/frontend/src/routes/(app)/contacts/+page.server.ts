import type { PageServerLoad } from './$types';
import type { ContactListResponse } from '$lib/types/contacts';

export const load: PageServerLoad = async ({ fetch, url, parent }) => {
	await parent();

	const pageNumber = Number(url.searchParams.get('page') ?? '1');
	const pageSize = Number(url.searchParams.get('pageSize') ?? '10');
	const search = url.searchParams.get('search') ?? '';

	const params = new URLSearchParams();
	params.set('pageNumber', String(pageNumber));
	params.set('pageSize', String(pageSize));
	if (search) params.set('search', search);

	const response = await fetch(`${url.origin}/api/v1/contacts?${params.toString()}`);

	if (!response.ok) {
		return {
			contacts: null as ContactListResponse | null,
			search
		};
	}

	const data: ContactListResponse = await response.json();

	return {
		contacts: data,
		search
	};
};
