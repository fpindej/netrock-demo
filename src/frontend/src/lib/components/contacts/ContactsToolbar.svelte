<script lang="ts">
	import { Input } from '$lib/components/ui/input';
	import * as Select from '$lib/components/ui/select';
	import { Search } from '@lucide/svelte';
	import * as m from '$lib/paraglide/messages';

	interface Props {
		searchValue: string;
		onSearch: (value: string) => void;
		sortValue: string;
		onSort: (value: string) => void;
		totalCount: number;
	}

	let { searchValue, onSearch, sortValue, onSort, totalCount }: Props = $props();
</script>

<div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
	<div class="relative max-w-sm flex-1">
		<Search class="absolute start-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
		<Input
			type="search"
			placeholder={m.contacts_search()}
			class="ps-9"
			value={searchValue}
			oninput={(e) => onSearch(e.currentTarget.value)}
		/>
	</div>
	<div class="flex items-center justify-between gap-3 sm:justify-end">
		<p class="text-sm text-muted-foreground">
			{m.contacts_totalCount({ count: totalCount })}
		</p>
		<Select.Root
			type="single"
			value={sortValue}
			onValueChange={(v) => {
				if (v) onSort(v);
			}}
		>
			<Select.Trigger class="w-full sm:w-[180px]">
				{#if sortValue === 'newest'}
					{m.contacts_sortNewest()}
				{:else if sortValue === 'oldest'}
					{m.contacts_sortOldest()}
				{:else if sortValue === 'nameAsc'}
					{m.contacts_sortNameAsc()}
				{:else if sortValue === 'nameDesc'}
					{m.contacts_sortNameDesc()}
				{:else if sortValue === 'valueHigh'}
					{m.contacts_sortValueHigh()}
				{:else if sortValue === 'valueLow'}
					{m.contacts_sortValueLow()}
				{/if}
			</Select.Trigger>
			<Select.Content>
				<Select.Item value="newest" label={m.contacts_sortNewest()} />
				<Select.Item value="oldest" label={m.contacts_sortOldest()} />
				<Select.Item value="nameAsc" label={m.contacts_sortNameAsc()} />
				<Select.Item value="nameDesc" label={m.contacts_sortNameDesc()} />
				<Select.Item value="valueHigh" label={m.contacts_sortValueHigh()} />
				<Select.Item value="valueLow" label={m.contacts_sortValueLow()} />
			</Select.Content>
		</Select.Root>
	</div>
</div>
