# Database Design & Normalization Plan (Computer Catalog & Composition Model)

**Current Status:** [UPDATED - BUSINESS MODEL ALIGNED]
**Context:** The spreadsheet rows represent full computer products that can be browsed by category and composed from typed components.

## The Target Relational Model (3NF)

The schema is now centered on a clearer distinction between:

1. **Sellable/composable computer products** such as `Desktop PC`, `Laptop`, and `Workstation`
2. **Reusable component catalog items** such as CPUs, GPUs, RAM modules, storage drives, ports, and power supplies
3. **Real inventory assets** that point to a chosen product model

---

## 1. Core Reference Tables

### `Manufacturers`

Central lookup for brands such as Intel, AMD, NVIDIA, Kingston, Dell, Lenovo, and Generic.

### `DeviceCategories`

Represents the user-facing computer grouping:

- `Desktop PC`
- `Laptop`
- `Workstation`
- `Mini PC`
- `All-in-One`

### `ComponentTypes`

Represents the technical slot/type within a computer:

- `CPU`
- `RAM`
- `Storage`
- `GPU`
- `PSU`
- `Port`

---

## 2. Component Catalog

### `ComponentCatalogItems`

Stores reusable parts that can be selected when composing a machine.

Key attributes:

- `ComponentTypeId`
- `ManufacturerId`
- `ModelName`
- `SpecSummary`
- `NormalizedValue`
- `Unit`
- `MetadataJson`

Examples:

- `Intel Core i7-6700K` as a `CPU`
- `1 TB SSD` as a `Storage` item
- `USB 3.0` as a `Port`
- `500 W PSU` as a `PSU`

**Rationale:** This avoids duplication and allows the same component to be reused across many computer products.

---

## 3. Product Catalog (The Finished Computer)

### `CatalogProducts`

Represents the full machine shown in the UI and filtered by product category.

Key attributes:

- `DeviceCategoryId`
- `ProductName`
- `Sku`
- `Description`
- `BaseWeightKg`
- `PerformanceTier`
- `IsActive`

Examples:

- `Office Desktop A`
- `Gaming Workstation B`
- `Portable Laptop C`

**Rationale:** This is the correct business entity for the spreadsheet-style list. Each row in the image should map to one `CatalogProduct`.

---

## 4. Product Composition Mapping

### `ProductComponentSelections`

This join table links a `CatalogProduct` to its chosen components.

Key attributes:

- `CatalogProductId`
- `ComponentTypeId`
- `ComponentCatalogItemId`
- `Quantity`
- `SlotLabel`
- `DisplayOrder`
- `StandardValue`
- `Unit`
- `IsRequired`

Examples:

- one desktop has one `CPU`
- one desktop has one `GPU`
- one desktop has two USB 3.0 ports and four USB 2.0 ports
- one desktop has one `1 TB SSD`

**Rationale:** This is the composition engine of the system. It powers both the spreadsheet view and the “build a computer” workflow.

---

## 5. Real Fleet / Owned Devices

### `InventoryAssets`

Represents actual physical devices in stock or assigned to users.

Key attributes:

- `CatalogProductId`
- `AssetTag`
- `SerialNumber`
- `AssetName`
- `AssignedTo`
- `PhysicalLocation`
- `Status`
- `PurchaseDate`

**Rationale:** This cleanly separates the reusable product model from the real machines being tracked operationally.

---

## 6. Spreadsheet-Compatible Output

To support the exact visual layout shown in the image, the schema includes a flattened read model / SQL view that returns:

- `ProductName`
- `Category`
- `RAM`
- `Hard Disc`
- `Ports`
- `Video`
- `Weight`
- `Power Source`
- `CPU`

This view is ideal for the frontend grid and for simple business reporting.

---

## 7. Performance & Query Strategy

1. **Indexes**

   - `IX_ComponentCatalogItems_Type_Manufacturer`
   - `IX_CatalogProducts_DeviceCategory`
   - `IX_ProductComponentSelections_Product_Type`
   - `IX_InventoryAssets_Status`

2. **Why this model fits the use case**

   - supports listing products by business category
   - supports reusable component catalog management
   - supports a computer builder/composer UI
   - supports later expansion for laptops, monitors, or other device families

3. **Audit readiness**
   - `CreatedAt`, `UpdatedAt`, and `RowVersion` remain available for operational traceability and optimistic concurrency.
