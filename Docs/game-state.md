<!--
This document defines the complete game state machine architecture
and its mapping to Unity scenes and scene transitions.
-->

# Game State Machine Architecture

## Overview

The Racing Manager uses a **finite state machine (FSM)** to manage game progression across three gameplay loops:

1. **Tactical (Race Day)** - Real-time race decisions
2. **Operational (Daily Management)** - Resource allocation between events
3. **Strategic (Seasonal)** - Long-term team progression

The state machine maintains a `GameContext` that persists across all states, allowing seamless transitions while keeping the game state synchronized.

## Core State Machine Definition

```typescript
type GameState = 
  | MainMenuState
  | SeasonState
  | WeekState
  | RaceDayState;

interface StateMachine {
  current: GameState;
  previous?: GameState;
  context: GameContext;
  transitions: TransitionMap;
}

interface GameContext {
  season: number;
  currentDate: Date;
  resources: ResourcePool;
  team: TeamData;
  vehicles: VehicleData[];
  sponsors: SponsorData[];
  history: GameEvent[];
}
```

## State Definitions

### MainMenuState
**Type:** `"MainMenu"`

Entry point for the game. Handles new game creation and save file loading.

```typescript
interface MainMenuState {
  type: "MainMenu";
  newGameOptions?: NewGameConfig;
}
```

**Valid Transitions:**
- → `SeasonState` on "startNewGame" or "loadGame"
- → Exit to main menu scene

---

### SeasonState
**Type:** `"Season"`

Manages the entire competitive season. Player engages in sponsor negotiations, research tree development, and season-level planning.

```typescript
interface SeasonState {
  type: "Season";
  seasonNumber: number;
  weeks: Week[];
  seasonGoals: Objective[];
  rosterChanges: RosterEvent[];
  sponsorNegotiations: SponsorDeal[];
}
```

**Valid Transitions:**
- → `WeekState` on "advanceToWeek(n)"
- → `RaceDayState` on "skipToNextRaceDay"
- → `MainMenuState` on "returnToMainMenu" (saves game)

**UI Interactions:**
- Manage sponsor relationships
- View and upgrade R&D tech tree
- Review season objectives and standings
- Adjust team roster

---

### WeekState
**Type:** `"Week"`

The main hub for weekly gameplay. Combines the week overview with operational management (crew assignments, maintenance, purchases, training). The player views the week's schedule, manages their team, and enters races as desired.

```typescript
interface WeekState {
  type: "Week";
  weekNumber: number;
  currentDay: DayOfWeek;
  scheduledRaces: RaceEvent[];
  crewAssignments: CrewTask[];
  maintenance: MaintenanceQueue[];
  purchases: Purchase[];
  trainingSchedule: TrainingSession[];
  resourceDelta: ResourceChange;
}
```

**Valid Transitions:**
- → `RaceDayState` on "enterRaceDay"
- → `SeasonState` on "completeWeek" (progresses to next week or end-of-season)
- → `MainMenuState` on "pauseToMenu" (saves game)

**UI Interactions - Overview:**
- View week calendar and scheduled races
- Check resource changes for the week
- Quick-skip to next race day
- See team morale and fatigue indicators

**UI Interactions - Operations Management:**
- Assign crew members to vehicles or training
- Queue vehicle maintenance and repairs
- Purchase performance parts
- Schedule driver training sessions
- Review team skill progression and morale

---

### RaceDayState
**Type:** `"RaceDay"`

Represents the tactical race day loop. Uses turn-based mechanics where each run lasts ~4 seconds of simulation, then pauses to allow the player to review and adjust setup for the next run.

```typescript
interface RaceDayState {
  type: "RaceDay";
  raceId: string;
  bracket: BracketRound[];
  currentMatch: number;
  currentRun: number; // Best-of-three runs per match
  racePhase: "garage" | "running" | "results";
  playerSetup: {
    tireChoice: TireType;
    gearRatio: number;
    nitrousUsage: NitrousStrategy;
    driverMindset: "aggressive" | "conservative" | "balanced";
  };
  currentRunResult?: RunResult; // Elapsed time, telemetry, opponent comparison
  matchResults?: MatchResult[]; // Best-of-three results
  raceResults?: RaceResult; // Bracket completion results
}
```

