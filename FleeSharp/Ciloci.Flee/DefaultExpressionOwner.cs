using System;

namespace Ciloci.Flee
{
	internal class DefaultExpressionOwner
	{
		private static DefaultExpressionOwner OurInstance = new DefaultExpressionOwner();

		public static object Instance
		{
			get
			{
				return DefaultExpressionOwner.OurInstance;
			}
		}

		private DefaultExpressionOwner()
		{
		}
	}
}
