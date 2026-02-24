/**
 * Contact-related types matching the backend API response shapes.
 *
 * These are defined manually because the contacts endpoints
 * were added after the last openapi-typescript generation.
 * Once `pnpm run api:generate` includes contacts, these can
 * be replaced with derived types from `v1.d.ts`.
 */

export interface ContactResponse {
	id: string;
	firstName: string;
	lastName: string;
	email: string;
	company: string | null;
	status: string;
	source: string;
	value: number;
	notes: string | null;
	phone: string | null;
	ownerId: string;
	createdAt: string;
	updatedAt: string | null;
}

export interface ContactListResponse {
	items: ContactResponse[];
	totalCount: number;
	pageNumber: number;
	pageSize: number;
	totalPages: number;
	hasPreviousPage: boolean;
	hasNextPage: boolean;
}

export interface CreateContactBody {
	firstName: string;
	lastName: string;
	email: string;
	company?: string | null;
	status: ContactStatusEnum;
	source: ContactSourceEnum;
	value: number;
	notes?: string | null;
	phone?: string | null;
}

export interface UpdateContactBody {
	firstName: string;
	lastName: string;
	email: string;
	company?: string | null;
	status: ContactStatusEnum;
	source: ContactSourceEnum;
	value: number;
	notes?: string | null;
	phone?: string | null;
}

export type ContactStatusEnum = 'Lead' | 'Prospect' | 'Customer' | 'Churning';
export type ContactSourceEnum = 'Web' | 'Email' | 'Phone' | 'SocialMedia' | 'Referral' | 'Other';
