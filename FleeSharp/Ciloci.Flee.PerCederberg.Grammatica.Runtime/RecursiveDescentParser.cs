using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;

namespace Ciloci.Flee.PerCederberg.Grammatica.Runtime
{
	internal class RecursiveDescentParser : Parser
	{
		private class CallStack
		{
			private ArrayList nameStack;

			private ArrayList valueStack;

			public CallStack()
			{
				this.nameStack = new ArrayList();
				this.valueStack = new ArrayList();
			}

			public bool Contains(string name)
			{
				return this.nameStack.Contains(name);
			}

			public bool Contains(string name, int value)
			{
				int num = this.nameStack.Count - 1;
				bool Contains;
				for (int i = 0; i <= num; i++)
				{
					bool flag = this.nameStack[i].Equals(name) && this.valueStack[i].Equals(value);
					if (flag)
					{
						Contains = true;
						return Contains;
					}
				}
				Contains = false;
				return Contains;
			}

			public void Clear()
			{
				this.nameStack.Clear();
				this.valueStack.Clear();
			}

			public void Push(string name, int value)
			{
				this.nameStack.Add(name);
				this.valueStack.Add(value);
			}

			public void Pop()
			{
				bool flag = this.nameStack.Count > 0;
				if (flag)
				{
					this.nameStack.RemoveAt(this.nameStack.Count - 1);
					this.valueStack.RemoveAt(this.valueStack.Count - 1);
				}
			}
		}

		public RecursiveDescentParser(Tokenizer tokenizer) : base(tokenizer)
		{
		}

		public RecursiveDescentParser(Tokenizer tokenizer, Analyzer analyzer) : base(tokenizer, analyzer)
		{
		}

		public override void AddPattern(ProductionPattern pattern)
		{
			bool flag = pattern.IsMatchingEmpty();
			if (flag)
			{
				throw new ParserCreationException(ParserCreationException.ErrorType.INVALID_PRODUCTION, pattern.Name, "zero elements can be matched (minimum is one)");
			}
			bool flag2 = pattern.IsLeftRecursive();
			if (flag2)
			{
				throw new ParserCreationException(ParserCreationException.ErrorType.INVALID_PRODUCTION, pattern.Name, "left recursive patterns are not allowed");
			}
			base.AddPattern(pattern);
		}

		public override void Prepare()
		{
			base.Prepare();
			base.SetInitialized(false);
			IEnumerator e = base.GetPatterns().GetEnumerator();
			while (e.MoveNext())
			{
				this.CalculateLookAhead((ProductionPattern)e.Current);
			}
			base.SetInitialized(true);
		}

		protected override Node ParseStart()
		{
			Node node = this.ParsePattern(base.GetStartPattern());
			Token token = base.PeekToken(0);
			bool flag = token != null;
			if (flag)
			{
				ArrayList list = new ArrayList(1);
				list.Add("<EOF>");
				throw new ParseException(ParseException.ErrorType.UNEXPECTED_TOKEN, token.ToShortString(), list, token.StartLine, token.StartColumn);
			}
			return node;
		}

		private Node ParsePattern(ProductionPattern pattern)
		{
			ProductionPatternAlternative defaultAlt = pattern.DefaultAlternative;
			int num = pattern.Count - 1;
			Node ParsePattern;
			for (int i = 0; i <= num; i++)
			{
				ProductionPatternAlternative alt = pattern[i];
				bool flag = defaultAlt != alt && this.IsNext(alt);
				if (flag)
				{
					ParsePattern = this.ParseAlternative(alt);
					return ParsePattern;
				}
			}
			bool flag2 = defaultAlt == null || !this.IsNext(defaultAlt);
			if (flag2)
			{
				this.ThrowParseException(this.FindUnion(pattern));
			}
			ParsePattern = this.ParseAlternative(defaultAlt);
			return ParsePattern;
		}

