/**
 * Client-side permission utilities.
 * Mirrors backend AppPermissions constants.
 *
 * Permissions are always based on the user's real JWT claims — the demo role
 * switcher calls the backend to change the actual role, so no client-side
 * masking is needed.
 */

import type { User } from '$lib/types';

export const Permissions = {
	Users: {
		View: 'users.view',
		ViewPii: 'users.view_pii',
		Manage: 'users.manage',
		AssignRoles: 'users.assign_roles'
	},
	Roles: {
		View: 'roles.view',
		Manage: 'roles.manage'
	},
	Jobs: {
		View: 'jobs.view',
		Manage: 'jobs.manage'
	}
} as const;

/** Returns true if the user is a SuperAdmin (implicit all permissions). */
export function isSuperAdmin(user: User | null | undefined): boolean {
	return user?.roles?.includes('SuperAdmin') ?? false;
}

/** Returns true if the user has a specific permission based on their real JWT claims. */
export function hasPermission(user: User | null | undefined, permission: string): boolean {
	if (!user) return false;

	if (isSuperAdmin(user)) return true;
	return user.permissions?.includes(permission) ?? false;
}

/** Returns true if the user has at least one of the given permissions. */
export function hasAnyPermission(user: User | null | undefined, permissions: string[]): boolean {
	return permissions.some((p) => hasPermission(user, p));
}
