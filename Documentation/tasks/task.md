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

## Task 7.10 - Use Genre Dropdown in Add Game Form

**Acceptance Criteria**

* Add Game form renders the `Genre` field as a dropdown/select.
* Dropdown options display genre names instead of raw genre IDs.
* Selecting a genre submits the corresponding `GenreId` internally.
* Dropdown includes all genres available in the supporting `Genres` table.
* Dropdown includes an empty/default option such as `Select a genre`.
* Game creation continues to persist `GenreId` without changing the database schema.
* Existing validation for required/valid `GenreId` remains enforced.
* Existing Author dropdown behavior remains unchanged.
* UI remains aligned with `Documentation/visual-reference`.
* Tests cover Add Game genre dropdown data and create flow preserving `GenreId`.
* Project builds successfully.
* Manual UI validation confirms the Add Game form displays genre names and saves the selected genre correctly.

---

## Task 7.11 - Use Publisher Dropdown in Game Forms

**Acceptance Criteria**

* Game form renders the `Publisher` field as a dropdown/select.
* Dropdown options display publisher names instead of raw publisher IDs.
* Selecting a publisher submits the corresponding `PublisherId` internally.
* Dropdown includes all publishers available in the supporting `Publishers` table.
* Dropdown includes an empty/default option such as `Select a publisher`.
* Game creation continues to persist `PublisherId` without changing the database schema.
* Existing unique rule for `Name + PublisherId` remains preserved.
* Existing Author and Genre dropdown behavior remains unchanged.
* UI remains aligned with `Documentation/visual-reference`.
* Tests cover publisher dropdown data and create flow preserving `PublisherId`.
* Project builds successfully.
* Manual UI validation confirms the game form displays publisher names and saves the selected publisher correctly.

---

## Task 7.12 - Align Games Filter Actions and Typography

**Acceptance Criteria**

* Games filter actions are grouped together in the same visual row.
* The `Include inactive` checkbox, `Search`, `Clear` when visible, and `Add Game` controls are left-aligned below the Genre and Publisher filters.
* Filter actions no longer spread across the row due to unwanted flexible layout behavior.
* Checkbox text and action button text use consistent sizing, weight, and alignment.
* Existing Games filters, pagination, and create navigation remain unchanged.
* UI remains aligned with `Documentation/visual-reference`.
* Project builds successfully.
* Manual UI validation confirms the Games filter action row stays aligned on desktop and remains usable on smaller screens.

---

## Task 7.13 - Stabilize Games Filter Action Row When Clear Button Is Visible

**Acceptance Criteria**

* Games filter action row remains visually aligned when `Clear` is visible.
* `Search`, `Clear`, and `Add Game` controls stay grouped with consistent spacing.
* The action row does not stretch, jump, or create awkward gaps when `Clear` appears or disappears.
* `Include inactive` remains aligned with the same action group.
* Button height, icon alignment, font size, and font weight remain consistent.
* The layout remains usable on smaller screens, wrapping as one coherent action group if needed.
* Existing filters, clear behavior, pagination, and Add Game navigation remain unchanged.
* UI remains aligned with `Documentation/visual-reference`.
* Project builds successfully.
* Manual UI validation confirms both hidden and visible `Clear` states.

---

## Task 7.14 - Use Publisher Dropdown Filter in Games List

**Acceptance Criteria**

* Games list Publisher filter is rendered as a dropdown/select.
* Dropdown options display publisher names instead of publisher IDs.
* Selecting a publisher submits/uses the corresponding `PublisherId` internally.
* Dropdown includes all publishers available in the supporting `Publishers` table.
* Dropdown includes an empty/default option to show all publishers.
* Publisher filter works together with existing filters.
* Existing pagination remains unchanged.
* Games table continues to display publisher names.
* Existing Game form publisher dropdown behavior remains unchanged.
* UI remains aligned with `Documentation/visual-reference`.
* Tests cover publisher dropdown data and filtering by selected publisher.
* Project builds successfully.
* Manual UI validation confirms the Games filter displays publisher names and filters correctly.

---

## Task 7.15 - Use Partial Game Name Filter in Matches List

**Acceptance Criteria**

* Matches list filter UI displays a `Game Name` filter instead of `Game Id`.
* The `Game Name` filter accepts partial text input.
* Filtering by game name returns matches whose related game title contains the entered text.
* Matching uses database-side partial matching behavior equivalent to `LIKE '%value%'`.
* Filtering is case-insensitive when supported by the configured SQL Server collation.
* Existing Match ID filter remains unchanged.
* Existing pagination remains unchanged.
* Existing Matches table display remains unchanged.
* Existing match creation and score editing behavior remains unchanged.
* Clear action resets the Game Name filter.
* Pagination links preserve the Game Name filter value.
* Tests cover partial game name filtering by a middle or ending part of the title.
* Project builds successfully.
* Manual UI validation confirms searching by part of a game title returns the expected matches.

