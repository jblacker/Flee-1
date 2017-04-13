using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Ciloci.Flee.PerCederberg.Grammatica.Runtime
{
	internal class Analyzer
	{
		public Node Analyze(Node node)
		{
			ParserLogException log = new ParserLogException();
			node = this.Analyze(node, log);
			bool flag = log.Count > 0;
			if (flag)
			{
				throw log;
			}
			return node;
		}

		private Node Analyze(Node node, ParserLogException log)
		{
			int errorCount = log.Count;
			bool flag = node is Production;
			Node Analyze;
			if (flag)
			{
				Production prod = (Production)node;
				prod = new Production(prod.Pattern);
				try
				{
					this.Enter(prod);
				}
				catch (ParseException expr_36)
				{
					ProjectData.SetProjectError(expr_36);
					ParseException e = expr_36;
					log.AddError(e);
					ProjectData.ClearProjectError();
				}
				int num = node.Count - 1;
				for (int i = 0; i <= num; i++)
				{
					try
					{
						this.Child(prod, this.Analyze(node[i], log));
					}
					catch (ParseException expr_7A)
					{
						ProjectData.SetProjectError(expr_7A);
						ParseException e2 = expr_7A;
						log.AddError(e2);
						ProjectData.ClearProjectError();
					}
				}
				try
				{
					Analyze = this.Exit(prod);
					return Analyze;
				}
				catch (ParseException expr_AF)
				{
					ProjectData.SetProjectError(expr_AF);
					ParseException e3 = expr_AF;
					bool flag2 = errorCount == log.Count;
					if (flag2)
					{
						log.AddError(e3);
					}
					ProjectData.ClearProjectError();
				}
			}
			else
			{
				node.Values.Clear();
				try
				{
					this.Enter(node);
				}
				catch (ParseException expr_F5)
				{
					ProjectData.SetProjectError(expr_F5);
					ParseException e4 = expr_F5;
					log.AddError(e4);
					ProjectData.ClearProjectError();
				}
				try
				{
					Analyze = this.Exit(node);
					return Analyze;
				}
				catch (ParseException expr_11A)
				{
					ProjectData.SetProjectError(expr_11A);
					ParseException e5 = expr_11A;
					bool flag3 = errorCount == log.Count;
					if (flag3)
					{
						log.AddError(e5);
					}
					ProjectData.ClearProjectError();
				}
			}
			Analyze = null;
			return Analyze;
		}

		public virtual void Enter(Node node)
		{
		}

		public virtual Node Exit(Node node)
		{
			return node;
		}

		public virtual void Child(Production node, Node child)
		{
			node.AddChild(child);
		}

		protected Node GetChildAt(Node node, int pos)
		{
			bool flag = node == null;
			if (flag)
			{
				throw new ParseException(ParseException.ErrorType.INTERNAL, "attempt to read 'null' parse tree node", -1, -1);
			}
			Node child = node[pos];
			bool flag2 = child == null;
			if (flag2)
			{
				throw new ParseException(ParseException.ErrorType.INTERNAL, Conversions.ToString(Conversions.ToDouble("node '" + node.Name + "' has no child at position ") + (double)pos), node.StartLine, node.StartColumn);
			}
			return child;
		}

		protected Node GetChildWithId(Node node, int id)
		{
			bool flag = node == null;
			if (flag)
			{
				throw new ParseException(ParseException.ErrorType.INTERNAL, "attempt to read 'null' parse tree node", -1, -1);
			}
			int num = node.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				Node child = node[i];
				bool flag2 = child != null && child.Id == id;
				if (flag2)
				{
					return child;
				}
			}
			throw new ParseException(ParseException.ErrorType.INTERNAL, Conversions.ToString(Conversions.ToDouble("node '" + node.Name + "' has no child with id ") + (double)id), node.StartLine, node.StartColumn);
		}

		protected object GetValue(Node node, int pos)
		{
			bool flag = node == null;
			if (flag)
			{
				throw new ParseException(ParseException.ErrorType.INTERNAL, "attempt to read 'null' parse tree node", -1, -1);
			}
			object value = RuntimeHelpers.GetObjectValue(node.Values[pos]);
			bool flag2 = value == null;
			if (flag2)
			{
				throw new ParseException(ParseException.ErrorType.INTERNAL, Conversions.ToString(Conversions.ToDouble("node '" + node.Name + "' has no value at position ") + (double)pos), node.StartLine, node.StartColumn);
			}
			return value;
		}

		protected int GetIntValue(Node node, int pos)
		{
			object value = RuntimeHelpers.GetObjectValue(this.GetValue(node, pos));
			bool flag = value is int;
			if (flag)
			{
				return Conversions.ToInteger(value);
			}
			throw new ParseException(ParseException.ErrorType.INTERNAL, Conversions.ToString(Conversions.ToDouble("node '" + node.Name + "' has no integer value at position ") + (double)pos), node.StartLine, node.StartColumn);
		}

		protected string GetStringValue(Node node, int pos)
		{
			object value = RuntimeHelpers.GetObjectValue(this.GetValue(node, pos));
			bool flag = value is string;
			if (flag)
			{
				return (string)value;
			}
			throw new ParseException(ParseException.ErrorType.INTERNAL, Conversions.ToString(Conversions.ToDouble("node '" + node.Name + "' has no string value at position ") + (double)pos), node.StartLine, node.StartColumn);
		}

		protected ArrayList GetChildValues(Node node)
		{
			ArrayList result = new ArrayList();
			int num = node.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				Node child = node[i];
				ArrayList values = child.Values;
				bool flag = values != null;
				if (flag)
				{
					result.AddRange(values);
				}
			}
			return result;
		}
	}
}
