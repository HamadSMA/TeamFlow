# TeamFlow — Ordered Build Checklist (Rescoped)

---

## 1. Repository & Local Setup

- [x] Initialize Git repository
- [x] Add .gitignore
- [x] Add README.md
- [x] Create feature branch
- [x] Create .NET 8 ASP.NET Core MVC project
- [x] Verify EF Core tooling and package versions
- [x] Local app runs successfully

---

## 2. Identity Foundation

- [x] Add ASP.NET Core Identity UI
- [x] Integrate Identity with EF Core
- [x] Extend Identity user (ApplicationUser)
- [x] Create ApplicationDbContext
- [x] Create initial EF Core migration
- [x] Apply migrations to database
- [x] Employee self-registration works

---

## 3. Roles & Admin Bootstrap

- [x] Create roles: Admin, Employee
- [x] Seed roles on application startup
- [x] Seed default Admin user using environment variables
- [x] Assign Admin role to default Admin user
- [x] Verify Admin login works

---

## 4. Authorization

- [x] Configure authorization policies
- [x] Restrict Admin-only pages

---

## 5. Domain Models

- [x] Team entity
- [x] One-to-many relationship: Team → Users
- [ ] Status enum
- [ ] Store current user status
- [ ] StatusHistory entity
- [x] Configure relationships and constraints

---

## 6. Admin: Team Management

- [ ] Team list page
- [ ] Create team
- [ ] Edit team
- [ ] Delete team
- [ ] Prevent deleting team with members
- [ ] Team details page with members

---

## 7. Admin: User Management

- [ ] List users
- [ ] Assign team
- [ ] Activate / deactivate user

---

## 8. Account State Enforcement

- [ ] Add IsActive flag
- [ ] Prevent deactivated users from logging in

---

## 9. Employee: Status Updates

- [ ] Status update UI
- [ ] Optional status note
- [ ] Store last updated timestamp
- [ ] Persist status updates
- [ ] Lightweight POST / fetch endpoint

---

## 10. Dashboard

- [ ] Role-aware dashboard
- [ ] Admin sees all teams and users
- [ ] Employee sees own team
- [ ] Display user name
- [ ] Display status
- [ ] Display note
- [ ] Display last updated time

---

## 11. Database: Azure MySQL Flexible Server

- [ ] Create Azure MySQL Flexible Server
- [ ] Choose SKU
- [ ] Select region
- [ ] Configure authentication mode
- [ ] Configure public networking
- [ ] Configure firewall rules
- [ ] Enforce SSL/TLS
- [ ] Create database
- [ ] Create least-privilege database user
- [ ] Add Pomelo.EntityFrameworkCore.MySql
- [ ] Configure MySQL connection via environment variables

---

## 12. Deployment: Azure App Service

- [ ] Create App Service Plan
- [ ] Choose pricing tier
- [ ] Create App Service
- [ ] Select .NET 8 runtime
- [ ] Configure environment variables
- [ ] Configure connection strings
- [ ] Decide migration strategy
- [ ] Run EF Core migrations against Azure MySQL
- [ ] Deploy app to Azure App Service

---

## 13. Verification & Demo Readiness

- [ ] Verify authentication online
- [ ] Verify role-based authorization
- [ ] Verify team visibility
- [ ] Verify status updates
- [ ] Verify dashboard data
- [ ] Review App Service logs
- [ ] Resolve MySQL / SSL / firewall issues
- [ ] App stable online
