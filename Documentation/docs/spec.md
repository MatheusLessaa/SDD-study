# Board Game Results Aggregator — Spec

## 1. Overview

This application is a **server-side ASP.NET MVC web application** designed to manage board game results locally.

The system allows users to:

* Manage Players
* Manage Games
* Register and manage Matches (Partidas)

The application does **not include authentication** and is intended for **local usage only** in its first version.

---

## 2. Architecture

The system follows a **layered MVC architecture per context**, where each domain contains:

* Repository
* Repository Interface
* Service
* Service Interface
* Controller
* Area (Views organized per context)

Each context is **isolated and self-contained**.

---

## 3. Database

### 3.1 General

* Database: SQL Server
* Approach: Simple relational model with some denormalization for Matches

---

## 4. Context: Players

### 4.1 Entity: Player

| Field    | Type   | Rules            |
| -------- | ------ | ---------------- |
| Id       | INT    | Primary Key      |
| FullName | STRING | Required, UNIQUE |
| WhatsApp | STRING | Required, UNIQUE |
| IsActive | BOOL   | Default: true    |

---

### 4.2 Business Rules

* FullName must be unique
* WhatsApp must be unique
* Players cannot be physically deleted
* Deletion = set `IsActive = false`
* Inactive players:

  * Do not appear in selection lists
  * Still exist in historical matches

---

### 4.3 Features

* List players (max 20 per page)
* Pagination (fixed size = 20)
* Filters:

  * FullName (partial match)
  * WhatsApp (partial match)
* Sorting:

  * Default: FullName ASC
* Actions:

  * Create
  * Edit
  * Deactivate

---

## 5. Context: Games

### 5.1 Entity: Game

| Field       | Type   | Rules        |
| ----------- | ------ | ------------ |
| Id          | INT    | Primary Key  |
| Name        | STRING | Required     |
| PublisherId | INT    | FK           |
| GenreId     | INT    | FK           |
| Author      | STRING | Required     |
| TimesPlayed | INT    | Default 0    |
| MaxPlayers  | INT    | Required     |
| IsActive    | BOOL   | Default true |

---

### 5.2 Business Rules

* Name + PublisherId must be unique
* Games cannot be deleted
* Deletion = set `IsActive = false`
* Inactive games:

  * Do not appear in selection lists
  * Still exist in historical matches

---

### 5.3 Features

* List games (max 20 per page)
* Filters:

  * Name (partial)
  * Publisher
  * Author (partial)
  * Genre
  * Id
* Sorting:

  * Default: Name ASC
* Actions:

  * Create
  * Edit
  * Deactivate

---

## 6. Context: Matches (Partidas)

### 6.1 Entity: Match

| Field          | Type   | Rules                            |
| -------------- | ------ | -------------------------------- |
| Id             | INT    | Primary Key                      |
| GameId         | INT    | FK                               |
| PlayerIds      | STRING | Comma-separated (e.g., "1,5,8")  |
| Scores         | STRING | Comma-separated (e.g., "10,7,3") |
| WinnerPlayerId | INT    | FK                               |

---

### 6.2 Data Format Rules

* PlayerIds and Scores must:

  * Have the same number of elements
  * Maintain positional consistency

* Example:

  * PlayerIds = "1,5,8"
  * Scores = "10,7,3"
  * Player 1 → Score 10
  * Player 5 → Score 7
  * Player 8 → Score 3

* If score is not provided:

  * Default value = 0

---

### 6.3 Business Rules

* A match must contain at least 1 player
* Number of players must not exceed Game.MaxPlayers
* WinnerPlayerId must:

  * Exist in PlayerIds list
* Scores:

  * Always required logically (default = 0)

---

### 6.4 Editing Rules

* It is NOT allowed to change:

  * Game
  * Players

* It is allowed to change:

  * Scores only

* Winner must be recalculated after score update:

  * Highest score wins
  * In case of tie (same highest score), the winner is the tied player whose `FullName` comes first in ascending alphabetical order.

---

### 6.5 Features

* List matches (max 20 per page)

* Filters:

  * Match Id
  * Game Id
  * Game Name (via join)
  * Player Name [FUTURE]

* Sorting:

  * Default: Id DESC

* Actions:

  * Create
  * Edit (scores only)
  * Delete [NEED CLARIFICATION: hard delete vs soft delete]

---

### 6.6 Create Match Flow

1. Select Game
2. Add Players via modal
3. System enforces:

   * No duplicate players
   * Respect MaxPlayers
4. Input Scores (optional → default = 0)
5. System calculates Winner automatically

---

### 6.7 Player Selection Modal

* Based on Players list screen
* Supports:

  * Filtering
  * Pagination
* Multi-selection enabled
* Constraints:

  * Cannot select same player twice
  * Cannot exceed Game.MaxPlayers

---

## 7. Context: Supporting Tables

### 7.1 Genres

| Field | Type   |
| ----- | ------ |
| Id    | INT    |
| Name  | STRING |

---

### 7.2 Publishers

| Field | Type   |
| ----- | ------ |
| Id    | INT    |
| Name  | STRING |

---

### 7.3 Rules

* Initially static (seeded data)
* Not editable via UI in v1
* [FUTURE] Will support CRUD operations

---

## 8. Validation Rules (Global)

* All list endpoints must:

  * Enforce pagination (20 items)
* All filters must:

  * Be combinable
  * Use partial match when applicable
* Backend must validate:

  * MaxPlayers constraint
  * Winner consistency
  * Player uniqueness in match

---

## 9. Testing

### 9.1 Strategy

* Unit Tests:

  * Services layer
* Integration Tests:

  * Controllers

### 9.2 Framework

* xUnit

---

## 10. Non-Functional Requirements

* Application must run locally
* No authentication required
* Simple UI (no SPA required)
* UI implementation must use the images and code in `Documentation/visual-reference` as the visual and structural reference for layout, components, spacing, and interaction patterns.
* Maintainable modular structure (per context MVC)

---

## 11. Future Enhancements

* Player statistics (wins, matches)
* Genre-driven rules (cooperative games, no winner, etc.)
* Editable Genres and Publishers
* Advanced match filtering (by player name)
* Replace string-based player storage with relational model

---

## 12. Out of Scope

* Authentication / Authorization
* Real-time updates
* External integrations (WhatsApp APIs, etc.)

---
