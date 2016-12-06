using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AmbientLightNet.Infrastructure.ColorAveraging;
using AmbientLightNet.Infrastructure.ColorTransformer;
using NUnit.Framework;

namespace AmbientLightNet.UnitTests
{
	[TestFixture]
    public class ConstructorTests
    {
		[Test]
		public void ColorTransformers_HaveConfigConstructor()
		{
			IEnumerable<Type> types =
				Assembly.GetAssembly(typeof (IColorTransformer))
					.GetExportedTypes()
					.Where(x => x.IsClass && typeof(IColorTransformer).IsAssignableFrom(x));

			foreach (Type type in types)
			{
				Assert.NotNull(type.GetConstructor(new[] {typeof (IDictionary<string, object>)}));
			}
		}

		[Test]
		public void ColorAveragingServices_HaveConfigConstructor()
		{
			IEnumerable<Type> types =
				Assembly.GetAssembly(typeof (IColorAveragingService))
					.GetExportedTypes()
					.Where(x => x.IsClass && typeof (IColorAveragingService).IsAssignableFrom(x));

			foreach (Type type in types)
			{
				Assert.NotNull(type.GetConstructor(new[] {typeof (IDictionary<string, object>)}));
			}
		}
    }
}