**Valid Transitions:**
- → `RaceDayState` on "runNext" (player confirms setup and runs next race)
- → `RaceDayState` on "nextMatch" (match complete, advances through bracket)
- → `WeekState` on "completeRaceDay" (all bracket matches finished)
- → `MainMenuState` on "pauseToMenu" (saves game, can resume from same garage state)

**UI Interactions - Garage Phase:**
- Select tires and view their properties (grip, wear, cost)
- Adjust gear ratio within vehicle's capability range
- Configure nitrous usage strategy (when to deploy, intensity)
- Set driver mindset (affects aggression in sim)
- Review setup change history and recommended adjustments

**UI Interactions - Running Phase:**
- Watch 4-second race simulation with telemetry overlay
- Can't make changes while run is executing
- View opponent's car and real-time comparison

**UI Interactions - Results Phase:**
- Review run telemetry: launch reaction, acceleration profile, top speed
- See win/loss and time differential vs opponent
- For best-of-three, see current match record
- Option to adjust setup and run again, or accept result and proceed
- For final match victory, see prize money and reputation gain

---

## State Transition Map

```
MainMenu
  ├─ New Game → SeasonState (Season 1, Week 1)
  └─ Load Game → [Saved State]

SeasonState
  ├─ Manage Sponsors → [Dialog/Modal]
  ├─ View R&D Tree → [Transient Research UI]
  ├─ Advance to Week → WeekState
  ├─ Skip to Next Race → RaceDayState
  ├─ End Season → Victory/Failure Screen → MainMenu
  └─ Pause → MainMenu

WeekState (Hub)
  ├─ Manage Crew → [Crew Assignment UI]
  ├─ Queue Maintenance → [Maintenance UI]
  ├─ Purchase Parts → [Shop UI]
  ├─ Train Driver → [Training Dialog]
  ├─ Enter Race → RaceDayState
  ├─ Skip to Next Race → RaceDayState (fast-forward)
  ├─ Skip Day → WeekState (same state, next day)
  ├─ Complete Week → SeasonState
  └─ Pause → MainMenu

RaceDayState
  ├─ Garage Phase (Turn-Based)
  │  ├─ Adjust Tuning → [Garage UI]
  │  ├─ Review Previous Runs → [Telemetry UI]
  │  └─ Confirm Settings → Running Phase
  ├─ Running Phase (4-Second Simulation)
  │  ├─ Execute Run → Results Phase
  │  └─ (Player waits, no input)
  ├─ Results Phase (Turn-Based)
  │  ├─ Review Telemetry → [Detailed Run Data]
  │  ├─ Run Again → Garage Phase (adjust setup)
  │  ├─ Accept Result → Next Match (if best-of-three complete)
  │  ├─ Run Out → Next Match (opponent forfeits remaining runs)
  │  └─ Quit Match → WeekState (forfeit tournament)
  ├─ Match Complete
  │  ├─ Advance to Next Match → Garage Phase
  │  └─ Tournament Complete → WeekState (view final rewards)
  └─ Pause (Any Phase) → MainMenu (resume returns to same garage)
```

---

## Unity Scene Architecture

### Persistent Scenes (Load Once)

These scenes load at game startup and persist throughout the entire gameplay session:

| Scene | Responsibility |
|-------|-----------------|
| `GameManager` | State machine orchestration, context persistence, main event dispatcher |
| `AudioManager` | Music and SFX pooling, dynamic audio state management |
| `PersistenceManager` | Save/load system, serialization of GameContext |

### Transient Scenes (Load/Unload per State)

These scenes load additively based on the current game state and unload when transitioning away:

| State | Associated Scenes |
|-------|-------------------|
| `MainMenu` | `MainMenu` |
| `Season` | `SeasonOverview` |
| `Week` | `WeekHub`, `CrewUI`, `MaintenanceUI`, `ShopUI` (modular panels load/unload as needed) |
| `RaceDay` | `RaceGarage`, `RaceSimulation` (both always loaded for seamless turn-based flow) |

