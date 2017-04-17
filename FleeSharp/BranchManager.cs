namespace Flee
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

            for (int i = 0; i < this.myBranchInfos.Count; i++)
                {
                betweenBranches.Clear();
                this.FindBetweenBranches(this.myBranchInfos[i], betweenBranches);
                var longBranchesBetween = this.CountLongBranches(betweenBranches);
                this.myBranchInfos[i].AdjustForLongBranchesBetween(longBranchesBetween);
            }
            var longBranchCount = 0;

            for (int i = 0; i < this.myBranchInfos.Count; i++)
            {
                myBranchInfos[i].BakeIsLongBranch();
                myBranchInfos[i].AdjustForLongBranches(longBranchCount);
                longBranchCount += Convert.ToInt32(myBranchInfos[i].IsLongBranch);
            }
        }

        private int CountLongBranches(ICollection<BranchInfo> dest)
        {
            var count = 0;
            dest.Each(d => count += Convert.ToInt32(d.ComputeIsLongBranch()));
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
            myBranchInfos.Each(m =>
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

        public bool IsLongBranch(FleeILGenerator ilg, Label target)
        {
            var startLoc = new IlLocation(ilg.Length);
            var bi = new BranchInfo(startLoc, target);
            var index = this.myBranchInfos.IndexOf(bi);
            bi = this.myBranchInfos[index];
            return bi.IsLongBranch;
        }

        public void AddBranch(FleeILGenerator ilg, Label target)
        {
            var startLoc = new IlLocation(ilg.Length);
            var bi = new BranchInfo(startLoc, target);
            this.myBranchInfos.Add(bi);
        }

        public Label FindLabel(object key)
        {
            return this.myKeyLabelMap[RuntimeHelpers.GetObjectValue(key)];
        }

        public Label GetLabel(object key, FleeILGenerator ilg)
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

        public void MarkLabel(FleeILGenerator ilg, Label target)
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