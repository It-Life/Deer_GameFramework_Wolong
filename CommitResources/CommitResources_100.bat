@echo off  #需要密码

cd /d %~dp0/% 

rsync -avzP --port=873 --progress --delete --password-file=pass/rsyncd.secrets Wolong_100 bundles@121.4.195.168::bundles

pause