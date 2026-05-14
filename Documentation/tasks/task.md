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

## UI Reference Requirement

For every UI-related task in this section, the implementation must use the images and code in `Documentation/visual-reference` as the visual and structural reference.

Reference assets currently include:

* `player-tab-reference.jpg`
* `player-tab-reference.html`
* `new-match-reference.png`
* `new-match-reference.html`
* `new-match-modal-reference.png`
* `new-match-modal-reference.html`

The UI does not need to copy these files verbatim, but layout, spacing, visual hierarchy, component behavior, and interaction patterns must be aligned with them unless a task explicitly says otherwise.

---

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

# 7. Improvements

## Task 7.1 — Update Player Phone Layout to Brazilian Format

**Acceptance Criteria**

* ✔ Player phone/WhatsApp input uses the Brazilian phone layout.
* ✔ Expected layout example: `(32) 9 1111-1111`.
* ✔ Player list displays phone/WhatsApp values using the Brazilian layout.
* ✔ Player phone/WhatsApp filter placeholder uses the Brazilian layout.
* ✔ Existing uniqueness rule for `WhatsApp` remains preserved.
* ✔ Tests cover valid Brazilian phone values and relevant invalid formats.

---

## Task 7.2 — Add Automatic Brazilian Phone Mask to Player Fields

**Acceptance Criteria**

* ✔ Player create WhatsApp field auto-formats raw digits while typing.
* ✔ Player edit WhatsApp field auto-formats raw digits while typing.
* ✔ The field accepts 10 or 11 digits total.
* ✔ 10 digits are formatted as `(32) 1111-1111`.
* ✔ 11 digits are formatted as `(32) 9 1111-1111`.
* ✔ Incomplete or unsupported phone lengths are rejected by validation.
* ✔ Existing uniqueness rule for `WhatsApp` remains preserved.
* ✔ Tests cover both 10-digit and 11-digit Brazilian phone values.

---

## Task 7.3 — Add Match Creation Timestamp

**Acceptance Criteria**

* ✔ Match entity includes a required `CreatedAt` field.
* ✔ New matches set `CreatedAt` from the current server time when created.
* ✔ Match creation input does not require users to provide `CreatedAt`.
* ✔ Match view/list DTO exposes `CreatedAt`.
* ✔ Match table displays the match creation date/time.
* ✔ Editing match scores does not change `CreatedAt`.
* ✔ Database schema is updated with a migration.
* ✔ Tests cover timestamp creation and preservation during score edits.

---

## Task 7.4 — Display Genre and Publisher Names in Games Table

**Acceptance Criteria**

* ✔ Games table displays the genre name instead of `GenreId`.
* ✔ Games table displays the publisher name instead of `PublisherId`.
* ✔ Game create/edit flow continues to store FK IDs.
* ✔ Game list DTO or view model exposes genre and publisher display names.
* ✔ Existing game filters continue to work.
* ✔ Tests cover genre and publisher names in game list data.

---

## Task 7.5 — Increment Game Times Played on Match Creation

**Acceptance Criteria**

* ✔ Creating a match increments the selected Game `TimesPlayed` by 1.
* ✔ Failed match creation does not increment `TimesPlayed`.
* ✔ Editing match scores does not change `TimesPlayed`.
* ✔ Existing match validation behavior remains unchanged.
* ✔ Tests cover successful increment and non-increment cases.

---

## Task 7.6 — Add Author FK and Dropdown to Games

**Acceptance Criteria**

* ✔ Authors supporting table exists.
* ✔ Game stores `AuthorId` as an FK instead of free-text author.
* ✔ Add Game displays author names in a dropdown.
* ✔ Add Game submits selected `AuthorId` to the backend.
* ✔ Game create/edit flow persists `AuthorId`.
* ✔ Games table displays author name.
* ✔ Invalid author IDs are rejected by database or service validation.
* ✔ Migration updates the schema safely.
* ✔ Tests cover author FK mapping and dropdown-backed create flow.

---

## Task 7.7 - Remove Account-Related Items from Topbar

**Acceptance Criteria**

* The topbar no longer displays the notification bell icon.
* The topbar no longer displays the help/question icon.
* The topbar no longer displays the user/avatar/profile placeholder.
* The layout remains visually aligned after removing the right-side items.
* No authentication, logged-user, account, or notification affordance remains in the base layout.
* Unused CSS related only to the removed topbar account items is removed or left only if still used elsewhere.
* Project builds successfully.
* Manual UI validation confirms the topbar, sidebar, and page content still render correctly.

---

## Task 7.8 - Display Game and Player Names in Matches List

**Acceptance Criteria**

* The Matches table displays the game name as the primary text in the Match column.
* The Matches table no longer uses generic primary labels such as `Match #1` when a game name is available.
* The Players column displays player full names instead of raw comma-separated player IDs.
* Player names preserve the same order as the stored `PlayerIds` sequence.
* Raw IDs may remain only as secondary metadata when useful.
* Existing match filters and pagination continue to work.
* Existing match creation and score editing behavior remains unchanged.
* Tests cover game name and player name mapping for match list data.
* Project builds successfully.
* Manual UI validation confirms the Matches list displays human-readable names.

---

## Task 7.9 - Use Genre Dropdown Filter in Games List

**Acceptance Criteria**

* Games list Genre filter is rendered as a dropdown/select.
* Dropdown options display genre names instead of genre IDs.
* Selecting a genre submits/uses the corresponding `GenreId` internally.
* Dropdown includes all genres available in the supporting `Genres` table.
* Dropdown includes an empty/default option to show all genres.
* Genre filter works together with existing filters.
* Existing pagination remains unchanged.
* Games table continues to display genre names.
* UI remains aligned with `Documentation/visual-reference`.
* Tests cover genre dropdown data and filtering by selected genre.
* Project builds successfully.
* Manual UI validation confirms the Games filter displays genre names and filters correctly.

---
