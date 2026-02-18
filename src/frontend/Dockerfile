FROM node:22-alpine AS base
RUN corepack enable
WORKDIR /app

FROM base AS prod-deps
COPY package.json pnpm-lock.yaml .npmrc ./
RUN --mount=type=cache,id=pnpm,target=/pnpm/store pnpm install --prod --frozen-lockfile

FROM base AS builder
COPY package.json pnpm-lock.yaml .npmrc ./
RUN --mount=type=cache,id=pnpm,target=/pnpm/store pnpm install --frozen-lockfile
COPY . .

# SvelteKit $env/static/public vars are inlined at build time.
# Pass via: docker build --build-arg PUBLIC_TURNSTILE_SITE_KEY=...
ARG PUBLIC_TURNSTILE_SITE_KEY
ENV PUBLIC_TURNSTILE_SITE_KEY=${PUBLIC_TURNSTILE_SITE_KEY}

RUN pnpm run build

FROM node:22-alpine
WORKDIR /app
COPY --from=builder /app/build build/
COPY --from=prod-deps /app/node_modules node_modules/
COPY package.json .
EXPOSE 3000
ENV NODE_ENV=production
ENV PORT=3000
CMD [ "node", "build" ]
