#!/bin/bash
set -e  # Exit immediately if a command exits with a non-zero status

echo "Starting service..."

# Create the service unit file if it doesn't exist
if [ ! -f "/etc/systemd/system/streaming-application.service" ]; then
    echo "Creating service unit file..."
    
    cat << EOF | sudo tee /etc/systemd/system/streaming-application.service
[Unit]
Description=Streaming Application Service
After=network.target

[Service]
Type=simple
ExecStart=/usr/bin/dotnet /var/www/streaming-application/Streamling.dll
WorkingDirectory=/var/www/streaming-application
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=streaming-application
User=root
Environment=ASPNETCORE_ENVIRONMENT=Production

[Install]
WantedBy=multi-user.target
EOF
fi

echo "Reloading systemd..."
sudo systemctl daemon-reload

echo "Enabling service..."
sudo systemctl enable streaming-application.service

echo "Starting service..."
sudo systemctl start streaming-application.service || true

# Don't wait for the service to fully initialize
echo "Service start command issued. Continuing deployment."
# Write a startup verification script to run later
cat << EOF | sudo tee /var/www/streaming-application/verify_startup.sh
#!/bin/bash
MAX_ATTEMPTS=60
ATTEMPT=0
SERVICE_STARTED=false

while [ \$ATTEMPT -lt \$MAX_ATTEMPTS ]; do
    if sudo systemctl is-active --quiet streaming-application.service; then
        echo "\$(date) - Service started successfully." >> /var/www/streaming-application/startup.log
        SERVICE_STARTED=true
        break
    fi
    echo "\$(date) - Waiting for service to start... (\${ATTEMPT}/\${MAX_ATTEMPTS})" >> /var/www/streaming-application/startup.log
    ATTEMPT=\$((ATTEMPT+1))
    sleep 5
done

if [ "\$SERVICE_STARTED" = false ]; then
    echo "\$(date) - Service failed to start within timeout period." >> /var/www/streaming-application/startup.log
    echo "Service logs:" >> /var/www/streaming-application/startup.log
    sudo journalctl -u streaming-application.service -n 50 >> /var/www/streaming-application/startup.log
fi
EOF

sudo chmod +x /var/www/streaming-application/verify_startup.sh
# Run the verification in the background
nohup sudo /var/www/streaming-application/verify_startup.sh > /dev/null 2>&1 &

echo "Service deployment process completed. Service initialization continuing in background."