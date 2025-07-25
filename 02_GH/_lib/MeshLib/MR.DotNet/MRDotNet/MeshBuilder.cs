using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MR.DotNet;

public class MeshBuilder
{
	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private unsafe static extern MRVertMap* mrVertMapNew();

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private unsafe static extern void mrVertMapFree(MRVertMap* vertMap);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private unsafe static extern int mrMeshBuilderUniteCloseVertices(IntPtr mesh, float closeDist, bool uniteOnlyBd, MRVertMap* optionalVertOldToNew);

	public unsafe static int UniteCloseVertices(ref Mesh mesh, float closeDist, bool uniteOnlyBd, List<VertId>? optionalVertOld2New = null)
	{
		if (optionalVertOld2New == null)
		{
			return mrMeshBuilderUniteCloseVertices(mesh.varMesh(), closeDist, uniteOnlyBd, null);
		}
		MRVertMap* ptr = mrVertMapNew();
		int result = mrMeshBuilderUniteCloseVertices(mesh.varMesh(), closeDist, uniteOnlyBd, ptr);
		optionalVertOld2New.Clear();
		for (int i = 0; i < (int)ptr->size; i++)
		{
			int id = Marshal.ReadInt32(IntPtr.Add(ptr->data, i * 4));
			optionalVertOld2New.Add(new VertId(id));
		}
		mrVertMapFree(ptr);
		return result;
	}
}
