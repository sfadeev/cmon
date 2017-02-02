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

sudo journalctl -fu dotnet-cmon.service
sudo journalctl -fu dotnet-cmon-web.service

sudo journalctl -fu dotnet-cmon-web.service --since "2016-10-18" --until "2016-10-18 04:00"
