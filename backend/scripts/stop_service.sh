#!/bin/bash

SERVICE_NAME="streaming-application.service"

echo "Stopping existing service..."

if systemctl list-units --full -all | grep -q "$SERVICE_NAME"; then
    sudo systemctl stop "$SERVICE_NAME"
    echo "Service $SERVICE_NAME stopped successfully."
else
    echo "Service $SERVICE_NAME does not exist. Skipping stop operation."
fi