---

## Scene Transition Implementation

### State-to-Scenes Mapping

```typescript
const stateToScenes: Record<GameState['type'], string[]> = {
  MainMenu: ['Persistent', 'MainMenu'],
  Season: ['Persistent', 'SeasonOverview'],
  Week: ['Persistent', 'WeekHub'],
  RaceDay: ['Persistent', 'RaceGarage', 'RaceSimulation']
};

// UI panels (CrewUI, MaintenanceUI, ShopUI) load/unload on-demand within WeekState
const activeWeekPanels: string[] = [];
```

### OnStateChange Handler

When the state machine transitions between states:

```typescript
onStateChange(from: GameState, to: GameState) {
  const fromScenes = stateToScenes[from.type] || [];
  const toScenes = stateToScenes[to.type] || [];
  
  // Unload scenes no longer needed
  const scenesToUnload = difference(fromScenes, toScenes);
  scenesToUnload.forEach(s => SceneManager.UnloadSceneAsync(s));
  
  // Load new scenes required
  const scenesToLoad = difference(toScenes, fromScenes);
  scenesToLoad.forEach(s => SceneManager.LoadSceneAsync(s, LoadSceneMode.Additive));
}
```

### Context Synchronization

The `GameContext` updates are broadcast to all active scenes:

```typescript
onContextUpdate(context: GameContext) {
  // Update UI displays
  FindObjectOfType<UIManager>().UpdateDisplay(context);
  FindObjectOfType<TeamRoster>().UpdateTeamView(context.team);
  FindObjectOfType<ResourceDisplay>().UpdateResources(context.resources);
  
  // Update game systems
  FindObjectOfType<VehicleManager>().UpdateVehicles(context.vehicles);
  FindObjectOfType<SponsorManager>().UpdateSponsors(context.sponsors);
}
```

---

## Turn-Based Race Mechanics

### Run Lifecycle

Each race run follows this turn-based sequence:

1. **Garage Phase** (Player controls, indeterminate time)
   - Player adjusts vehicle setup (tire choice, gear ratio, nitrous)
   - Player sets driver mindset
   - Player reviews telemetry from previous runs
   - Player confirms setup and commits to the run

2. **Running Phase** (Automated, ~4 seconds)
   - Race simulation executes with confirmed setup
   - Physics engine simulates launch, acceleration, top speed
   - Telemetry is recorded (ET, mph, 60ft time, gear engagement)
   - Opponent (AI) simultaneously executes their setup
   - Run result is calculated (win/loss, time differential)

3. **Results Phase** (Player controls, indeterminate time)
   - Run telemetry is displayed to player
   - For best-of-three: Match record shown (e.g., "1-0")
   - Player can:
     - **Run Again** → Return to Garage Phase (retry with new setup)
     - **Accept Result** → Proceed to next match (if best-of-three complete)
     - **Run Out** → Opponent must forfeit remaining runs
     - **Quit Match** → Forfeit tournament, return to WeekState

### Why Turn-Based?

- **Strategic Depth:** Player has time to analyze performance and adjust strategy
- **Pacing:** No time pressure; player plays at their own pace between 4-second runs
- **Learning Curve:** New players can experiment without rushing
- **Telemetry Review:** Each run's data informs the next setup decision
- **Accessibility:** No real-time twitch reflexes required

### Telemetry Data

After each run, the player can review:

```typescript
interface RunTelemetry {
  elapsedTime: number; // ET in milliseconds
  topSpeed: number; // MPH
  sixtyFootTime: number; // 0-60 in milliseconds
  wheelSpinRPM: number; // Launch wheel spin
  gearChangePoints: number[]; // When each shift occurred
  nitrousEngagement: number; // Time nitrous was active
  driverReactionTime: number; // How fast they launched
  trackCondition: string; // Surface grip level
  weatherEffect: string; // Wind, temperature impact
}
```

---

## Event Handling & Messaging

### State Change Events

