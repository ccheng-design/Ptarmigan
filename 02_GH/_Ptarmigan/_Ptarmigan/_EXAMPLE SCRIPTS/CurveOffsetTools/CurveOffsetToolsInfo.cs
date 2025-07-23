using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace CurveOffsetTools;

public class CurveOffsetToolsInfo : GH_AssemblyInfo
{
	public override string Name => "CurveOffsetTools";

	public override Bitmap Icon => null;

	public override string Description => "Tools related to generalized curve offsets";

	public override Guid Id => new Guid("c8b9b5eb-5c25-419e-a91f-aceabcf22c35");

	public override string AuthorName => "ShapeDiver GmbH";

	public override string AuthorContact => "contact@shapediver.com";

	public override string Version => "1.0.0.0";

	public override string AssemblyVersion => ((GH_AssemblyInfo)this).Version;
}
