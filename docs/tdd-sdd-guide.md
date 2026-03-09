# TDD & SDD Guide for the Starter Template

This document explains how to apply **Test-Driven Development (TDD)** and
**Specification-Driven Development (SDD)** when building new features in this
full-stack .NET 8 / React 18 application.

---

## Why TDD and SDD?

| Practice | What it means here |
|----------|--------------------|
| **TDD** | Write a failing test first, make it pass with the smallest production code change, then refactor. |
| **SDD** | Start from a written specification (OpenAPI schema, acceptance criteria, or a GitHub issue) and translate it directly into tests before writing production code. |

Together they ensure that every line of production code is justified by a
requirement, every requirement is verified by a test, and the resulting codebase
stays easy to change.

---

## Backend (ASP.NET Core 8)

### 1. Start from an API specification

Before writing any C# code, describe the new endpoint in plain language or
as an OpenAPI fragment:

```yaml
# Example: POST /v1/employees
requestBody:
  required: true
  content:
    application/json:
      schema:
        $ref: '#/components/schemas/CreateEmployeeRequest'
responses:
  '201':
    description: Employee created
    content:
      application/json:
        schema:
          $ref: '#/components/schemas/ItemResponseDto_EmployeeDto'
  '400':
    description: Validation error (ORG-VAL-001)
  '500':
    description: Internal server error (ORG-INT-001)
```

This spec becomes the acceptance criteria for all tests you are about to write.

---

### 2. Red → Green → Refactor cycle

```
1. Write one failing test
2. Write the minimum production code to make it green
3. Refactor (clean up duplication, apply standards)
4. Repeat
```

#### Step-by-step example — adding an Employees endpoint

**a) Create the DTO first (no logic yet)**

```csharp
// src/Api/DTOs/Employees/EmployeeDto.cs
/// <summary>Represents an employee in the system.</summary>
public class EmployeeDto
{
    /// <summary>Unique identifier.</summary>
    [JsonPropertyName("employeeId")]
    public int EmployeeId { get; set; }

    /// <summary>Full display name.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Indicates whether the employee is currently active.</summary>
    [JsonPropertyName("activeIndicator")]
    public bool ActiveIndicator { get; set; }
}
```

**b) Write a failing repository test (Red)**

```csharp
// tests/Api.Tests/Repositories/EmployeeRepositoryTests.cs
[Fact]
public async Task GetAllAsync_WithNoFilter_ReturnsAllEmployees()
{
    // Arrange
    _context.Employees.AddRange(
        new Employee { Id = 1, Name = "Alice", ActiveIndicator = true },
        new Employee { Id = 2, Name = "Bob",   ActiveIndicator = false });
    await _context.SaveChangesAsync();

    // Act
    var results = await _repository.GetAllAsync(new EmployeeQuery(), CancellationToken.None);

    // Assert
    Assert.Equal(2, results.Count());
}
```

This test fails to compile — intentionally. The repository interface and
implementation do not exist yet.

**c) Write the minimum production code to pass (Green)**

Create `IEmployeeRepository`, `EmployeeRepository`, and `AppDbContext` with just
enough code to satisfy the test. Do not add extra features at this stage.

**d) Write a failing service test**

```csharp
[Fact]
public async Task GetAllAsync_WhenRepositoryReturnsData_ReturnsMappedEnvelope()
{
    // Arrange
    _mockRepository
        .Setup(r => r.GetAllAsync(It.IsAny<EmployeeQuery>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(new[] { new Employee { Id = 1, Name = "Alice", ActiveIndicator = true } });

    // Act
    var result = await _service.GetAllAsync(new EmployeeQuery(), CancellationToken.None);

    // Assert
    Assert.Single(result.Items);
    Assert.Equal("Alice", result.Items.First().Name);
}
```

**e) Implement the service to make it pass (Green)**

Only add the mapping logic needed to satisfy this test.

**f) Write an integration test for the controller**

```csharp
[Fact]
public async Task GetAll_WhenCalled_ReturnsOkWithEmployeeCollection()
{
    // Arrange & Act
    var response = await _testClient.GetFromJsonAsync<CollectionResponseDto<EmployeeDto>>(
        "/v1/employees");

    // Assert
    Assert.NotNull(response);
    Assert.NotNull(response.Items);
}
```

---

### 3. Test layer responsibilities

| Layer | Framework | Responsibility |
|-------|-----------|----------------|
| **Repository** | xUnit + EF InMemory | SQL/LINQ correctness, filter logic, pagination |
| **Service** | xUnit + Moq | Business logic, mapping, error handling |
| **Controller** | xUnit + `WebApplicationFactory` | HTTP contract, status codes, envelope shape |

**Rule of thumb:** mock one layer at a time. Repository tests never mock the
database; service tests always mock the repository; integration tests verify the
full pipeline with an in-memory database.

---

### 4. Naming conventions (enforced by code review)

