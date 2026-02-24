import { beforeEach, describe, expect, it } from 'vitest';
import type { User } from '$lib/types';
import { hasAnyPermission, hasPermission, isSuperAdmin, Permissions } from './permissions';
import { demoState } from '$lib/state';

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

// ── hasPermission (demo masking — browser: true from global test setup) ──

describe('hasPermission', () => {
	beforeEach(() => {
		demoState.viewingAs = 'Admin';
	});

	it('returns false for null user regardless of demo role', () => {
		expect(hasPermission(null, Permissions.Users.View)).toBe(false);
	});

	it('returns false for undefined user regardless of demo role', () => {
		expect(hasPermission(undefined, Permissions.Users.View)).toBe(false);
	});

	describe('viewing as Admin', () => {
		beforeEach(() => {
			demoState.viewingAs = 'Admin';
		});

		it('grants Admin-scoped permissions to any user', () => {
			const user = makeUser({ roles: ['User'], permissions: [] });
			expect(hasPermission(user, Permissions.Users.View)).toBe(true);
			expect(hasPermission(user, Permissions.Users.Manage)).toBe(true);
			expect(hasPermission(user, Permissions.Users.AssignRoles)).toBe(true);
			expect(hasPermission(user, Permissions.Roles.View)).toBe(true);
			expect(hasPermission(user, Permissions.Roles.Manage)).toBe(true);
			expect(hasPermission(user, Permissions.Jobs.View)).toBe(true);
			expect(hasPermission(user, Permissions.Jobs.Manage)).toBe(true);
		});

		it('does not grant ViewPii or unknown permissions', () => {
			const user = makeUser();
			expect(hasPermission(user, Permissions.Users.ViewPii)).toBe(false);
			expect(hasPermission(user, 'some.custom.permission')).toBe(false);
		});

		it('ignores the actual user permissions array', () => {
			const user = makeUser({ permissions: [Permissions.Users.ViewPii] });
			expect(hasPermission(user, Permissions.Users.ViewPii)).toBe(false);
		});
	});

	describe('viewing as User', () => {
		beforeEach(() => {
			demoState.viewingAs = 'User';
		});

		it('grants no permissions regardless of actual user role', () => {
			const user = makeUser({
				roles: ['SuperAdmin'],
				permissions: [Permissions.Users.View, Permissions.Users.Manage]
			});
			expect(hasPermission(user, Permissions.Users.View)).toBe(false);
			expect(hasPermission(user, Permissions.Users.Manage)).toBe(false);
			expect(hasPermission(user, Permissions.Roles.View)).toBe(false);
			expect(hasPermission(user, Permissions.Jobs.View)).toBe(false);
		});
	});
});

// ── hasAnyPermission ────────────────────────────────────────────────

describe('hasAnyPermission', () => {
	beforeEach(() => {
		demoState.viewingAs = 'Admin';
	});

	it('returns true when any requested permission is in Admin scope', () => {
		const user = makeUser();
		expect(hasAnyPermission(user, [Permissions.Users.View, Permissions.Roles.Manage])).toBe(true);
	});

	it('returns false when no requested permission is in Admin scope', () => {
		const user = makeUser();
		expect(hasAnyPermission(user, [Permissions.Users.ViewPii, 'some.custom.permission'])).toBe(
			false
		);
	});

	it('returns false for empty permissions list', () => {
		const user = makeUser();
		expect(hasAnyPermission(user, [])).toBe(false);
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

	it('viewing as User denies all permissions', () => {
		demoState.viewingAs = 'User';
		const user = makeUser();
		expect(hasAnyPermission(user, [Permissions.Users.View, Permissions.Users.Manage])).toBe(false);
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
