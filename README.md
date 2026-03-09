# APPWeb

Blazor Server web application built with .NET 8 following Hexagonal Architecture (Ports and Adapters). Consumes the WBAPI REST API to provide a complete CRUD interface for Products with JWT-based authentication.


## Solution structure

```
APPWeb/
├── src/
│   ├── APPWeb.Domain/         # Entities, port interfaces (IProductService, IAuthService)
│   ├── APPWeb.Application/    # Application services, DTOs, API client interfaces
│   ├── APPWeb.Infrastructure/ # HTTP adapters (ProductApiClient, AuthApiClient), AuthService
│   └── APPWeb.Web/            # Blazor Server pages, layouts, Program.cs
└── APPWeb.sln
```


## Design patterns

| Pattern | Where applied |
|---------|---------------|
| Hexagonal Architecture (Ports and Adapters) | All layers communicate through port interfaces defined in Domain |
| Facade / Application Service | `ProductAppService` wraps `IProductApiClient` behind `IProductService` |
| Adapter | `ProductApiClient`, `AuthApiClient`, and `UserApiClient` translate HTTP responses into domain objects |
| State | `AuthService` holds the active JWT token and user role in memory for the server-side session |
| Observer (Event) | `IAuthService.StateChanged` event notifies UI components (e.g. `NavMenu`) of login/logout changes |
| Dependency Injection | `AddApplication()` and `AddInfrastructure()` extension methods wire all dependencies |


## Prerequisites

- .NET 8 SDK (https://dotnet.microsoft.com/download)
- WBAPI running locally. See the WBAPI README for setup instructions.


## Configuration

All settings live in `src/APPWeb.Web/appsettings.json`.

```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7299"
  }
}
```

Settings reference:

| Key | Description | Required |
|-----|-------------|----------|
| `ApiSettings:BaseUrl` | Base URL of the running WBAPI instance. No trailing slash. | Yes |

If WBAPI is running on a different address (for example on a server or different port), update `BaseUrl` to match. The application will not start making API calls until a user logs in.


## Running the application

Start WBAPI first (see WBAPI README), then start APPWeb:

HTTPS profile (recommended):

```
dotnet run --project src/APPWeb.Web --launch-profile https
```

HTTP only profile:

```
dotnet run --project src/APPWeb.Web --launch-profile http
```

The application will be available at:

- HTTPS: https://localhost:7287
- HTTP:  http://localhost:5152

Both applications must be running at the same time for the CRUD operations to work.


## First use

1. Start WBAPI.
2. Register an Admin user by calling `POST /api/auth/register` with `"role": "Admin"` (via Swagger or Postman). This initial admin account cannot be created from the UI.
3. Start APPWeb.
4. Navigate to `https://localhost:7287/login` and sign in with the Admin credentials.
5. Navigate to `/products` to manage the product catalog.
6. Navigate to `/users` to manage user accounts and assign roles.

Note: Self-registration via `/register` creates accounts with the `Invitado` role (read-only access). An Admin must upgrade the role via the Users page.


## Available pages

| Route | Description | Auth required |
|-------|-------------|---------------|
| `/` | Home page — shows role info when authenticated, access notice for Invitado | No |
| `/login` | Login form. Redirects to `/products` (User/Admin) or `/` (Invitado) after signing in | No |
| `/register` | Self-registration form. Creates an account with `Invitado` role | No |
| `/products` | Product list with delete confirmation | Yes |
| `/products/create` | Create product form | Yes (Admin) |
| `/products/edit/{id}` | Edit product form pre-populated from API | Yes (Admin) |
| `/products/{id}` | Product detail view | Yes |
| `/users` | User management — list all users, change roles, delete accounts | Yes (Admin) |

Role permissions summary:

| Role | Products (read) | Products (write) | User management | Registration |
|------|----------------|-----------------|-----------------|-------------|
| Admin | ✅ | ✅ | ✅ | N/A |
| User | ✅ | ❌ | ❌ | N/A |
| Invitado | ❌ (home page notice) | ❌ | ❌ | Self-registration |

Attempting to access protected pages while unauthenticated will redirect to `/login`.


## CORS requirement

WBAPI is configured to accept requests only from `https://localhost:7287` and `http://localhost:5152`. If you change the port APPWeb runs on, you must also update the `WithOrigins` call in `WBAPI/src/WBAPI.API/Program.cs` and restart the API.


## Git workflow

### Branch model

| Branch | Purpose | Merge strategy |
|--------|---------|----------------|
| `main` | Production. Tagged with semver on every release. Protected — no direct pushes. | Merge commit from `qa` |
| `qa` | Staging / QA sign-off. Receives only pre-validated code from `dev`. | Merge commit from `dev` |
| `dev` | Integration branch. All feature branches merge here first. | Squash merge from feature branches |
| `feature/*` | One branch per feature or task, always branched from `dev`. | Squash merged into `dev` via PR |
| `fix/*` | Non-production bug fixes, branched from `dev`. | Squash merged into `dev` via PR |
| `hotfix/*` | Critical production fixes, branched from `main`. | Merge commit into `main` + `dev` |
| `chore/*` | Dependency upgrades, CI changes, non-functional work. | Squash merged into `dev` via PR |

### Standard feature flow

```
# 1 — start from dev
git checkout dev
git pull origin dev
git checkout -b feature/my-feature

# 2 — develop, commit with conventional commit messages
git add -A
git commit -m "feat: describe what this does"

# 3 — push and open PR into dev
git push -u origin feature/my-feature
gh pr create --base dev --head feature/my-feature --title "feat: my feature" --body "..."
gh pr merge --squash --delete-branch

# 4 — promote dev → qa
gh pr create --base qa --head dev --title "promote: dev → qa vX.Y.Z" --body "..."
gh pr merge --merge

# 5 — promote qa → main (release)
gh pr create --base main --head qa --title "release: vX.Y.Z" --body "..."
gh pr merge --merge
git tag vX.Y.Z && git push origin vX.Y.Z
```

### Hotfix flow

```
git checkout main
git pull origin main
git checkout -b hotfix/critical-bug-description

# fix, commit
git commit -m "fix: describe critical fix"
git push -u origin hotfix/critical-bug-description

# PR hotfix → main
gh pr create --base main --head hotfix/... --title "hotfix: ..."
gh pr merge --merge
git tag vX.Y.Z-patch && git push origin vX.Y.Z-patch

# Back-merge into dev so the fix is not lost
git checkout dev && git merge main && git push origin dev
```

### Commit message conventions

Follows [Conventional Commits](https://www.conventionalcommits.org/):

| Prefix | When to use |
|--------|-------------|
| `feat:` | New feature |
| `fix:` | Bug fix |
| `test:` | Adding or updating tests |
| `docs:` | Documentation only |
| `refactor:` | Code restructure without behaviour change |
| `chore:` | Tooling, dependencies, CI/CD |

### Version tags

Tags follow [Semantic Versioning](https://semver.org/) (`vMAJOR.MINOR.PATCH`) and are created on `main` only after a release merge.
