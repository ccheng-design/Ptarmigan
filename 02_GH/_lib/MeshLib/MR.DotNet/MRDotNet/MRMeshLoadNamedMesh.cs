using System;

namespace MR.DotNet;

internal struct MRMeshLoadNamedMesh
{
	public IntPtr name;

	public IntPtr mesh;

	public MRAffineXf3f xf;

	public int skippedFaceCount;

	public int duplicatedVertexCount;

	public MRMeshLoadNamedMesh()
	{
		xf = default(MRAffineXf3f);
		name = IntPtr.Zero;
		mesh = IntPtr.Zero;
		skippedFaceCount = 0;
		duplicatedVertexCount = 0;
	}
}