```typescript
public class GameStateEvents
{
  public static event Action<GameState, GameState> OnStateChanged;
  public static event Action<GameContext> OnContextUpdated;
  public static event Action<string> OnTransitionRejected;
}
```

### Pause & Resume

The game can be paused and resumed from any state. Pausing:
1. Freezes the current state (no transitions allowed)
2. Unloads state-specific scenes
3. Loads the pause menu scene
4. Resumes by returning to the prior state

---

## Save & Load System

### Serialization

Only the `GameContext` needs to be serialized:

```typescript
[System.Serializable]
public class GameSaveData
{
  public int Season;
  public System.DateTime CurrentDate;
  public ResourcePool Resources;
  public TeamData Team;
  public VehicleData[] Vehicles;
  public SponsorData[] Sponsors;
  public GameEvent[] History;
  
  // Which state were we in?
  public string CurrentStateType;
}
```

### Load Path

1. Player selects "Load Game"
2. Main menu loads save file
3. State machine transitions to the saved state type
4. GameContext is restored
5. Associated scenes load

---

## Implementation Phases

### Phase 1: Core State Machine
- Build `StateMachine` class with state enumeration (4 states: MainMenu, Season, Week, RaceDay)
- Implement transition validation logic
- Create `GameContext` persistence system
- Set up basic event dispatching

### Phase 2: Scene Management Integration
- Wire `StateMachine` to Unity `SceneManager`
- Implement scene loader/unloader based on state changes
- Handle modular UI panel loading/unloading within WeekState (CrewUI, MaintenanceUI, ShopUI)
- Manage persistent scenes that never unload

### Phase 3: Week Hub UI & Operations
- Build WeekHub scene with calendar and week overview
- Implement crew assignment UI panel (loads within WeekState)
- Implement maintenance queue UI panel (loads within WeekState)
- Implement shop UI panel for purchases (loads within WeekState)
- Set up modular panel system that loads/unloads on-demand

### Phase 4: Race Day Implementation
- Build garage phase UI (vehicle setup, tuning, telemetry review)
- Implement race simulation (4-second runs, turn-based results review)
- Create telemetry display and analysis tools
- Wire bracket progression and match management

### Phase 5: Save/Load & Pause System
- Implement serialization of `GameContext`
- Build save file manager with multiple slots
- Add pause functionality (state freezing + menu scene)
- Handle resume from pause and recovery from crashes

### Phase 6: Polish & Debug
- Add state transition logging and visualization
- Implement transition guards for invalid moves
- Add recovery mechanisms for edge cases
- Performance profiling for scene loading

---

## Design Rationale

### Why FSM?

- **Clarity:** Each state is self-contained with clear entry/exit points
- **Maintainability:** Adding new gameplay modes (draft, off-season, training) requires minimal refactoring
- **Debugging:** State history makes it easier to trace bugs
- **Save/Load:** Context is independent of scene state

### Why Separate Context?

- **Flexibility:** UI updates independently from state transitions
- **Testability:** Context logic can be unit tested without scene loading
- **Persistence:** Same serialization for saves and network play

### Why Additive Scene Loading?

- **Performance:** Only load what's needed, unload when done
- **Memory:** Persistent scenes stay loaded, reducing redundant initialization
- **Transitions:** Smooth scene fades by overlapping load/unload

---

## Edge Cases & Recovery

### Invalid Transitions

Some transitions are only valid in specific contexts:
- Can't enter `RaceDayState` if no race is scheduled
- Can't complete operations if mandatory crew assignments are pending
- Can't advance season if critical research is incomplete

All invalid transitions are rejected at the state machine level with an `OnTransitionRejected` event.

### Mid-State Pause

If the player pauses during `RaceDayState`, the race simulation is frozen (not unloaded). Resume returns to the exact same race simulation state.

### Scene Load Failure

If a scene fails to load, the state machine logs the error and attempts reload. If retry fails, the player is prompted to return to main menu.

---

## Future Extensions

- **Multiplayer:** Sync `GameContext` across network
- **Replay System:** Record state transitions and context snapshots
- **Time Travel:** Debug tool to rewind to previous states
- **Modding:** Allow custom states and transitions via plugins
