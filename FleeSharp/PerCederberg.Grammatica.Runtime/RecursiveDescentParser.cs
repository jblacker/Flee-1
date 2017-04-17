namespace Flee.PerCederberg.Grammatica.Runtime
{
    using System.Collections;
    using System.Collections.Generic;
    using Exceptions;
    using Microsoft.VisualBasic.CompilerServices;

    internal class RecursiveDescentParser : Parser
	{
		private class CallStack
		{
			private readonly ArrayList nameStack;

			private readonly ArrayList valueStack;

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
				var num = this.nameStack.Count - 1;
			    for (var i = 0; i <= num; i++)
				{
					var flag = this.nameStack[i].Equals(name) && this.valueStack[i].Equals(value);
					if (flag)
					{
					    return true;
					}
				}
			    return false;
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
				var flag = this.nameStack.Count > 0;
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
			var flag = pattern.IsMatchingEmpty();
			if (flag)
			{
				throw new ParserCreationException(ParserCreationException.ErrorType.INVALID_PRODUCTION, pattern.Name, "zero elements can be matched (minimum is one)");
			}
			var flag2 = pattern.IsLeftRecursive();
			if (flag2)
			{
				throw new ParserCreationException(ParserCreationException.ErrorType.INVALID_PRODUCTION, pattern.Name, "left recursive patterns are not allowed");
			}
			base.AddPattern(pattern);
		}

		public override void Prepare()
		{
			base.Prepare();
			this.SetInitialized(false);

		    foreach (var pattern in this.GetPatterns())
		    {
                this.CalculateLookAhead(pattern);
            }
			this.SetInitialized(true);
		}

		protected override Node ParseStart()
		{
			var node = this.ParsePattern(this.GetStartPattern());
			var token = this.PeekToken(0);
			var flag = token != null;
			if (flag)
			{
			    var list = new[] {"<EOF>"};
				throw new ParseException(ParseException.ErrorType.UnexpectedToken, token.ToShortString(), list, token.StartLine, token.StartColumn);
			}
			return node;
		}

		private Node ParsePattern(ProductionPattern pattern)
		{
			var defaultAlt = pattern.DefaultAlternative;
			var num = pattern.Count - 1;
			Node parsePattern;
			for (var i = 0; i <= num; i++)
			{
				var alt = pattern[i];
			    // ReSharper disable once PossibleUnintendedReferenceComparison
				var flag = defaultAlt != alt && this.IsNext(alt);
				if (flag)
				{
					parsePattern = this.ParseAlternative(alt);
					return parsePattern;
				}
			}
			var flag2 = defaultAlt == null || !this.IsNext(defaultAlt);
			if (flag2)
			{
				this.ThrowParseException(this.FindUnion(pattern));
			}
			parsePattern = this.ParseAlternative(defaultAlt);
			return parsePattern;
		}

		private Node ParseAlternative(ProductionPatternAlternative alt)
		{
			var node = new Production(alt.Pattern);
			this.EnterNode(node);
			var num = alt.Count - 1;
			for (var i = 0; i <= num; i++)
			{
				try
				{
					this.ParseElement(node, alt[i]);
				}
				catch (ParseException parseException)
				{
					ProjectData.SetProjectError(parseException);
					var e = parseException;
					this.AddError(e, true);
					this.NextToken();
					i--;
					ProjectData.ClearProjectError();
				}
			}
			return this.ExitNode(node);
		}

		private void ParseElement(Production node, ProductionPatternElement elem)
		{
			var num = elem.MaxCount - 1;
			for (var i = 0; i <= num; i++)
			{
				var flag = i < elem.MinCount || this.IsNext(elem);
				if (!flag)
				{
					break;
				}
				var flag2 = elem.IsToken();
				if (flag2)
				{
					Node child = this.NextToken(elem.Id);
					this.EnterNode(child);
					this.AddNode(node, this.ExitNode(child));
				}
				else
				{
					var child = this.ParsePattern(this.GetPattern(elem.Id));
					this.AddNode(node, child);
				}
			}
		}

		private bool IsNext(ProductionPattern pattern)
		{
			var set = pattern.LookAhead;
			var flag = set == null;
			return !flag && set.IsNext(this);
		}

		private bool IsNext(ProductionPatternAlternative alt)
		{
			var set = alt.LookAheadSet;
			var flag = set == null;
			return !flag && set.IsNext(this);
		}

		private bool IsNext(ProductionPatternElement elem)
		{
			var set = elem.LookAhead;
			var flag = set != null;
			bool isNext;
			if (flag)
			{
				isNext = set.IsNext(this);
			}
			else
			{
			    var flag2 = elem.IsToken();
			    isNext = flag2 ? elem.IsMatch(this.PeekToken(0)) : this.IsNext(this.GetPattern(elem.Id));
			}
			return isNext;
		}

		private void CalculateLookAhead(ProductionPattern pattern)
		{
			var previous = new LookAheadSet(0);
			var length = 1;
			var stack = new CallStack();
			stack.Push(pattern.Name, 1);
			var result = new LookAheadSet(1);
			var alternatives = new LookAheadSet[pattern.Count - 1 + 1];
			var num = pattern.Count - 1;
			for (var i = 0; i <= num; i++)
			{
				var alt = pattern[i];
				alternatives[i] = this.FindLookAhead(alt, 1, 0, stack, null);
				alt.LookAheadSet = alternatives[i];
				result.AddAll(alternatives[i]);
			}
			var flag = pattern.LookAhead == null;
			if (flag)
			{
				pattern.LookAhead = result;
			}
			var conflicts = this.FindConflicts(pattern, 1);
			while (conflicts.Size() > 0)
			{
				length++;
				stack.Clear();
				stack.Push(pattern.Name, length);
				conflicts.AddAll(previous);
				var num2 = pattern.Count - 1;
				for (var i = 0; i <= num2; i++)
				{
					var alt = pattern[i];
					var flag2 = alternatives[i].Intersects(conflicts);
					if (flag2)
					{
						alternatives[i] = this.FindLookAhead(alt, length, 0, stack, conflicts);
						alt.LookAheadSet = alternatives[i];
					}
					var flag3 = alternatives[i].Intersects(conflicts);
					if (flag3)
					{
						var flag4 = pattern.DefaultAlternative == null;
						if (flag4)
						{
							pattern.DefaultAlternative = alt;
						}
						else
						{
						    // ReSharper disable once PossibleUnintendedReferenceComparison
							var flag5 = pattern.DefaultAlternative != alt;
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
			var num3 = pattern.Count - 1;
			for (var i = 0; i <= num3; i++)
			{
				this.CalculateLookAhead(pattern[i], 0);
			}
		}

		private void CalculateLookAhead(ProductionPatternAlternative alt, int pos)
		{
			var previous = new LookAheadSet(0);
			var length = 1;
			var flag = pos >= alt.Count;
			if (!flag)
			{
				var pattern = alt.Pattern;
				var elem = alt[pos];
				var flag2 = elem.MinCount == elem.MaxCount;
				if (flag2)
				{
					this.CalculateLookAhead(alt, pos + 1);
				}
				else
				{
					var first = this.FindLookAhead(elem, 1, new CallStack(), null);
					var follow = this.FindLookAhead(alt, 1, pos + 1, new CallStack(), null);
					var location = "at position " + Conversions.ToString(pos + 1);
					var conflicts = this.FindConflicts(pattern.Name, location, first, follow);
					while (conflicts.Size() > 0)
					{
						length++;
						conflicts.AddAll(previous);
						first = this.FindLookAhead(elem, length, new CallStack(), conflicts);
						follow = this.FindLookAhead(alt, length, pos + 1, new CallStack(), conflicts);
						first = first.CreateCombination(follow);
						elem.LookAhead = first;
						var flag3 = first.Intersects(conflicts);
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

		private LookAheadSet FindLookAhead(ProductionPattern pattern, int length, CallStack stack, LookAheadSet filter)
		{
			var flag = stack.Contains(pattern.Name, length);
			if (flag)
			{
				throw new ParserCreationException(ParserCreationException.ErrorType.INFINITE_LOOP, pattern.Name, null);
			}
			stack.Push(pattern.Name, length);
			var result = new LookAheadSet(length);
			var num = pattern.Count - 1;
			for (var i = 0; i <= num; i++)
			{
				var temp = this.FindLookAhead(pattern[i], length, 0, stack, filter);
				result.AddAll(temp);
			}
			stack.Pop();
			return result;
		}

		private LookAheadSet FindLookAhead(ProductionPatternAlternative alt, int length, int pos, CallStack stack, LookAheadSet filter)
		{
			var flag = length <= 0 || pos >= alt.Count;
			LookAheadSet findLookAhead;
			if (flag)
			{
				findLookAhead = new LookAheadSet(0);
			}
			else
			{
				var first = this.FindLookAhead(alt[pos], length, stack, filter);
				var flag2 = alt[pos].MinCount == 0;
				if (flag2)
				{
					first.AddEmpty();
				}
				var flag3 = filter == null;
				if (flag3)
				{
					length -= first.GetMinLength();
					var flag4 = length > 0;
					if (flag4)
					{
						var follow = this.FindLookAhead(alt, length, pos + 1, stack, null);
						first = first.CreateCombination(follow);
					}
				}
				else
				{
					var flag5 = filter.IsOverlap(first);
					if (flag5)
					{
						var overlaps = first.CreateOverlaps(filter);
						length -= overlaps.GetMinLength();
						filter = filter.CreateFilter(overlaps);
						var follow = this.FindLookAhead(alt, length, pos + 1, stack, filter);
						first.RemoveAll(overlaps);
						first.AddAll(overlaps.CreateCombination(follow));
					}
				}
				findLookAhead = first;
			}
			return findLookAhead;
		}

		private LookAheadSet FindLookAhead(ProductionPatternElement elem, int length, CallStack stack, LookAheadSet filter)
		{
			var first = this.FindLookAhead(elem, length, 0, stack, filter);
			var result = new LookAheadSet(length);
			result.AddAll(first);
			var flag = filter == null || !filter.IsOverlap(result);
			LookAheadSet findLookAhead;
			if (flag)
			{
				findLookAhead = result;
			}
			else
			{
				var flag2 = elem.MaxCount == 2147483647;
				if (flag2)
				{
					first = first.CreateRepetitive();
				}
				var max = elem.MaxCount;
				var flag3 = length < max;
				if (flag3)
				{
					max = length;
				}
				var num = max - 1;
				for (var i = 1; i <= num; i++)
				{
					first = first.CreateOverlaps(filter);
					var flag4 = first.Size() <= 0 || first.GetMinLength() >= length;
					if (flag4)
					{
						break;
					}
					var follow = this.FindLookAhead(elem, length, 0, stack, filter.CreateFilter(first));
					first = first.CreateCombination(follow);
					result.AddAll(first);
				}
				findLookAhead = result;
			}
			return findLookAhead;
		}

	    // ReSharper disable once UnusedParameter.Local
		private LookAheadSet FindLookAhead(ProductionPatternElement elem, int length, int dummy, CallStack stack, LookAheadSet filter)
		{
			var flag = elem.IsToken();
			LookAheadSet result;
			if (flag)
			{
				result = new LookAheadSet(length);
				result.Add(elem.Id);
			}
			else
			{
				var pattern = this.GetPattern(elem.Id);
				result = this.FindLookAhead(pattern, length, stack, filter);
				var flag2 = stack.Contains(pattern.Name);
				if (flag2)
				{
					result = result.CreateRepetitive();
				}
			}
			return result;
		}

		private LookAheadSet FindConflicts(ProductionPattern pattern, int maxLength)
		{
			var result = new LookAheadSet(maxLength);
			var num = pattern.Count - 1;
			for (var i = 0; i <= num; i++)
			{
				var set = pattern[i].LookAheadSet;
				var num2 = i - 1;
				for (var j = 0; j <= num2; j++)
				{
					var set2 = pattern[j].LookAheadSet;
					result.AddAll(set.CreateIntersection(set2));
				}
			}
			var flag = result.IsRepetitive();
			if (flag)
			{
				this.ThrowAmbiguityException(pattern.Name, null, result);
			}
			return result;
		}

		private LookAheadSet FindConflicts(string pattern, string location, LookAheadSet set1, LookAheadSet set2)
		{
			var result = set1.CreateIntersection(set2);
			var flag = result.IsRepetitive();
			if (flag)
			{
				this.ThrowAmbiguityException(pattern, location, result);
			}
			return result;
		}

		private LookAheadSet FindUnion(ProductionPattern pattern)
		{
			var length = 0;
			var num = pattern.Count - 1;
			LookAheadSet result;
			for (var i = 0; i <= num; i++)
			{
				result = pattern[i].LookAheadSet;
				var flag = result.GetMaxLength() > length;
				if (flag)
				{
					length = result.GetMaxLength();
				}
			}
			result = new LookAheadSet(length);
			var num2 = pattern.Count - 1;
			for (var i = 0; i <= num2; i++)
			{
				result.AddAll(pattern[i].LookAheadSet);
			}
			return result;
		}

		private void ThrowParseException(LookAheadSet set)
		{
			var list = new List<string>();
			while (set.IsNext(this, 1))
			{
				set = set.CreateNextSet(this.NextToken().Id);
			}
			var initials = set.GetInitialTokens();
			var num = initials.Length - 1;
			for (var i = 0; i <= num; i++)
			{
				list.Add(this.GetTokenDescription(initials[i]));
			}
			var token = this.NextToken();
			throw new ParseException(ParseException.ErrorType.UnexpectedToken, token.ToShortString(), list, token.StartLine, token.StartColumn);
		}

		private void ThrowAmbiguityException(string pattern, string location, LookAheadSet set)
		{
			var list = new ArrayList();
			var initials = set.GetInitialTokens();
			var num = initials.Length - 1;
			for (var i = 0; i <= num; i++)
			{
				list.Add(this.GetTokenDescription(initials[i]));
			}
			throw new ParserCreationException(ParserCreationException.ErrorType.INHERENT_AMBIGUITY, pattern, location, list);
		}
	}
}
