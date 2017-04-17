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

namespace Flee
{
    using System;
    using System.Reflection.Emit;

    internal class BranchInfo : IEquatable<BranchInfo>
    {
        private readonly IlLocation myEnd;
        private readonly IlLocation myStart;

        private Label myLabel;

        public BranchInfo(IlLocation startLocation, Label endLabel)
        {
            this.myStart = startLocation;
            this.myLabel = endLabel;
            this.myEnd = new IlLocation();
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

        public void AdjustForLongBranches(int longBranchCount)
        {
            this.myStart.AdjustForLongBranch(longBranchCount);
        }

        public void BakeIsLongBranch()
        {
            this.IsLongBranch = this.ComputeIsLongBranch();
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
            var flag = this.myLabel.Equals(target);
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

        public bool IsLongBranch { get; private set; }
    }
}