# StockDaddy marketing website

Static landing page for StockDaddy—features, pricing, and contact. No backend; deploy free on Cloudflare Pages.

## Local development

```bash
cd Website
npm install
npm run dev
```

Open http://localhost:5173

## Build

```bash
npm run build
```

Output goes to `dist/`.

## Environment variables

Copy `.env.example` to `.env.local` for local dev, or set these in Cloudflare Pages → Settings → Environment variables:

| Variable | Description |
|----------|-------------|
| `VITE_CONTACT_EMAIL` | Email shown on the site and used for mailto fallback (default: `sales@stockdaddy.app`) |
| `VITE_FORMSPREE_ENDPOINT` | Optional Formspree form URL, e.g. `https://formspree.io/f/xxxxxxxx` |

Without Formspree, the contact form opens the user's email client with a pre-filled message.

### Formspree setup (optional)

1. Create a free account at [formspree.io](https://formspree.io) (50 submissions/month on free tier).
2. Create a new form and copy the endpoint URL.
3. Set `VITE_FORMSPREE_ENDPOINT` in `.env.local` (dev) or Cloudflare Pages env vars (production).
4. Submit a test message from the contact section.

## Deploy to Cloudflare Pages (free)

1. **Push this repo to GitHub** (if not already).

2. **Create a Cloudflare account** at [dash.cloudflare.com](https://dash.cloudflare.com) (free).

3. **Workers & Pages → Create → Pages → Connect to Git**:
   - Select your StockDaddy repository.
   - Choose the branch to deploy (e.g. `main`).

4. **Build settings**:

   | Setting | Value |
   |---------|-------|
   | Framework preset | None (or Vite) |
   | Root directory | `Website` |
   | Build command | `npm run build` |
   | Build output directory | `dist` |

5. **Environment variables** (optional but recommended):
   - `VITE_CONTACT_EMAIL` — your sales inbox
   - `VITE_FORMSPREE_ENDPOINT` — your Formspree URL

6. **Save and deploy**. Cloudflare assigns a free subdomain such as `stockdaddy.pages.dev`.

7. **Custom domain (optional later)** — Pages → Custom domains → add your domain (~$10–15/yr registrar cost).

### Deploy on every push

Cloudflare Pages rebuilds automatically when you push to the connected branch. No GitHub Action required.

## Optional: Cloudflare Web Analytics

1. Cloudflare dashboard → Web Analytics → Add a site.
2. Copy the beacon script snippet.
3. Paste it before `</body>` in `index.html` if you want privacy-friendly analytics.

## Site sections

- Hero with POS screenshot
- Product features (from the real StockDaddy app)
- Target audience
- Pricing: Starter $39/mo, Business $79/mo, Self-hosted $1,999
- Contact form (Formspree or mailto)
- Footer with privacy note

## Assets

- `public/favicon.svg` — StockDaddy brand icon (blue, matches app)
- `public/screenshots/` — stylized product previews (POS, customers, credit, mobile PWA)

Replace SVG previews with real PNG/WebP screenshots from the running app when you have captures ready.
