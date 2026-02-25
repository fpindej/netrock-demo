/**
 * Reactive shell preference state for OS-aware code blocks.
 * Auto-detects OS and persists user override to localStorage.
 */

import { IS_WINDOWS } from '$lib/utils/platform';

/** Supported shell types for code block display. */
export type Shell = 'unix' | 'powershell';

const STORAGE_KEY = 'preferred-shell';

function getStoredShell(): Shell {
	if (typeof window === 'undefined') return 'unix';
	try {
		const stored = localStorage.getItem(STORAGE_KEY);
		if (stored === 'unix' || stored === 'powershell') return stored;
	} catch {
		// localStorage may be unavailable
	}
	return IS_WINDOWS ? 'powershell' : 'unix';
}

/**
 * Shell preference state object.
 * Use `shellState.shell` to read the current preference.
 */
export const shellState = $state({
	shell: getStoredShell()
});

/**
 * Set the preferred shell and persist to localStorage.
 */
export function setShell(value: Shell): void {
	shellState.shell = value;
	try {
		localStorage.setItem(STORAGE_KEY, value);
	} catch {
		// localStorage may be unavailable — state still works in-memory
	}
}
