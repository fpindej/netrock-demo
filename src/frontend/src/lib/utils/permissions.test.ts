import { describe, expect, it } from 'vitest';
import type { User } from '$lib/types';
import { hasAnyPermission, hasPermission, isSuperAdmin, Permissions } from './permissions';

/** Creates a minimal User object for testing. */
function makeUser(overrides: Partial<User> = {}): User {
	return {
		id: '00000000-0000-0000-0000-000000000001',
		username: 'test@example.com',
		email: 'test@example.com',
		roles: [],
		permissions: [],
		...overrides
	};
}

// ── isSuperAdmin ────────────────────────────────────────────────────

describe('isSuperAdmin', () => {
	it('returns true when user has SuperAdmin role', () => {
		const user = makeUser({ roles: ['SuperAdmin'] });
		expect(isSuperAdmin(user)).toBe(true);
	});

	it('returns true when SuperAdmin is among multiple roles', () => {
		const user = makeUser({ roles: ['User', 'SuperAdmin', 'Admin'] });
		expect(isSuperAdmin(user)).toBe(true);
	});

	it('returns false when user has Admin but not SuperAdmin', () => {
		const user = makeUser({ roles: ['Admin'] });
		expect(isSuperAdmin(user)).toBe(false);
	});

	it('returns false when user has no roles', () => {
		const user = makeUser({ roles: [] });
		expect(isSuperAdmin(user)).toBe(false);
	});

	it('returns false for null user', () => {
		expect(isSuperAdmin(null)).toBe(false);
	});

	it('returns false for undefined user', () => {
		expect(isSuperAdmin(undefined)).toBe(false);
	});

	it('returns false when roles property is undefined', () => {
		const user = makeUser();
		delete user.roles;
		expect(isSuperAdmin(user)).toBe(false);
	});
});

// ── hasPermission ───────────────────────────────────────────────────

describe('hasPermission', () => {
	it('returns true when user has the exact permission', () => {
		const user = makeUser({ permissions: [Permissions.Users.View] });
		expect(hasPermission(user, Permissions.Users.View)).toBe(true);
	});

	it('returns false when user lacks the permission', () => {
		const user = makeUser({ permissions: [Permissions.Users.View] });
		expect(hasPermission(user, Permissions.Users.Manage)).toBe(false);
	});

	it('returns false when user has no permissions', () => {
		const user = makeUser({ permissions: [] });
		expect(hasPermission(user, Permissions.Users.View)).toBe(false);
	});

	it('SuperAdmin viewing as Admin has Admin-level permissions', () => {
		const user = makeUser({ roles: ['SuperAdmin'], permissions: [] });
		// Default demo role is Admin — SuperAdmin sees masked Admin permissions
		expect(hasPermission(user, Permissions.Users.Manage)).toBe(true);
		expect(hasPermission(user, Permissions.Roles.View)).toBe(true);
		expect(hasPermission(user, Permissions.Jobs.View)).toBe(true);
	});

	it('SuperAdmin viewing as Admin does not have permissions outside Admin scope', () => {
		const user = makeUser({ roles: ['SuperAdmin'], permissions: [] });
		// Roles.Manage and Jobs.Manage are NOT in ADMIN_PERMISSIONS
		expect(hasPermission(user, Permissions.Roles.Manage)).toBe(false);
		expect(hasPermission(user, Permissions.Jobs.Manage)).toBe(false);
		expect(hasPermission(user, 'some.custom.permission')).toBe(false);
	});

	it('returns false for null user', () => {
		expect(hasPermission(null, Permissions.Users.View)).toBe(false);
	});

	it('returns false for undefined user', () => {
		expect(hasPermission(undefined, Permissions.Users.View)).toBe(false);
	});

	it('returns false when permissions property is undefined', () => {
		const user = makeUser();
		delete user.permissions;
		expect(hasPermission(user, Permissions.Users.View)).toBe(false);
	});

	it('non-SuperAdmin with explicit permission returns true', () => {
		const user = makeUser({
			roles: ['Admin'],
			permissions: [Permissions.Users.View, Permissions.Users.Manage]
		});
		expect(hasPermission(user, Permissions.Users.Manage)).toBe(true);
	});

	it('does not grant permissions from a different permission string', () => {
		const user = makeUser({ permissions: ['users.view'] });
		expect(hasPermission(user, 'users.view_pii')).toBe(false);
	});
});

// ── hasAnyPermission ────────────────────────────────────────────────

describe('hasAnyPermission', () => {
	it('returns true when user has one of the requested permissions', () => {
		const user = makeUser({ permissions: [Permissions.Users.View] });
		expect(hasAnyPermission(user, [Permissions.Users.View, Permissions.Users.Manage])).toBe(true);
	});

	it('returns true when user has all of the requested permissions', () => {
		const user = makeUser({
			permissions: [Permissions.Users.View, Permissions.Users.Manage]
		});
		expect(hasAnyPermission(user, [Permissions.Users.View, Permissions.Users.Manage])).toBe(true);
	});

	it('returns false when user has none of the requested permissions', () => {
		const user = makeUser({ permissions: [Permissions.Jobs.View] });
		expect(hasAnyPermission(user, [Permissions.Users.View, Permissions.Users.Manage])).toBe(false);
	});

	it('returns false for empty permissions list', () => {
		const user = makeUser({ permissions: [Permissions.Users.View] });
		expect(hasAnyPermission(user, [])).toBe(false);
	});

	it('SuperAdmin viewing as Admin satisfies Admin-scoped permission checks', () => {
		const user = makeUser({ roles: ['SuperAdmin'], permissions: [] });
		// Users.Manage is in ADMIN_PERMISSIONS, so hasAny should return true
		expect(hasAnyPermission(user, [Permissions.Users.Manage, Permissions.Roles.Manage])).toBe(true);
	});

	it('returns false for null user', () => {
		expect(hasAnyPermission(null, [Permissions.Users.View])).toBe(false);
	});

	it('returns false for undefined user', () => {
		expect(hasAnyPermission(undefined, [Permissions.Users.View])).toBe(false);
	});

	it('returns false for null user even with empty permissions list', () => {
		expect(hasAnyPermission(null, [])).toBe(false);
	});
});

// ── Permissions constant ────────────────────────────────────────────

describe('Permissions constant', () => {
	it('exposes Users permissions', () => {
		expect(Permissions.Users.View).toBe('users.view');
		expect(Permissions.Users.ViewPii).toBe('users.view_pii');
		expect(Permissions.Users.Manage).toBe('users.manage');
		expect(Permissions.Users.AssignRoles).toBe('users.assign_roles');
	});

	it('exposes Roles permissions', () => {
		expect(Permissions.Roles.View).toBe('roles.view');
		expect(Permissions.Roles.Manage).toBe('roles.manage');
	});

	it('exposes Jobs permissions', () => {
		expect(Permissions.Jobs.View).toBe('jobs.view');
		expect(Permissions.Jobs.Manage).toBe('jobs.manage');
	});
});
