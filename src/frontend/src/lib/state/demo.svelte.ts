/**
 * Reactive demo role state for the role switcher.
 * Persists the selected "viewing as" role to localStorage so it
 * survives page refreshes. Client-only singleton.
 */

import { browser } from '$app/environment';

/** The three demo roles a user can preview. */
export type DemoRole = 'User' | 'Admin' | 'SuperAdmin';

const STORAGE_KEY = 'netrock-demo-role';

function canUseLocalStorage(): boolean {
	return browser && typeof globalThis.localStorage !== 'undefined';
}

function createDemoState() {
	let viewingAs = $state<DemoRole>(
		(canUseLocalStorage() ? (localStorage.getItem(STORAGE_KEY) as DemoRole | null) : null) ??
			'SuperAdmin'
	);

	return {
		/** The role the demo user is currently viewing the app as. */
		get viewingAs() {
			return viewingAs;
		},
		set viewingAs(role: DemoRole) {
			viewingAs = role;
			if (canUseLocalStorage()) localStorage.setItem(STORAGE_KEY, role);
		}
	};
}

/** Global demo state singleton. */
export const demoState = createDemoState();
