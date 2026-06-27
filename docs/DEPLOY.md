# Deploy for FREE (no credit card) — Vercel + Render + Neon

This deploys a **public live link** using three free, no-card services:

```
Neon      → PostgreSQL database
Render    → the .NET 10 API (Docker)
Vercel    → the Vue frontend
```

> The app already supports this: PostgreSQL provider, a `Dockerfile`, `$PORT` binding, and
> `frontend/vercel.json`. You only need to create the accounts and paste a few settings.

⚠️ **Free-tier notes:** Render's free API **sleeps after ~15 min idle** (first hit wakes it in ~30s —
open your link once before showing the manager). **Uploaded images don't persist** across restarts on
free Render (the seeded product photos are fine; admin-uploaded ones may disappear after a redeploy).

---

## Step 1 — Database on Neon (5 min)

1. Go to **https://neon.tech** → **Sign up** (use your GitHub account).
2. **Create a project** (any name, e.g. `arla-connect`). Region: pick the closest (e.g. Frankfurt).
3. After it's created, open **Connection Details** → copy the connection info. You need a **.NET / Npgsql**
   connection string. Build it from the values Neon shows:
   ```
   Host=<your-host>.neon.tech;Database=<db>;Username=<user>;Password=<password>;SSL Mode=Require;Trust Server Certificate=true
   ```
   (Neon also shows a ready connection string — make sure yours ends with `SSL Mode=Require;Trust Server Certificate=true`.)
4. **Save that string** — it's your `ConnectionStrings__DefaultConnection` for Render.

## Step 2 — API on Render (10 min)

1. Go to **https://render.com** → **Sign up** with GitHub.
2. **New ► Web Service** → **Connect** your `arla-connect` repo (authorise Render to read it).
3. Render detects the **Dockerfile**. Set:
   - **Name:** `arla-connect-api`
   - **Region:** closest to you
   - **Instance type:** **Free**
   - **Health Check Path:** `/health`
4. Add **Environment Variables** (Advanced → Add Environment Variable):
   | Key | Value |
   |-----|-------|
   | `ConnectionStrings__DefaultConnection` | *(the Neon string from Step 1)* |
   | `Database__Provider` | `Postgres` |
   | `Jwt__Key` | a long random secret (32+ chars) |
   | `Jwt__Issuer` | `arla-connect` |
   | `Jwt__Audience` | `arla-connect-client` |
   | `ASPNETCORE_ENVIRONMENT` | `Production` |
   | `Cors__AllowedOrigins__0` | `https://PLACEHOLDER` *(update after Step 3)* |
5. **Create Web Service** → wait for the build (first one takes a few minutes).
6. Copy your API URL: `https://arla-connect-api.onrender.com`. Test it: open `…/health` → should say `Healthy`.

## Step 3 — Frontend on Vercel (5 min)

1. Go to **https://vercel.com** → **Sign up** with GitHub.
2. **Add New ► Project** → **Import** the `arla-connect` repo.
3. Set:
   - **Root Directory:** `frontend`
   - **Framework Preset:** Vite (auto-detected)
4. Add an **Environment Variable**:
   | Key | Value |
   |-----|-------|
   | `VITE_API_BASE_URL` | `https://arla-connect-api.onrender.com/api` *(your Render URL + `/api`)* |
5. **Deploy** → copy your live URL: `https://arla-connect.vercel.app`.

## Step 4 — Connect them (CORS) (2 min)

1. Back in **Render** → your service → **Environment** → edit `Cors__AllowedOrigins__0`:
   ```
   https://arla-connect.vercel.app    (your exact Vercel URL, no trailing slash)
   ```
2. Save → Render redeploys automatically.

## Step 5 — Test the live link 🎉

1. Open your **Vercel URL**. (First load wakes the sleeping API — ~30s.)
2. Log in:
   - Buyer: `demo@arla-connect.test` / `Password123!`
   - Admin: `admin@arla.com` / `Admin123!`
3. Browse, order, claim, open the admin back-office — it's all live.

## Auto-deploy (the "CI/CD" part)
Render and Vercel are now connected to your GitHub repo — **every `git push` automatically rebuilds and
redeploys** both. That's continuous deployment, set up once.

## Generate a JWT secret (for `Jwt__Key`)
Any 32+ character random string works. Quick one in PowerShell:
```powershell
[Convert]::ToBase64String((1..48 | ForEach-Object { Get-Random -Max 256 }))
```
