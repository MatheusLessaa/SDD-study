# Tasks — Board Game Results Aggregator (Detailed Acceptance Criteria)

---

# 1. Project Setup

## Task 1.1 — Create Solution Structure

**Acceptance Criteria**

* ✔ Solution `BoardGameApp` is created
* ✔ Projects exist:

  * Web (ASP.NET MVC)
  * Application (Class Library)
  * Domain (Class Library)
  * Infrastructure (Class Library)
* ✔ Project references:

  * Web → Application
  * Application → Domain
  * Infrastructure → Domain + Application
* ✔ Solution builds without errors
* ✔ Solution runs without runtime errors

---

## Task 1.2 — Install Dependencies

**Acceptance Criteria**

* ✔ EF Core packages installed in Infrastructure and Web
* ✔ xUnit available for test generation
* ✔ No version conflicts
* ✔ Solution builds successfully

---

## Task 1.3 — Configure DbContext

**Acceptance Criteria**

* ✔ `AppDbContext` exists in Infrastructure
* ✔ DbContext configured with SQL Server provider
* ✔ Connection string configured in Web
* ✔ DbContext registered in dependency injection
* ✔ Application starts successfully

---

## Task 1.4 — Enable Migrations

**Acceptance Criteria**

* ✔ Migrations enabled
* ✔ Initial migration created
* ✔ Database created successfully
* ✔ Migration applied without errors
* ✔ Application connects to database

---

# 2. Domain Layer

## Task 2.1 — Create Player Entity

**Acceptance Criteria**

* ✔ Entity contains all required fields
* ✔ Properties use correct data types
* ✔ Entity compiles
* ✔ Matches spec (including IsActive)

---

## Task 2.2 — Create Game Entity

**Acceptance Criteria**

* ✔ All fields implemented
* ✔ FK fields included (PublisherId, GenreId)
* ✔ Entity compiles

---

## Task 2.3 — Create Match Entity

**Acceptance Criteria**

* ✔ Fields implemented as defined
* ✔ PlayerIds and Scores are strings
* ✔ Entity compiles

---

## Task 2.4 — Create Genre Entity

## Task 2.5 — Create Publisher Entity

**Acceptance Criteria**

* ✔ Entities exist
* ✔ Fields correctly defined
* ✔ Entities compile

---

# 3. Infrastructure Layer

## Task 3.1 — Configure DbSets

**Acceptance Criteria**

* ✔ All entities mapped in DbContext
* ✔ DbSets accessible
* ✔ No runtime mapping errors

---

## Task 3.2 — Configure Entity Constraints

**Acceptance Criteria**

* ✔ Unique constraint on Player.FullName
* ✔ Unique constraint on Player.WhatsApp
* ✔ Composite unique constraint on Game (Name + PublisherId)
* ✔ Constraints validated at DB level

---

## Task 3.3 — Create Initial Migration with Schema

**Acceptance Criteria**

* ✔ Tables created:

  * Players
  * Games
  * Matches
  * Genres
  * Publishers
* ✔ Constraints applied correctly
* ✔ Migration runs successfully

---

## Task 3.4 — Seed Initial Data

**Acceptance Criteria**

* ✔ 3 Genres inserted
* ✔ 3 Publishers inserted
* ✔ Data exists after migration
* ✔ No duplication on re-run

---

## Task 3.5 — Create PlayerRepository

**Acceptance Criteria**

* ✔ Supports Create, Update, GetById
* ✔ Supports paginated listing (20 items)
* ✔ Supports filtering (Name, WhatsApp)
* ✔ Filters are combinable
* ✔ Only active players returned by default
* ✔ No runtime errors

---

## Task 3.6 — Create GameRepository

**Acceptance Criteria**

* ✔ Supports CRUD operations (soft delete)
* ✔ Supports filters (Name, Author, Genre, Publisher, Id)
* ✔ Pagination implemented
* ✔ Only active games returned by default

---

## Task 3.7 — Create MatchRepository

**Acceptance Criteria**

* ✔ Supports Create, Update, GetById
* ✔ Supports paginated listing
* ✔ Supports filtering (Id, GameId)
* ✔ No runtime errors

---

# 4. Application Layer

## Task 4.1 — Create Player DTOs

**Acceptance Criteria**

* ✔ DTOs created for Create, Update, View
* ✔ DTOs do not expose Entity directly
* ✔ Proper field mapping possible

---

## Task 4.2 — Create Game DTOs

## Task 4.3 — Create Match DTOs

**Acceptance Criteria**

