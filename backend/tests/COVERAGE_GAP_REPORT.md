# Coverage Gap Report

Generated for: `backend/src/Api/`  
Test project: `backend/tests/Api.Tests/`

---

## Summary

- **Total source classes:** 6 (HealthController, TransactionIdMiddleware, TransactionIdMiddlewareExtensions, ServiceCollectionExtensions + 7 DTO data classes)
- **Classes with meaningful tests:** 1 (HealthController — integration only)
- **Classes without tests:** 5
- **Estimated current coverage:** ~30% (integration tests exercise the HTTP pipeline end-to-end, but all unit-level branches and paths are untested)

---

## Coverage Matrix

| Layer | Class | Method | Has Test? | Priority | Notes |
|-------|-------|--------|-----------|----------|-------|
| Controller | HealthController | GetHealth | ⚠️ | High | 6 integration tests only — no unit tests; `TransactionId` fallback path, env/version/links untested in isolation |
| Middleware | TransactionIdMiddleware | InvokeAsync | ❌ | High | No unit tests at all; `Items` injection, `OnStarting` header, uniqueness all untested |
| Middleware | TransactionIdMiddlewareExtensions | UseTransactionId | ❌ | Low | Trivial one-liner wrapping `UseMiddleware<T>`; covered implicitly by integration tests |
| Extension | ServiceCollectionExtensions | AddApplicationServices | ❌ | Medium | No tests; currently a stub — should verify contract (returns same collection, no throw) |
| DTO | ItemResponseDto\<T\> | — | N/A | Low | Pure data class, no logic |
| DTO | CollectionResponseDto\<T\> | — | N/A | Low | Pure data class, no logic |
| DTO | MetadataDto | — | N/A | Low | Pure data class, no logic |
| DTO | LinksDto | — | N/A | Low | Pure data class, no logic |
| DTO | ErrorResponseDto / ErrorDetailDto | — | N/A | Low | Pure data class, no logic |
| DTO | HealthResponseDto | — | N/A | Low | Pure data class, no logic |
| Bootstrap | Program.cs | — | ⚠️ | Low | Covered implicitly by existing integration tests |

### Uncovered Logic Branches Inside Already-Integrated Methods

| Class | Method | Uncovered Branch | Priority |
|-------|--------|-----------------|----------|
| HealthController | GetHealth | `HttpContext.Items["TransactionId"]` is null → fallback `Guid.NewGuid()` | High |
| HealthController | GetHealth | Assembly version is null → fallback `"1.0.0"` | Low |

---

## Execution Plan (ordered by priority)

1. **`Tests/Middleware/TransactionIdMiddlewareTests.cs`** — unit-test `InvokeAsync`: Items injection, `OnStarting` header callback (using `TestHttpResponseFeature` helper), uniqueness across calls, next-delegate invocation
2. **`Tests/Controllers/HealthControllerUnitTests.cs`** — unit-test `GetHealth`: envelope shape, environment from mocked `IWebHostEnvironment`, TransactionId sourced from `HttpContext.Items`, TransactionId fallback to new GUID, `Links.Self` construction, null fields on `Metadata` and `Links`
3. **`Tests/Extensions/ServiceCollectionExtensionsTests.cs`** — contract tests for `AddApplicationServices`: returns same `IServiceCollection` instance, does not throw
4. **`Utils/TestDataBuilders.cs`** — add `BuildItemResponse<T>` and `BuildCollectionResponse<T>` envelope-wrapping helpers

---

## Results

<!-- Appended after test run -->

### Tests Added

| Layer | File | Tests |
|-------|------|-------|
| Middleware | `Tests/Middleware/TransactionIdMiddlewareTests.cs` | 7 |
| Controller (Unit) | `Tests/Controllers/HealthControllerUnitTests.cs` | 12 |
| Extension | `Tests/Extensions/ServiceCollectionExtensionsTests.cs` | 2 |
| Utility update | `Utils/TestDataBuilders.cs` | N/A (helpers) |
| **Total** | **3 new files + 1 updated** | **21 new tests** |

### Coverage Estimate

| Area | Before | After |
|------|--------|-------|
| TransactionIdMiddleware | 0% | ~95% (all branches covered; `UseTransactionId` extension remains integration-only) |
| HealthController | ~60% (integration) | ~100% (both TransactionId paths, env mock, links, metadata) |
| ServiceCollectionExtensions | 0% | 100% |
| **Overall estimate** | ~30% | **~85%** |

### Files Created / Modified

| Operation | Path |
|-----------|------|
| Created | `tests/Api.Tests/Middleware/TransactionIdMiddlewareTests.cs` |
| Created | `tests/Api.Tests/Controllers/HealthControllerUnitTests.cs` |
| Created | `tests/Api.Tests/Extensions/ServiceCollectionExtensionsTests.cs` |
| Modified | `tests/Api.Tests/Utils/TestDataBuilders.cs` |
| Created | `tests/COVERAGE_GAP_REPORT.md` |

### Notes

- No production code was modified.
- `TransactionIdMiddlewareExtensions.UseTransactionId` is a one-line call to `UseMiddleware<T>` and is adequately covered by the existing integration tests; a dedicated unit test would only test the framework itself.
- The `OnStarting` callback cannot be fired by `DefaultHttpContext` without a real server pipeline. A private `TestHttpResponseFeature` helper class (nested inside the test class) was used to capture and fire those callbacks deterministically.
- Assembly version in test environments resolves to `"1.0.0.0"` (set by the SDK default); the version-is-null fallback path (`?? "1.0.0"`) cannot be exercised without IL manipulation and is excluded by design.
