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
    using System.Collections.Generic;
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
                var prod = (Production) node;
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
                throw new ParseException(ParseException.ErrorType.Internal,
                    Conversions.ToString(Conversions.ToDouble("node '" + node.Name + "' has no child at position ") + pos),
                    node.StartLine, node.StartColumn);
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
            throw new ParseException(ParseException.ErrorType.Internal,
                Conversions.ToString(Conversions.ToDouble("node '" + node.Name + "' has no child with id ") + id), node.StartLine,
                node.StartColumn);
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
                throw new ParseException(ParseException.ErrorType.Internal,
                    Conversions.ToString(Conversions.ToDouble("node '" + node.Name + "' has no value at position ") + pos),
                    node.StartLine, node.StartColumn);
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
            throw new ParseException(ParseException.ErrorType.Internal,
                Conversions.ToString(Conversions.ToDouble("node '" + node.Name + "' has no integer value at position ") + pos),
                node.StartLine, node.StartColumn);
        }

        protected string GetStringValue(Node node, int pos)
        {
            var value = RuntimeHelpers.GetObjectValue(this.GetValue(node, pos));
            var flag = value is string;
            if (flag)
            {
                return (string) value;
            }
            throw new ParseException(ParseException.ErrorType.Internal,
                Conversions.ToString(Conversions.ToDouble("node '" + node.Name + "' has no string value at position ") + pos),
                node.StartLine, node.StartColumn);
        }

        protected List<object> GetChildValues(Node node)
        {
            var result = new List<object>();
            var num = node.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                var child = node[i];
                var values = child.ValuesArrayList;
                var flag = values != null;
                if (flag)
                {
                    foreach (var o in values)
                    {
                        result.Add(o);
                    }
                    //result.AddRange((object)values);
                }
            }
            return result;
        }
    }
}