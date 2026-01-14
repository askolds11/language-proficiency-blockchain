FROM git.zzdats.lv/library/nginx:alpine

COPY dist/ /usr/share/nginx/html
COPY docker/30-envsubst-content.sh /docker-entrypoint.d
COPY docker/nginx.conf /etc/nginx/conf.d/default.conf
RUN chmod -R +x /docker-entrypoint.d