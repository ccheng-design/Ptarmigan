using System.Collections.Generic;

namespace MR.DotNet;

public struct OneMeshContour
{
	public List<OneMeshIntersection> intersections;

	public bool closed;

	public OneMeshContour(List<OneMeshIntersection> intersections, bool closed)
	{
		this.intersections = intersections;
		this.closed = closed;
	}
}
