# Reflection

1. **Why is it important to standardize error responses in a public-facing API?**
Standardizing error responses keeps the API's contract predictable for every consumer, prevents leaking stack traces or internal details (a real security risk for a financial services API), and lets client teams write one generic error-parsing path instead of one per endpoint.

2. **What's the difference between a custom exception and a generic one (like `Exception`)?**
A generic `Exception` tells you something went wrong but not what or how to react. A custom exception (`NotFoundException`, `ValidationException`) encodes domain intent, so the middleware can deterministically map it to the right HTTP status and message instead of guessing.

3. **How does using Problem Details (RFC 7807) help API consumers?**
RFC 7807 Problem Details gives a machine-readable, self-describing error shape (`type`, `title`, `status`, `detail`, `instance`) that's already understood by many HTTP client libraries and tools, so consumers get structured errors instead of ad-hoc JSON that varies by team or endpoint.
