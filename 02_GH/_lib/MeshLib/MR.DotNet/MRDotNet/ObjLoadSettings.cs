using System.Runtime.InteropServices;

namespace MR.DotNet;

public struct ObjLoadSettings
{
	[MarshalAs(UnmanagedType.U1)]
	public bool customXf;

	[MarshalAs(UnmanagedType.U1)]
	public bool countSkippedFaces;

	public ObjLoadSettings()
	{
		customXf = false;
		countSkippedFaces = false;
	}
}
