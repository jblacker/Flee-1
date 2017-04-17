using System;

namespace Flee
{
    internal class IlLocation : IEquatable<IlLocation>, IComparable<IlLocation>
    {
        private int myPosition;

        private const int LongBranchAdjust = 3;

        private const int BrSLength = 2;

        public IlLocation()
        {
        }

        public IlLocation(int position)
        {
            this.myPosition = position;
        }

        public void SetPosition(int position)
        {
            this.myPosition = position;
        }

        public void AdjustForLongBranch(int longBranchCount)
        {
            this.myPosition += longBranchCount * 3;
        }

        public bool IsLongBranch(IlLocation target)
        {
            return Utility.IsLongBranch(this.myPosition + 2, target.myPosition);
        }

        public bool Equals1(IlLocation other)
        {
            return this.myPosition == other.myPosition;
        }

        public override string ToString()
        {
            return this.myPosition.ToString("x");
        }

        public int CompareTo(IlLocation other)
        {
            return this.myPosition.CompareTo(other.myPosition);
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(IlLocation other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return this.myPosition == other.myPosition;
        }

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
        /// <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />. </param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return this.Equals((IlLocation) obj);
        }

        /// <summary>Serves as a hash function for a particular type. </summary>
        /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
        public override int GetHashCode()
        {
            return this.myPosition;
        }

        /// <summary>Returns a value that indicates whether the values of two <see cref="T:Ciloci.Flee.ILLocation" /> objects are equal.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        public static bool operator ==(IlLocation left, IlLocation right)
        {
            return Equals(left, right);
        }

        /// <summary>Returns a value that indicates whether two <see cref="T:Ciloci.Flee.ILLocation" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static bool operator !=(IlLocation left, IlLocation right)
        {
            return !Equals(left, right);
        }
    }
}