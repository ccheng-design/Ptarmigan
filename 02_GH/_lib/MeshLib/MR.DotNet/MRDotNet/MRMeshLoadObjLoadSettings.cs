using System;

namespace MR.DotNet;

internal struct MRMeshLoadObjLoadSettings
{
	public bool customXf;

	public bool countSkippedFaces;

	public IntPtr callback;

	public MRMeshLoadObjLoadSettings()
	{
		customXf = false;
		countSkippedFaces = false;
		callback = IntPtr.Zero;
	}
}
