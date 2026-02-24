<script lang="ts">
	import { Play } from '@lucide/svelte';
	import { Button } from '$lib/components/ui/button';
	import { setSidebarCollapsed } from '$lib/state';
	import * as m from '$lib/paraglide/messages';

	async function startTour() {
		const [{ driver }] = await Promise.all([
			import('driver.js'),
			import('driver.js/dist/driver.css')
		]);

		const isMobile = window.innerWidth < 768;
		let sheetTrigger: HTMLButtonElement | null = null;

		function isSheetOpen(): boolean {
			return sheetTrigger?.getAttribute('aria-expanded') === 'true';
		}

		function openMobileSheet() {
			if (isMobile && sheetTrigger && !isSheetOpen()) {
				sheetTrigger.click();
			}
		}

		function closeMobileSheet() {
			if (isMobile && sheetTrigger && isSheetOpen()) {
				sheetTrigger.click();
			}
		}

		if (isMobile) {
			sheetTrigger = document.querySelector('header button') as HTMLButtonElement | null;
			openMobileSheet();
		} else {
			setSidebarCollapsed(false);
		}

		await new Promise((r) => setTimeout(r, 350));

		const side = isMobile ? ('bottom' as const) : ('right' as const);

		const driverObj = driver({
			showProgress: true,
			animate: true,
			stagePadding: 8,
			stageRadius: 8,
			popoverClass: 'netrock-tour',
			nextBtnText: m.tour_next(),
			prevBtnText: m.tour_prev(),
			doneBtnText: m.tour_done(),
			progressText: m.tour_progress({ current: '{{current}}', total: '{{total}}' }),
			onDestroyed: () => {
				closeMobileSheet();
			},
			steps: [
				...(!isMobile
					? [
							{
								element: '[data-tour="sidebar-brand"]',
								popover: {
									title: m.tour_welcome_title(),
									description: m.tour_welcome_description(),
									side: 'right' as const,
									align: 'center' as const
								}
							}
						]
					: []),
				{
					element: '[data-tour="nav-for-you"]',
					popover: {
						title: m.tour_forYou_title(),
						description: m.tour_forYou_description(),
						side,
						align: 'center' as const
					}
				},
				{
					element: '[data-tour="nav-guide"]',
					popover: {
						title: m.tour_guide_title(),
						description: m.tour_guide_description(),
						side,
						align: 'center' as const
					}
				},
				{
					element: '[data-tour="nav-how-it-works"]',
					popover: {
						title: m.tour_howItWorks_title(),
						description: m.tour_howItWorks_description(),
						side,
						align: 'center' as const
					}
				},
				{
					element: '[data-tour="nav-contacts"]',
					popover: {
						title: m.tour_contacts_title(),
						description: m.tour_contacts_description(),
						side,
						align: 'center' as const
					}
				},
				{
					element: '[data-tour="nav-analytics"]',
					popover: {
						title: m.tour_analytics_title(),
						description: m.tour_analytics_description(),
						side,
						align: 'center' as const
					}
				},
				{
					element: '[data-tour="nav-admin"]',
					popover: {
						title: m.tour_admin_title(),
						description: m.tour_admin_description(),
						side,
						align: 'center' as const
					}
				},
				...(!isMobile
					? [
							{
								element: '[data-tour="sidebar-tools"]',
								popover: {
									title: m.tour_tools_title(),
									description: m.tour_tools_description(),
									side: 'right' as const,
									align: 'center' as const
								}
							}
						]
					: []),
				{
					element: '[data-tour="role-switcher"]',
					popover: {
						title: m.tour_roleSwitcher_title(),
						description: m.tour_roleSwitcher_description(),
						side: 'top' as const,
						align: 'end' as const,
						onPrevClick: () => {
							if (isMobile) {
								openMobileSheet();
								setTimeout(() => driverObj.movePrevious(), 350);
							} else {
								driverObj.movePrevious();
							}
						}
					},
					onHighlightStarted: () => {
						closeMobileSheet();
					}
				}
			]
		});

		driverObj.drive();
	}
</script>

<Button onclick={startTour} variant="outline" class="w-full sm:w-auto">
	<Play class="me-2 h-4 w-4" />
	{m.tour_startButton()}
</Button>

<style>
	/* Driver.js popover theming — match NETrock design system */
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
</style>
