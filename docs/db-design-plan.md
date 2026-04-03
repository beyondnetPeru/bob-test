# Database Design & Normalization Plan (Unified Product Model)

**Current Status:** [REFACTORED - DESIGN]
**Context:** Scene 1 - Unified Database Architecture for the Hardware Inventory System.

## The Unified Relational Model (3NF)

Based on the architectural pivot, we have unified all specific component types (Processors, Graphics Cards, Port Types, Storage Types) into a single, highly flexible **"Products"** catalog. This model is optimized for rapid hardware evolution and diverse component configurations.

### 1. Standardization Core

- **Manufacturers**: Central lookup for all component brands (Intel, AMD, NVIDIA, etc.).
- **ProductCategories**: Defines the _nature_ of the component (e.g., `CPU`, `GPU`, `RAM`, `STORAGE`, `PORT`, `PSU`).

### 2. The Universal Catalog

- **Products**:
  - `CategoryId` (FK) - Link to Category.
  - `ManufacturerId` (FK) - Link to Manufacturer.
  - `ModelName` - Specific model (e.g., "Core i7-6700K", "USB 3.0", "500GB SSD").
- **Rationale:** This allows the system to treat a CPU and a Port type with the same relational gravity, simplifying queries and reporting across the inventory.

### 3. Hardware Asset (The Host Machine)

- **HardwareInventory**:
  - `Name` / `Serial` (Unique identifiers for the machine)
  - `WeightKg` (DECIMAL - Standardized machine weight)
  - `PerformanceTier` (Calculated field)
  - `CreatedAt` / `UpdatedAt` (Shadow properties/Audit columns)

### 4. Configuration Mapping (Dynamic Composition)

- **AssetConfigurations**:
  - `InventoryId` (FK)
  - `ProductId` (FK)
  - `Quantity` (INT) - How many of this product are in the machine.
  - `StandardValue` (DECIMAL) - Standardized capacity or metric (e.g., 16.0 for RAM, 1000.0 for Storage).
  - `Location` (STRING) - Metadata for physical position (e.g., "DIMM 1", "Front Panel", "Internal Slot").
- **Rationale:** This junction table is the most powerful part of the schema. It allows for any combination of components, providing the "Product-centric" view requested while maintaining deep traceability.

---

## Performance Optimization Strategy

1. **Indexes:**
   - `IX_Config_Inventory_Product`: Composite index for rapid spec retrieval.
   - `IX_Products_CategoryId`: For filtering inventory by specific component types (e.g., "Show all machines with GPUs").
2. **Audit Ready:**
   - Shadow properties are implemented via `RowVersion` and `DateTimeOffset` in the DDL to support concurrency and historical tracking across all configuration changes.
