# Multi-Calendar Property Management Automation System

An automated cleaning scheduling system that integrates reservation data from multiple Property Management Systems ([Uplisting](https://www.uplisting.io), [Hostaway](https://www.hostaway.com)) into Amazon RDS and synchronizes with [Connecteam](https://connecteam.com) for workforce management. This solution eliminates manual data entry, processes 10,000+ records per sync cycle, and reduces the workload equivalent of 5 full-time employees.

## Problem Statement

Cleaning company needs to schedule cleaning services after guest checkouts, but this process requires manually checking reservation data across multiple PMS platforms ([Uplisting](https://www.uplisting.io), [Hostaway](https://www.hostaway.com)) and then manually creating cleaning jobs in [Connecteam](https://connecteam.com) workforce management system.

**Manual Process Issues:**

- Staff must log into multiple PMS dashboards to check checkout times and property details(e.g. Uplisting)
  ![](https://p.ipic.vip/xmbuvc.png)
- Manual data entry into Connecteam for each cleaning job
- Time-consuming and error-prone process
- Requires 5+ people to manage the workload

**Business Need:** Automate the entire workflow and provide a unified dashboard for reservation management.

### Solution

I designed this process which can batch fetch the reservation from 1 year ago and 2 year after, this include the process of:

1. Fetch the property list
2. Fetch the reservation information for each property
3. Filter the reservation info by its state(each reservation have different state, they may transer to each other, and some not block the calendar, e.g. query)
   ![](https://p.ipic.vip/ohdat8.png)
4. Compare the filtered reservation with the reservation info in the database, reflecting in frontend multicalendar
5. Handle deleting, updating, and adding, update the database, and send updated information to connecteam
   ![](https://p.ipic.vip/wk4rdl.png)

The sync cycle will be done in every 10 minutes.

Here is the roadmap of this process(Backend Logic):
![](https://p.ipic.vip/euj6e2.png)

### Result

The process successfully process over 10,000 records per sync cycle, reduced up to 5 people's workload.

Here is the datshboard, with different colors indicating different reservation status:
![](https://p.ipic.vip/2jp8b0.png)
They can also get the details of property by clicking property at left sidebar:
![](https://p.ipic.vip/1kc9u4.png)
And reservation details:
![](https://p.ipic.vip/trl3jq.png)

## Tech stack

### Frontend

- **Framework**: React 18.3.1 with TypeScript
- **Build Tool**: Vite 5.4.9
- **UI Components**: Material-UI (@mui/material)
- **Calendar Component**: @bitnoi.se/react-scheduler
- **HTTP Client**: Axios
- **Styling**: Emotion (@emotion/react, @emotion/styled)
- **Date Handling**: Day.js
- **Development**: ESLint, TypeScript

### Backend

- **Framework**: ASP.NET Core 8.0 (C#)
- **ORM**: Entity Framework Core 8.0.11
- **Database Provider**: Pomelo.EntityFrameworkCore.MySql 8.0.2
- **API Documentation**: Swagger/OpenAPI (Swashbuckle.AspNetCore 6.6.2)
- **Architecture**: Clean Architecture with 4 layers:
  - **Controllers**: API endpoints and request handling
  - **Services**: Business logic and external API integration
  - **Repository**: Data access layer
  - **Models**: DTOs and entities

### Database & Infrastructure

- **Database**: Amazon RDS MySQL
- **Web Server**: Nginx (frontend), Kestrel (backend)
- **CI/CD**: AWS CodePipeline for automated deployment
- **Deployment**: AWS CodeDeploy with EC2 instances
- **Operating System**: AWS Linux 2023

### External Integrations

- **Property Management Systems**:
  - Hostaway API (reservation data)
  - Uplisting API (reservation data)
- **Staff Management**: Connecteam API (shift scheduling)
- **Email Service**: Gmail SMTP
- **Webhooks**: Real-time updates from PMS platforms(deprecated)

### Development Tools

- **API Testing**: Swagger UI, HTTP files
- **Database Migrations**: Entity Framework Core Migrations
- **Logging**: Built-in ASP.NET Core logging with custom middleware
- **Error Handling**: Global exception filters

## Database Structure

![](https://p.ipic.vip/a2u00b.png)

## Disclaimer

This repository is a sanitized version of a private production system originally deployed with a complete CI/CD pipeline. All credentials, API keys, and sensitive data have been anonymized for security purposes. This project represents my own work and does not contain any proprietary intellectual property.
