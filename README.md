# cgeo
Implementation of some computational geometry algorithms.
## Delaunay triangulation
Simple incremental algorithm with dynamic search cache.

Step 1. Create superstructure.
Step 2. Perform step 3-4 for each node from input.
Step 3. Add node to triangulation.
    A) Find triangle in which falls this node (or on rib).
    B) If node lies in epsilon-neighborhood of any vertex of triangle - ignore this node.
    C) If node fall on rib, then this rib splits on two new, and each triangle adjacent with this rib
       also splits in two new.
    D) If node falls in triangle - split this triangle in three new.
Step 4. Check Delaunay condition for new triangles and perform required changes.

## Convex hull
Graham scan (Not implemented yet).