* ✔ DTOs exist
* ✔ DTOs align with spec fields

---

## Task 4.4 — Create PlayerService

**Acceptance Criteria**

* ✔ Enforces unique FullName
* ✔ Enforces unique WhatsApp
* ✔ Implements deactivate logic
* ✔ Prevents hard delete
* ✔ Throws meaningful errors
* ✔ Unit tests cover basic validation

---

## Task 4.5 — Create GameService

**Acceptance Criteria**

* ✔ Enforces Name + Publisher uniqueness
* ✔ Handles activation/deactivation
* ✔ Validates MaxPlayers
* ✔ Unit tests created

---

## Task 4.6 — Create MatchService

**Acceptance Criteria**

* ✔ Parses PlayerIds correctly
* ✔ Parses Scores correctly
* ✔ Default score = 0 when empty
* ✔ Prevents duplicate players
* ✔ Validates MaxPlayers constraint
* ✔ Winner calculated correctly
* ✔ Winner belongs to PlayerIds
* ✔ Throws error when mismatch occurs
* ✔ Unit tests validate:

  * Winner calculation
  * Duplicate players
  * Score mismatch

---

## Task 4.7 — Implement Match Score Update Logic

**Acceptance Criteria**

* ✔ Only scores can be updated
* ✔ Player list remains unchanged
* ✔ Winner recalculated correctly
* ✔ No side effects on Game or Players
* ✔ Unit tests validate behavior

---

## Task 4.8 — Create Unit Tests for Services

**Acceptance Criteria**

* ✔ Tests exist for all Services
* ✔ Basic rules validated
* ✔ All tests pass
* ✔ No failing test cases

---

# 5. Web Layer

## Task 5.1 — Configure Dependency Injection

**Acceptance Criteria**

* ✔ All Services registered
* ✔ All Repositories registered
* ✔ Application runs without DI errors

---

## Task 5.2 — Create Base Layout

**Acceptance Criteria**

* ✔ Sidebar implemented (left)
* ✔ Links:

  * Players
  * Games
  * Matches
* ✔ Main content renders correctly
* ✔ Layout reusable across pages

---

## Task 5.3 — Create Players Area

## Task 5.4 — Create PlayerController

## Task 5.5 — Create Player Views

**Acceptance Criteria**

* ✔ List page displays players
* ✔ Create page works
* ✔ Edit page works
* ✔ Deactivate action works
* ✔ Navigation works correctly

---

## Task 5.6 — Implement Player Filters & Pagination

**Acceptance Criteria**

* ✔ Max 20 records per page
* ✔ Filters work individually
* ✔ Filters work combined
* ✔ Pagination works correctly

---

## Task 5.7 — Create Games Area / Controller / Views

**Acceptance Criteria**

* ✔ CRUD operations functional
* ✔ Deactivation works
* ✔ UI consistent with Players

---

## Task 5.8 — Implement Game Filters

**Acceptance Criteria**

* ✔ All filters work
* ✔ Filters combinable
* ✔ Pagination respected

---

## Task 5.9 — Create Matches Area

## Task 5.10 — Create MatchController

## Task 5.11 — Create Match Views

**Acceptance Criteria**

* ✔ List matches displayed
* ✔ Create flow functional
* ✔ Edit (scores only) functional

---

## Task 5.12 — Implement Player Selection Modal

**Acceptance Criteria**

* ✔ Modal opens correctly
* ✔ Displays player list
* ✔ Supports filtering
* ✔ Multi-select enabled
* ✔ Prevents duplicate selection
* ✔ Enforces MaxPlayers limit

---

## Task 5.13 — Implement Match Creation Flow

**Acceptance Criteria**

* ✔ Game selection works
* ✔ Players added via modal
* ✔ Scores input works
* ✔ Winner auto-calculated
* ✔ Match saved successfully

---

## Task 5.14 — Implement Match Editing (Scores Only)

**Acceptance Criteria**

* ✔ Only scores editable
* ✔ Players and Game locked
* ✔ Winner recalculated
* ✔ Changes persisted

---

## Task 5.15 — Implement Match Filters

**Acceptance Criteria**

* ✔ Filter by Id works
* ✔ Filter by GameId works
* ✔ Pagination works
* ✔ [FUTURE] Player name filter not required

---

# 6. Global Validation Gates

Every task must satisfy:

* ✔ Project builds successfully
* ✔ No runtime exceptions
* ✔ Business rules enforced
* ✔ Code compiles cleanly
* ✔ Tests pass (when applicable)

---
