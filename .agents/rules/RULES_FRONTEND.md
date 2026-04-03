---
trigger: always_on
---

# 🎬 PRODUCTION RULE: FRONTEND (React, Vite, RTK, Tailwind)

## @agent-directive: Architecture & State Management

- **CONSTRAINT:** Follow the Rules of Hooks strictly. Class components are forbidden.
- **REQUIREMENT:** Use **RTK Query** exclusively for server-state fetching and caching. Do NOT use `useEffect` for API calls. Reserve Redux slices strictly for global client-side state.
- **REQUIREMENT:** Enforce the Single Responsibility Principle (SRP) for components. If a component exceeds 150 lines, extract its logic into custom hooks or break it down into smaller sub-components.

## @agent-directive: TypeScript & Safety

- **CONSTRAINT:** Enable `"strict": true`. The `any` type is strictly forbidden. Use `unknown` with type guards.
- **REQUIREMENT:** Utilize Vite's absolute path aliasing (e.g., `import Button from '@/components/Button'`) to avoid fragile relative imports.

## @agent-directive: Performance & Clean Code

- **REQUIREMENT:** Implement code-splitting using `React.lazy` and `Suspense` for route-level components to minimize the initial JS payload.
- **REQUIREMENT:** Optimize for Core Web Vitals. Use `React.memo`, `useMemo`, and `useCallback` only when profiling indicates a re-render bottleneck in large data grids.
- **CONSTRAINT:** Keep JSX clean. Extract complex conditional rendering logic into separate functions or discrete components.
