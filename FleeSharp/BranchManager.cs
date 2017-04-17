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

namespace FleeSharp
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using Extensions;

    internal class BranchManager
    {
        private readonly IList<BranchInfo> myBranchInfos;

        private readonly IDictionary<object, Label> myKeyLabelMap;

        public BranchManager()
        {
            this.myBranchInfos = new List<BranchInfo>();
            this.myKeyLabelMap = new Dictionary<object, Label>();
        }

        public void ComputeBranches()
        {
            var betweenBranches = new List<BranchInfo>();

            for (var i = 0; i < this.myBranchInfos.Count; i++)
            {
                betweenBranches.Clear();
                this.FindBetweenBranches(this.myBranchInfos[i], betweenBranches);
                var longBranchesBetween = this.CountLongBranches(betweenBranches);
                this.myBranchInfos[i].AdjustForLongBranchesBetween(longBranchesBetween);
            }
            var longBranchCount = 0;

            for (var i = 0; i < this.myBranchInfos.Count; i++)
            {
                this.myBranchInfos[i].BakeIsLongBranch();
                this.myBranchInfos[i].AdjustForLongBranches(longBranchCount);
                longBranchCount += Convert.ToInt32(this.myBranchInfos[i].IsLongBranch);
            }
        }

        private int CountLongBranches(ICollection<BranchInfo> dest)
        {
            var count = 0;
            dest.Each(d => count += Convert.ToInt32((bool) d.ComputeIsLongBranch()));
            //try
            //{
            //    var enumerator = dest.GetEnumerator();
            //    while (enumerator.MoveNext())
            //    {
            //        var bi = enumerator.Current;
            //        count += Convert.ToInt32(bi.ComputeIsLongBranch());
            //    }
            //}
            //finally
            //{
            //    IEnumerator<BranchInfo> enumerator;
            //    if (enumerator != null)
            //    {
            //        enumerator.Dispose();
            //    }
            //}
            return count;
        }

        private void FindBetweenBranches(BranchInfo target, ICollection<BranchInfo> dest)
        {
            this.myBranchInfos.Each(m =>
            {
                if (m.IsBetween(target))
                {
                    dest.Add(m);
                }
            });
            //try
            //{
            //    var enumerator = this.myBranchInfos.GetEnumerator();
            //    while (enumerator.MoveNext())
            //    {
            //        var bi = enumerator.Current;
            //        var flag = bi.IsBetween(target);
            //        if (flag)
            //        {
            //            dest.Add(bi);
            //        }
            //    }
            //}
            //finally
            //{
            //    IEnumerator<BranchInfo> enumerator;
            //    if (enumerator != null)
            //    {
            //        enumerator.Dispose();
            //    }
            //}
        }

        public bool IsLongBranch(FleeIlGenerator ilg, Label target)
        {
            var startLoc = new IlLocation(ilg.Length);
            var bi = new BranchInfo(startLoc, target);
            var index = this.myBranchInfos.IndexOf(bi);
            bi = this.myBranchInfos[index];
            return bi.IsLongBranch;
        }

        public void AddBranch(FleeIlGenerator ilg, Label target)
        {
            var startLoc = new IlLocation(ilg.Length);
            var bi = new BranchInfo(startLoc, target);
            this.myBranchInfos.Add(bi);
        }

        public Label FindLabel(object key)
        {
            return this.myKeyLabelMap[RuntimeHelpers.GetObjectValue(key)];
        }

        public Label GetLabel(object key, FleeIlGenerator ilg)
        {
            Label lbl;
            var flag = !this.myKeyLabelMap.TryGetValue(RuntimeHelpers.GetObjectValue(key), out lbl);
            if (flag)
            {
                lbl = ilg.DefineLabel();
                this.myKeyLabelMap.Add(RuntimeHelpers.GetObjectValue(key), lbl);
            }
            return lbl;
        }

        public bool HasLabel(object key)
        {
            return this.myKeyLabelMap.ContainsKey(RuntimeHelpers.GetObjectValue(key));
        }

        public void MarkLabel(FleeIlGenerator ilg, Label target)
        {
            var pos = ilg.Length;
            this.myBranchInfos.Each(b => b.Mark(target, pos));

            //try
            //{
            //    var enumerator = this.myBranchInfos.GetEnumerator();
            //    while (enumerator.MoveNext())
            //    {
            //        var bi = enumerator.Current;
            //        bi.Mark(target, pos);
            //    }
            //}
            //finally
            //{
            //    IEnumerator<BranchInfo> enumerator;
            //    if (enumerator != null)
            //    {
            //        enumerator.Dispose();
            //    }
            //}
        }

        public override string ToString()
        {
            var arr = new string[this.myBranchInfos.Count - 1 + 1];
            var num = this.myBranchInfos.Count - 1;
            for (var i = 0; i <= num; i++)
            {
                arr[i] = this.myBranchInfos[i].ToString();
            }
            return string.Join(Environment.NewLine, arr);
        }
    }
}