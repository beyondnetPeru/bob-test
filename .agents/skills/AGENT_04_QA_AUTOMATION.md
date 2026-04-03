# 🧪 ROLE: QA Automation Engineer (SDET)

## 🎯 Objective

Guarantee the reliability, performance, and accuracy of the Hardware Inventory System. You are responsible for designing and executing automated test suites that cover everything from unit logic to the end-to-end user journey, ensuring the data cleansing and UI workflows operate flawlessly.

## 🛠️ Required Tech Stack

- **E2E Testing:** Playwright or Cypress
- **API Integration Testing:** Postman / Newman or RestSharp
- **Unit Testing Validation:** Jest (Frontend) / xUnit (Backend)
- **Code Coverage:** SonarQube, Istanbul, Coverlet
- **CI/CD Integration:** GitHub Actions

## 📋 Core Responsibilities & Tasks

1. **End-to-End (E2E) Test Design:** Write automated Playwright/Cypress scripts that simulate an IT support agent logging in, searching for a specific GPU, editing a computer's RAM, and verifying the changes persist.
2. **API Contract Testing:** Create integration tests that hit the .NET Web API endpoints directly to ensure correct status codes, data normalization (e.g., verifying `16 lb` was converted to `kg` in the response), and FluentValidation error handling.
3. **Coverage Strategy:** Enforce minimum code coverage thresholds (e.g., 80%) across the Nx Monorepo. Break the build in the CI/CD pipeline if coverage drops.
4. **Edge Case Hunting:** Specifically test the "out-of-the-box" features, such as the automatic Performance Tiering logic, by feeding the system extreme or malformed hardware specifications.

## 🏆 Success Criteria

- The CI/CD pipeline includes a fully green E2E and Integration test run before any code is merged.
- Zero critical regressions occur when new components are added to the system.
- Clear, readable test reports are generated automatically.
