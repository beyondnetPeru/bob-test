/*
 * HARDWARE INVENTORY MANAGEMENT SYSTEM - UNIFIED PRODUCT SCHEMA (v2.0)
 * Target: SQL Server / SQL Express
 * Normalization: 3rd Normal Form (3NF) - Unified Catalog Strategy
 */

-- [1] LOOKUP TABLES (Standardization)
IF OBJECT_ID('dbo.Manufacturers', 'U') IS NOT NULL DROP TABLE dbo.Manufacturers;
CREATE TABLE dbo.Manufacturers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE,
    CreatedAt DATETIMEOFFSET DEFAULT SYSDATETIMEOFFSET(),
    UpdatedAt DATETIMEOFFSET DEFAULT SYSDATETIMEOFFSET()
);

IF OBJECT_ID('dbo.ProductCategories', 'U') IS NOT NULL DROP TABLE dbo.ProductCategories;
CREATE TABLE dbo.ProductCategories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE -- 'CPU', 'GPU', 'RAM', 'STORAGE', 'PORT', 'PSU', 'CHASSIS'
);

-- [2] THE UNIVERSAL PRODUCT CATALOG
IF OBJECT_ID('dbo.Products', 'U') IS NOT NULL DROP TABLE dbo.Products;
CREATE TABLE dbo.Products (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CategoryId INT NOT NULL,
    ManufacturerId INT NOT NULL,
    ModelName NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    
    FOREIGN KEY (CategoryId) REFERENCES dbo.ProductCategories(Id),
    FOREIGN KEY (ManufacturerId) REFERENCES dbo.Manufacturers(Id)
);

-- [3] HARDWARE ASSETS (The Fleets)
IF OBJECT_ID('dbo.HardwareInventory', 'U') IS NOT NULL DROP TABLE dbo.HardwareInventory;
CREATE TABLE dbo.HardwareInventory (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    AssetName NVARCHAR(255) NULL, -- Optional name for the machine
    WeightKg DECIMAL(10,2) NOT NULL, -- Standardized to kg (Decimal)
    PerformanceTier NVARCHAR(50) NULL, -- 'Entry', 'Mid-Range', 'High-End', 'Workstation'
    
    -- Audit Shadow Properties
    CreatedAt DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    UpdatedAt DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    RowVersion ROWVERSION, -- For optimistic concurrency
);

-- [4] CONFIGURATION MAPPING (Unified Component Relationship)
IF OBJECT_ID('dbo.AssetConfigurations', 'U') IS NOT NULL DROP TABLE dbo.AssetConfigurations;
CREATE TABLE dbo.AssetConfigurations (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    InventoryId UNIQUEIDENTIFIER NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL DEFAULT 1,
    StandardValue DECIMAL(18,4) NULL, -- 16.0 (RAM), 1000.0 (Storage), 3.2 (Clock GHz)
    Location NVARCHAR(100) NULL, -- 'DIMM 1', 'SATA 3', 'Back Panel', 'Internal'
    
    FOREIGN KEY (InventoryId) REFERENCES dbo.HardwareInventory(Id) ON DELETE CASCADE,
    FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id)
);

-- [5] PERFORMANCE OPTIMIZATION (Indexes)
CREATE INDEX IX_Products_Category ON dbo.Products(CategoryId);
CREATE INDEX IX_Products_Manufacturer ON dbo.Products(ManufacturerId);
CREATE INDEX IX_HardwareInventory_PerformanceTier ON dbo.HardwareInventory(PerformanceTier);
CREATE INDEX IX_AssetConfig_InventoryId ON dbo.AssetConfigurations(InventoryId);
CREATE INDEX IX_AssetConfig_ProductId ON dbo.AssetConfigurations(ProductId);

-- [6] INITIAL DATA (Seed Core Catalog)
INSERT INTO dbo.Manufacturers (Name) VALUES ('Intel'), ('AMD'), ('NVIDIA'), ('Generic'), ('Kingston'), ('Seagate');
INSERT INTO dbo.ProductCategories (Name) VALUES ('CPU'), ('GPU'), ('RAM'), ('STORAGE'), ('PORT'), ('PSU'), ('CHASSIS');

-- Sample Product Definitions (Examples of the Unified Concept)
INSERT INTO dbo.Products (CategoryId, ManufacturerId, ModelName) VALUES 
(1, 1, 'Core i7-6700K'), -- CPU
(2, 3, 'GeForce GTX 1080'), -- GPU
(4, 6, '1TB SSD'), -- STORAGE (as a Product model)
(5, 4, 'USB 3.0'); -- PORT (as a Product concept)
GO
