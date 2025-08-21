# Copilot Instructions for Car Auction Bidding System

## Overview

This monorepo contains a car auction platform with a .NET backend (Clean Architecture, EF Core, Quartz) and React + Vite + TypeScript frontend. Key workflows include CSV/image import, auction lifecycle management, JWT authentication, and real-time bidding.

## Architecture Patterns

### Backend (`backend/Car_Auction/`)

- **Clean Architecture**: Domain → Application → Infrastructure → API layers
- **Domain**: Entities (`Vehicle`, `AuctionVehicle`, `BidHistory`), value objects, domain services
- **Infrastructure**: EF Core repositories, UnitOfWork, Quartz jobs (`AuctionStartJob`, `VehicleImportJob`)
- **Application**: DTOs, service interfaces, business orchestration
- **API**: Controllers with JWT auth, middleware (`ExceptionHandlingMiddleware`)

### Frontend (`frontend/`)

- **React + Vite + TypeScript**: Fast dev with HMR
- **State Management**: React Query for server state, localStorage for auth
- **UI**: Bootstrap 5 + React Bootstrap components
- **Routing**: React Router with protected routes (`ProtectedRoute`) and layout separation

## Core Workflows

### Auction Lifecycle

1. **Import Settings**: `ImportAuctionSetting` loads tactics/criteria from `auctionSetting.json`
2. **Vehicle Selection**: `LoadAuctionVehicle` scores vehicles against criteria, creates `AuctionVehicle` entries
3. **Session Management**: `MoveNextSession` moves unsold vehicles to next sale channel step
4. **Multi-Step Selling**: Vehicles progress through online discounts → hot sale → physical auction

### Data Import Pipeline

- **CSV Import**: `VehicleCsvImportService` validates VINs, creates vehicles with placeholder images
- **Image Processing**: `VehicleImageImportService` extracts ZIP files by VIN, associates images
- **Validation**: 17-char alphanumeric VINs, required fields, duplicate prevention
- **Storage**: Files in `LoadData/` directory structure

### Authentication Flow

- **JWT Tokens**: Backend generates, frontend stores in localStorage
- **Axios Interceptors**: Auto-attach Bearer tokens, handle 401 redirects
- **Protected Routes**: `ProtectedRoute` wrapper checks auth state
- **User Context**: Balance, username, email stored and displayed in navbar

## Developer Commands

### Backend (.NET 9)

- **Build**: `dotnet build` in `CarAuction.API/`
- **Run**: `dotnet run` in `CarAuction.API/` (port 5291)
- **Migrations**: `dotnet ef migrations add <name>` in API project
- **Database**: SQL Server, connection in `appsettings.json`

### Frontend (Vite + React)

- **Install**: `npm install` in `frontend/`
- **Dev Server**: `npm run dev` (with HMR)
- **Build**: `npm run build` (TypeScript check + Vite bundle)
- **Lint**: `npm run lint` (ESLint with TypeScript rules)

## Project Conventions

### Backend Patterns

- **Repository + UoW**: Always use `IUnitOfWork`, never direct `DbContext`
- **Response Wrapper**: All APIs return `ResponseResult<T>` for consistency
- **Dependency Injection**: Register in `DependencyInjection.cs` extensions
- **Concurrency**: `AuctionVehicle` uses `[Timestamp]` for optimistic concurrency
- **Configuration**: Settings in `appsettings.json`, options pattern for DI

### Frontend Patterns

- **Hooks for Data**: `useAuth`, `useVehicles`, `useReports` with React Query
- **Form Validation**: React Hook Form + Zod schemas (`loginSchema`, `bidSchema`)
- **Error Handling**: Toast notifications, consistent error display
- **Component Structure**: Pages → components → services → hooks
- **Type Safety**: TypeScript interfaces for all API responses and form data

### Key Integration Points

- **API Base URL**: `http://localhost:5291/api` (configurable in `axios.ts`)
- **File Serving**: Static files served from `LoadData/Images/` directory
- **Quartz Jobs**: Scheduled in `DependencyInjection.cs`, triggered by `AuctionStartJob`
- **CORS**: Backend configured for frontend origin

## Critical Files

- **Backend Entry**: `CarAuction.API/Program.cs` (auth, DI, middleware setup)
- **DI Registration**: `CarAuction.Infrastructure/DI/DependencyInjection.cs`
- **Auth Service**: `CarAuction.Application/Services/AuthService.cs` (JWT generation)
- **Frontend Entry**: `frontend/src/main.tsx` (React Query, router setup)
- **Auth Utils**: `frontend/src/shared/utils/auth.ts` (token management)
- **API Client**: `frontend/src/shared/utils/axios.ts` (interceptors, base config)

## Examples

- **Add Entity**: Create in Domain → Add repository interface → Implement in Infrastructure → Register in DI
- **New API Endpoint**: Controller method → Service method → Repository if needed → Frontend service hook
- **Auction Step**: Modify `auctionSetting.json` → Run import → Vehicles auto-progress through steps
