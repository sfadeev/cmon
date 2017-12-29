# https://docs.microsoft.com/en-us/aspnet/core/publishing/linuxproduction#monitoring-our-application

# /etc/nginx/sites-available/default
sudo nginx -t
sudo nginx -s reload

# /etc/systemd/system/
systemctl enable dotnet-cmon.service
systemctl start dotnet-cmon.service
systemctl stop dotnet-cmon.service
systemctl status dotnet-cmon.service

systemctl enable dotnet-cmon-web.service
systemctl start dotnet-cmon-web.service
systemctl stop dotnet-cmon-web.service
systemctl status dotnet-cmon-web.service

# /etc/systemd/system/
systemctl enable asktaskbot.service
systemctl start asktaskbot.service
systemctl stop asktaskbot.service
systemctl status asktaskbot.service

sudo journalctl -fu asktaskbot

sudo journalctl -fu dotnet-cmon.service
sudo journalctl -fu dotnet-cmon-web.service

sudo journalctl -fu dotnet-cmon-web.service --since "2016-10-18" --until "2016-10-18 04:00"

# https://www.digitalocean.com/community/tutorials/how-to-secure-nginx-with-let-s-encrypt-on-ubuntu-16-04

#IMPORTANT NOTES:
# - Congratulations! Your certificate and chain have been saved at
#   /etc/letsencrypt/live/ccu.montr.net/fullchain.pem. Your cert will
#   expire on 2017-05-15. To obtain a new version of the certificate in
#   the future, simply run Let's Encrypt again.

# sudo letsencrypt certonly -a webroot --webroot-path=/var/www/html -d montr.net
sudo letsencrypt certonly -a webroot --webroot-path=/var/www/ccu.montr.net -d ccu.montr.net
sudo letsencrypt certonly -a webroot --webroot-path=/var/www/asktaskbot.montr.net -d asktaskbot.montr.net

sudo certbot --nginx -d asktaskbot.montr.net
#  - Congratulations! Your certificate and chain have been saved at:
#    /etc/letsencrypt/live/asktaskbot.montr.net/fullchain.pem
#    Your key file has been saved at:
#    /etc/letsencrypt/live/asktaskbot.montr.net/privkey.pem
#    Your cert will expire on 2018-03-17. To obtain a new or tweaked
#    version of this certificate in the future, simply run certbot again
#    with the "certonly" option. To non-interactively renew *all* of
#    your certificates, run "certbot renew"



# https://www.ssllabs.com/ssltest/analyze.html?d=ccu.montr.net

# list all processes
ps aufx

# disk usage
df -h

# grant wrile to logs
chown blackish /var/log/cmon

# htop - an interactive process viewer for Unix
htop

# backup postgres
pg_dump -U postgres -h localhost cmon-prod -F tar -f 20170509-cmon-prod.backup
tar czf 20170509-cmon-prod.backup.tar.gz 20170509-cmon-prod.backup
