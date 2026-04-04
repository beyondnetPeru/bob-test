/*
 * COMPUTER CATALOG & COMPOSITION SCHEMA (v3.0)
 * Target: SQL Server / SQL Express
 * Normalization: 3rd Normal Form (3NF)
 * Business rule: each spreadsheet row is a complete computer product composed from reusable typed components.
 */

-- Drop dependent objects first
IF OBJECT_ID('dbo.vwProductSpecificationSheet', 'V') IS NOT NULL DROP VIEW dbo.vwProductSpecificationSheet;
IF OBJECT_ID('dbo.InventoryAssets', 'U') IS NOT NULL DROP TABLE dbo.InventoryAssets;
IF OBJECT_ID('dbo.ProductComponentSelections', 'U') IS NOT NULL DROP TABLE dbo.ProductComponentSelections;
IF OBJECT_ID('dbo.CatalogProducts', 'U') IS NOT NULL DROP TABLE dbo.CatalogProducts;
IF OBJECT_ID('dbo.ComponentCatalogItems', 'U') IS NOT NULL DROP TABLE dbo.ComponentCatalogItems;
IF OBJECT_ID('dbo.ComponentTypes', 'U') IS NOT NULL DROP TABLE dbo.ComponentTypes;
IF OBJECT_ID('dbo.DeviceCategories', 'U') IS NOT NULL DROP TABLE dbo.DeviceCategories;
IF OBJECT_ID('dbo.Manufacturers', 'U') IS NOT NULL DROP TABLE dbo.Manufacturers;
GO

-- [1] LOOKUPS
CREATE TABLE dbo.Manufacturers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE,
    CreatedAt DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    UpdatedAt DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET()
);
GO

CREATE TABLE dbo.DeviceCategories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(80) NOT NULL UNIQUE,
    Description NVARCHAR(255) NULL
);
GO

CREATE TABLE dbo.ComponentTypes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE,
    DisplayName NVARCHAR(80) NOT NULL,
    SortOrder INT NOT NULL DEFAULT 0
);
GO

-- [2] REUSABLE COMPONENT CATALOG
CREATE TABLE dbo.ComponentCatalogItems (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ComponentTypeId INT NOT NULL,
    ManufacturerId INT NOT NULL,
    ModelName NVARCHAR(255) NOT NULL,
    SpecSummary NVARCHAR(255) NULL,
    NormalizedValue DECIMAL(18,4) NULL,
    Unit NVARCHAR(30) NULL,
    MetadataJson NVARCHAR(MAX) NULL,
    CreatedAt DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    UpdatedAt DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),

    CONSTRAINT FK_ComponentCatalogItems_ComponentType
        FOREIGN KEY (ComponentTypeId) REFERENCES dbo.ComponentTypes(Id),
    CONSTRAINT FK_ComponentCatalogItems_Manufacturer
        FOREIGN KEY (ManufacturerId) REFERENCES dbo.Manufacturers(Id),
    CONSTRAINT UQ_ComponentCatalogItems UNIQUE (ComponentTypeId, ManufacturerId, ModelName)
);
GO

-- [3] FINISHED COMPUTER PRODUCT CATALOG
CREATE TABLE dbo.CatalogProducts (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    DeviceCategoryId INT NOT NULL,
    ProductName NVARCHAR(150) NOT NULL,
    Sku NVARCHAR(100) NULL UNIQUE,
    Description NVARCHAR(MAX) NULL,
    BaseWeightKg DECIMAL(10,2) NOT NULL,
    PerformanceTier NVARCHAR(50) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    UpdatedAt DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    RowVersion ROWVERSION,

    CONSTRAINT FK_CatalogProducts_DeviceCategory
        FOREIGN KEY (DeviceCategoryId) REFERENCES dbo.DeviceCategories(Id)
);
GO

-- [4] PRODUCT COMPOSITION / BUILDER SELECTIONS
CREATE TABLE dbo.ProductComponentSelections (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    CatalogProductId UNIQUEIDENTIFIER NOT NULL,
    ComponentTypeId INT NOT NULL,
    ComponentCatalogItemId INT NOT NULL,
    Quantity INT NOT NULL DEFAULT 1,
    SlotLabel NVARCHAR(100) NULL,
    DisplayOrder INT NOT NULL DEFAULT 0,
    StandardValue DECIMAL(18,4) NULL,
    Unit NVARCHAR(30) NULL,
    IsRequired BIT NOT NULL DEFAULT 1,

    CONSTRAINT CK_ProductComponentSelections_Quantity CHECK (Quantity > 0),
    CONSTRAINT FK_ProductComponentSelections_Product
        FOREIGN KEY (CatalogProductId) REFERENCES dbo.CatalogProducts(Id) ON DELETE CASCADE,
    CONSTRAINT FK_ProductComponentSelections_ComponentType
        FOREIGN KEY (ComponentTypeId) REFERENCES dbo.ComponentTypes(Id),
    CONSTRAINT FK_ProductComponentSelections_ComponentItem
        FOREIGN KEY (ComponentCatalogItemId) REFERENCES dbo.ComponentCatalogItems(Id),
    CONSTRAINT UQ_ProductComponentSelection UNIQUE (CatalogProductId, ComponentTypeId, ComponentCatalogItemId, SlotLabel)
);
GO

