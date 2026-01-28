# Stage de développement
FROM node:20-alpine AS development
WORKDIR /app
COPY package*.json ./
COPY tsconfig.json ./
COPY next.config.ts ./
COPY tailwind.config.js ./
COPY postcss.config.mjs ./
COPY components.json ./
RUN npm install
EXPOSE 3000
CMD ["npm", "run", "dev"]

# Stage de build
FROM node:20-alpine AS builder
WORKDIR /app
COPY package*.json ./
COPY tsconfig.json ./
COPY next.config.ts ./
COPY tailwind.config.js ./
COPY postcss.config.mjs ./
RUN npm install
COPY src ./src
COPY public ./public
RUN npm run build

# Stage de production
FROM node:20-alpine AS runner
WORKDIR /app
ENV NODE_ENV=production
COPY --from=builder /app/.next/standalone ./
COPY --from=builder /app/.next/static ./.next/static
COPY --from=builder /app/public ./public
EXPOSE 3000
CMD ["node", "server.js"]
