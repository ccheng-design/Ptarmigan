// ============================================================================
// Nest3D — 3D bin-packing (nesting) for Grasshopper C# component
// Uses Sharp3DBinPacking.dll (netstandard2.0, works in Rhino 7 + Rhino 8)
//
// SETUP
//   Rhino 8 (new C# Script component):
//     Add at the very top of the script:   #r "C:\path\to\Sharp3DBinPacking.dll"
//   Rhino 7 (legacy C# component):
//     Right-click component -> Manage Assemblies -> add Sharp3DBinPacking.dll
//
// COMPONENT INPUTS (set type hints + access as noted)
//   G : Geometry, List access, type hint GeometryBase   (meshes and/or breps)
//   B : Box, Item access                                 (container box)
//   S : double, Item access                              (gap between items, model units)
//   R : bool, Item access                                (allow tipping items over; false = rotate about Z only)
//
// COMPONENT OUTPUTS
//   N   : nested geometry (aligned with G; null if item can never fit)
//   X   : transforms per item (aligned with G)
//   C   : placement boxes per item (aligned with G)
//   BIN : bin index per item (0 = inside B, 1+ = overflow bins drawn next to B, -1 = impossible)
//   REP : text report
// ============================================================================

private void RunScript(List<GeometryBase> G, Box B, double S, bool R,
    ref object N, ref object X, ref object C, ref object BIN, ref object REP)
{
    var report = new System.Text.StringBuilder();
    int n = G == null ? 0 : G.Count;
    var outGeo = new GeometryBase[n];
    var outXform = new object[n];
    var outBox = new object[n];
    var outBin = new int[n];
    for (int i = 0; i < n; i++) outBin[i] = -1;

    if (n == 0 || !B.IsValid) { REP = "No geometry or invalid box."; return; }

    double tol = RhinoDocument.ModelAbsoluteTolerance;
    double binW = B.X.Length, binH = B.Y.Length, binD = B.Z.Length;

    // World AABB per item, inflated by spacing
    var dims = new Vector3d[n];
    var centers = new Point3d[n];
    var cuboids = new List<Sharp3DBinPacking.Cuboid>();
    var impossible = new List<int>();

    for (int i = 0; i < n; i++)
    {
        if (G[i] == null) { impossible.Add(i); continue; }
        BoundingBox bb = G[i].GetBoundingBox(true);
        double dx = Math.Max(bb.Diagonal.X, tol) + S;
        double dy = Math.Max(bb.Diagonal.Y, tol) + S;
        double dz = Math.Max(bb.Diagonal.Z, tol) + S;
        dims[i] = new Vector3d(dx, dy, dz);
        centers[i] = bb.Center;

        if (!CanEverFit(dx, dy, dz, binW, binH, binD, R))
        {
            impossible.Add(i);
            report.AppendLine(string.Format("Item {0}: too big for container in any allowed orientation.", i));
            continue;
        }
        cuboids.Add(new Sharp3DBinPacking.Cuboid((decimal)dx, (decimal)dy, (decimal)dz, 0, i));
    }

    if (cuboids.Count == 0) { REP = report.ToString() + "Nothing packable."; return; }

    // Pack. Bin axes: Width=X, Height=Y, Depth=Z (kept consistent on placement).
    var param = new Sharp3DBinPacking.BinPackParameter(
        (decimal)binW, (decimal)binH, (decimal)binD, 0, R, cuboids);
    var packer = Sharp3DBinPacking.BinPacker.GetDefault(Sharp3DBinPacking.BinPackerVerifyOption.BestOnly);
    var result = packer.Pack(param);
    var bins = result.BestResult;

    // Plane at the box's min corner, oriented like the box
    Plane corner = new Plane(B.PointAt(0, 0, 0), B.Plane.XAxis, B.Plane.YAxis);
    Transform toBox = Transform.PlaneToPlane(Plane.WorldXY, corner);

    for (int b = 0; b < bins.Count; b++)
    {
        // Overflow bins get drawn beside the container along its X axis
        Vector3d overflow = corner.XAxis * (b * binW * 1.15);

        foreach (var c in bins[b])
        {
            int i = (int)c.Tag;
            outBin[i] = b;

            double pw = (double)c.Width, ph = (double)c.Height, pd = (double)c.Depth;

            // Find a proper (det=+1) axis-aligned rotation mapping original dims -> packed dims
            Transform rot; Vector3d permDims;
            if (!FindRotation(dims[i], new Vector3d(pw, ph, pd), tol, out rot, out permDims))
            { rot = Transform.Identity; permDims = dims[i]; } // dims equal within tol; identity is fine

            // Rotate about the item's bbox center, then move its (new) AABB min corner
            // to the packed position (+ half-spacing so the gap surrounds each item)
            Transform rotAboutCenter =
                Transform.Translation(centers[i] - Point3d.Origin) * rot * Transform.Translation(Point3d.Origin - centers[i]);
            Point3d rotatedMin = centers[i] - permDims * 0.5;
            Point3d targetLocal = new Point3d((double)c.X + S * 0.5, (double)c.Y + S * 0.5, (double)c.Z + S * 0.5);
            Transform move = Transform.Translation(new Point3d(targetLocal) - rotatedMin);

            Transform final = toBox * move * rotAboutCenter;
            final = Transform.Translation(overflow) * final;

            var dup = G[i].Duplicate();
            dup.Transform(final);
            outGeo[i] = dup;
            outXform[i] = final;

            // Placement box (the packed cuboid, minus spacing) for visual debugging
            Point3d p0 = corner.Origin + overflow
                + corner.XAxis * ((double)c.X + S * 0.5) + corner.YAxis * ((double)c.Y + S * 0.5) + corner.ZAxis * ((double)c.Z + S * 0.5);
            Plane pp = new Plane(p0, corner.XAxis, corner.YAxis);
            outBox[i] = new Box(pp, new Interval(0, pw - S), new Interval(0, ph - S), new Interval(0, pd - S));
        }
    }

    int inBox = 0, over = 0;
    for (int i = 0; i < n; i++) { if (outBin[i] == 0) inBox++; else if (outBin[i] > 0) over++; }
    report.AppendLine(string.Format("Placed {0} in container, {1} in {2} overflow bin(s), {3} impossible.",
        inBox, over, Math.Max(0, bins.Count - 1), impossible.Count));

    N = outGeo; X = outXform; C = outBox; BIN = outBin; REP = report.ToString();
}