		private Node ParseAlternative(ProductionPatternAlternative alt)
		{
			Production node = new Production(alt.Pattern);
			base.EnterNode(node);
			int num = alt.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				try
				{
					this.ParseElement(node, alt[i]);
				}
				catch (ParseException expr_35)
				{
					ProjectData.SetProjectError(expr_35);
					ParseException e = expr_35;
					base.AddError(e, true);
					base.NextToken();
					i--;
					ProjectData.ClearProjectError();
				}
			}
			return base.ExitNode(node);
		}

		private void ParseElement(Production node, ProductionPatternElement elem)
		{
			int num = elem.MaxCount - 1;
			for (int i = 0; i <= num; i++)
			{
				bool flag = i < elem.MinCount || this.IsNext(elem);
				if (!flag)
				{
					break;
				}
				bool flag2 = elem.IsToken();
				if (flag2)
				{
					Node child = base.NextToken(elem.Id);
					base.EnterNode(child);
					base.AddNode(node, base.ExitNode(child));
				}
				else
				{
					Node child = this.ParsePattern(base.GetPattern(elem.Id));
					base.AddNode(node, child);
				}
			}
		}

		private bool IsNext(ProductionPattern pattern)
		{
			LookAheadSet set = pattern.LookAhead;
			bool flag = set == null;
			return !flag && set.IsNext(this);
		}

		private bool IsNext(ProductionPatternAlternative alt)
		{
			LookAheadSet set = alt.LookAhead;
			bool flag = set == null;
			return !flag && set.IsNext(this);
		}

		private bool IsNext(ProductionPatternElement elem)
		{
			LookAheadSet set = elem.LookAhead;
			bool flag = set != null;
			bool IsNext;
			if (flag)
			{
				IsNext = set.IsNext(this);
			}
			else
			{
				bool flag2 = elem.IsToken();
				if (flag2)
				{
					IsNext = elem.IsMatch(base.PeekToken(0));
				}
				else
				{
					IsNext = this.IsNext(base.GetPattern(elem.Id));
				}
			}
			return IsNext;
		}

