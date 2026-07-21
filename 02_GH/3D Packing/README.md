# 3D Nesting for Grasshopper — resource pack

Built 2026-07-18 from source with .NET 8 SDK (csc, Release, deterministic),
compiled against the netstandard 2.0 reference assembly. Both DLLs reference
ONLY `netstandard 2.0.0.0`, so they load in Rhino 7 (.NET Framework 4.8) and
Rhino 8 (.NET 7) without modification. Smoke-tested: both pack correctly,
rotation handling verified.

## Files
- `Sharp3DBinPacking.dll` — 303248153/Sharp3DBinPacking (MIT). Guillotine +
  Shelf heuristics, multiple bins, per-item Tag, AllowRotateVertically flag.
  **This is the one the GH script uses.**
- `CromulentBisgetti.ContainerPacking.dll` — davidmchapman/3DContainerPacking
  (MIT). EB-AFIT algorithm: single container, full rotation, very good fill
  rates, reports unpacked items. Swap in later if you want denser packs.
- `Nest3D_GH_Component.cs` — paste into a C# component. Setup + input/output
  spec is in the header comment.
- `sources.zip` — both repos' source, so you can rebuild or read the algorithms.

## Referencing the DLL
- Rhino 8 C# Script component: `#r "C:\path\to\Sharp3DBinPacking.dll"` at the
  top of the script.
- Rhino 7 legacy C# component: right-click -> Manage Assemblies -> add.
- If Windows blocks the DLL (downloaded-file mark-of-the-web): file
  Properties -> Unblock, or `Unblock-File` in PowerShell.

## API cheat sheet (Sharp3DBinPacking)
```csharp
var param  = new BinPackParameter(binW, binH, binD, 0, allowRotateVertically, cuboids);
param.ShuffleCount = 5;                    // more shuffles = better/slower
var packer = BinPacker.GetDefault(BinPackerVerifyOption.BestOnly);
var result = packer.Pack(param);           // result.BestResult = List<IList<Cuboid>>
// each Cuboid: X,Y,Z = min-corner position; Width/Height/Depth = placed (rotated) dims; Tag = yours
```
Axis convention used in the GH script: Width=X, Height=Y, Depth=Z, consistently
for both the bin and the items — so it just works with Rhino boxes.

## API cheat sheet (EB-AFIT)
```csharp
var res = PackingService.Pack(containers, items, new List<int>{1}); // 1 = EB_AFIT
var r = res[0].AlgorithmPackingResults[0];
// r.PackedItems (CoordX/Y/Z + PackDimX/Y/Z), r.UnpackedItems, r.PercentContainerVolumePacked
```
Note: EB-AFIT dims are `decimal`; it packs one container per call — loop
containers yourself for overflow behavior.

## Approach / limitations
This is AABB (bounding-box) nesting: each mesh/brep is packed via its
axis-aligned bounding box with orthogonal rotations. True-shape 3D nesting
(interlocking irregular geometry) is a much harder problem — the usual
next steps are voxel-based collision packing or physics drop-simulation
(e.g., Kangaroo collisions) seeded with this AABB layout.