---

## Task 7.16 - Remove Scores Column from Matches List

**Acceptance Criteria**

* Matches table no longer displays a `Scores` column header.
* Matches table rows no longer display score values in a dedicated column.
* Scores remain stored and available for match score editing.
* Existing `Edit Scores` action remains unchanged.
* Existing winner calculation and score update behavior remains unchanged.
* Existing Matches filters and pagination remain unchanged.
* Table column alignment remains visually clean after removing the column.
* Empty-state row colspan is updated to match the new column count.
* UI remains aligned with `Documentation/visual-reference`.
* Project builds successfully.
* Manual UI validation confirms the Matches list renders without the `Scores` column.

---

## Task 7.17 - Display Match Creation Date in Brazilian Date-Only Format

**Acceptance Criteria**

* Matches list `Created` column displays only the date.
* Date format is Brazilian day/month/year: `dd/MM/yyyy`.
* The time/hour is not displayed in the Matches list.
* The underlying `CreatedAt` value remains stored as date/time in the database.
* Match creation still sets `CreatedAt` from current server time.
* Editing match scores still does not change `CreatedAt`.
* Existing Matches filters and pagination remain unchanged.
* Existing match creation and score editing behavior remains unchanged.
* Tests cover formatting of a known `CreatedAt` value as `dd/MM/yyyy`.
* Project builds successfully.
* Manual UI validation confirms the Matches list shows dates like `14/05/2026`.

---

## Task 7.18 - Add Match Inspect Details Screen

**Acceptance Criteria**

* Matches table displays an inspect/view action button on each row.
* The inspect/view button uses an eye-style icon or equivalent inspect visual affordance.
* Clicking the inspect/view button opens a read-only match details screen.
* The details screen displays the game title.
* The details screen displays the match code/id.
* The details screen displays the match creation date.
* The details screen displays every match player with that player's corresponding score.
* Player names and scores preserve the positional relationship from `PlayerIds` and `Scores`.
* The details screen does not allow editing scores, game, players, or winner.
* The implementation uses the existing Matches repository, service, service interface, and existing Matches controller flow.
* The controller remains thin and does not contain business/data mapping logic.
* No database schema change or migration is introduced.
* UI remains aligned with the existing `Documentation/visual-reference` patterns until a specific future visual reference is provided.
* Tests cover loading match details and mapping players to their respective scores.
* Project builds successfully.
* Manual UI validation confirms the inspect button opens the details screen and displays the expected match information.

---

## Task 7.19 - Open Match Inspect Details in Modal Partial

**Acceptance Criteria**

* Matches table keeps an inspect/view action button on each row.
* Clicking the inspect/view button opens a modal instead of navigating to a separate details page.
* The modal displays the same match details content created for Task 7.18.
* Match details content is extracted into a reusable partial view.
* The partial view displays game title, match code/id, match creation date, winner, and all players with their respective scores.
* Player names and scores preserve the positional relationship from `PlayerIds` and `Scores`.
* The modal is read-only and does not allow editing scores, game, players, or winner.
* The modal can be closed without losing the current Matches list filters or pagination state.
* The implementation continues to use the existing Matches repository, service, service interface, and existing Matches controller flow.
* The controller remains thin and does not contain business/data mapping logic.
* No database schema change or migration is introduced.
* UI remains aligned with the existing `Documentation/visual-reference` modal patterns.
* Tests cover the partial/modal details action returning the expected match data.
* Project builds successfully.
* Manual UI validation confirms the inspect button opens and closes the modal and displays the expected match information.

---

## Task 7.20 - Add Authors Area with CRUD Flow

**Acceptance Criteria**

* Authors appears as a new item in the left sidebar navigation.
* Authors list page displays existing authors.
* Authors list supports pagination using the project default page size.
* Authors list supports filtering by partial author name.
* Create Author flow works and persists a new author.
* Edit Author flow works and updates the author name.
* Details/View flow displays author data in read-only mode.
* Delete flow removes authors only when they are not referenced by games.
* Delete flow rejects authors already used by games with a clear validation message.
* Author names are required.
* Author names are unique.
* Implementation uses repository, service, service interface, controller, view models, and views.
* Controllers remain thin and do not contain business/data mapping logic.
* Existing Game author dropdown behavior remains unchanged.
* UI remains aligned with `Documentation/visual-reference`.
* Tests cover repository behavior, service validation, controller flow, and delete rejection for authors used by games.
* Project builds successfully.
* Manual UI validation confirms Authors navigation and CRUD screens render correctly.

---
