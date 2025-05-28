# Project Overview

This is a project integrated reservation data from multiple PMS(Uplisting,Hosyaway) into RDS, and sync information into Connecteam

## Initiative

The company should provide cleaning services to property when the guest leave, but they should check reservation information(time of leaving, property information, notes) from varous property management platform dashboard, e.g. uplisting:
![](https://p.ipic.vip/xmbuvc.png)
and put it into Connecteam for cleaning job scheduling **manually**:
![](https://p.ipic.vip/wk4rdl.png)

They want to automate this process, as well as have a dashboard like which in PMS to view the reservation information.

### Solution

I designed this process which can batch fetch the reservation from 1 year ago and 2 year after, this include the process of:

1. Fetch the property list
2. Fetch the reservation information for each property
3. Filter the reservation info by its state(each reservation have different state, they may transer to each other, and some not block the calendar, e.g. query)
   ![](https://p.ipic.vip/ohdat8.png)
4. Compare the filtered reservation with the reservation info in the database, reflecting in frontend multicalendar
5. Handle deleting, updating, and adding, update the database, and send updated information to connecteam

The sync cycle will be done in every 10 minutes.

Here is the roadmap of this process(Backend Logic):
![](https://p.ipic.vip/euj6e2.png)

### Result

The process successfully process over 10,000 records per sync cycle, reduced up to 5 people's workload.

This is the datshboard, with different colors indicating different reservation status:
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

## Databse structure

![](https://p.ipic.vip/a2u00b.png)
