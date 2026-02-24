/**
 * Client-side permission utilities.
 * Mirrors backend AppPermissions constants.
 *
 * When the demo role switcher is active, permissions are masked to
 * match the selected role so users can preview the app as different roles.
 */

import { browser } from '$app/environment';
import { demoState, type DemoRole } from '$lib/state';
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

/** Permissions granted to the Admin demo role. */
const ADMIN_PERMISSIONS: string[] = [
	Permissions.Users.View,
	Permissions.Users.Manage,
	Permissions.Users.AssignRoles,
	Permissions.Users.ViewPii,
	Permissions.Roles.View,
	Permissions.Jobs.View
];

/**
 * Returns the effective permissions for a demo role.
 * Admin = a curated subset. User = none.
 */
function getDemoPermissions(role: DemoRole): string[] {
	switch (role) {
		case 'Admin':
			return ADMIN_PERMISSIONS;
		case 'User':
			return [];
	}
}

/** Returns true if the user is a SuperAdmin (implicit all permissions). */
export function isSuperAdmin(user: User | null | undefined): boolean {
	return user?.roles?.includes('SuperAdmin') ?? false;
}

/** Returns true if the user has a specific permission based on the active demo role. */
export function hasPermission(user: User | null | undefined, permission: string): boolean {
	if (!user) return false;

	// Client-side: demo role masking for all users
	if (browser) {
		const demoPerms = getDemoPermissions(demoState.viewingAs);
		return demoPerms.includes(permission);
	}

	// Server-side: use real permissions (SuperAdmin has implicit all)
	if (isSuperAdmin(user)) return true;
	return user.permissions?.includes(permission) ?? false;
}

/** Returns true if the user has at least one of the given permissions. */
export function hasAnyPermission(user: User | null | undefined, permissions: string[]): boolean {
	return permissions.some((p) => hasPermission(user, p));
}
