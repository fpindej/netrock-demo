<script lang="ts">
	import { Play } from '@lucide/svelte';
	import { Button } from '$lib/components/ui/button';
	import { goto, invalidateAll } from '$app/navigation';
	import { resolve } from '$app/paths';
	import { browserClient } from '$lib/api';
	import { setSidebarCollapsed, demoState } from '$lib/state';
	import * as m from '$lib/paraglide/messages';

	type StepDef = {
		page?: string;
		element?: string;
		title: string;
		description: string;
		side?: 'top' | 'bottom' | 'left' | 'right';
		align?: 'start' | 'center' | 'end';
	};

	async function startTour() {
		const [{ driver }] = await Promise.all([
			import('driver.js'),
			import('driver.js/dist/driver.css')
		]);

		const isMobile = window.innerWidth < 768;

		if (!isMobile) {
			setSidebarCollapsed(false);
		}

		// Ensure admin access for the full tour
		const originalRole = demoState.viewingAs;
		if (originalRole !== 'Admin') {
			const { response } = await browserClient.POST('/api/v1/demo/elevate', {
				body: { role: 'Admin' }
			});
			if (response.ok) {
				demoState.viewingAs = 'Admin';
				await invalidateAll();
			}
		}

		await new Promise((r) => setTimeout(r, 350));

		// -- Step definitions with target pages -----------------------------------

		const stepDefs: StepDef[] = [
			// Welcome
			...(isMobile
				? [
						{
							page: resolve('/guide'),
							title: m.tour_welcome_title(),
							description: m.tour_welcome_description()
						} satisfies StepDef
					]
				: [
						{
							page: resolve('/guide'),
							element: '[data-tour="sidebar-brand"]',
							title: m.tour_welcome_title(),
							description: m.tour_welcome_description(),
							side: 'right' as const,
							align: 'center' as const
						} satisfies StepDef
					]),
			// Getting Started - Steps
			{
				page: resolve('/getting-started'),
				element: '[data-tour="getting-started-steps"]',
				title: m.tour_gettingStarted_title(),
				description: m.tour_gettingStarted_description(),
				side: 'bottom',
				align: 'center'
			},
			// For You - Personas
			{
				page: resolve('/for-you'),
				element: '[data-tour="for-you-personas"]',
				title: m.tour_personas_title(),
				description: m.tour_personas_description(),
				side: 'bottom',
				align: 'center'
			},
			// For You - Diagram
			{
				page: resolve('/for-you'),
				element: '[data-tour="for-you-diagram"]',
				title: m.tour_diagram_title(),
				description: m.tour_diagram_description(),
				side: isMobile ? 'bottom' : 'left',
				align: 'center'
			},
			// How It Works - Architecture
			{
				page: resolve('/how-it-works'),
				element: '[data-tour="how-it-works-architecture"]',
				title: m.tour_architecture_title(),
				description: m.tour_architecture_description(),
				side: 'bottom',
				align: 'center'
			},
			// How It Works - Tech Stack
			{
				page: resolve('/how-it-works'),
				element: '[data-tour="how-it-works-tech-stack"]',
				title: m.tour_techStack_title(),
				description: m.tour_techStack_description(),
				side: 'bottom',
				align: 'center'
			},
			// Contacts
			{
				page: resolve('/contacts'),
				element: '[data-tour="contacts-content"]',
				title: m.tour_contacts_title(),
				description: m.tour_contacts_description(),
				side: isMobile ? 'bottom' : 'left',
				align: 'start'
			},
			// Analytics
			{
				page: resolve('/analytics'),
				element: '[data-tour="analytics-content"]',
				title: m.tour_analytics_title(),
				description: m.tour_analytics_description(),
				side: isMobile ? 'bottom' : 'left',
				align: 'start'
			},
			// Admin
			{
				page: resolve('/admin/users'),
				element: '[data-tour="admin-content"]',
				title: m.tour_admin_title(),
				description: m.tour_admin_description(),
				side: isMobile ? 'bottom' : 'left',
				align: 'start'
			},
			// Role Switcher
			{
				element: '[data-tour="role-switcher"]',
				title: m.tour_roleSwitcher_title(),
				description: m.tour_roleSwitcher_description(),
				side: 'top',
				align: 'end'
			},
			// Sidebar Tools (desktop only)
			...(!isMobile
				? [
						{
							element: '[data-tour="sidebar-tools"]',
							title: m.tour_tools_title(),
							description: m.tour_tools_description(),
							side: 'right' as const,
							align: 'center' as const
						} satisfies StepDef
					]
				: [])
		];

		// -- Navigation helper ----------------------------------------------------

		async function navigateAndWait(path: string) {
			// eslint-disable-next-line svelte/no-navigation-without-resolve -- paths are pre-resolved via resolve() in stepDefs
			await goto(path);
			const main = document.querySelector('main');
			if (main) main.scrollTop = 0;
			// Wait for SvelteKit page transition (300ms animation) + DOM settle
			await new Promise((r) => setTimeout(r, 600));
		}

		// -- Build Driver.js steps with navigation callbacks ----------------------

		let driverObj: ReturnType<typeof driver> | undefined;

		const steps = stepDefs.map((step, i) => ({
			...(step.element ? { element: step.element } : {}),
			popover: {
				title: step.title,
				description: step.description,
				...(step.side ? { side: step.side } : {}),
				...(step.align ? { align: step.align } : {}),
				onNextClick: async () => {
					const next = stepDefs[i + 1];
					if (next?.page && window.location.pathname !== next.page) {
						await navigateAndWait(next.page);
					}
					driverObj?.moveNext();
				},
				onPrevClick: async () => {
					const prev = stepDefs[i - 1];
					if (prev?.page && window.location.pathname !== prev.page) {
						await navigateAndWait(prev.page);
					}
					driverObj?.movePrevious();
				}
			}
		}));

		driverObj = driver({
			showProgress: true,
			animate: true,
			smoothScroll: true,
			stagePadding: 8,
			stageRadius: 8,
			popoverClass: 'netrock-tour',
			nextBtnText: m.tour_next(),
			prevBtnText: m.tour_prev(),
			doneBtnText: m.tour_done(),
			progressText: m.tour_progress({ current: '{{current}}', total: '{{total}}' }),
			steps,
			onDestroyed: async () => {
				if (demoState.viewingAs !== originalRole) {
					const { response } = await browserClient.POST('/api/v1/demo/elevate', {
						body: { role: originalRole }
					});
					if (response.ok) {
						demoState.viewingAs = originalRole;
						await invalidateAll();
					}
				}
			}
		});

		driverObj.drive();
	}
