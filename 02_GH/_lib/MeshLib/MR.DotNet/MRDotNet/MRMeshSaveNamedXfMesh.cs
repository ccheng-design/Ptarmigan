using System;

namespace MR.DotNet;

internal struct MRMeshSaveNamedXfMesh
{
	public string name;

	public MRAffineXf3f toWorld;

	public IntPtr mesh;

	public MRMeshSaveNamedXfMesh()
	{
		toWorld = default(MRAffineXf3f);
		name = "";
		mesh = IntPtr.Zero;
	}
}
