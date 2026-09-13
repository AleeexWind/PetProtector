# Typical tasks
Concise prompts and expected response formats for three common coding tasks.
Use the prompt as-is, filling in placeholders. The response format is what the assignee should return after the work is done.
---
## 1. Module coverage with tests
Raise test coverage for a named module. Do not change production behavior unless a change is required to make the code testable, and call that out.
### Prompt example
```
Add tests for `src/pricing/discount.py`.
Scope:
- Cover public functions in this module only.
- Do not change production code unless a seam is required for tests. If you must change it, keep behavior identical and list the change.
- Follow existing test layout and style in `tests/`.
Targets:
- Line coverage for this module ≥ 90%.
- Include happy path, edge cases, and error/invalid input.
Constraints:
- No new dependencies.
- Do not add tests for unrelated modules.
Return:
- Files added/changed
- Coverage before → after for this module
- Cases still uncovered, if any
```
### Expected response format
```
## Coverage
- Module: `src/pricing/discount.py`
- Before: 42% lines / 3 of 8 branches
- After: 94% lines / 7 of 8 branches
- Remaining: branch in `apply_coupon` when coupon is expired (needs clock injection)
## Changes
- Added: `tests/pricing/test_discount.py`
- Production: none
## Tests
- `test_percentage_discount_applies`
- `test_zero_and_negative_amount_rejected`
- `test_unknown_coupon_raises`
- `test_stacking_disabled_by_default`
## Verification
- Command: `pytest tests/pricing/test_discount.py --cov=src.pricing.discount`
- Result: pass
```
---
## 2. Bug fix according to the description
Fix the reported bug and nothing else. Match the described expected behavior. Add or update a regression test that fails before the fix and passes after.
### Prompt example
```
Fix: checkout total ignores a 10% coupon when the cart has only one item.
Description:
- Actual: coupon is skipped for single-item carts; total stays at full price.
- Expected: coupon applies to the cart subtotal regardless of item count.
- Repro: add 1 item ($20), apply coupon `SAVE10`, pay → charged $20 instead of $18.
Constraints:
- Change only code needed to fix this bug.
- Do not refactor unrelated code.
- Add a regression test for the single-item + coupon case.
Return:
- Root cause (1–3 sentences)
- Files changed
- Regression test name
- How you verified the fix
```
### Expected response format
```
## Root cause
`apply_coupon()` returned early when `len(items) < 2`, so single-item carts never reached discount logic.
## Fix
- `src/checkout/coupon.py`: removed the item-count guard; coupon now applies to subtotal.
## Tests
- Added `test_single_item_cart_applies_percent_coupon`
- Fails on old code, passes after the fix
## Verification
- Command: `pytest tests/checkout/test_coupon.py`
- Result: pass
- Manual repro: 1 × $20 + `SAVE10` → $18
```
---
## 3. Refactoring without changing behavior
Restructure code for clarity or maintainability. Public behavior, APIs, and outputs must stay the same. Existing tests should pass without rewriting assertions to match a new contract.
### Prompt example
```
Refactor `src/orders/status_machine.py` without changing behavior.
Goal:
- Replace the nested if/elif status transitions with an explicit transition table.
- Keep the public function signatures and return values unchanged.
Constraints:
- No feature work, no bug fixes, no API changes.
- Do not update tests except imports/paths if you move files.
- Existing tests in `tests/orders/` must pass unchanged.
Return:
- What changed structurally
- What did not change (API, behavior)
- Test command and result
```
### Expected response format
```
## Refactor
- `src/orders/status_machine.py`: nested conditionals replaced with `TRANSITIONS` dict and a single lookup.
- Extracted `_ensure_allowed(current, next)` for the same validation rules as before.
## Unchanged
- Public API: `transition(order, next_status)`, `can_transition(...)`
- Status names, error types, and messages
- No test assertion changes
## Verification
- Command: `pytest tests/orders/`
- Result: pass (same 14 tests as before)
```