namespace Flee.PerCederberg.Grammatica.Runtime
{
    using System.Collections;
    using System.Runtime.CompilerServices;
    using Exceptions;
    using Microsoft.VisualBasic.CompilerServices;

    internal class Analyzer
	{
		public Node Analyze(Node node)
		{
			var log = new ParserLogException();
			node = this.Analyze(node, log);
			var flag = log.Count > 0;
			if (flag)
			{
				throw log;
			}
			return node;
		}

		private Node Analyze(Node node, ParserLogException log)
		{
			var errorCount = log.Count;
			var flag = node is Production;
			Node analyze;
			if (flag)
			{
				var prod = (Production)node;
				prod = new Production(prod.Pattern);
				try
				{
					this.Enter(prod);
				}
				catch (ParseException exception)
				{
					ProjectData.SetProjectError(exception);
					var e = exception;
					log.AddError(e);
					ProjectData.ClearProjectError();
				}
				var num = node.Count - 1;
				for (var i = 0; i <= num; i++)
				{
					try
					{
						this.Child(prod, this.Analyze(node[i], log));
					}
					catch (ParseException expr_7A)
					{
						ProjectData.SetProjectError(expr_7A);
						var e2 = expr_7A;
						log.AddError(e2);
						ProjectData.ClearProjectError();
					}
				}
				try
				{
					analyze = this.Exit(prod);
					return analyze;
				}
				catch (ParseException exception)
				{
					ProjectData.SetProjectError(exception);
					var e3 = exception;
					var flag2 = errorCount == log.Count;
					if (flag2)
					{
						log.AddError(e3);
					}
					ProjectData.ClearProjectError();
				}
			}
			else
			{
				node.ValuesArrayList.Clear();
				try
				{
					this.Enter(node);
				}
				catch (ParseException exception)
				{
					ProjectData.SetProjectError(exception);
					var e4 = exception;
					log.AddError(e4);
					ProjectData.ClearProjectError();
				}
				try
				{
					analyze = this.Exit(node);
					return analyze;
				}
				catch (ParseException exception)
				{
					ProjectData.SetProjectError(exception);
					var e5 = exception;
					var flag3 = errorCount == log.Count;
					if (flag3)
					{
						log.AddError(e5);
					}
					ProjectData.ClearProjectError();
				}
			}
            return null;
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
			var flag = node == null;
			if (flag)
			{
				throw new ParseException(ParseException.ErrorType.Internal, "attempt to read 'null' parse tree node", -1, -1);
			}
			var child = node[pos];
			var flag2 = child == null;
			if (flag2)
			{
				throw new ParseException(ParseException.ErrorType.Internal, Conversions.ToString(Conversions.ToDouble("node '" + node.Name + "' has no child at position ") + pos), node.StartLine, node.StartColumn);
			}
			return child;
		}

		protected Node GetChildWithId(Node node, int id)
		{
			var flag = node == null;
			if (flag)
			{
				throw new ParseException(ParseException.ErrorType.Internal, "attempt to read 'null' parse tree node", -1, -1);
			}
			var num = node.Count - 1;
			for (var i = 0; i <= num; i++)
			{
				var child = node[i];
				var flag2 = child != null && child.Id == id;
				if (flag2)
				{
					return child;
				}
			}
			throw new ParseException(ParseException.ErrorType.Internal, Conversions.ToString(Conversions.ToDouble("node '" + node.Name + "' has no child with id ") + id), node.StartLine, node.StartColumn);
		}

		protected object GetValue(Node node, int pos)
		{
			var flag = node == null;
			if (flag)
			{
				throw new ParseException(ParseException.ErrorType.Internal, "attempt to read 'null' parse tree node", -1, -1);
			}
			var value = RuntimeHelpers.GetObjectValue(node.ValuesArrayList[pos]);
			var flag2 = value == null;
			if (flag2)
			{
				throw new ParseException(ParseException.ErrorType.Internal, Conversions.ToString(Conversions.ToDouble("node '" + node.Name + "' has no value at position ") + pos), node.StartLine, node.StartColumn);
			}
			return value;
		}

		protected int GetIntValue(Node node, int pos)
		{
			var value = RuntimeHelpers.GetObjectValue(this.GetValue(node, pos));
			var flag = value is int;
			if (flag)
			{
				return Conversions.ToInteger(value);
			}
			throw new ParseException(ParseException.ErrorType.Internal, Conversions.ToString(Conversions.ToDouble("node '" + node.Name + "' has no integer value at position ") + pos), node.StartLine, node.StartColumn);
		}

		protected string GetStringValue(Node node, int pos)
		{
			var value = RuntimeHelpers.GetObjectValue(this.GetValue(node, pos));
			var flag = value is string;
			if (flag)
			{
				return (string)value;
			}
			throw new ParseException(ParseException.ErrorType.Internal, Conversions.ToString(Conversions.ToDouble("node '" + node.Name + "' has no string value at position ") + pos), node.StartLine, node.StartColumn);
		}

		protected ArrayList GetChildValues(Node node)
		{
			var result = new ArrayList();
			var num = node.Count - 1;
			for (var i = 0; i <= num; i++)
			{
				var child = node[i];
				var values = child.ValuesArrayList;
				var flag = values != null;
				if (flag)
				{
					result.AddRange(values);
				}
			}
			return result;
		}
	}
}
