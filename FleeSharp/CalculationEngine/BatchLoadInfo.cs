// ReSharper disable InconsistentNaming
namespace Flee.CalculationEngine
{
    internal class BatchLoadInfo
	{
		public string Name;

		public string ExpressionText;

		public ExpressionContext Context;

		public BatchLoadInfo(string name, string text, ExpressionContext context)
		{
			this.Name = name;
			this.ExpressionText = text;
			this.Context = context;
		}
	}
}
