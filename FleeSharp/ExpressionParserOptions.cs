// ' This library is free software; you can redistribute it and/or
// ' modify it under the terms of the GNU Lesser General Public License
// ' as published by the Free Software Foundation; either version 2.1
// ' of the License, or (at your option) any later version.
// ' 
// ' This library is distributed in the hope that it will be useful,
// ' but WITHOUT ANY WARRANTY; without even the implied warranty of
// ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// ' Lesser General Public License for more details.
// ' 
// ' You should have received a copy of the GNU Lesser General Public
// ' License along with this library; if not, write to the Free
// ' Software Foundation, Inc., 59 Temple Place, Suite 330, Boston,
// ' MA 02111-1307, USA.
// ' 
// ' Flee - Fast Lightweight Expression Evaluator
// ' Copyright © 2007 Eugene Ciloci
// ' Updated to .net 4.6 Copyright 2017 Steven Hoff

namespace FleeSharp
{
    using System.Globalization;
    using Microsoft.VisualBasic.CompilerServices;

    public class ExpressionParserOptions
    {
        private readonly ExpressionContext myOwner;

        private readonly CultureInfo myParseCulture;

        private readonly NumberStyles numberStyles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent;
        private PropertyDictionary myProperties;

        internal ExpressionParserOptions(ExpressionContext owner)
        {
            this.myOwner = owner;
            this.myProperties = new PropertyDictionary();
            this.myParseCulture = (CultureInfo) CultureInfo.InvariantCulture.Clone();
            this.InitializeProperties();
        }

        public void RecreateParser()
        {
            this.myOwner.RecreateParser();
        }

        internal ExpressionParserOptions Clone()
        {
            var copy = (ExpressionParserOptions) this.MemberwiseClone();
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

        public string DateTimeFormat
        {
            get { return this.myProperties.GetValue<string>("DateTimeFormat"); }
            set { this.myProperties.SetValue("DateTimeFormat", value); }
        }

        public char DecimalSeparator
        {
            get { return this.myProperties.GetValue<char>("DecimalSeparator"); }
            set
            {
                this.myProperties.SetValue("DecimalSeparator", value);
                this.myParseCulture.NumberFormat.NumberDecimalSeparator = Conversions.ToString(value);
            }
        }

        public char FunctionArgumentSeparator
        {
            get { return this.myProperties.GetValue<char>("FunctionArgumentSeparator"); }
            set { this.myProperties.SetValue("FunctionArgumentSeparator", value); }
        }

        public bool RequireDigitsBeforeDecimalPoint
        {
            get { return this.myProperties.GetValue<bool>("RequireDigitsBeforeDecimalPoint"); }
            set { this.myProperties.SetValue("RequireDigitsBeforeDecimalPoint", value); }
        }

        public char StringQuote
        {
            get { return this.myProperties.GetValue<char>("StringQuote"); }
            set { this.myProperties.SetValue("StringQuote", value); }
        }
    }
}