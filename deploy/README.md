# Deploying to a Hostinger VPS

The whole site runs on one VPS with Docker Compose: PostgreSQL, the ASP.NET Core API, and Caddy
(serves the React build, proxies `/api`, `/mcp` and `/health` to the API, and gets HTTPS
certificates from Let's Encrypt on its own).

Hostinger's shared / Web / Cloud hosting plans cannot run this: they serve static files and PHP
with MySQL only, and the API needs .NET 10 and PostgreSQL. Use a **VPS (KVM)** plan. KVM 1
(1 vCPU, 4 GB RAM) is enough for this site; KVM 2 gives more room for builds.

```
Internet ──443──▶ web (Caddy) ──▶ /srv           React build (static files)
                       │
                       └── /api, /mcp, /health ──▶ api (ASP.NET Core, :8080) ──▶ db (PostgreSQL)
```

Only Caddy is published on the host (ports 80 and 443). The API and the database are reachable only
inside the Docker network.

## Files

| File | What it does |
|------|--------------|
| `deploy/docker-compose.yml` (this repo) | The three containers, volumes and settings |
| `deploy/.env.example` (this repo) | The values you fill in (copy to `.env`) |
| `Dockerfile` (this repo) | Builds and runs the API (.NET 10 runtime, non-root user) |
| `Dockerfile` (frontend repo) | Builds the React app, then packs it into a Caddy image |
| `Caddyfile` (frontend repo) | HTTPS, proxy rules, SPA fallback, cache headers, 20 MB upload limit |

The site lives in two repositories: this backend repository (`portfolio-api`) and the frontend
repository (`portfolio-web`). On the server they are cloned side by side; the compose file builds
the website from `WEB_DIR` (default `../../portfolio-web`).

## 1. Buy and prepare the VPS

1. In hPanel, buy a VPS and choose the **Ubuntu 24.04 with Docker** template (Docker and Compose
   come installed). Plain Ubuntu works too: install Docker with `curl -fsSL https://get.docker.com | sh`.
2. Note the server's IP address. Set a root password or add your SSH key in hPanel.
3. Connect: `ssh root@YOUR_SERVER_IP`
4. Turn on the firewall, allowing only SSH and web traffic:

   ```bash
   ufw allow OpenSSH && ufw allow 80 && ufw allow 443 && ufw --force enable
   ```

## 2. Point the domain at the server

In your domain's DNS (hPanel → Domains → DNS, or wherever the domain is registered):

- `A` record, name `@`, value `YOUR_SERVER_IP`
- optionally `A` record, name `www`, same IP (then add `www.yourdomain` to the Caddyfile site line)

Wait until `ping yourdomain` answers with the server IP. Caddy cannot get a certificate before that.

## 3. Google sign-in for the live domain

Google Cloud Console → APIs & Services → Credentials → your OAuth client →
**Authorized JavaScript origins** → add `https://yourdomain`. Keep `http://localhost:5173` for local work.

## 4. Get the code on the server

```bash
mkdir -p /opt/portfolio && cd /opt/portfolio
git clone https://github.com/AshikurWebDev/portfolio-api.git
git clone https://github.com/AshikurWebDev/portfolio-web.git
```

Use your real repository URLs if you named them differently, and set `WEB_DIR` in `.env` to match.

## 5. Fill in the settings

```bash
cd /opt/portfolio/portfolio-api/deploy
cp .env.example .env
nano .env
```

| Value | How to get it |
|-------|---------------|
| `DOMAIN` | e.g. `ashikur.dev`, no `https://` |
| `POSTGRES_PASSWORD` | `openssl rand -base64 32` |
| `JWT_SIGNING_KEY` | `openssl rand -base64 48` (at least 32 characters) |
| `GOOGLE_CLIENT_ID` | same id as `VITE_GOOGLE_CLIENT_ID` in `web/.env` |
| `ADMIN_EMAIL` | the Gmail address allowed into `/admin` |
| `WEB_DIR` | path to the frontend repository from `deploy/` (default `../../portfolio-web`) |

`.env` holds secrets. Keep it on the server only.

## 6. Start

```bash
docker compose up -d --build
docker compose logs -f api     # Ctrl+C to stop watching
```

The first start applies the database migrations and creates the Admin role, the two projects and
the three draft posts (`Database__MigrateOnStartup`). Later starts add only what is missing.

Check:

```bash
curl https://yourdomain/health          # {"status":"ok"}
curl https://yourdomain/api/projects
```

Then open `https://yourdomain/admin`, sign in with Google using `ADMIN_EMAIL`, review and publish the
draft posts, and upload designs in the Design tab.

## Updating the site

```bash
cd /opt/portfolio/portfolio-web && git pull
cd /opt/portfolio/portfolio-api && git pull
cd deploy && docker compose up -d --build
```

`VITE_*` values are baked into the JavaScript when the web image is built. After changing `DOMAIN`
or `GOOGLE_CLIENT_ID`, rebuild with the command above. Restarting is not enough.

## Backups

The database (posts, design images, users) lives in the `pgdata` volume. Take a dump now and then:

```bash
docker compose exec -T db pg_dump -U portfolio portfolio | gzip > ~/portfolio-$(date +%F).sql.gz
```

Restore into a fresh stack: `gunzip -c file.sql.gz | docker compose exec -T db psql -U portfolio portfolio`.
Hostinger VPS snapshots (hPanel) are a second safety net.

## After it is live

- Google Search Console: add the domain and submit `https://yourdomain/sitemap.xml`.
- Check the social preview with a link-preview tool (it uses `og-image.jpg`).

## Notes

- The API log shows a DataProtection warning about keys not being persisted. The API signs in with JWTs
  and refresh tokens stored in the database, not cookies protected by DataProtection, so this is harmless.
- `docker compose down` stops the site and keeps the data. `docker compose down -v` **deletes** the
  database and certificates.
- Tested locally on 2026-11-16 with `DOMAIN=localhost:8443`: health, API, client-side routes, a 16 MB
  upload passing the proxy (rejected 401 by the API when signed out), 25 MB refused 413 by Caddy,
  sitemap/canonical built with the domain, and on a fresh database: migrations applied and the
  projects, draft posts and skills seeded.