</script>

<Button onclick={startTour} class="tour-cta w-full sm:w-auto">
	<Play class="me-2 h-4 w-4" />
	{m.tour_startButton()}
</Button>

<style>
	/* Pulsing glow to draw attention to the tour button */
	:global(.tour-cta) {
		box-shadow: 0 0 0 0 hsl(var(--primary) / 0.4);
		animation: tour-pulse 2.5s ease-in-out infinite;
	}

	@keyframes tour-pulse {
		0%,
		100% {
			box-shadow: 0 0 0 0 hsl(var(--primary) / 0.4);
		}
		50% {
			box-shadow: 0 0 0 6px hsl(var(--primary) / 0);
		}
	}

	/* Driver.js popover theming - match NETrock design system */
	:global(.netrock-tour) {
		background-color: hsl(var(--popover)) !important;
		color: hsl(var(--popover-foreground)) !important;
		border: 1px solid hsl(var(--border)) !important;
		border-radius: var(--radius) !important;
		box-shadow:
			0 4px 24px rgba(0, 0, 0, 0.12),
			0 1px 8px rgba(0, 0, 0, 0.06) !important;
	}

	:global(.netrock-tour .driver-popover-title) {
		font-size: 0.975rem !important;
		font-weight: 600 !important;
		color: hsl(var(--popover-foreground)) !important;
	}

	:global(.netrock-tour .driver-popover-description) {
		color: hsl(var(--muted-foreground)) !important;
		font-size: 0.875rem !important;
	}

	:global(.netrock-tour .driver-popover-footer button.driver-popover-next-btn) {
		background-color: hsl(var(--primary)) !important;
		color: hsl(var(--primary-foreground)) !important;
		border: none !important;
		border-radius: calc(var(--radius) - 2px) !important;
		text-shadow: none !important;
		font-weight: 500 !important;
	}

	:global(.netrock-tour .driver-popover-footer button.driver-popover-prev-btn) {
		background-color: transparent !important;
		color: hsl(var(--muted-foreground)) !important;
		border: 1px solid hsl(var(--border)) !important;
		border-radius: calc(var(--radius) - 2px) !important;
		text-shadow: none !important;
		font-weight: 500 !important;
	}

	:global(.netrock-tour .driver-popover-progress-text) {
		color: hsl(var(--muted-foreground)) !important;
		font-size: 0.8rem !important;
	}

	:global(.netrock-tour .driver-popover-close-btn) {
		color: hsl(var(--muted-foreground)) !important;
	}

	:global(.netrock-tour .driver-popover-close-btn:hover) {
		color: hsl(var(--foreground)) !important;
	}

	/* Arrow colors to match popover background */
	:global(.netrock-tour.driver-popover-arrow-side-left .driver-popover-arrow) {
		border-inline-end-color: hsl(var(--popover)) !important;
	}

	:global(.netrock-tour.driver-popover-arrow-side-right .driver-popover-arrow) {
		border-inline-start-color: hsl(var(--popover)) !important;
	}

	:global(.netrock-tour.driver-popover-arrow-side-top .driver-popover-arrow) {
		border-block-end-color: hsl(var(--popover)) !important;
	}

	:global(.netrock-tour.driver-popover-arrow-side-bottom .driver-popover-arrow) {
		border-block-start-color: hsl(var(--popover)) !important;
	}

	/* Dark mode adjustments */
	:global(.dark .netrock-tour) {
		box-shadow:
			0 4px 24px rgba(0, 0, 0, 0.4),
			0 1px 8px rgba(0, 0, 0, 0.2) !important;
	}

	/* Respect reduced motion */
	@media (prefers-reduced-motion: reduce) {
		:global(.tour-cta) {
			animation: none;
		}
	}
</style>
