# Stage 1: Build
FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npx nx build webapp

# Stage 2: Run
FROM nginx:alpine
COPY apps/webapp/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/apps/webapp/dist /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
