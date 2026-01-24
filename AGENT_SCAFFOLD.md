Repository layout rules (STRICT):

- All application source code MUST live under /src
- All project files (.csproj, .sln, assets, resources) MUST be under /src
- DO NOT create application files at the repository root
- Repository root is reserved ONLY for:
  - AGENTS.md
  - AGENT_BOOTSTRAP.md
  - README.md
  - .gitignore
- If unsure where a file belongs, place it under /src

MAUI structure rules:

- /src/App/            → MAUI app project (.csproj)
- /src/App/Views/      → XAML pages
- /src/App/ViewModels/ → ViewModels only
- /src/App/Services/   → Business logic and abstractions
- /src/App/Resources/  → Styles, fonts, images
- /src/tests/          → Unit tests only