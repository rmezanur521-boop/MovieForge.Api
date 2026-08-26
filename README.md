# MovieForge API

Backend service for **MovieForge** — a fake movie store showcase. Generates realistic, reproducible, localized movie data (titles, cast, genres, reviews, likes, and trailer specs) entirely on the server, on demand, with no database.

## Tech Stack

- **.NET 8** (ASP.NET Core Web API)
- **Bogus** — seeded fake-data generation
- **Swashbuckle / Swagger** — API documentation (Development only)

## Features

- Deterministic, seed-based data generation — the same seed always produces the same movies
- Multi-locale support (`en-US`, `bn-BD`) driven entirely by an external JSON lookup file (`Locales/locales.json`) — no hardcoded region data in source code
- Probabilistic likes/reviews generation supporting fractional averages (e.g. `avgLikes=3.7`)
- Parameter independence: changing likes/reviews does not regenerate title, cast, genre, or trailer data
- Seeded trailer specs (animation style, camera movement, color palette, light-ray pattern) consumed by the frontend canvas renderer
- CORS-enabled for the MovieForge frontend

## Project Structure

```
MovieForge.Api/
├── Controllers/
│   └── MoviesController.cs      # GET /api/movies
├── Services/
│   ├── IMovieGeneratorService.cs
│   └── MovieGeneratorService.cs # core generation logic
├── Models/                      # Movie, ReviewItem, TrailerSpec, etc.
├── Locales/
│   └── locales.json             # all region-specific content
└── Program.cs
```

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)

### Run locally
```bash
git clone <this-repo-url>
cd MovieForge.Api
dotnet restore
dotnet run
```

The API will start at `http://localhost:5203` (see `Properties/launchSettings.json`). Swagger UI is available at `/swagger` in Development mode.

### Configuration
Set the frontend origin allowed by CORS via `FrontendUrl` in `appsettings.json` (defaults to `http://localhost:5173`).

## API

### `GET /api/movies`

| Query Param | Type   | Default | Description                          |
|-------------|--------|---------|---------------------------------------|
| `seed`      | long   | —       | Seed for reproducible generation      |
| `locale`    | string | en-US   | `en-US` or `bn-BD`                    |
| `page`      | int    | 0       | Zero-based page index                 |
| `pageSize`  | int    | 10      | Records per page (1–100)              |
| `avgLikes`  | double | 2       | Average likes per movie (0–10)        |
| `avgReviews`| double | 2       | Average reviews per movie (0–10)      |

Returns a JSON array of `Movie` objects for that page.

## Deployment

A `Dockerfile` is included for containerized deployment (e.g. Render, Railway, Fly.io).

## License

Built as part of an internship assignment (Itransition).
