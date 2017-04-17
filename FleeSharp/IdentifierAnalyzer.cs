using System;
using System.Collections.Generic;

namespace Flee
{
    using PerCederberg.Grammatica.Runtime;

    internal class IdentifierAnalyzer : Analyzer
    {
        private readonly IDictionary<int, string> myIdentifiers;

        private int myMemberExpressionCount;

        private bool myInFieldPropertyExpression;

        public IdentifierAnalyzer()
        {
            this.myIdentifiers = new Dictionary<int, string>();
        }

        public override Node Exit(Node node)
        {
            var id = node.Id;
            if (id != 1034)
            {
                if (id == 2019)
                {
                    this.ExitFieldPropertyExpression();
                }
            }
            else
            {
                this.ExitIdentifier((Token)node);
            }
            return node;
        }

        public override void Enter(Node node)
        {
            var id = node.Id;
            if (id != 2015)
            {
                if (id == 2019)
                {
                    this.EnterFieldPropertyExpression();
                }
            }
            else
            {
                this.EnterMemberExpression();
            }
        }

        private void ExitIdentifier(Token node)
        {
            var flag = !this.myInFieldPropertyExpression;
            if (!flag)
            {
                var flag2 = !this.myIdentifiers.ContainsKey(this.myMemberExpressionCount);
                if (flag2)
                {
                    this.myIdentifiers.Add(this.myMemberExpressionCount, node.Image);
                }
            }
        }

        private void EnterMemberExpression()
        {
            this.myMemberExpressionCount++;
        }

        private void EnterFieldPropertyExpression()
        {
            this.myInFieldPropertyExpression = true;
        }

        private void ExitFieldPropertyExpression()
        {
            this.myInFieldPropertyExpression = false;
        }

        public void Reset()
        {
            this.myIdentifiers.Clear();
            this.myMemberExpressionCount = -1;
        }

        public ICollection<string> GetIdentifiers(ExpressionContext context)
        {
            var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            var ei = context.Imports;

            foreach (var identifier in this.myIdentifiers.Values)
            {
                //        string identifier = enumerator.Current;
                var flag = ei.HasNamespace(identifier);
                if (!flag)
                {
                    var flag2 = context.Variables.ContainsKey(identifier);
                    if (!flag2)
                    {
                        dict[identifier] = null;
                    }
                }
            }

            //try
            //{
            //    IEnumerator<string> enumerator = this.myIdentifiers.Values.GetEnumerator();
            //    while (enumerator.MoveNext())
            //    {
            //        string identifier = enumerator.Current;
            //        bool flag = ei.HasNamespace(identifier);
            //        if (!flag)
            //        {
            //            bool flag2 = context.Variables.ContainsKey(identifier);
            //            if (!flag2)
            //            {
            //                dict[identifier] = null;
            //            }
            //        }
            //    }
            //}
            //finally
            //{
            //    IEnumerator<string> enumerator;
            //    if (enumerator != null)
            //    {
            //        enumerator.Dispose();
            //    }
            //}
            return dict.Keys;
        }
    }
}