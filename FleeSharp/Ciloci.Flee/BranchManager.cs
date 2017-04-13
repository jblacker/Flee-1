using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Ciloci.Flee
{
	internal class BranchManager
	{
		private IList<BranchInfo> MyBranchInfos;

		private IDictionary<object, Label> MyKeyLabelMap;

		public BranchManager()
		{
			this.MyBranchInfos = new List<BranchInfo>();
			this.MyKeyLabelMap = new Dictionary<object, Label>();
		}

		public void ComputeBranches()
		{
			List<BranchInfo> betweenBranches = new List<BranchInfo>();
			try
			{
				IEnumerator<BranchInfo> enumerator = this.MyBranchInfos.GetEnumerator();
				while (enumerator.MoveNext())
				{
					BranchInfo bi = enumerator.Current;
					betweenBranches.Clear();
					this.FindBetweenBranches(bi, betweenBranches);
					int longBranchesBetween = this.CountLongBranches(betweenBranches);
					bi.AdjustForLongBranchesBetween(longBranchesBetween);
				}
			}
			finally
			{
				IEnumerator<BranchInfo> enumerator;
				if (enumerator != null)
				{
					enumerator.Dispose();
				}
			}
			int longBranchCount = 0;
			try
			{
				IEnumerator<BranchInfo> enumerator2 = this.MyBranchInfos.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					BranchInfo bi2 = enumerator2.Current;
					bi2.BakeIsLongBranch();
					bi2.AdjustForLongBranches(longBranchCount);
					longBranchCount += Convert.ToInt32(bi2.IsLongBranch);
				}
			}
			finally
			{
				IEnumerator<BranchInfo> enumerator2;
				if (enumerator2 != null)
				{
					enumerator2.Dispose();
				}
			}
		}

		private int CountLongBranches(ICollection<BranchInfo> dest)
		{
			int count = 0;
			try
			{
				IEnumerator<BranchInfo> enumerator = dest.GetEnumerator();
				while (enumerator.MoveNext())
				{
					BranchInfo bi = enumerator.Current;
					count += Convert.ToInt32(bi.ComputeIsLongBranch());
				}
			}
			finally
			{
				IEnumerator<BranchInfo> enumerator;
				if (enumerator != null)
				{
					enumerator.Dispose();
				}
			}
			return count;
		}

		private void FindBetweenBranches(BranchInfo target, ICollection<BranchInfo> dest)
		{
			try
			{
				IEnumerator<BranchInfo> enumerator = this.MyBranchInfos.GetEnumerator();
				while (enumerator.MoveNext())
				{
					BranchInfo bi = enumerator.Current;
					bool flag = bi.IsBetween(target);
					if (flag)
					{
						dest.Add(bi);
					}
				}
			}
			finally
			{
				IEnumerator<BranchInfo> enumerator;
				if (enumerator != null)
				{
					enumerator.Dispose();
				}
			}
		}

		public bool IsLongBranch(FleeILGenerator ilg, Label target)
		{
			IlLocation startLoc = new IlLocation(ilg.Length);
			BranchInfo bi = new BranchInfo(startLoc, target);
			int index = this.MyBranchInfos.IndexOf(bi);
			bi = this.MyBranchInfos[index];
			return bi.IsLongBranch;
		}

		public void AddBranch(FleeILGenerator ilg, Label target)
		{
			IlLocation startLoc = new IlLocation(ilg.Length);
			BranchInfo bi = new BranchInfo(startLoc, target);
			this.MyBranchInfos.Add(bi);
		}

		public Label FindLabel(object key)
		{
			return this.MyKeyLabelMap[RuntimeHelpers.GetObjectValue(key)];
		}

		public Label GetLabel(object key, FleeILGenerator ilg)
		{
			Label lbl = default(Label);
			bool flag = !this.MyKeyLabelMap.TryGetValue(RuntimeHelpers.GetObjectValue(key), out lbl);
			if (flag)
			{
				lbl = ilg.DefineLabel();
				this.MyKeyLabelMap.Add(RuntimeHelpers.GetObjectValue(key), lbl);
			}
			return lbl;
		}

		public bool HasLabel(object key)
		{
			return this.MyKeyLabelMap.ContainsKey(RuntimeHelpers.GetObjectValue(key));
		}

		public void MarkLabel(FleeILGenerator ilg, Label target)
		{
			int pos = ilg.Length;
			try
			{
				IEnumerator<BranchInfo> enumerator = this.MyBranchInfos.GetEnumerator();
				while (enumerator.MoveNext())
				{
					BranchInfo bi = enumerator.Current;
					bi.Mark(target, pos);
				}
			}
			finally
			{
				IEnumerator<BranchInfo> enumerator;
				if (enumerator != null)
				{
					enumerator.Dispose();
				}
			}
		}

		public override string ToString()
		{
			string[] arr = new string[this.MyBranchInfos.Count - 1 + 1];
			int num = this.MyBranchInfos.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				arr[i] = this.MyBranchInfos[i].ToString();
			}
			return string.Join(Environment.NewLine, arr);
		}
	}
}
