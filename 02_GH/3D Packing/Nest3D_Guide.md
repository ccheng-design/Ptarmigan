# Nest3D — Input & Output Guide

A 3D nesting component for Grasshopper. Feed it meshes or breps and a container
box; it packs each object by its bounding box using orthogonal rotations, and
can iteratively search for better layouts while you watch.

---

## Inputs

**G — Geometry** *(List Access)*
The objects to nest: meshes, breps, or a mix. Each object is represented by its
world axis-aligned bounding box, so the packing quality depends on how well a
box approximates each shape. Order matters only for bookkeeping — every output
is index-aligned with this list, so item 3 in G corresponds to item 3 in N, X,
C, and BIN.

**B — Container Box** *(Item Access)*
The volume to nest into. A Box component is the intended input and its
orientation is respected — a rotated box nests in its own coordinate system.
You can also plug in a BoundingBox or any geometry, in which case its bounding
box is used.

**S — Spacing** *(Item Access, default 0)*
Minimum clearance in model units. Applied symmetrically: half the spacing on
every side of each item, giving the full spacing between neighbors and to the
container walls. Useful for tolerances in fabrication, packing foam, or just
visual breathing room. Note that spacing inflates every item's effective size,
so generous spacing can push items into overflow.

**Run — Search Toggle** *(Item Access, default false)*
The engine switch. While true, the component self-loops (schedules its own next
solution every ~100 ms) and runs packing attempts each tick, always remembering
the best layout it has ever seen. Flip it false and the loop stops, displaying
that best layout. Nothing is lost by stopping — the search resumes from the
same best when you toggle it back on. Be aware a definition saved with Run on
will resume looping when reopened.

**Iter — Iteration Budget** *(Item Access, default 0)*
A total-attempts cap. Set 200 and the search freezes itself after 200 attempts
even if Run is still on — a "set and walk away" mode. Set 0 for no cap: it
runs until you toggle Run off. The banner shows progress as current/budget
when a budget is set.

**Seed — Random Seed** *(Item Access, default 0)*
Controls reproducibility. 0 means a fresh random sequence each session. Any
other value makes the entire search deterministic: same inputs + same seed =
same sequence of layouts, every time. (Fixing the seed also switches off the
library's internal time-seeded shuffles, since those would break determinism —
so seeded runs explore slightly less per attempt but repeat exactly.) Changing
the seed mid-search reseeds future attempts without discarding the best found.

**Mut — Mutation** *(Item Access, 0–100, default 10)*
The exploration dial. Each attempt sorts items biggest-volume-first, then
perturbs that order with random noise; Mut is the noise strength in percent.
At 0 every attempt tries nearly the same greedy order — fast but shallow. At
higher values the order wanders further from greedy, wasting more attempts but
occasionally discovering layouts greedy sorting can never reach. 10–30 is a
sensible range; go higher if the search plateaus early.

**Pop — Population** *(Item Access, default 5)*
Packing attempts per solve tick. Raising it searches faster in wall-clock time
(more attempts between canvas redraws) but makes the display choppier and each
tick heavier. Lower it to 1 for the smoothest live animation — one tick, one
attempt, one frame.

**Rot — All Rotations** *(Item Access, default true)*
Orientation freedom. True allows any orthogonal orientation — items may lie
down, stand up, or turn on any axis in 90° steps. False restricts rotation to
the vertical axis only: items keep their up direction and may only spin/swap
their horizontal footprint. Use false when objects must stay upright (parts
with a required print/build orientation, containers that can't tip).

**Pack — Packing Algorithm** *(Item Access, 0/1/2, default 0)*
Which heuristic family places the boxes. Wire a Value List with:
Auto = 0, Shelf = 1, Guillotine = 2. Shelf builds layered rows — predictable,
stable-looking stacks. Guillotine recursively subdivides the free volume —
usually denser but more jumbled. Auto races all four internal variants each
attempt and keeps whichever wins, at roughly double the cost of picking one.

**Live — Live View** *(Item Access, default false)*
Display mode, not search behavior. False shows the best-so-far, which visibly
changes only when a new record is set — the layout "locks in" improvements.
True shows the newest attempt every tick regardless of quality, so the pile
continuously rearranges while searching. The best is tracked silently either
way, and the display snaps back to it the moment the search stops.

**Reset — Restart Button** *(Item Access, momentary)*
Wire a Button, not a toggle. Pressing it wipes the stored best layout,
iteration counter, and random state, then restarts the search from scratch
with the same inputs. A toggle left on would re-wipe every tick and pin the
counter near zero. Note the search also resets automatically whenever the
problem itself changes — different geometry, container, spacing, or Rot — and
whenever the script is edited or Rhino restarts.

---

## Outputs

**N — Nested Geometry**
The input objects, duplicated and moved into their packed positions.
Index-aligned with G; null where an item can never fit in the container in any
allowed orientation.

**X — Transforms**
The exact transform applied to each item (aligned with G). This is the most
reusable output: feed proxy meshes into G for speed, then apply X to the
original heavy breps with a native Transform component to get identical
placement.

**C — Placement Boxes**
Each item's allocated slot, shrunk by the spacing — i.e., the box the geometry
actually sits in. Good for previewing the layout without rendering the real
geometry, and for checking clearances.

**BIN — Bin Index**
Where each item ended up: 0 = inside the container, 1 and up = overflow bins
drawn beside the container for anything that didn't fit, -1 = impossible (too
big in every allowed orientation). In practice this is the output you filter
on — cull N by BIN = 0 to isolate what fits.

**REP — Report**
Human-readable status: how many items placed where, the active settings, the
iteration count, best fill percentage, stack height, and whether the search is
running, frozen, or out of budget.

---

## Reading the banner

The text under the component mirrors the search state at a glance:
"Iter 47 • 74%" — searching, best fill so far. "Iter 47 • LIVE" — searching in
live view. "Iter 200/200 • done" — budget exhausted, frozen on best.
"Iter 47 • frozen" — Run is off, holding best.

## What the fill percentage means

The percentage (in the banner and as "best fill" in REP) is container volume
utilization: the combined volume of all packed slots inside the container,
divided by the container's volume, times 100. Three things to know about it:

It measures *bounding boxes, not geometry*. Each slot is the item's bounding
box inflated by the spacing, so the number tells you how much of the container
is claimed — the actual material volume inside is lower, because bounding
boxes contain air around any non-boxy shape.

It only counts the container (bin 0). Overflow bins don't factor in, which
creates one subtlety: kicking a small item out to overflow can *raise* the
percentage while making the overall result worse. That's why the search ranks
fewer bins above higher fill — the percentage is the tie-breaker among layouts
that fit equally much, not the primary goal.

100% is unreachable in practice. For randomly-sized boxes, fills in the 60–85%
range are typical for this class of algorithm; past that, gains come in
smaller and rarer steps. If the number plateaus for hundreds of iterations,
it has probably found what this heuristic can find — raise Mut to search
wider, or accept the layout.

## What "better" means

When comparing layouts the search prefers, in order: fewer bins (everything in
the container beats anything overflowing), then more volume packed into the
container, then a lower stack height inside it. That last tie-breaker means
the search keeps compacting the pile downward even after everything fits.

## Limits worth remembering

This is bounding-box nesting: no part ever intersects another, but concave
shapes can't interlock, so real-world density tops out well below true-shape
nesting. Long diagonal parts are the worst case — their boxes are mostly air.
If you later need shape-aware packing, this layout makes a good seed for a
voxel or physics-based (e.g., Kangaroo collision) refinement pass.
