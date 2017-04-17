using System;
using System.Collections;
using System.Collections.Generic;

namespace Flee
{
    using System.Linq;
    using Extensions;

    internal class ArgumentList
    {
        private readonly IList<ExpressionElement> myElements;

        public ExpressionElement this[int index] => this.myElements[index];

        public int Count => this.myElements.Count;

        public ArgumentList(ICollection elements)
        {
            var arr = new ExpressionElement[elements.Count - 1 + 1];
            elements.CopyTo(arr, 0);
            this.myElements = arr;
        }

        private string[] GetArgumentTypeNames()
        {
            return this.myElements.Map(i => i.ResultType.Name).ToArray();
        }

        public Type[] GetArgumentTypes()
        {
            return this.myElements.Map(i => i.ResultType).ToArray();

        }

        public override string ToString()
        {
            var typeNames = this.GetArgumentTypeNames();
            return Utility.FormatList(typeNames);
        }

        public ExpressionElement[] ToArray()
        {
            var arr = new ExpressionElement[this.myElements.Count - 1 + 1];
            this.myElements.CopyTo(arr, 0);
            return arr;
        }
    }
}