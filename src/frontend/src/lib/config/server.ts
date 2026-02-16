import { env } from '$env/dynamic/private';

export const SERVER_CONFIG = {
	API_URL: env.API_URL || 'http://localhost:13002',
	/** Additional origins allowed through the API proxy CSRF check (e.g. ngrok, reverse proxy). */
	ALLOWED_ORIGINS:
		env.ALLOWED_ORIGINS?.split(',')
			.map((o) => o.trim())
			.filter(Boolean) ?? []
};
