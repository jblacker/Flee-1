using System;
using System.Reflection.Emit;

namespace Ciloci.Flee
{
	internal class BranchInfo : IEquatable<BranchInfo>
	{
		private readonly IlLocation myStart;

		private readonly IlLocation myEnd;

		private Label myLabel;

		private bool myIsLongBranch;

		public bool IsLongBranch => this.myIsLongBranch;

	    public BranchInfo(IlLocation startLocation, Label endLabel)
		{
			this.myStart = startLocation;
			this.myLabel = endLabel;
			this.myEnd = new IlLocation();
		}

		public void AdjustForLongBranches(int longBranchCount)
		{
			this.myStart.AdjustForLongBranch(longBranchCount);
		}

		public void BakeIsLongBranch()
		{
			this.myIsLongBranch = this.ComputeIsLongBranch();
		}

		public void AdjustForLongBranchesBetween(int betweenLongBranchCount)
		{
			this.myEnd.AdjustForLongBranch(betweenLongBranchCount);
		}

		public bool IsBetween(BranchInfo other)
		{
			return this.myStart.CompareTo(other.myStart) > 0 && this.myStart.CompareTo(other.myEnd) < 0;
		}

		public bool ComputeIsLongBranch()
		{
			return this.myStart.IsLongBranch(this.myEnd);
		}

		public void Mark(Label target, int position)
		{
			bool flag = this.myLabel.Equals(target);
			if (flag)
			{
				this.myEnd.SetPosition(position);
			}
		}

		public bool Equals1(BranchInfo other)
		{
			return this.myStart.Equals1(other.myStart) && this.myLabel.Equals(other.myLabel);
		}

		public override string ToString()
		{
			return $"{this.myStart} -> {this.myEnd} (L={this.myStart.IsLongBranch(this.myEnd)})";
		}

	    /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
	    /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
	    /// <param name="other">An object to compare with this object.</param>
	    public bool Equals(BranchInfo other)
	    {
	        if (other == null)
	        {
	            return false;
	        }
	        return this.myStart.Equals(other.myStart) && this.myLabel.Equals(other.myLabel);
	    }
	}
}
