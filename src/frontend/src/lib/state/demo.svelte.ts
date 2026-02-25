/**
 * Reactive demo role state for the role switcher.
 * Persists the selected "viewing as" role to localStorage so it
 * survives page refreshes. Client-only singleton.
 */

import { browser } from '$app/environment';

/** The two demo roles a user can preview. */
export type DemoRole = 'User' | 'Admin';

const STORAGE_KEY = 'netrock-demo-role';
const VALID_ROLES: ReadonlySet<string> = new Set<DemoRole>(['User', 'Admin']);

function canUseLocalStorage(): boolean {
	return browser && typeof globalThis.localStorage !== 'undefined';
}

function readStoredRole(): DemoRole | null {
	if (!canUseLocalStorage()) return null;
	const stored = localStorage.getItem(STORAGE_KEY);
	return stored && VALID_ROLES.has(stored) ? (stored as DemoRole) : null;
}

function createDemoState() {
	let viewingAs = $state<DemoRole>(readStoredRole() ?? 'Admin');

	return {
		/** The role the demo user is currently viewing the app as. */
		get viewingAs() {
			return viewingAs;
		},
		set viewingAs(role: DemoRole) {
			viewingAs = role;
			if (canUseLocalStorage()) localStorage.setItem(STORAGE_KEY, role);
		},
		/**
		 * Sets the initial role from the user's actual roles when there is
		 * no stored preference yet. Picks the highest available demo role.
		 */
		initializeFromRoles(roles: string[] | undefined) {
			if (readStoredRole()) return;
			viewingAs = roles?.includes('Admin') ? 'Admin' : 'User';
		}
	};
}

/** Global demo state singleton. */
export const demoState = createDemoState();