		private void CalculateLookAhead(ProductionPattern pattern)
		{
			LookAheadSet previous = new LookAheadSet(0);
			int length = 1;
			RecursiveDescentParser.CallStack stack = new RecursiveDescentParser.CallStack();
			stack.Push(pattern.Name, 1);
			LookAheadSet result = new LookAheadSet(1);
			LookAheadSet[] alternatives = new LookAheadSet[pattern.Count - 1 + 1];
			int num = pattern.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				ProductionPatternAlternative alt = pattern[i];
				alternatives[i] = this.FindLookAhead(alt, 1, 0, stack, null);
				alt.LookAhead = alternatives[i];
				result.AddAll(alternatives[i]);
			}
			bool flag = pattern.LookAhead == null;
			if (flag)
			{
				pattern.LookAhead = result;
			}
			LookAheadSet conflicts = this.FindConflicts(pattern, 1);
			while (conflicts.Size() > 0)
			{
				length++;
				stack.Clear();
				stack.Push(pattern.Name, length);
				conflicts.AddAll(previous);
				int num2 = pattern.Count - 1;
				for (int i = 0; i <= num2; i++)
				{
					ProductionPatternAlternative alt = pattern[i];
					bool flag2 = alternatives[i].Intersects(conflicts);
					if (flag2)
					{
						alternatives[i] = this.FindLookAhead(alt, length, 0, stack, conflicts);
						alt.LookAhead = alternatives[i];
					}
					bool flag3 = alternatives[i].Intersects(conflicts);
					if (flag3)
					{
						bool flag4 = pattern.DefaultAlternative == null;
						if (flag4)
						{
							pattern.DefaultAlternative = alt;
						}
						else
						{
							bool flag5 = pattern.DefaultAlternative != alt;
							if (flag5)
							{
								result = alternatives[i].CreateIntersection(conflicts);
								this.ThrowAmbiguityException(pattern.Name, null, result);
							}
						}
					}
				}
				previous = conflicts;
				conflicts = this.FindConflicts(pattern, length);
			}
			int num3 = pattern.Count - 1;
			for (int i = 0; i <= num3; i++)
			{
				this.CalculateLookAhead(pattern[i], 0);
			}
		}

		private void CalculateLookAhead(ProductionPatternAlternative alt, int pos)
		{
			LookAheadSet previous = new LookAheadSet(0);
			int length = 1;
			bool flag = pos >= alt.Count;
			if (!flag)
			{
				ProductionPattern pattern = alt.Pattern;
				ProductionPatternElement elem = alt[pos];
				bool flag2 = elem.MinCount == elem.MaxCount;
				if (flag2)
				{
					this.CalculateLookAhead(alt, pos + 1);
				}
				else
				{
					LookAheadSet first = this.FindLookAhead(elem, 1, new RecursiveDescentParser.CallStack(), null);
					LookAheadSet follow = this.FindLookAhead(alt, 1, pos + 1, new RecursiveDescentParser.CallStack(), null);
					string location = "at position " + Conversions.ToString(pos + 1);
					LookAheadSet conflicts = this.FindConflicts(pattern.Name, location, first, follow);
					while (conflicts.Size() > 0)
					{
						length++;
						conflicts.AddAll(previous);
						first = this.FindLookAhead(elem, length, new RecursiveDescentParser.CallStack(), conflicts);
						follow = this.FindLookAhead(alt, length, pos + 1, new RecursiveDescentParser.CallStack(), conflicts);
						first = first.CreateCombination(follow);
						elem.LookAhead = first;
						bool flag3 = first.Intersects(conflicts);
						if (flag3)
						{
							first = first.CreateIntersection(conflicts);
							this.ThrowAmbiguityException(pattern.Name, location, first);
						}
						previous = conflicts;
						conflicts = this.FindConflicts(pattern.Name, location, first, follow);
					}
					this.CalculateLookAhead(alt, pos + 1);
				}
			}
		}

		private LookAheadSet FindLookAhead(ProductionPattern pattern, int length, RecursiveDescentParser.CallStack stack, LookAheadSet filter)
		{
			bool flag = stack.Contains(pattern.Name, length);
			if (flag)
			{
				throw new ParserCreationException(ParserCreationException.ErrorType.INFINITE_LOOP, pattern.Name, null);
			}
			stack.Push(pattern.Name, length);
			LookAheadSet result = new LookAheadSet(length);
			int num = pattern.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				LookAheadSet temp = this.FindLookAhead(pattern[i], length, 0, stack, filter);
				result.AddAll(temp);
			}
			stack.Pop();
			return result;
		}

		private LookAheadSet FindLookAhead(ProductionPatternAlternative alt, int length, int pos, RecursiveDescentParser.CallStack stack, LookAheadSet filter)
		{
			bool flag = length <= 0 || pos >= alt.Count;
			LookAheadSet FindLookAhead;
			if (flag)
			{
				FindLookAhead = new LookAheadSet(0);
			}
			else
			{
				LookAheadSet first = this.FindLookAhead(alt[pos], length, stack, filter);
				bool flag2 = alt[pos].MinCount == 0;
				if (flag2)
				{
					first.AddEmpty();
				}
				bool flag3 = filter == null;
				if (flag3)
				{
					length -= first.GetMinLength();
					bool flag4 = length > 0;
					if (flag4)
					{
						LookAheadSet follow = this.FindLookAhead(alt, length, pos + 1, stack, null);
						first = first.CreateCombination(follow);
					}
				}
				else
				{
					bool flag5 = filter.IsOverlap(first);
					if (flag5)
					{
						LookAheadSet overlaps = first.CreateOverlaps(filter);
						length -= overlaps.GetMinLength();
						filter = filter.CreateFilter(overlaps);
						LookAheadSet follow = this.FindLookAhead(alt, length, pos + 1, stack, filter);
						first.RemoveAll(overlaps);
						first.AddAll(overlaps.CreateCombination(follow));
					}
				}
				FindLookAhead = first;
			}
			return FindLookAhead;
		}

		private LookAheadSet FindLookAhead(ProductionPatternElement elem, int length, RecursiveDescentParser.CallStack stack, LookAheadSet filter)
		{
			LookAheadSet first = this.FindLookAhead(elem, length, 0, stack, filter);
			LookAheadSet result = new LookAheadSet(length);
			result.AddAll(first);
			bool flag = filter == null || !filter.IsOverlap(result);
			LookAheadSet FindLookAhead;
			if (flag)
			{
				FindLookAhead = result;
			}
			else
			{
				bool flag2 = elem.MaxCount == 2147483647;
				if (flag2)
				{
					first = first.CreateRepetitive();
				}
				int max = elem.MaxCount;
				bool flag3 = length < max;
				if (flag3)
				{
					max = length;
				}
				int num = max - 1;
				for (int i = 1; i <= num; i++)
				{
					first = first.CreateOverlaps(filter);
					bool flag4 = first.Size() <= 0 || first.GetMinLength() >= length;
					if (flag4)
					{
						break;
					}
					LookAheadSet follow = this.FindLookAhead(elem, length, 0, stack, filter.CreateFilter(first));
					first = first.CreateCombination(follow);
					result.AddAll(first);
				}
				FindLookAhead = result;
			}
			return FindLookAhead;
		}

		private LookAheadSet FindLookAhead(ProductionPatternElement elem, int length, int dummy, RecursiveDescentParser.CallStack stack, LookAheadSet filter)
		{
			bool flag = elem.IsToken();
			LookAheadSet result;
			if (flag)
			{
				result = new LookAheadSet(length);
				result.Add(elem.Id);
			}
			else
			{
				ProductionPattern pattern = base.GetPattern(elem.Id);
				result = this.FindLookAhead(pattern, length, stack, filter);
				bool flag2 = stack.Contains(pattern.Name);
				if (flag2)
				{
					result = result.CreateRepetitive();
				}
			}
			return result;
		}

		private LookAheadSet FindConflicts(ProductionPattern pattern, int maxLength)
		{
			LookAheadSet result = new LookAheadSet(maxLength);
			int num = pattern.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				LookAheadSet set = pattern[i].LookAhead;
				int num2 = i - 1;
				for (int j = 0; j <= num2; j++)
				{
					LookAheadSet set2 = pattern[j].LookAhead;
					result.AddAll(set.CreateIntersection(set2));
				}
			}
			bool flag = result.IsRepetitive();
			if (flag)
			{
				this.ThrowAmbiguityException(pattern.Name, null, result);
			}
			return result;
		}

		private LookAheadSet FindConflicts(string pattern, string location, LookAheadSet set1, LookAheadSet set2)
		{
			LookAheadSet result = set1.CreateIntersection(set2);
			bool flag = result.IsRepetitive();
			if (flag)
			{
				this.ThrowAmbiguityException(pattern, location, result);
			}
			return result;
		}

		private LookAheadSet FindUnion(ProductionPattern pattern)
		{
			int length = 0;
			int num = pattern.Count - 1;
			LookAheadSet result;
			for (int i = 0; i <= num; i++)
			{
				result = pattern[i].LookAhead;
				bool flag = result.GetMaxLength() > length;
				if (flag)
				{
					length = result.GetMaxLength();
				}
			}
			result = new LookAheadSet(length);
			int num2 = pattern.Count - 1;
			for (int i = 0; i <= num2; i++)
			{
				result.AddAll(pattern[i].LookAhead);
			}
			return result;
		}

		private void ThrowParseException(LookAheadSet set)
		{
			ArrayList list = new ArrayList();
			while (set.IsNext(this, 1))
			{
				set = set.CreateNextSet(base.NextToken().Id);
			}
			int[] initials = set.GetInitialTokens();
			int num = initials.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				list.Add(base.GetTokenDescription(initials[i]));
			}
			Token token = base.NextToken();
			throw new ParseException(ParseException.ErrorType.UNEXPECTED_TOKEN, token.ToShortString(), list, token.StartLine, token.StartColumn);
		}

		private void ThrowAmbiguityException(string pattern, string location, LookAheadSet set)
		{
			ArrayList list = new ArrayList();
			int[] initials = set.GetInitialTokens();
			int num = initials.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				list.Add(base.GetTokenDescription(initials[i]));
			}
			throw new ParserCreationException(ParserCreationException.ErrorType.INHERENT_AMBIGUITY, pattern, location, list);
		}
	}
}
