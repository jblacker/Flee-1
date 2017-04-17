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

namespace Flee.CalculationEngine
{
    using System;
    using System.Collections.Generic;

    public sealed class BatchLoader
    {
        private readonly DependencyManager<string> myDependencies;
        private readonly IDictionary<string, BatchLoadInfo> myNameInfoMap;

        internal BatchLoader()
        {
            this.myNameInfoMap = new Dictionary<string, BatchLoadInfo>(StringComparer.OrdinalIgnoreCase);
            this.myDependencies = new DependencyManager<string>(StringComparer.OrdinalIgnoreCase);
        }

        public void Add(string atomName, string expression, ExpressionContext context)
        {
            Utility.AssertNotNull(atomName, "atomName");
            Utility.AssertNotNull(expression, "expression");
            Utility.AssertNotNull(context, "context");
            var info = new BatchLoadInfo(atomName, expression, context);
            this.myNameInfoMap.Add(atomName, info);
            this.myDependencies.AddTail(atomName);
            var references = this.GetReferences(expression, context);

            foreach (var reference in references)
            {
                this.myDependencies.AddTail(reference);

                this.myDependencies.AddDepedency(reference, atomName);
            }
        }

        public bool Contains(string atomName)
        {
            return this.myNameInfoMap.ContainsKey(atomName);
        }

        internal BatchLoadInfo[] GetBachInfos()
        {
            var tails = this.myDependencies.GetTails();
            var sources = this.myDependencies.GetSources(tails);
            var result = this.myDependencies.TopologicalSort(sources);
            var infos = new BatchLoadInfo[result.Count - 1 + 1];
            var num = result.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                infos[i] = this.myNameInfoMap[result[i]];
            }
            return infos;
        }

        private ICollection<string> GetReferences(string expression, ExpressionContext context)
        {
            var analyzer = context.ParseIdentifiers(expression);
            return analyzer.GetIdentifiers(context);
        }
    }
}