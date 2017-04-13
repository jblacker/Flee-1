using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
using System.Text;

namespace Ciloci.Flee.PerCederberg.Grammatica.Runtime
{
	internal abstract class Parser
	{
		private bool initialized;

		private Tokenizer m_tokenizer;

		private Analyzer m_analyzer;

		private ArrayList patterns;

		private Hashtable patternIds;

		private ArrayList tokens;

		private ParserLogException errorLog;

		private int errorRecovery;

		public Tokenizer Tokenizer
		{
			get
			{
				return this.m_tokenizer;
			}
		}

		public Analyzer Analyzer
		{
			get
			{
				return this.m_analyzer;
			}
		}

		internal Parser(Tokenizer tokenizer) : this(tokenizer, null)
		{
		}

		internal Parser(Tokenizer tokenizer, Analyzer analyzer)
		{
			this.patterns = new ArrayList();
			this.patternIds = new Hashtable();
			this.tokens = new ArrayList();
			this.errorLog = new ParserLogException();
			this.errorRecovery = -1;
			this.m_tokenizer = tokenizer;
			bool flag = analyzer == null;
			if (flag)
			{
				this.m_analyzer = new Analyzer();
			}
			else
			{
				this.m_analyzer = analyzer;
			}
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
			bool flag = pattern.Count <= 0;
			if (flag)
			{
				throw new ParserCreationException(ParserCreationException.ErrorType.INVALID_PRODUCTION, pattern.Name, "no production alternatives are present (must have at least one)");
			}
			bool flag2 = this.patternIds.ContainsKey(pattern.Id);
			if (flag2)
			{
				throw new ParserCreationException(ParserCreationException.ErrorType.INVALID_PRODUCTION, pattern.Name, Conversions.ToString(Conversions.ToDouble("another pattern with the same id (") + (double)pattern.Id + Conversions.ToDouble(") has already been added")));
			}
			this.patterns.Add(pattern);
			this.patternIds.Add(pattern.Id, pattern);
			this.SetInitialized(false);
		}

		public virtual void Prepare()
		{
			bool flag = this.patterns.Count <= 0;
			if (flag)
			{
				throw new ParserCreationException(ParserCreationException.ErrorType.INVALID_PARSER, "no production patterns have been added");
			}
			int num = this.patterns.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				this.CheckPattern((ProductionPattern)this.patterns[i]);
			}
			this.SetInitialized(true);
		}

