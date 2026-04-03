---
trigger: always_on
---

# 🎬 PRODUCTION RULE: UX/UI & ACCESSIBILITY

## @agent-directive: Pragmatic Realism (Everyday IT Dashboard)

- **CONSTRAINT:** Reject generic layouts. Design high-density, highly readable data grids tailored for desktop utility. It must look like a realistic application used by IT professionals.
- **REQUIREMENT:** Follow Jakob's Law: utilize standard, expected UI patterns for data management (e.g., sticky headers for tables, visible quick-filters, bulk-action checkboxes).

## @agent-directive: Tailwind CSS Best Practices

- **CONSTRAINT:** Do not clutter JSX with massive Tailwind strings. Use a utility library like `clsx` or `tailwind-merge` to handle conditional classes cleanly.
- **REQUIREMENT:** Apply the Class Variance Authority (CVA) pattern for building reusable, strongly-typed UI components (e.g., standardizing Button sizes and variants).

## @agent-directive: Accessibility (WCAG 2.1 AA)

- **REQUIREMENT:** The interface MUST be fully navigable via keyboard. Ensure all interactive elements have visible `:focus-visible` rings.
- **REQUIREMENT:** Enforce strict color contrast ratios for text and use semantic HTML/ARIA attributes for data tables and modals.
