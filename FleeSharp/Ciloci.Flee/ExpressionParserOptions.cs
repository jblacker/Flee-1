using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Globalization;

namespace Ciloci.Flee
{
	public class ExpressionParserOptions
	{
		private PropertyDictionary myProperties;

		private readonly ExpressionContext myOwner;

		private readonly CultureInfo myParseCulture;

		private readonly NumberStyles numberStyles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent;

		public string DateTimeFormat
		{
			get
			{
				return this.myProperties.GetValue<string>("DateTimeFormat");
			}
			set
			{
				this.myProperties.SetValue("DateTimeFormat", value);
			}
		}

		public bool RequireDigitsBeforeDecimalPoint
		{
			get
			{
				return this.myProperties.GetValue<bool>("RequireDigitsBeforeDecimalPoint");
			}
			set
			{
				this.myProperties.SetValue("RequireDigitsBeforeDecimalPoint", value);
			}
		}

		public char DecimalSeparator
		{
			get
			{
				return this.myProperties.GetValue<char>("DecimalSeparator");
			}
			set
			{
				this.myProperties.SetValue("DecimalSeparator", value);
				this.myParseCulture.NumberFormat.NumberDecimalSeparator = Conversions.ToString(value);
			}
		}

		public char FunctionArgumentSeparator
		{
			get
			{
				return this.myProperties.GetValue<char>("FunctionArgumentSeparator");
			}
			set
			{
				this.myProperties.SetValue("FunctionArgumentSeparator", value);
			}
		}

		public char StringQuote
		{
			get
			{
				return this.myProperties.GetValue<char>("StringQuote");
			}
			set
			{
				this.myProperties.SetValue("StringQuote", value);
			}
		}

		internal ExpressionParserOptions(ExpressionContext owner)
		{
			this.myOwner = owner;
			this.myProperties = new PropertyDictionary();
			this.myParseCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
			this.InitializeProperties();
		}

		public void RecreateParser()
		{
			this.myOwner.RecreateParser();
		}

		internal ExpressionParserOptions Clone()
		{
			ExpressionParserOptions copy = (ExpressionParserOptions)base.MemberwiseClone();
			copy.myProperties = this.myProperties.Clone();
			return copy;
		}

		internal double ParseDouble(string image)
		{
			return double.Parse(image, NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, this.myParseCulture);
		}

		internal float ParseSingle(string image)
		{
			return float.Parse(image, NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, this.myParseCulture);
		}

		internal decimal ParseDecimal(string image)
		{
			return decimal.Parse(image, NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, this.myParseCulture);
		}

		private void InitializeProperties()
		{
			this.DateTimeFormat = "dd/MM/yyyy";
			this.RequireDigitsBeforeDecimalPoint = false;
			this.DecimalSeparator = '.';
			this.FunctionArgumentSeparator = ',';
			this.StringQuote = '"';
		}
	}
}