		private void CheckPattern(ProductionPattern pattern)
		{
			int num = pattern.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				this.CheckAlternative(pattern.Name, pattern[i]);
			}
		}

		private void CheckAlternative(string name, ProductionPatternAlternative alt)
		{
			int num = alt.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				this.CheckElement(name, alt[i]);
			}
		}

		private void CheckElement(string name, ProductionPatternElement elem)
		{
			bool flag = elem.IsProduction() && this.GetPattern(elem.Id) == null;
			if (flag)
			{
				throw new ParserCreationException(ParserCreationException.ErrorType.INVALID_PRODUCTION, name, Conversions.ToString(Conversions.ToDouble("an undefined production pattern id (") + (double)elem.Id + Conversions.ToDouble(") is referenced")));
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
			bool flag = !this.initialized;
			if (flag)
			{
				this.Prepare();
			}
			this.Reset();
			try
			{
				root = this.ParseStart();
			}
			catch (ParseException expr_2A)
			{
				ProjectData.SetProjectError(expr_2A);
				ParseException e = expr_2A;
				this.AddError(e, true);
				ProjectData.ClearProjectError();
			}
			bool flag2 = this.errorLog.Count > 0;
			if (flag2)
			{
				throw this.errorLog;
			}
			return root;
		}

		protected abstract Node ParseStart();

		internal void AddError(ParseException e, bool recovery)
		{
			bool flag = this.errorRecovery <= 0;
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
			return (ProductionPattern)this.patternIds[id];
		}

		internal ProductionPattern GetStartPattern()
		{
			bool flag = this.patterns.Count <= 0;
			ProductionPattern GetStartPattern;
			if (flag)
			{
				GetStartPattern = null;
			}
			else
			{
				GetStartPattern = (ProductionPattern)this.patterns[0];
			}
			return GetStartPattern;
		}

		internal ICollection GetPatterns()
		{
			return this.patterns;
		}

		internal void EnterNode(Node node)
		{
			bool flag = !node.IsHidden() && this.errorRecovery < 0;
			if (flag)
			{
				try
				{
					this.m_analyzer.Enter(node);
				}
				catch (ParseException expr_29)
				{
					ProjectData.SetProjectError(expr_29);
					ParseException e = expr_29;
					this.AddError(e, false);
					ProjectData.ClearProjectError();
				}
			}
		}

		internal Node ExitNode(Node node)
		{
			bool flag = !node.IsHidden() && this.errorRecovery < 0;
			Node ExitNode;
			if (flag)
			{
				try
				{
					ExitNode = this.m_analyzer.Exit(node);
					return ExitNode;
				}
				catch (ParseException expr_29)
				{
					ProjectData.SetProjectError(expr_29);
					ParseException e = expr_29;
					this.AddError(e, false);
					ProjectData.ClearProjectError();
				}
			}
			ExitNode = node;
			return ExitNode;
		}

		internal void AddNode(Production node, Node child)
		{
			bool flag = this.errorRecovery >= 0;
			if (!flag)
			{
				bool flag2 = node.IsHidden();
				if (flag2)
				{
					node.AddChild(child);
				}
				else
				{
					bool flag3 = child != null && child.IsHidden();
					if (flag3)
					{
						int num = child.Count - 1;
						for (int i = 0; i <= num; i++)
						{
							this.AddNode(node, child[i]);
						}
					}
					else
					{
						try
						{
							this.m_analyzer.Child(node, child);
						}
						catch (ParseException expr_77)
						{
							ProjectData.SetProjectError(expr_77);
							ParseException e = expr_77;
							this.AddError(e, false);
							ProjectData.ClearProjectError();
						}
					}
				}
			}
		}

		internal Token NextToken()
		{
			Token token = this.PeekToken(0);
			bool flag = token != null;
			if (flag)
			{
				this.tokens.RemoveAt(0);
				return token;
			}
			throw new ParseException(ParseException.ErrorType.UNEXPECTED_EOF, null, this.m_tokenizer.GetCurrentLine(), this.m_tokenizer.GetCurrentColumn());
		}

		internal Token NextToken(int id)
		{
			Token token = this.NextToken();
			bool flag = token.Id == id;
			if (flag)
			{
				bool flag2 = this.errorRecovery > 0;
				if (flag2)
				{
					this.errorRecovery--;
				}
				return token;
			}
			ArrayList list = new ArrayList(1);
			list.Add(this.m_tokenizer.GetPatternDescription(id));
			throw new ParseException(ParseException.ErrorType.UNEXPECTED_TOKEN, token.ToShortString(), list, token.StartLine, token.StartColumn);
		}

		internal Token PeekToken(int steps)
		{
			Token PeekToken;
			while (steps >= this.tokens.Count)
			{
				try
				{
					Token token = this.m_tokenizer.Next();
					bool flag = token == null;
					if (flag)
					{
						PeekToken = null;
						return PeekToken;
					}
					this.tokens.Add(token);
				}
				catch (ParseException expr_2E)
				{
					ProjectData.SetProjectError(expr_2E);
					ParseException e = expr_2E;
					this.AddError(e, true);
					ProjectData.ClearProjectError();
				}
			}
			PeekToken = (Token)this.tokens[steps];
			return PeekToken;
		}

		public override string ToString()
		{
			StringBuilder buffer = new StringBuilder();
			int num = this.patterns.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				buffer.Append(this.ToString((ProductionPattern)this.patterns[i]));
				buffer.Append("\n");
			}
			return buffer.ToString();
		}

		private string ToString(ProductionPattern prod)
		{
			StringBuilder buffer = new StringBuilder();
			StringBuilder indent = new StringBuilder();
			buffer.Append(prod.Name);
			buffer.Append(" (");
			buffer.Append(prod.Id);
			buffer.Append(") ");
			int num = buffer.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				indent.Append(" ");
			}
			buffer.Append("= ");
			indent.Append("| ");
			int num2 = prod.Count - 1;
			for (int i = 0; i <= num2; i++)
			{
				bool flag = i > 0;
				if (flag)
				{
					buffer.Append(indent);
				}
				buffer.Append(this.ToString(prod[i]));
				buffer.Append("\n");
			}
			int num3 = prod.Count - 1;
			for (int i = 0; i <= num3; i++)
			{
				LookAheadSet set = prod[i].LookAhead;
				bool flag2 = set.GetMaxLength() > 1;
				if (flag2)
				{
					buffer.Append("Using ");
					buffer.Append(set.GetMaxLength());
					buffer.Append(" token look-ahead for alternative ");
					buffer.Append(i + 1);
					buffer.Append(": ");
					buffer.Append(set.ToString(this.m_tokenizer));
					buffer.Append("\n");
				}
			}
			return buffer.ToString();
		}

		private string ToString(ProductionPatternAlternative alt)
		{
			StringBuilder buffer = new StringBuilder();
			int num = alt.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				bool flag = i > 0;
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
			StringBuilder buffer = new StringBuilder();
			int min = elem.MinCount;
			int max = elem.MaxCount;
			bool flag = min == 0 && max == 1;
			if (flag)
			{
				buffer.Append("[");
			}
			bool flag2 = elem.IsToken();
			if (flag2)
			{
				buffer.Append(this.GetTokenDescription(elem.Id));
			}
			else
			{
				buffer.Append(this.GetPattern(elem.Id).Name);
			}
			bool flag3 = min == 0 && max == 1;
			if (flag3)
			{
				buffer.Append("]");
			}
			else
			{
				bool flag4 = min == 0 && max == 2147483647;
				if (flag4)
				{
					buffer.Append("*");
				}
				else
				{
					bool flag5 = min == 1 && max == 2147483647;
					if (flag5)
					{
						buffer.Append("+");
					}
					else
					{
						bool flag6 = min != 1 || max != 1;
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
			bool flag = this.m_tokenizer == null;
			string GetTokenDescription;
			if (flag)
			{
				GetTokenDescription = "";
			}
			else
			{
				GetTokenDescription = this.m_tokenizer.GetPatternDescription(token);
			}
			return GetTokenDescription;
		}
	}
}
