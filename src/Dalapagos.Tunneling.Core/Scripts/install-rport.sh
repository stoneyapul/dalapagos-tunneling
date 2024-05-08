echo "Installing RPort Server."
sshpass -p $1 ssh -o StrictHostKeyChecking=no $2 /bin/bash <<EOF
  export TERM=linux
  sudo -i
  curl https://get.openrport.io -o rport-install.sh
  bash rport-install.sh --no-2fa --skip-guacd --skip-nat --fqdn $3

  mv rportd-installation.txt /home/dlpgadmin

  ssh-keyscan 127.0.0.1 | ssh-keygen -lf - > /home/dlpgadmin/fingerprints.txt

  systemctl stop rportd.service
  sed -i 's/#ban_time = 3600/ban_time = 300/g' /etc/rport/rportd.conf
  sed -i 's,#pairing_url = "https://pairing\.example\.com",pairing_url = "https://pairing\.openrport\.io",g' /etc/rport/rportd.conf
  sed -i 's/#data_storage_duration = "7d"/data_storage_duration = "8h"/g' /etc/rport/rportd.conf
  systemctl start rportd.service

  snap install --classic certbot
  ln -s /snap/bin/certbot /usr/bin/certbot
  snap set certbot trust-plugin-with-root=ok
EOF