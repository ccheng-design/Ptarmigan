using System.Collections.ObjectModel;

namespace MR.DotNet;

public interface MeshOrPoints
{
	ReadOnlyCollection<Vector3f> Points { get; }

	BitSetReadOnly ValidPoints { get; }

	Box3f BoundingBox { get; }
}