```
{MethodName}_{Scenario}_{ExpectedResult}

Examples:
  GetAllAsync_WithActiveFilter_ReturnsOnlyActiveEmployees
  CreateAsync_WhenNameIsEmpty_ThrowsValidationException
  GetByIdAsync_WhenNotFound_ReturnsNull
```

---

### 5. Test organisation checklist

- [ ] Every new source file has a corresponding test file
- [ ] All test classes use `// Arrange` / `// Act` / `// Assert` comments
- [ ] `#region` blocks group tests by method name
- [ ] `TestDataBuilders` has a builder for every new entity/DTO
- [ ] `CustomWebApplicationFactory` replaces the real `DbContext` with in-memory

---

## Frontend (React 18 + TypeScript)

### 1. Start from component acceptance criteria

Before writing any JSX, write a brief spec in plain language:

```
Component: EmployeeList
Given: The API returns a list of employees
When: The component mounts
Then:
  - A loading spinner is shown while the request is in flight
  - Each employee's name is rendered in a table row
  - An error message is shown if the API call fails
  - The total count from metadata is shown in the heading
```

Each "Then" clause becomes a `it(...)` block.

---

### 2. Red → Green → Refactor cycle

**a) Write a failing test (Red)**

```tsx
// src/routes/employees/EmployeeListPage.test.tsx
it('renders a row for each employee returned by the API', async () => {
  // Arrange
  global.fetch = vi.fn().mockResolvedValue({
    ok: true,
    json: vi.fn().mockResolvedValue(buildEmployeeCollectionEnvelope()),
  });

  // Act
  render(<MemoryRouter><EmployeeListPage /></MemoryRouter>);

  // Assert
  await waitFor(() => {
    expect(screen.getByText('Alice')).toBeInTheDocument();
    expect(screen.getByText('Bob')).toBeInTheDocument();
  });
});
```

The test fails immediately because `EmployeeListPage` does not exist.

**b) Create the minimal component (Green)**

Implement just enough JSX to render employee names after a fetch.

**c) Add edge case tests before adding edge case code**

```tsx
it('shows the loading spinner during fetch', () => { ... });
it('shows an error message when the API call rejects', async () => { ... });
it('shows "No employees found" when the list is empty', async () => { ... });
```

Write each test, watch it fail, then add the code.

---

### 3. Test layer responsibilities

| Layer | Framework | Responsibility |
|-------|-----------|----------------|
| **Utility / API client** | Vitest | `executeApiRequest` behaviour, error wrapping |
| **UI components** | Vitest + RTL | Rendering, accessibility, user interaction |
| **Page / route components** | Vitest + RTL | Fetch integration, state transitions, full page rendering |

---

### 4. Mocking conventions

Always mock `fetch` at the test level using `vi.fn()`. Never mock the
`executeApiRequest` helper directly — that would prevent testing the error
wrapping logic.

```ts
// Pattern for a successful API mock
global.fetch = vi.fn().mockResolvedValue({
  ok: true,
  json: vi.fn().mockResolvedValue(buildEnvelope()),
} as unknown as Response);

// Pattern for a failed API mock
global.fetch = vi.fn().mockRejectedValue(new Error('Network error'));

// Pattern for a non-2xx HTTP response
global.fetch = vi.fn().mockResolvedValue({
  ok: false,
  status: 404,
  statusText: 'Not Found',
  text: vi.fn().mockResolvedValue(''),
} as unknown as Response);
```

---

### 5. Test organisation checklist

- [ ] Every new component file has a corresponding `ComponentName.test.tsx`
- [ ] Tests are grouped with `describe` blocks matching the component name
- [ ] Loading, success, and error states are tested for every data-fetching component
- [ ] ARIA roles (`role="alert"`, `role="status"`) are verified for accessibility
- [ ] `vi.resetAllMocks()` is called in `beforeEach` in every test file

---

## Branching and CI strategy

```
feature/<timestamp>-<slug>
  ↳ Write failing tests first (RED commit)
  ↳ Write production code (GREEN commit)
  ↳ Refactor (REFACTOR commit)
  ↳ Open PR → CI runs dotnet test + npm test + coverage check
```

Suggested CI gates:

| Check | Threshold |
|-------|-----------|
| Backend test pass | 100% |
| Frontend test pass | 100% |
| Backend line coverage | ≥ 80% |
| Frontend line coverage | ≥ 80% |

---

## Running tests locally

### Backend

```bash
# Run all tests
cd backend && dotnet test

# Run with coverage report
cd backend && dotnet test --collect:"XPlat Code Coverage"
```

### Frontend

```bash
cd frontend

# Run once (CI mode)
npm test

# Watch mode (TDD loop)
npm run test:watch

# Coverage report
npm run test:coverage
```

---

## Quick reference: what to write before writing production code

| New artifact | Write this first |
|--------------|-----------------|
| Repository method | Repository unit test using EF InMemory |
| Service method | Service unit test using Moq repository |
| Controller endpoint | Integration test via `CustomWebApplicationFactory` |
| React component | RTL test for rendered output |
| React page (data-fetching) | RTL tests for loading / success / error states |
| TypeScript utility | Vitest unit test with mocked `fetch` |
