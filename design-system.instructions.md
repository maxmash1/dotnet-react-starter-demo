---
description: "Use when: styling, theming, UI components, design tokens, color palette, buttons, forms, inputs, cards, layout, typography, auth pages, login/signup pages, CSS, Tailwind classes, dark mode, responsive design, animations, gradients, component library setup."
---

# Design System — Agent Hub

This document defines the cohesive design system extracted from the Agent Hub application. Apply these conventions to all new apps to maintain visual consistency across the product suite.

---

## Stack

- **CSS Framework**: Tailwind CSS v4 (via `@tailwindcss/postcss`)
- **Component Library**: @radix-ui/themes `^3.3.0` (dark appearance, violet accent)
- **Icons**: @radix-ui/react-icons `^1.3.2`
- **Font**: Inter (Google Fonts, loaded via `next/font/google`, CSS variable `--font-inter`)
- **No custom tailwind.config** — uses Tailwind v4 defaults with inline `@theme` overrides

### Global CSS

```css
@import "tailwindcss";

@theme inline {
  --font-sans: var(--font-inter);
}
```

### Root Layout Theme

```tsx
<body className="min-h-screen bg-gray-950 text-white antialiased">
  <Theme appearance="dark" accentColor="violet" radius="large">
```

---

## Color Palette

### Base / Neutral

| Role | Token | Hex (approx) |
|------|-------|---------------|
| Page background | `bg-gray-950` | #030712 |
| Surface / card bg | `bg-gray-900/50` | semi-transparent |
| Input bg | `bg-gray-800/50` | semi-transparent |
| Primary text | `text-white` | #ffffff |
| Secondary text | `text-gray-400` | #9ca3af |
| Tertiary text | `text-gray-500` | #6b7280 |
| Borders | `border-gray-800` | #1f2937 |
| Subtle borders | `border-white/10` | white @ 10% |
| Hover surfaces | `hover:bg-gray-900` | — |

### Primary Accent — Violet

| Role | Token |
|------|-------|
| Button / CTA bg | `bg-violet-600` |
| Button hover | `hover:bg-violet-500` |
| Focus ring | `focus:ring-violet-500` |
| Focus border | `focus:border-violet-500` |
| Gradient start | `from-violet-400` |
| Decorative glow | `bg-violet-600/20` |
| Badge / ping dot | `bg-violet-400` / `bg-violet-500` |
| Card hover border | `hover:border-violet-500/40` |

### Secondary Accent — Fuchsia

| Role | Token |
|------|-------|
| Gradient end | `to-fuchsia-400` |
| Decorative glow | `bg-fuchsia-600/20` |

### Status Colors

| Status | Text | Background |
|--------|------|------------|
| Success / Output | `text-green-400` | — |
| Error | `text-red-400` | `bg-red-900/20`, `border-red-800/50` |
| Warning / Executing | `text-yellow-400` | — |
| Info / Generating | `text-blue-400` | — |

---

## Typography

### Font

- **Family**: Inter (sans-serif)
- **Rendering**: `antialiased`

### Scale

| Use | Classes |
|-----|---------|
| Hero headline | `text-5xl sm:text-7xl font-bold tracking-tight` |
| Section heading | `text-3xl font-bold tracking-tight` |
| Card heading | `text-lg font-semibold` / `text-xl font-semibold` |
| Body | `text-base` / `text-sm` |
| UI label / metadata | `text-xs text-gray-400` |
| Section label | `text-sm font-semibold tracking-wider text-violet-400 uppercase` |
| Paragraph | `text-lg leading-8 text-gray-400` |

### Weights

- `font-bold` — headlines, hero
- `font-semibold` — subheadings, card titles, labels
- `font-medium` — buttons, form labels

---

## Buttons

### Radix Themed Buttons

```tsx
// Primary CTA (large)
<Button asChild size="3" variant="solid">

// Secondary CTA (large)
<Button asChild size="3" variant="outline" color="gray">

// Utility / toolbar (small, ghost)
<Button variant="ghost" size="1">
```

Radix sizes: `"1"` (small), `"2"` (medium), `"3"` (large)
Variants: `solid`, `outline`, `ghost`

### Custom Tailwind Buttons (auth forms)

```tsx
<button className="w-full rounded-md bg-violet-600 px-4 py-2 text-sm font-medium text-white hover:bg-violet-500 focus:outline-none focus:ring-2 focus:ring-violet-500 focus:ring-offset-2 focus:ring-offset-gray-950 disabled:opacity-50">
```

Key patterns:
- `w-full` for form-width buttons
- `rounded-md` border radius
- Focus: `focus:ring-2 focus:ring-violet-500 focus:ring-offset-2 focus:ring-offset-gray-950`
- Disabled: `disabled:opacity-50`

