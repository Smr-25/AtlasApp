import * as React from "react";
import { useState, useEffect } from "react";
import { Input } from "@/components/ui/input";

export type Country = { code: string; dial: string; label: string; emoji?: string };

const COUNTRIES: Country[] = [
	{ code: 'AZ', dial: '+994', label: 'Azerbaijan', emoji: '🇦🇿' },
	{ code: 'US', dial: '+1', label: 'United States', emoji: '🇺🇸' },
	{ code: 'GB', dial: '+44', label: 'United Kingdom', emoji: '🇬🇧' },
	{ code: 'DE', dial: '+49', label: 'Germany', emoji: '🇩🇪' },
	{ code: 'IN', dial: '+91', label: 'India', emoji: '🇮🇳' },
	{ code: 'RU', dial: '+7', label: 'Russia', emoji: '🇷🇺' },
];

const GROUPS: Record<string, number[]> = {
	AZ: [2, 3, 2, 2], // 50 123 45 67
	US: [3, 3, 4], // 123 456 7890
	GB: [4, 3, 4],
	DE: [3, 3, 4],
	IN: [5, 5],
	RU: [3, 3, 2, 2],
};

function formatWithGroups(digits: string, groups: number[]) {
	if (!groups || groups.length === 0) return digits;
	const parts: string[] = [];
	let idx = 0;
	for (const g of groups) {
		if (idx >= digits.length) break;
		parts.push(digits.substr(idx, g));
		idx += g;
	}
	if (idx < digits.length) parts.push(digits.substr(idx));
	return parts.join(' ');
}

interface PhoneInputProps {
	value?: string;
	onChange?: (value: string) => void;
	placeholder?: string;
}

export default function PhoneInput({ value = '', onChange, placeholder }: PhoneInputProps) {
	// value is full E.164 like +994501234567
	const [country, setCountry] = useState<Country>(COUNTRIES[0]);
	const [local, setLocal] = useState<string>("");

	useEffect(() => {
		if (!value) return;
		// try parse leading dial
		const found = COUNTRIES.find(c => value.startsWith(c.dial));
		if (found) {
			setCountry(found);
			setLocal(value.slice(found.dial.length));
		} else {
			// fallback: set as raw
			setLocal(value.replace(/^\+/, ''));
		}
	}, [value]);

	useEffect(() => {
		const full = (country?.dial || '') + (local || '');
		onChange?.(full);
	}, [country, local]);

	return (
		<div className="flex gap-2 items-center">
			{/* country select */}
			<div className="flex items-center rounded-md border bg-card overflow-hidden">
				<select
					aria-label="Country"
					value={country.code}
					onChange={(e) => {
						const c = COUNTRIES.find(x => x.code === e.target.value) || COUNTRIES[0];
						setCountry(c);
						setLocal('');
					}}
					className="bg-transparent px-3 py-2 text-sm"
				>
					{COUNTRIES.map(c => (
						<option key={c.code} value={c.code}>{c.emoji ? `${c.emoji} ` : ''}{c.dial}</option>
					))}
				</select>
			</div>

			<div className="flex-1">
				<div className="flex items-center gap-2">
					<div className="px-3 py-2 rounded-l-md bg-muted text-sm flex items-center gap-2">
						<span>{country.emoji}</span>
						<span className="font-mono">{country.dial}</span>
					</div>
					<Input
						placeholder={placeholder || "501234567"}
						value={
							formatWithGroups(
								local,
								GROUPS[country.code] || [3, 3, 3]
							) || ""
						}
						onChange={(e) => {
							const v = e.target.value.replace(/[^0-9]/g, "");
							const maxLength = (GROUPS[country.code] || [9]).reduce((s, n) => s + n, 0);
							const limited = v.slice(0, maxLength);
							setLocal(limited);
						}}
						aria-label="Phone number"
						className="rounded-r-md"
					/>
				</div>
			</div>
		</div>
	);
}
