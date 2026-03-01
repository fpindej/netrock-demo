# Security — Not an Afterthought

> Back to [README](../README.md)

NETrock is built **security-first**. Every decision defaults to the most restrictive option, then selectively opens what's needed.

## Authentication & Session Security

- **JWT in HttpOnly cookies** — tokens never touch JavaScript, immune to XSS theft
- **Refresh token rotation** — single-use tokens with automatic family revocation on reuse detection (stolen token → all sessions invalidated)
- **Security stamp validation** — permission changes propagate to active sessions via SHA-256 hashed stamps in JWT claims, cached in Redis for performance
- **Soft refresh** — role/permission changes invalidate access tokens but preserve refresh tokens, so users silently re-authenticate instead of getting force-logged-out
- **Remember me** — persistent refresh tokens with configurable expiry, non-persistent sessions cleared on browser close

### Two-Factor Authentication (2FA)

- **TOTP via ASP.NET Identity** — RFC 6238 time-based one-time passwords, compatible with any authenticator app (Google Authenticator, Authy, 1Password, etc.)
- **Challenge token flow** — on login with 2FA enabled, the server returns a 32 crypto-random-byte challenge token (SHA-256 hashed in DB) with a 5-minute TTL and single-use enforcement
- **Brute-force protection** — max 5 failed verification attempts per challenge token before automatic invalidation
- **Recovery codes** — 10 single-use recovery codes generated at 2FA setup, each usable as a fallback when the authenticator app is unavailable
- **Cleanup job** — Hangfire recurring job purges expired challenge tokens to prevent unbounded table growth

## Authorization & Access Control

- **Permission-based authorization** — atomic permissions (`users.view`, `users.manage`, `roles.manage`, …) enforced on every endpoint via `[RequirePermission]`
- **Role hierarchy protection** — SuperAdmin > Admin > User, with privilege escalation prevention (can't assign roles at or above your own rank)
- **Self-protection rules** — can't lock your own account, can't delete yourself, can't remove your own roles
- **System role guards** — SuperAdmin/Admin/User cannot be deleted or renamed, SuperAdmin permissions are implicit (never stored in DB)
- **Frontend mirrors backend** — route guards, nav filtering, and conditional rendering use the same permission claims, but the backend is always authoritative

## Transport & Headers

- **CORS production safeguard** — startup guard rejects `AllowAllOrigins` in non-development environments
- **CSP with nonce mode** — script-src locked down, Turnstile CAPTCHA whitelisted explicitly
- **Security headers on every response** — `X-Content-Type-Options: nosniff`, `X-Frame-Options: DENY`, `Referrer-Policy`, `Permissions-Policy`, HSTS in production
- **CSRF protection** — Origin header validation in the SvelteKit API proxy for all state-changing requests

## Rate Limiting & Input Validation

- **Rate limiting** — global + per-endpoint policies (registration has stricter limits), configurable per environment, with IP and user partitioning
- **Input validation everywhere** — FluentValidation on backend (even if frontend already validates), Data Annotations flowing into OpenAPI spec

## Data Protection

- **Soft delete** — nothing is ever truly gone, every mutation tracked with who/when audit fields
- **Audit trail** — automatic `CreatedAt/By`, `UpdatedAt/By`, `DeletedAt/By` on every entity via EF Core interceptor
- **Dev config stripping** — `appsettings.Development.json` and `appsettings.Testing.json` excluded from production Docker images

## Reporting a Vulnerability

Found a security issue? Please report it privately — see [SECURITY.md](../SECURITY.md) for the full disclosure policy.
