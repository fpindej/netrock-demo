import type { Action } from 'svelte/action';

/** Scroll-triggered reveal animation. Applied via JS to avoid SSR flash. */
export const reveal: Action<HTMLElement, number | undefined> = (node, delay = 0) => {
	if (
		typeof window !== 'undefined' &&
		window.matchMedia('(prefers-reduced-motion: reduce)').matches
	)
		return;

	node.style.opacity = '0';
	node.style.transform = 'translateY(20px)';
	node.style.transition = `opacity 0.6s ease-out ${delay}ms, transform 0.6s ease-out ${delay}ms`;

	const observer = new IntersectionObserver(
		(entries) => {
			const entry = entries[0];
			if (entry?.isIntersecting) {
				node.style.opacity = '1';
				node.style.transform = 'none';
				observer.disconnect();
			}
		},
		{ threshold: 0.1 }
	);
	observer.observe(node);
	return { destroy: () => observer.disconnect() };
};
