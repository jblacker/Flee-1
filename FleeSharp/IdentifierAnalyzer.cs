using System;
using System.Collections.Generic;

namespace Flee
{
    using PerCederberg.Grammatica.Runtime;

    internal class IdentifierAnalyzer : Analyzer
    {
        private IDictionary<int, string> MyIdentifiers;

        private int MyMemberExpressionCount;

        private bool MyInFieldPropertyExpression;

        public IdentifierAnalyzer()
        {
            this.MyIdentifiers = new Dictionary<int, string>();
        }

        public override Node Exit(Node node)
        {
            int id = node.Id;
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
            int id = node.Id;
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
            bool flag = !this.MyInFieldPropertyExpression;
            if (!flag)
            {
                bool flag2 = !this.MyIdentifiers.ContainsKey(this.MyMemberExpressionCount);
                if (flag2)
                {
                    this.MyIdentifiers.Add(this.MyMemberExpressionCount, node.Image);
                }
            }
        }

        private void EnterMemberExpression()
        {
            this.MyMemberExpressionCount++;
        }

        private void EnterFieldPropertyExpression()
        {
            this.MyInFieldPropertyExpression = true;
        }

        private void ExitFieldPropertyExpression()
        {
            this.MyInFieldPropertyExpression = false;
        }

        public void Reset()
        {
            this.MyIdentifiers.Clear();
            this.MyMemberExpressionCount = -1;
        }

        public ICollection<string> GetIdentifiers(ExpressionContext context)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            ExpressionImports ei = context.Imports;
            try
            {
                IEnumerator<string> enumerator = this.MyIdentifiers.Values.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    string identifier = enumerator.Current;
                    bool flag = ei.HasNamespace(identifier);
                    if (!flag)
                    {
                        bool flag2 = context.Variables.ContainsKey(identifier);
                        if (!flag2)
                        {
                            dict[identifier] = null;
                        }
                    }
                }
            }
            finally
            {
                IEnumerator<string> enumerator;
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }
            return dict.Keys;
        }
    }
}