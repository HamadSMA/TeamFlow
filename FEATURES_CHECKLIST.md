# TeamFlow – Features Checklist

This checklist tracks all required features for the TeamFlow internal web application.
Each item will be marked `[x]` only when fully implemented, tested, and committed.

---

## 1. Repository & Project Setup

- [x] Initialize Git repository
- [x] Add .gitignore (ASP.NET Core + macOS)
- [x] Add README.md
- [x] Create feature branch
- [x] Create .NET 8 ASP.NET Core MVC project

---

## 2. Authentication & Authorization (ASP.NET Core Identity)

## 2. Authentication & Authorization (ASP.NET Core Identity)

- [x] Add ASP.NET Core Identity
- [x] Integrate Identity with EF Core
- [x] Extend Identity user (ApplicationUser)
- [ ] Add IsActive flag (disable login)
- [ ] Create roles: Admin, Employee
- [ ] Seed roles on first run
- [ ] Seed default Admin user
- [ ] Configure authorization policies
- [ ] Restrict Admin-only pages
- [ ] Employee self-registration (default Employee)
- [ ] Admin-required team assignment
- [ ] Prevent deactivated users from logging in

---

## 3. Database & Entity Framework Core

- [ ] Add EF Core packages (.NET 8 compatible)
- [ ] Add Pomelo.EntityFrameworkCore.MySql
- [ ] Create ApplicationDbContext
- [ ] Integrate IdentityDbContext
- [ ] Configure MySQL connection via environment variables
- [ ] Create initial EF Core migration
- [ ] Apply migrations to database

---

## 4. Domain Models

- [ ] Team entity
- [ ] One-to-many relationship: Team → Users
- [ ] Status enum (Available, Busy, InMeeting, Offline)
- [ ] Store current user status
- [ ] StatusHistory entity
- [ ] Configure relationships and constraints

---

## 5. Team Management (Admin only)

- [ ] Team list page
- [ ] Create team
- [ ] Edit team
- [ ] Delete team
- [ ] Prevent deleting team with members
- [ ] Team details page with members

---

## 6. User Management (Admin only)

- [ ] List users
- [ ] Filter users by team
- [ ] Filter users by active/inactive
- [ ] Create user with temporary password
- [ ] Assign role
- [ ] Assign team
- [ ] Activate / deactivate user (soft disable)

---

## 7. Status Tracking (Employee)

- [ ] Status update UI
- [ ] Optional status note
- [ ] Store last updated timestamp
- [ ] Lightweight POST / fetch endpoint
- [ ] Persist status updates

---

## 8. Activity History

- [ ] Record every status change
- [ ] Store old status
- [ ] Store new status
- [ ] Store timestamp
- [ ] View history per user (Admin)
- [ ] View history per team (Admin)
- [ ] Filter history by date range

---

## 9. Dashboard

- [ ] Role-aware dashboard
- [ ] Admin: view all teams and users
- [ ] Employee: view own team only
- [ ] Display user name
- [ ] Display status
- [ ] Display note
- [ ] Display last updated time
- [ ] Sort by status priority
- [ ] Sort by most recent update

---

## 10. Azure – MySQL Flexible Server

- [ ] Create Azure MySQL Flexible Server
- [ ] Choose demo-appropriate SKU
- [ ] Select region
- [ ] Configure authentication mode
- [ ] Configure public networking
- [ ] Configure firewall rules
- [ ] Understand and enforce SSL/TLS
- [ ] Create database
- [ ] Create least-privilege database user

---

## 11. Azure – App Service

- [ ] Create App Service Plan
- [ ] Choose pricing tier for demo
- [ ] Create App Service (Web App)
- [ ] Select .NET 8 runtime
- [ ] Configure environment variables
- [ ] Configure connection strings

---

## 12. Deployment & Migrations

- [ ] Decide migration strategy
- [ ] Run EF Core migrations against Azure MySQL
- [ ] Verify schema in Azure
- [ ] Deploy app to Azure App Service

---

## 13. Verification & Troubleshooting

- [ ] Verify authentication online
- [ ] Verify role-based authorization
- [ ] Verify dashboard data
- [ ] Verify status updates
- [ ] Verify activity history
- [ ] Review App Service logs
- [ ] Resolve MySQL / SSL / firewall issues

---

## 14. Final Demo Readiness

- [ ] Default Admin login works
- [ ] Employee login works
- [ ] Teams visible
- [ ] Status updates work
- [ ] App stable online
