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

namespace FleeSharp.PerCederberg.Grammatica.Runtime
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using Exceptions;
    using Microsoft.VisualBasic.CompilerServices;

    internal abstract class Parser
    {
        private readonly Hashtable patternIds;

        private readonly List<ProductionPattern> patterns;

        private readonly ArrayList tokens;

        private ParserLogException errorLog;

        private int errorRecovery;
        private bool initialized;

        internal Parser(Tokenizer tokenizer)
            : this(tokenizer, null)
        {
        }

        internal Parser(Tokenizer tokenizer, Analyzer analyzer)
        {
            this.patterns = new List<ProductionPattern>();
            this.patternIds = new Hashtable();
            this.tokens = new ArrayList();
            this.errorLog = new ParserLogException();
            this.errorRecovery = -1;
            this.Tokenizer = tokenizer;
            var flag = analyzer == null;
            this.Analyzer = flag ? new Analyzer() : analyzer;
        }

        public Tokenizer GetTokenizer()
        {
            return this.Tokenizer;
        }

        public Analyzer GetAnalyzer()
        {
            return this.Analyzer;
        }

        internal void SetInitialized(bool initialized)
        {
            this.initialized = initialized;
        }

        public virtual void AddPattern(ProductionPattern pattern)
        {
            var flag = pattern.Count <= 0;
            if (flag)
            {
                throw new ParserCreationException(ParserCreationException.ErrorType.INVALID_PRODUCTION, pattern.Name,
                    "no production alternatives are present (must have at least one)");
            }
            var flag2 = this.patternIds.ContainsKey(pattern.Id);
            if (flag2)
            {
                throw new ParserCreationException(ParserCreationException.ErrorType.INVALID_PRODUCTION, pattern.Name,
                    Conversions.ToString(Conversions.ToDouble("another pattern with the same id (")
                        + pattern.Id + Conversions.ToDouble(") has already been added")));
            }
            this.patterns.Add(pattern);
            this.patternIds.Add(pattern.Id, pattern);
            this.SetInitialized(false);
        }

        public virtual void Prepare()
        {
            var flag = this.patterns.Count <= 0;
            if (flag)
            {
                throw new ParserCreationException(ParserCreationException.ErrorType.INVALID_PARSER,
                    "no production patterns have been added");
            }
            var num = this.patterns.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                this.CheckPattern(this.patterns[i]);
            }
            this.SetInitialized(true);
        }

        private void CheckPattern(ProductionPattern pattern)
        {
            var num = pattern.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                this.CheckAlternative(pattern.Name, pattern[i]);
            }
        }

        private void CheckAlternative(string name, ProductionPatternAlternative alt)
        {
            var num = alt.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                this.CheckElement(name, alt[i]);
            }
        }

        private void CheckElement(string name, ProductionPatternElement elem)
        {
            var flag = elem.IsProduction() && this.GetPattern(elem.Id) == null;
            if (flag)
            {
                throw new ParserCreationException(ParserCreationException.ErrorType.INVALID_PRODUCTION, name,
                    Conversions.ToString(Conversions.ToDouble("an undefined production pattern id (") +
                        elem.Id + Conversions.ToDouble(") is referenced")));
            }
        }

        public void Reset()
        {
            this.tokens.Clear();
            this.errorLog = new ParserLogException();
            this.errorRecovery = -1;
        }

        public Node Parse()
        {
            Node root = null;
            var flag = !this.initialized;
            if (flag)
            {
                this.Prepare();
            }
            this.Reset();
            try
            {
                root = this.ParseStart();
            }
            catch (ParseException parseException)
            {
                ProjectData.SetProjectError(parseException);
                var e = parseException;
                this.AddError(e, true);
                ProjectData.ClearProjectError();
            }
            var flag2 = this.errorLog.Count > 0;
            if (flag2)
            {
                throw this.errorLog;
            }
            return root;
        }

        protected abstract Node ParseStart();

        internal void AddError(ParseException e, bool recovery)
        {
            var flag = this.errorRecovery <= 0;
            if (flag)
            {
                this.errorLog.AddError(e);
            }
            if (recovery)
            {
                this.errorRecovery = 3;
            }
        }

        internal ProductionPattern GetPattern(int id)
        {
            return (ProductionPattern) this.patternIds[id];
        }

        internal ProductionPattern GetStartPattern()
        {
            var flag = this.patterns.Count <= 0;
            ProductionPattern getStartPattern;
            if (flag)
            {
                getStartPattern = null;
            }
            else
            {
                getStartPattern = this.patterns[0];
            }
            return getStartPattern;
        }

        internal ICollection<ProductionPattern> GetPatterns()
        {
            return this.patterns;
        }

        internal void EnterNode(Node node)
        {
            var flag = !node.IsHidden() && this.errorRecovery < 0;
            if (flag)
            {
                try
                {
                    this.Analyzer.Enter(node);
                }
                catch (ParseException parseException)
                {
                    ProjectData.SetProjectError(parseException);
                    var e = parseException;
                    this.AddError(e, false);
                    ProjectData.ClearProjectError();
                }
            }
        }

        internal Node ExitNode(Node node)
        {
            var flag = !node.IsHidden() && this.errorRecovery < 0;
            Node exitNode;
            if (flag)
            {
                try
                {
                    exitNode = this.Analyzer.Exit(node);
                    return exitNode;
                }
                catch (ParseException parseException)
                {
                    ProjectData.SetProjectError(parseException);
                    var e = parseException;
                    this.AddError(e, false);
                    ProjectData.ClearProjectError();
                }
            }
            exitNode = node;
            return exitNode;
        }

        internal void AddNode(Production node, Node child)
        {
            var flag = this.errorRecovery >= 0;
            if (!flag)
            {
                var flag2 = node.IsHidden();
                if (flag2)
                {
                    node.AddChild(child);
                }
                else
                {
                    var flag3 = child != null && child.IsHidden();
                    if (flag3)
                    {
                        var num = child.Count - 1;
                        for (var i = 0; i <= num; i++)
                        {
                            this.AddNode(node, child[i]);
                        }
                    }
                    else
                    {
                        try
                        {
                            this.Analyzer.Child(node, child);
                        }
                        catch (ParseException parseException)
                        {
                            ProjectData.SetProjectError(parseException);
                            var e = parseException;
                            this.AddError(e, false);
                            ProjectData.ClearProjectError();
                        }
                    }
                }
            }
        }

        internal Token NextToken()
        {
            var token = this.PeekToken(0);
            var flag = token != null;
            if (flag)
            {
                this.tokens.RemoveAt(0);
                return token;
            }
            throw new ParseException(ParseException.ErrorType.UnexpectedEof, null, this.Tokenizer.GetCurrentLine(),
                this.Tokenizer.GetCurrentColumn());
        }

        internal Token NextToken(int id)
        {
            var token = this.NextToken();
            var flag = token.Id == id;
            if (flag)
            {
                var flag2 = this.errorRecovery > 0;
                if (flag2)
                {
                    this.errorRecovery--;
                }
                return token;
            }
            var list = new List<string>(1) {this.Tokenizer.GetPatternDescription(id)};
            throw new ParseException(ParseException.ErrorType.UnexpectedToken, token.ToShortString(), list, token.StartLine,
                token.StartColumn);
        }

        internal Token PeekToken(int steps)
        {
            while (steps >= this.tokens.Count)
            {
                try
                {
                    var token = this.Tokenizer.Next();
                    var flag = token == null;
                    if (flag)
                    {
                        return null;
                    }
                    this.tokens.Add(token);
                }
                catch (ParseException expr_2E)
                {
                    ProjectData.SetProjectError(expr_2E);
                    var e = expr_2E;
                    this.AddError(e, true);
                    ProjectData.ClearProjectError();
                }
            }
            var peekToken = (Token) this.tokens[steps];
            return peekToken;
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            var num = this.patterns.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                buffer.Append(this.ToString(this.patterns[i]));
                buffer.Append("\n");
            }
            return buffer.ToString();
        }

        private string ToString(ProductionPattern prod)
        {
            var buffer = new StringBuilder();
            var indent = new StringBuilder();
            buffer.Append(prod.Name);
            buffer.Append(" (");
            buffer.Append(prod.Id);
            buffer.Append(") ");
            var num = buffer.Length - 1;
            for (var i = 0; i <= num; i++)
            {
                indent.Append(" ");
            }
            buffer.Append("= ");
            indent.Append("| ");
            var num2 = prod.Count - 1;
            for (var i = 0; i <= num2; i++)
            {
                var flag = i > 0;
                if (flag)
                {
                    buffer.Append(indent);
                }
                buffer.Append(this.ToString(prod[i]));
                buffer.Append("\n");
            }
            var num3 = prod.Count - 1;
            for (var i = 0; i <= num3; i++)
            {
                var set = prod[i].LookAheadSet;
                var flag2 = set.GetMaxLength() > 1;
                if (flag2)
                {
                    buffer.Append("Using ");
                    buffer.Append(set.GetMaxLength());
                    buffer.Append(" token look-ahead for alternative ");
                    buffer.Append(i + 1);
                    buffer.Append(": ");
                    buffer.Append(set.ToString(this.Tokenizer));
                    buffer.Append("\n");
                }
            }
            return buffer.ToString();
        }

        private string ToString(ProductionPatternAlternative alt)
        {
            var buffer = new StringBuilder();
            var num = alt.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                var flag = i > 0;
                if (flag)
                {
                    buffer.Append(" ");
                }
                buffer.Append(this.ToString(alt[i]));
            }
            return buffer.ToString();
        }

        private string ToString(ProductionPatternElement elem)
        {
            var buffer = new StringBuilder();
            var min = elem.MinCount;
            var max = elem.MaxCount;
            var flag = min == 0 && max == 1;
            if (flag)
            {
                buffer.Append("[");
            }
            var flag2 = elem.IsToken();
            buffer.Append(flag2 ? this.GetTokenDescription(elem.Id) : this.GetPattern(elem.Id).Name);
            var flag3 = min == 0 && max == 1;
            if (flag3)
            {
                buffer.Append("]");
            }
            else
            {
                var flag4 = min == 0 && max == 2147483647;
                if (flag4)
                {
                    buffer.Append("*");
                }
                else
                {
                    var flag5 = min == 1 && max == 2147483647;
                    if (flag5)
                    {
                        buffer.Append("+");
                    }
                    else
                    {
                        var flag6 = min != 1 || max != 1;
                        if (flag6)
                        {
                            buffer.Append("{");
                            buffer.Append(min);
                            buffer.Append(",");
                            buffer.Append(max);
                            buffer.Append("}");
                        }
                    }
                }
            }
            return buffer.ToString();
        }

        internal string GetTokenDescription(int token)
        {
            var flag = this.Tokenizer == null;
            var getTokenDescription = flag ? "" : this.Tokenizer.GetPatternDescription(token);
            return getTokenDescription;
        }

        public Analyzer Analyzer { get; }

        public Tokenizer Tokenizer { get; }
    }
}