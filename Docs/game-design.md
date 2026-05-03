<!--
This document is iterative and incomplete.
Sections may be inconsistent or underdefined.
-->

# Game Overview
Drag racing team management simulation with tactical race decisions, operational management, and strategic season planning.

# Game Structure
- **Seasons**: Yearly cycles driving strategic decisions
- **Race Days**: Event-based tactical gameplay (1-2 per season week)
- **Operations**: Day-to-day management between race events

# Game Loops
1. **Tactical (Race Day)**:
   - Elimination bracket races (best-of-three format)
   - Turn-based race runs: Each run lasts ~4 seconds of simulation, then player decides next run setup
   - Pre-run decisions: Tire selection, gear ratios, nitrous usage, driver mindset
   - Win prize money based on placement
   > High-stakes, strategic decision-making between runs

2. **Operational (Weekly Management)**:
   - Car maintenance/repairs
   - Crew assignment and training
   - Resource acquisition and logistics
   - Driver morale management
   > Strategic resource allocation within each week

3. **Strategic (Seasonal)**:
   - Sponsor negotiations
   - Research and development
   - Facility upgrades
   - Driver contracts
   > Long-term progression defining team capabilities

# Systems
| System          | Core Function                     |
|-----------------|-----------------------------------|
| Racing          | Race simulation and outcome calc  |
| Team Management | Crew/driver stats and progression |
| Vehicle Tech    | Performance parts and upgrades    |
| Finance         | Budget/sponsors/expense tracking  |

# Player Actions
- **Pre-run**: Tune vehicle setup (tires, gear ratio, nitrous), set driver mindset
- **Between runs**: Review telemetry, adjust strategy for next run
- **During week**: Allocate crew, purchase parts, schedule training, manage maintenance
- **Off-season**: Negotiate deals, develop tech tree

# Resources
- **Primary**: Cash, Reputation, Research Points
- **Consumables**: Fuel, Tires, Engine Wear
- **Team Factors**: Driver Energy, Crew Skill Points

# Progression
- Unlock higher-tier racing events
- Improve team facilities
- Access advanced vehicle technologies
- Attract better drivers/sponsors

# Open Questions
1. How detailed should vehicle tuning mechanics be?
2. Balance between random events vs. deterministic outcomes
3. Depth of sponsor relationship management system
4. Should weather/seasonality affect races?