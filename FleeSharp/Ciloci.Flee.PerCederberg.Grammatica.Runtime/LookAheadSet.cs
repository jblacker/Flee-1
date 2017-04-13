using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ciloci.Flee.PerCederberg.Grammatica.Runtime
{
	internal class LookAheadSet
	{
		private class Sequence
		{
			private bool repeat;

			private ArrayList tokens;

			public Sequence()
			{
				this.tokens = new ArrayList(0);
			}

			public Sequence(bool repeat, int token)
			{
				this.tokens = new ArrayList(1);
				this.tokens.Add(token);
			}

			public Sequence(int length, LookAheadSet.Sequence seq)
			{
				this.repeat = seq.repeat;
				this.tokens = new ArrayList(length);
				bool flag = seq.Length() < length;
				if (flag)
				{
					length = seq.Length();
				}
				int num = length - 1;
				for (int i = 0; i <= num; i++)
				{
					this.tokens.Add(RuntimeHelpers.GetObjectValue(seq.tokens[i]));
				}
			}

			public Sequence(bool repeat, LookAheadSet.Sequence seq)
			{
				this.repeat = repeat;
				this.tokens = seq.tokens;
			}

			public int Length()
			{
				return this.tokens.Count;
			}

			public object GetToken(int pos)
			{
				bool flag = pos >= 0 && pos < this.tokens.Count;
				object GetToken;
				if (flag)
				{
					GetToken = this.tokens[pos];
				}
				else
				{
					GetToken = null;
				}
				return GetToken;
			}

			public override bool Equals(object obj)
			{
				bool flag = obj is LookAheadSet.Sequence;
				return flag && this.Equals((LookAheadSet.Sequence)obj);
			}

			public bool Equals(LookAheadSet.Sequence seq)
			{
				bool flag = this.tokens.Count != seq.tokens.Count;
				bool Equals;
				if (flag)
				{
					Equals = false;
				}
				else
				{
					int num = this.tokens.Count - 1;
					for (int i = 0; i <= num; i++)
					{
						bool flag2 = !this.tokens[i].Equals(RuntimeHelpers.GetObjectValue(seq.tokens[i]));
						if (flag2)
						{
							Equals = false;
							return Equals;
						}
					}
					Equals = true;
				}
				return Equals;
			}

			public bool StartsWith(LookAheadSet.Sequence seq)
			{
				bool flag = this.Length() < seq.Length();
				bool StartsWith;
				if (flag)
				{
					StartsWith = false;
				}
				else
				{
					int num = seq.tokens.Count - 1;
					for (int i = 0; i <= num; i++)
					{
						bool flag2 = !this.tokens[i].Equals(RuntimeHelpers.GetObjectValue(seq.tokens[i]));
						if (flag2)
						{
							StartsWith = false;
							return StartsWith;
						}
					}
					StartsWith = true;
				}
				return StartsWith;
			}

			public bool IsRepetitive()
			{
				return this.repeat;
			}

			public bool IsNext(Parser parser)
			{
				int num = this.tokens.Count - 1;
				bool IsNext;
				for (int i = 0; i <= num; i++)
				{
					int id = Conversions.ToInteger(this.tokens[i]);
					Token token = parser.PeekToken(i);
					bool flag = token == null || token.Id != id;
					if (flag)
					{
						IsNext = false;
						return IsNext;
					}
				}
				IsNext = true;
				return IsNext;
			}

			public bool IsNext(Parser parser, int length)
			{
				bool flag = length > this.tokens.Count;
				if (flag)
				{
					length = this.tokens.Count;
				}
				int num = length - 1;
				bool IsNext;
				for (int i = 0; i <= num; i++)
				{
					int id = Conversions.ToInteger(this.tokens[i]);
					Token token = parser.PeekToken(i);
					bool flag2 = token == null || token.Id != id;
					if (flag2)
					{
						IsNext = false;
						return IsNext;
					}
				}
				IsNext = true;
				return IsNext;
			}

			public override string ToString()
			{
				return this.ToString(null);
			}

			public string ToString(Tokenizer tokenizer)
			{
				StringBuilder buffer = new StringBuilder();
				bool flag = tokenizer == null;
				if (flag)
				{
					buffer.Append(this.tokens.ToString());
				}
				else
				{
					buffer.Append("[");
					int num = this.tokens.Count - 1;
					for (int i = 0; i <= num; i++)
					{
						int id = Conversions.ToInteger(this.tokens[i]);
						string str = tokenizer.GetPatternDescription(id);
						bool flag2 = i > 0;
						if (flag2)
						{
							buffer.Append(" ");
						}
						buffer.Append(str);
					}
					buffer.Append("]");
				}
				bool flag3 = this.repeat;
				if (flag3)
				{
					buffer.Append(" *");
				}
				return buffer.ToString();
			}

			public LookAheadSet.Sequence Concat(int length, LookAheadSet.Sequence seq)
			{
				LookAheadSet.Sequence res = new LookAheadSet.Sequence(length, this);
				bool flag = seq.repeat;
				if (flag)
				{
					res.repeat = true;
				}
				length -= this.Length();
				bool flag2 = length > seq.Length();
				if (flag2)
				{
					res.tokens.AddRange(seq.tokens);
				}
				else
				{
					int num = length - 1;
					for (int i = 0; i <= num; i++)
					{
						res.tokens.Add(RuntimeHelpers.GetObjectValue(seq.tokens[i]));
					}
				}
				return res;
			}

			public LookAheadSet.Sequence Subsequence(int start)
			{
				LookAheadSet.Sequence res = new LookAheadSet.Sequence(this.Length(), this);
				while (start > 0 && res.tokens.Count > 0)
				{
					res.tokens.RemoveAt(0);
					start--;
				}
				return res;
			}
		}

		private ArrayList elements;

		private int maxLength;

		public LookAheadSet(int maxLength)
		{
			this.elements = new ArrayList();
			this.maxLength = maxLength;
		}

		public LookAheadSet(int maxLength, LookAheadSet set) : this(maxLength)
		{
			this.AddAll(set);
		}

		public int Size()
		{
			return this.elements.Count;
		}

		public int GetMinLength()
		{
			int min = -1;
			int num = this.elements.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				LookAheadSet.Sequence seq = (LookAheadSet.Sequence)this.elements[i];
				bool flag = min < 0 || seq.Length() < min;
				if (flag)
				{
					min = seq.Length();
				}
			}
			return Conversions.ToInteger(Interaction.IIf(min < 0, 0, min));
		}

		public int GetMaxLength()
		{
			int max = 0;
			int num = this.elements.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				LookAheadSet.Sequence seq = (LookAheadSet.Sequence)this.elements[i];
				bool flag = seq.Length() > max;
				if (flag)
				{
					max = seq.Length();
				}
			}
			return max;
		}

		public int[] GetInitialTokens()
		{
			ArrayList list = new ArrayList();
			int num = this.elements.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				object token = RuntimeHelpers.GetObjectValue(((LookAheadSet.Sequence)this.elements[i]).GetToken(0));
				bool flag = token != null && !list.Contains(RuntimeHelpers.GetObjectValue(token));
				if (flag)
				{
					list.Add(RuntimeHelpers.GetObjectValue(token));
				}
			}
			int[] result = new int[list.Count - 1 + 1];
			int num2 = list.Count - 1;
			for (int i = 0; i <= num2; i++)
			{
				result[i] = Conversions.ToInteger(list[i]);
			}
			return result;
		}

		public bool IsRepetitive()
		{
			int num = this.elements.Count - 1;
			bool IsRepetitive;
			for (int i = 0; i <= num; i++)
			{
				LookAheadSet.Sequence seq = (LookAheadSet.Sequence)this.elements[i];
				bool flag = seq.IsRepetitive();
				if (flag)
				{
					IsRepetitive = true;
					return IsRepetitive;
				}
			}
			IsRepetitive = false;
			return IsRepetitive;
		}

		public bool IsNext(Parser parser)
		{
			int num = this.elements.Count - 1;
			bool IsNext;
			for (int i = 0; i <= num; i++)
			{
				LookAheadSet.Sequence seq = (LookAheadSet.Sequence)this.elements[i];
				bool flag = seq.IsNext(parser);
				if (flag)
				{
					IsNext = true;
					return IsNext;
				}
			}
			IsNext = false;
			return IsNext;
		}

		public bool IsNext(Parser parser, int length)
		{
			int num = this.elements.Count - 1;
			bool IsNext;
			for (int i = 0; i <= num; i++)
			{
				LookAheadSet.Sequence seq = (LookAheadSet.Sequence)this.elements[i];
				bool flag = seq.IsNext(parser, length);
				if (flag)
				{
					IsNext = true;
					return IsNext;
				}
			}
			IsNext = false;
			return IsNext;
		}

		public bool IsOverlap(LookAheadSet set)
		{
			int num = this.elements.Count - 1;
			bool IsOverlap;
			for (int i = 0; i <= num; i++)
			{
				bool flag = set.IsOverlap((LookAheadSet.Sequence)this.elements[i]);
				if (flag)
				{
					IsOverlap = true;
					return IsOverlap;
				}
			}
			IsOverlap = false;
			return IsOverlap;
		}

		private bool IsOverlap(LookAheadSet.Sequence seq)
		{
			int num = this.elements.Count - 1;
			bool IsOverlap;
			for (int i = 0; i <= num; i++)
			{
				LookAheadSet.Sequence elem = (LookAheadSet.Sequence)this.elements[i];
				bool flag = seq.StartsWith(elem) || elem.StartsWith(seq);
				if (flag)
				{
					IsOverlap = true;
					return IsOverlap;
				}
			}
			IsOverlap = false;
			return IsOverlap;
		}

		private bool Contains(LookAheadSet.Sequence elem)
		{
			return this.FindSequence(elem) != null;
		}

		public bool Intersects(LookAheadSet set)
		{
			int num = this.elements.Count - 1;
			bool Intersects;
			for (int i = 0; i <= num; i++)
			{
				bool flag = set.Contains((LookAheadSet.Sequence)this.elements[i]);
				if (flag)
				{
					Intersects = true;
					return Intersects;
				}
			}
			Intersects = false;
			return Intersects;
		}

		private LookAheadSet.Sequence FindSequence(LookAheadSet.Sequence elem)
		{
			int num = this.elements.Count - 1;
			LookAheadSet.Sequence FindSequence;
			for (int i = 0; i <= num; i++)
			{
				bool flag = this.elements[i].Equals(elem);
				if (flag)
				{
					FindSequence = (LookAheadSet.Sequence)this.elements[i];
					return FindSequence;
				}
			}
			FindSequence = null;
			return FindSequence;
		}

		private void Add(LookAheadSet.Sequence seq)
		{
			bool flag = seq.Length() > this.maxLength;
			if (flag)
			{
				seq = new LookAheadSet.Sequence(this.maxLength, seq);
			}
			bool flag2 = !this.Contains(seq);
			if (flag2)
			{
				this.elements.Add(seq);
			}
		}

		public void Add(int token)
		{
			this.Add(new LookAheadSet.Sequence(false, token));
		}

		public void AddAll(LookAheadSet set)
		{
			int num = set.elements.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				this.Add((LookAheadSet.Sequence)set.elements[i]);
			}
		}

		public void AddEmpty()
		{
			this.Add(new LookAheadSet.Sequence());
		}

		private void Remove(LookAheadSet.Sequence seq)
		{
			this.elements.Remove(seq);
		}

		public void RemoveAll(LookAheadSet set)
		{
			int num = set.elements.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				this.Remove((LookAheadSet.Sequence)set.elements[i]);
			}
		}

		public LookAheadSet CreateNextSet(int token)
		{
			LookAheadSet result = new LookAheadSet(this.maxLength - 1);
			int num = this.elements.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				LookAheadSet.Sequence seq = (LookAheadSet.Sequence)this.elements[i];
				object value = RuntimeHelpers.GetObjectValue(seq.GetToken(0));
				bool flag = value != null && token == Conversions.ToInteger(value);
				if (flag)
				{
					result.Add(seq.Subsequence(1));
				}
			}
			return result;
		}

		public LookAheadSet CreateIntersection(LookAheadSet set)
		{
			LookAheadSet result = new LookAheadSet(this.maxLength);
			int num = this.elements.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				LookAheadSet.Sequence seq = (LookAheadSet.Sequence)this.elements[i];
				LookAheadSet.Sequence seq2 = set.FindSequence(seq);
				bool flag = seq2 != null && seq.IsRepetitive();
				if (flag)
				{
					result.Add(seq2);
				}
				else
				{
					bool flag2 = seq2 != null;
					if (flag2)
					{
						result.Add(seq);
					}
				}
			}
			return result;
		}

		public LookAheadSet CreateCombination(LookAheadSet set)
		{
			LookAheadSet result = new LookAheadSet(this.maxLength);
			bool flag = this.Size() <= 0;
			LookAheadSet CreateCombination;
			if (flag)
			{
				CreateCombination = set;
			}
			else
			{
				bool flag2 = set.Size() <= 0;
				if (flag2)
				{
					CreateCombination = this;
				}
				else
				{
					int num = this.elements.Count - 1;
					for (int i = 0; i <= num; i++)
					{
						LookAheadSet.Sequence first = (LookAheadSet.Sequence)this.elements[i];
						bool flag3 = first.Length() >= this.maxLength;
						if (flag3)
						{
							result.Add(first);
						}
						else
						{
							bool flag4 = first.Length() <= 0;
							if (flag4)
							{
								result.AddAll(set);
							}
							else
							{
								int num2 = set.elements.Count - 1;
								for (int j = 0; j <= num2; j++)
								{
									LookAheadSet.Sequence second = (LookAheadSet.Sequence)set.elements[j];
									result.Add(first.Concat(this.maxLength, second));
								}
							}
						}
					}
					CreateCombination = result;
				}
			}
			return CreateCombination;
		}

		public LookAheadSet CreateOverlaps(LookAheadSet set)
		{
			LookAheadSet result = new LookAheadSet(this.maxLength);
			int num = this.elements.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				LookAheadSet.Sequence seq = (LookAheadSet.Sequence)this.elements[i];
				bool flag = set.IsOverlap(seq);
				if (flag)
				{
					result.Add(seq);
				}
			}
			return result;
		}

		public LookAheadSet CreateFilter(LookAheadSet set)
		{
			LookAheadSet result = new LookAheadSet(this.maxLength);
			bool flag = this.Size() <= 0 || set.Size() <= 0;
			LookAheadSet CreateFilter;
			if (flag)
			{
				CreateFilter = this;
			}
			else
			{
				int num = this.elements.Count - 1;
				for (int i = 0; i <= num; i++)
				{
					LookAheadSet.Sequence first = (LookAheadSet.Sequence)this.elements[i];
					int num2 = set.elements.Count - 1;
					for (int j = 0; j <= num2; j++)
					{
						LookAheadSet.Sequence second = (LookAheadSet.Sequence)set.elements[j];
						bool flag2 = first.StartsWith(second);
						if (flag2)
						{
							result.Add(first.Subsequence(second.Length()));
						}
					}
				}
				CreateFilter = result;
			}
			return CreateFilter;
		}

		public LookAheadSet CreateRepetitive()
		{
			LookAheadSet result = new LookAheadSet(this.maxLength);
			int num = this.elements.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				LookAheadSet.Sequence seq = (LookAheadSet.Sequence)this.elements[i];
				bool flag = seq.IsRepetitive();
				if (flag)
				{
					result.Add(seq);
				}
				else
				{
					result.Add(new LookAheadSet.Sequence(true, seq));
				}
			}
			return result;
		}

		public override string ToString()
		{
			return this.ToString(null);
		}

		public string ToString(Tokenizer tokenizer)
		{
			StringBuilder buffer = new StringBuilder();
			buffer.Append("{");
			int num = this.elements.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				LookAheadSet.Sequence seq = (LookAheadSet.Sequence)this.elements[i];
				buffer.Append("\n ");
				buffer.Append(seq.ToString(tokenizer));
			}
			buffer.Append("\n}");
			return buffer.ToString();
		}
	}
}
