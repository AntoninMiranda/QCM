# Stage de développement avec hot reload
FROM node:20-alpine AS development

WORKDIR /app

# Copier les fichiers de configuration
COPY package*.json ./
COPY tsconfig.json ./
COPY next.config.ts ./
COPY tailwind.config.js ./
COPY postcss.config.mjs ./
COPY components.json ./

# Installer les dépendances
RUN npm install

# Le code source sera monté via volume
EXPOSE 3000

CMD ["npm", "run", "dev"]

# Étape 1 : Build pour la production
FROM node:20-alpine AS builder

WORKDIR /app

# Argument de build pour l'URL de l'API
ARG NEXT_PUBLIC_API_URL
ENV NEXT_PUBLIC_API_URL=$NEXT_PUBLIC_API_URL

# Copier les fichiers de configuration et de dépendances
COPY package*.json ./
COPY tsconfig.json ./
COPY next.config.ts ./
COPY tailwind.config.js ./
COPY postcss.config.mjs ./

# Installer les dépendances
RUN npm install

# Copier le code source et les assets
COPY src ./src
COPY public ./public

# Build Next.js avec la variable d'environnement
RUN npm run build

# Étape 2 : Runtime pour la production
FROM node:20-alpine AS runner

WORKDIR /app
ENV NODE_ENV=production

# Copier les fichiers nécessaires depuis le build
COPY --from=builder /app/public ./public
COPY --from=builder /app/.next ./.next
COPY --from=builder /app/node_modules ./node_modules
COPY --from=builder /app/package.json ./package.json
COPY --from=builder /app/next.config.ts ./next.config.ts

EXPOSE 3000

CMD ["npx", "next", "start"]