-- [5] PHYSICAL / OWNED ASSETS
CREATE TABLE dbo.InventoryAssets (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CatalogProductId UNIQUEIDENTIFIER NOT NULL,
    AssetTag NVARCHAR(100) NULL UNIQUE,
    SerialNumber NVARCHAR(100) NULL UNIQUE,
    AssetName NVARCHAR(255) NOT NULL,
    AssignedTo NVARCHAR(150) NULL,
    PhysicalLocation NVARCHAR(150) NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'In Stock',
    PurchaseDate DATE NULL,
    CreatedAt DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    UpdatedAt DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    RowVersion ROWVERSION,

    CONSTRAINT FK_InventoryAssets_CatalogProduct
        FOREIGN KEY (CatalogProductId) REFERENCES dbo.CatalogProducts(Id)
);
GO

-- [6] PERFORMANCE OPTIMIZATION
CREATE INDEX IX_ComponentCatalogItems_Type_Manufacturer
    ON dbo.ComponentCatalogItems(ComponentTypeId, ManufacturerId);

CREATE INDEX IX_CatalogProducts_DeviceCategory
    ON dbo.CatalogProducts(DeviceCategoryId, IsActive);

CREATE INDEX IX_ProductComponentSelections_Product_Type
    ON dbo.ProductComponentSelections(CatalogProductId, ComponentTypeId, DisplayOrder);

CREATE INDEX IX_InventoryAssets_Status
    ON dbo.InventoryAssets(Status, PhysicalLocation);
GO

-- [7] SPREADSHEET-STYLE READ MODEL
CREATE VIEW dbo.vwProductSpecificationSheet AS
SELECT
    cp.Id AS CatalogProductId,
    cp.ProductName,
    dc.Name AS DeviceCategory,
    MAX(CASE WHEN ct.Name = 'RAM'
        THEN CONCAT(COALESCE(CAST(pcs.StandardValue AS NVARCHAR(50)), cci.SpecSummary, cci.ModelName), COALESCE(CONCAT(' ', NULLIF(pcs.Unit, '')), ''))
        END) AS [RAM],
    MAX(CASE WHEN ct.Name = 'Storage' THEN cci.ModelName END) AS [Hard Disc],
    STRING_AGG(CASE WHEN ct.Name = 'Port' THEN CONCAT(pcs.Quantity, ' x ', cci.ModelName) END, ', ') AS [Ports],
    MAX(CASE WHEN ct.Name = 'GPU' THEN cci.ModelName END) AS [Video],
    cp.BaseWeightKg AS [WeightKg],
    MAX(CASE WHEN ct.Name = 'PSU' THEN cci.ModelName END) AS [Power Source],
    MAX(CASE WHEN ct.Name = 'CPU' THEN cci.ModelName END) AS [CPU]
FROM dbo.CatalogProducts cp
INNER JOIN dbo.DeviceCategories dc ON dc.Id = cp.DeviceCategoryId
LEFT JOIN dbo.ProductComponentSelections pcs ON pcs.CatalogProductId = cp.Id
LEFT JOIN dbo.ComponentTypes ct ON ct.Id = pcs.ComponentTypeId
LEFT JOIN dbo.ComponentCatalogItems cci ON cci.Id = pcs.ComponentCatalogItemId
GROUP BY cp.Id, cp.ProductName, dc.Name, cp.BaseWeightKg;
GO

-- [8] MINIMAL REFERENCE DATA
INSERT INTO dbo.Manufacturers (Name)
VALUES ('Intel'), ('AMD'), ('NVIDIA'), ('Generic'), ('Kingston'), ('Seagate'), ('Samsung');

INSERT INTO dbo.DeviceCategories (Name, Description)
VALUES
('Desktop PC', 'Traditional office or home desktop computer'),
('Laptop', 'Portable notebook computer'),
('Workstation', 'High-performance computer for demanding tasks'),
('Mini PC', 'Compact form-factor desktop');

INSERT INTO dbo.ComponentTypes (Name, DisplayName, SortOrder)
VALUES
('CPU', 'CPU', 10),
('RAM', 'RAM', 20),
('Storage', 'Hard Disc', 30),
('GPU', 'Video', 40),
('PSU', 'Power Source', 50),
('Port', 'Ports', 60);
GO
