using System.Collections.Generic;

namespace AmbientLightNet.Infrastructure.Utils
{
	public class ReferenceComparer<T> : IEqualityComparer<T>
	{
		public bool Equals(T x, T y)
		{
			return object.ReferenceEquals(x, y);
		}

		public int GetHashCode(T obj)
		{
			return obj == null ? 0 : obj.GetHashCode();
		}
	}
}