---

## Form Inputs

### Text Inputs

```tsx
<input className="mt-1 block w-full rounded-md border border-gray-700 bg-gray-800/50 px-3 py-2 text-white placeholder-gray-500 shadow-sm focus:border-violet-500 focus:outline-none focus:ring-1 focus:ring-violet-500/20" />
```

### Text Area

```tsx
<textarea className="w-full resize-none rounded-lg border border-gray-700 bg-gray-900 px-4 py-3 text-sm text-gray-100 placeholder-gray-500 focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500" />
```

### Labels

```tsx
<label className="block text-sm font-medium text-gray-300">
```

---

## Cards & Containers

### Auth Page Card

```tsx
<div className="relative w-full max-w-md space-y-8 rounded-xl border border-gray-800 bg-gray-900/50 p-8 backdrop-blur-sm">
```

### Feature Card (interactive)

```tsx
<div className="group rounded-2xl border border-white/10 bg-white/5 p-6 transition-colors hover:border-violet-500/40 hover:bg-white/[0.07]">
```

### Icon Container (inside cards)

```tsx
<div className="mb-4 flex h-12 w-12 items-center justify-center rounded-lg bg-violet-600/10">
  <Icon className="h-6 w-6 text-violet-400" />
</div>
```

---

## Error / Alert

```tsx
<div className="rounded-md border border-red-800/50 bg-red-900/20 p-3 text-sm text-red-400">
```

---

## Layout Conventions

### Page Sections

```tsx
<section className="relative py-24 sm:py-32">
  <div className="mx-auto max-w-7xl px-6">
```

- Hero max-width: `max-w-4xl`
- Content max-width: `max-w-7xl`
- Standard horizontal padding: `px-6`
- Section vertical padding: `py-24 sm:py-32`

### Grid

```tsx
<div className="mt-16 grid gap-6 sm:grid-cols-2 lg:grid-cols-4">
```

### Flex Patterns

- Centering: `flex items-center justify-center`
- Column layout: `flex flex-col`
- Row layout: `flex flex-row` / `flex items-center gap-4`
- Responsive stack: `flex flex-col sm:flex-row`
- Gaps: `gap-2`, `gap-3`, `gap-4`, `gap-6`, `gap-8`

### Auth Page Layout

```tsx
<div className="flex min-h-screen items-center justify-center px-4">
  {/* Decorative background orbs (absolute positioned, pointer-events-none) */}
  <div className="relative w-full max-w-md space-y-8 rounded-xl border border-gray-800 bg-gray-900/50 p-8 backdrop-blur-sm">
    {/* Form content */}
  </div>
</div>
```

---

## Decorative Effects

### Background Glow Orbs

```tsx
<div className="pointer-events-none absolute inset-0 overflow-hidden">
  <div className="absolute -top-40 -right-40 h-80 w-80 rounded-full bg-violet-600/20 blur-3xl" />
  <div className="absolute -bottom-40 -left-40 h-80 w-80 rounded-full bg-fuchsia-600/20 blur-3xl" />
</div>
```

### Gradient Text

```tsx
<span className="bg-gradient-to-r from-violet-400 to-fuchsia-400 bg-clip-text text-transparent">
```

---

## Animations

| Effect | Classes |
|--------|---------|
| Pulsing dot | `animate-ping` on overlay + static dot beneath |
| Loading spinner | `animate-spin` |
| Hover transitions | `transition-colors` |

---

## Border Radius Scale

| Use | Class |
|-----|-------|
| Inputs, buttons, alerts | `rounded-md` |
| Text areas, icon containers, code panels | `rounded-lg` |
| Auth cards | `rounded-xl` |
| Feature cards | `rounded-2xl` |
| Badges, dots, decorative orbs | `rounded-full` |

---

## Code Editor / Monospace

```tsx
<div className="font-mono text-sm leading-6">
```

Syntax colors:
- Keywords: `text-purple-400`
- Strings: `text-green-400`
- Comments: `text-gray-500 italic`
- Line numbers: `text-gray-600`
- Default code text: `text-gray-200`

---

## Spacing Quick Reference

| Context | Padding |
|---------|---------|
| Card inner | `p-6` or `p-8` |
| Input inner | `px-3 py-2` |
| Textarea inner | `px-4 py-3` |
| Section | `py-24 sm:py-32` |
| Page horizontal | `px-6` |
| Auth card | `p-8` |
| Compact alerts | `p-3` |

---

## Responsive Breakpoints

Mobile-first approach using Tailwind defaults:
- `sm:` — 640px (2-col grids, larger hero text, horizontal nav)
- `md:` — 768px
- `lg:` — 1024px (4-col grids, sidebar layouts)