// ---------------------------------------------------------------------------
bool CanEverFit(double dx, double dy, double dz, double w, double h, double d, bool allowVertical)
{
    if (allowVertical)
    {
        var s = new[] { dx, dy, dz }; Array.Sort(s);
        var t = new[] { w, h, d }; Array.Sort(t);
        return s[0] <= t[0] && s[1] <= t[1] && s[2] <= t[2];
    }
    // Rotation about Z only: z must fit as-is, x/y may swap
    return dz <= d && ((dx <= w && dy <= h) || (dy <= w && dx <= h));
}

// Search the 24 proper axis-aligned rotations for one whose |R|*dims == packed dims
bool FindRotation(Vector3d d0, Vector3d d1, double tol, out Transform rot, out Vector3d permDims)
{
    int[][] perms = {
        new[]{0,1,2}, new[]{0,2,1}, new[]{1,0,2}, new[]{1,2,0}, new[]{2,0,1}, new[]{2,1,0} };
    double[] src = { d0.X, d0.Y, d0.Z };

    foreach (var p in perms)
    {
        // packed dim k comes from source axis p[k]
        if (Math.Abs(src[p[0]] - d1.X) > tol) continue;
        if (Math.Abs(src[p[1]] - d1.Y) > tol) continue;
        if (Math.Abs(src[p[2]] - d1.Z) > tol) continue;

        // Build rotation: column p[k] of R gets a ±1 in row k. Choose signs for det=+1.
        for (int signs = 0; signs < 8; signs++)
        {
            var m = new double[3, 3];
            double det = 1;
            for (int k = 0; k < 3; k++)
            {
                double s = ((signs >> k) & 1) == 0 ? 1 : -1;
                m[k, p[k]] = s;
            }
            det = m[0, 0] * (m[1, 1] * m[2, 2] - m[1, 2] * m[2, 1])
                - m[0, 1] * (m[1, 0] * m[2, 2] - m[1, 2] * m[2, 0])
                + m[0, 2] * (m[1, 0] * m[2, 1] - m[1, 1] * m[2, 0]);
            if (Math.Abs(det - 1.0) > 0.5) continue;

            Transform t = Transform.Identity;
            t.M00 = m[0, 0]; t.M01 = m[0, 1]; t.M02 = m[0, 2];
            t.M10 = m[1, 0]; t.M11 = m[1, 1]; t.M12 = m[1, 2];
            t.M20 = m[2, 0]; t.M21 = m[2, 1]; t.M22 = m[2, 2];
            rot = t;
            permDims = new Vector3d(src[p[0]], src[p[1]], src[p[2]]);
            return true;
        }
    }
    rot = Transform.Identity;
    permDims = d0;
    return false;
}
