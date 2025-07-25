using System;

namespace MR.DotNet;

public class MeshPart
{
	public Mesh mesh;

	public BitSet? region;

	internal MRMeshPart mrMeshPart;

	public unsafe MeshPart(Mesh mesh, BitSet? region = null)
	{
		this.mesh = mesh;
		this.region = region;
		mrMeshPart.mesh = mesh.mesh_;
		mrMeshPart.region = region?.bs_ ?? ((IntPtr)(void*)null);
	}
}
