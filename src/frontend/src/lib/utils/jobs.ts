import * as m from '$lib/paraglide/messages';

type BadgeVariant = 'default' | 'secondary' | 'destructive' | 'outline';
type TimelineVariant = 'default' | 'success' | 'warning' | 'destructive';

/**
 * Format a nullable date string for job-related display.
 *
 * @param date   - ISO date string, null, or undefined
 * @param fallback - Text to show when the date is absent (defaults to '-')
 */
export function formatJobDate(date: string | null | undefined, fallback: string = '-'): string {
	if (!date) return fallback;
	return new Date(date).toLocaleString();
}

/**
 * Map a job execution status (and optional pause state) to a Badge variant.
 */
export function getJobStatusVariant(
	status: string | null | undefined,
	isPaused?: boolean
): BadgeVariant {
	if (isPaused) return 'outline';
	switch (status) {
		case 'Succeeded':
			return 'default';
		case 'Failed':
			return 'destructive';
		case 'Processing':
		case 'Running':
			return 'secondary';
		default:
			return 'outline';
	}
}

/**
 * Map a job execution status (and optional pause state) to a human-readable i18n label.
 */
export function getJobStatusLabel(status: string | null | undefined, isPaused?: boolean): string {
	if (isPaused) return m.admin_jobs_status_paused();
	switch (status) {
		case 'Succeeded':
			return m.admin_jobs_status_succeeded();
		case 'Failed':
			return m.admin_jobs_status_failed();
		case 'Processing':
		case 'Running':
			return m.admin_jobs_status_running();
		default:
			return status ?? m.admin_jobs_status_idle();
	}
}

/**
 * Format an HH:MM:SS duration string into a compact human-readable form.
 */
export function formatJobDuration(duration: string | null | undefined): string {
	if (!duration) return '-';
	const match = duration.match(/(\d+):(\d+):(\d+)/);
	if (!match?.[1] || !match[2] || !match[3]) return duration;
	const hours = parseInt(match[1]);
	const minutes = parseInt(match[2]);
	const seconds = parseInt(match[3]);
	if (hours > 0) return `${hours}h ${minutes}m ${seconds}s`;
	if (minutes > 0) return `${minutes}m ${seconds}s`;
	return `${seconds}s`;
}

/**
 * Map a log entry level to a Timeline component variant.
 */
export function getLogLevelVariant(level: string | null | undefined): TimelineVariant {
	switch (level) {
		case 'Warning':
			return 'warning';
		case 'Error':
			return 'destructive';
		default:
			return 'default';
	}
}

/**
 * Map a log entry level to an i18n label.
 */
export function getLogLevelLabel(level: string | null | undefined): string {
	switch (level) {
		case 'Warning':
			return m.admin_jobDetail_logLevel_warning();
		case 'Error':
			return m.admin_jobDetail_logLevel_error();
		default:
			return m.admin_jobDetail_logLevel_info();
	}
}

/**
 * Map a triggered-by value to an i18n label.
 */
export function getTriggeredByLabel(triggeredBy: string | null | undefined): string {
	switch (triggeredBy) {
		case 'Manual':
			return m.admin_jobDetail_triggeredBy_manual();
		default:
			return m.admin_jobDetail_triggeredBy_schedule();
	}
}
