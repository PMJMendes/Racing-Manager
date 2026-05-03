---
name: planner
description: Expand, flesh out, or refine sections of the game design document (Docs/game-design.md). Use when the user asks to expand a section, flesh out mechanics, resolve open questions, or structure game systems.
disable-model-invocation: true
---

# Design Document Planner

## Process

1. **Read** `Docs/game-design.md` with the `Read` tool.
2. **Identify** what needs work: empty sections, open questions, or user-specified targets.
3. **Assess readiness**:
   - If the design intent is unclear or multiple directions are viable:
     - Propose alternatives instead of committing to one.
     - Defer detailed specification where appropriate.
4. **Expand** each target section:
   - Name mechanics using clear verb-noun phrases (e.g., "Upgrade Car").
   - Define inputs, outputs, and player feedback.
   - Specify failure states and recovery paths.
   - Connect to adjacent systems and the resource economy.
5. **Check consistency**:
   - New content must not conflict with existing systems, terminology, or resource constraints.
   - Do not change the intent of existing sections; extend and clarify instead.
6. **Resolve open questions**:
   - Offer 2–3 solutions with tradeoffs.
   - Recommend one only if there is a clear advantage.
   - Otherwise leave the decision open and explain why.

---

## Principles

- Player experience over implementation elegance.
- Prefer concrete next steps over vague direction.
- Prefer the simplest system that achieves the design goal.
- Avoid adding mechanics unless they create meaningful player decisions.
- Keep terminology consistent with the existing document.

---

## Output Format

- Markdown with clear section headers.
- Mermaid diagrams for system flows when helpful.
- Tables when comparing approaches.
- Blockquotes (`>`) for player impact callouts.

---

## Escalate When

- Core game pillars are undefined.
- The core loop is unclear or inconsistent.
- Technical constraints are unknown.
- Art direction is undefined.
- Target demographic is unclear.