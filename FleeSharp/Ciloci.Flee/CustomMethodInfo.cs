using System;
using System.Diagnostics;
using System.Reflection;

namespace Ciloci.Flee
{
	internal class CustomMethodInfo : IComparable<CustomMethodInfo>, IEquatable<CustomMethodInfo>
	{
		private readonly MethodInfo myTarget;

		private float myScore;

		public bool isParamArray;

		public Type[] myFixedArgTypes;

		public Type[] myParamArrayArgTypes;

		public Type paramArrayElementType;

		public MethodInfo Target => this.myTarget;

	    public CustomMethodInfo(MethodInfo target)
		{
			this.myTarget = target;
		}

		public void ComputeScore(Type[] argTypes)
		{
			var @params = this.myTarget.GetParameters();
			var flag = @params.Length == 0;
			if (flag)
			{
				this.myScore = 0f;
			}
			else
			{
				var isParamArray = this.isParamArray;
				if (isParamArray)
				{
					this.myScore = this.ComputeScoreForParamArray(@params, argTypes);
				}
				else
				{
					this.myScore = this.ComputeScoreInternal(@params, argTypes);
				}
			}
		}

		private float ComputeScoreInternal(ParameterInfo[] parameters, Type[] argTypes)
		{
			var sum = ComputeSum(parameters, argTypes);
			return (float)((double)sum / (double)argTypes.Length);
		}

		private static int ComputeSum(ParameterInfo[] parameters, Type[] argTypes)
		{
			Debug.Assert(parameters.Length == argTypes.Length);
			var sum = 0;
			var num = parameters.Length - 1;
			for (var i = 0; i <= num; i++)
			{
				sum += ImplicitConverter.GetImplicitConvertScore(argTypes[i], parameters[i].ParameterType);
			}
			return sum;
		}

		private float ComputeScoreForParamArray(ParameterInfo[] parameters, Type[] argTypes)
		{
			var paramArrayParameter = parameters[parameters.Length - 1];
			var fixedParameterCount = paramArrayParameter.Position;
			var fixedParameters = new ParameterInfo[fixedParameterCount - 1 + 1];
			Array.Copy(parameters, fixedParameters, fixedParameterCount);
			var fixedSum = ComputeSum(fixedParameters, this.myFixedArgTypes);
			var paramArrayElementType = paramArrayParameter.ParameterType.GetElementType();
			var paramArraySum = 0;
			var myParamArrayArgTypes = this.myParamArrayArgTypes;
			for (var i = 0; i < myParamArrayArgTypes.Length; i = checked(i + 1))
			{
				var argType = myParamArrayArgTypes[i];
				paramArraySum += ImplicitConverter.GetImplicitConvertScore(argType, paramArrayElementType);
			}
			var flag = argTypes.Length > 0;
			float score;
			if (flag)
			{
				score = (float)((double)(fixedSum + paramArraySum) / (double)argTypes.Length);
			}
			else
			{
				score = 0f;
			}
			return score + 1f;
		}

		public bool IsAccessible(MemberElement owner)
		{
			return owner.IsMemberAccessible(this.myTarget);
		}

		public bool IsMatch(Type[] argTypes)
		{
			var parameters = this.myTarget.GetParameters();
			var flag = parameters.Length == 0 & argTypes.Length == 0;
			bool isMatch;
			if (flag)
			{
				isMatch = true;
			}
			else
			{
				var flag2 = parameters.Length == 0 & argTypes.Length > 0;
				if (flag2)
				{
					isMatch = false;
				}
				else
				{
					var lastParam = parameters[parameters.Length - 1];
					var flag3 = !lastParam.IsDefined(typeof(ParamArrayAttribute), false);
					if (flag3)
					{
						var flag4 = parameters.Length != argTypes.Length;
						isMatch = (!flag4 && AreValidArgumentsForParameters(argTypes, parameters));
					}
					else
					{
						var flag5 = parameters.Length == argTypes.Length && AreValidArgumentsForParameters(argTypes, parameters);
						if (flag5)
						{
							isMatch = true;
						}
						else
						{
							var flag6 = this.IsParamArrayMatch(argTypes, parameters, lastParam);
							if (flag6)
							{
								this.isParamArray = true;
								isMatch = true;
							}
							else
							{
								isMatch = false;
							}
						}
					}
				}
			}
			return isMatch;
		}

		private bool IsParamArrayMatch(Type[] argTypes, ParameterInfo[] parameters, ParameterInfo paramArrayParameter)
		{
			var fixedParameterCount = paramArrayParameter.Position;
			var fixedArgTypes = new Type[fixedParameterCount - 1 + 1];
			var fixedParameters = new ParameterInfo[fixedParameterCount - 1 + 1];
			Array.Copy(argTypes, fixedArgTypes, fixedParameterCount);
			Array.Copy(parameters, fixedParameters, fixedParameterCount);
			var flag = !AreValidArgumentsForParameters(fixedArgTypes, fixedParameters);
			bool isParamArrayMatch;
			if (flag)
			{
				isParamArrayMatch = false;
			}
			else
			{
				this.paramArrayElementType = paramArrayParameter.ParameterType.GetElementType();
				var paramArrayArgTypes = new Type[argTypes.Length - fixedParameterCount - 1 + 1];
				Array.Copy(argTypes, fixedParameterCount, paramArrayArgTypes, 0, paramArrayArgTypes.Length);
				var array = paramArrayArgTypes;
				checked
				{
					for (var i = 0; i < array.Length; i++)
					{
						var argType = array[i];
						var flag2 = !ImplicitConverter.EmitImplicitConvert(argType, this.paramArrayElementType, null);
						if (flag2)
						{
							return false;
						}
					}
					this.myFixedArgTypes = fixedArgTypes;
					this.myParamArrayArgTypes = paramArrayArgTypes;
					isParamArrayMatch = true;
				}
			}
			return isParamArrayMatch;
		}

		private static bool AreValidArgumentsForParameters(Type[] argTypes, ParameterInfo[] parameters)
		{
			Debug.Assert(argTypes.Length == parameters.Length);
			var num = argTypes.Length - 1;
			for (var i = 0; i <= num; i++)
			{
				var flag = !ImplicitConverter.EmitImplicitConvert(argTypes[i], parameters[i].ParameterType, null);
				if (flag)
				{
                    return false;
                }
            }
            return true;
		}

		int IComparable<CustomMethodInfo>.CompareTo(CustomMethodInfo other)
		{
			return this.myScore.CompareTo(other.myScore);
		}


	    /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
	    /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
	    /// <param name="other">An object to compare with this object.</param>
	    public bool Equals(CustomMethodInfo other)
	    {
	        if (ReferenceEquals(null, other))
	        {
	            return false;
	        }
	        if (ReferenceEquals(this, other))
	        {
	            return true;
	        }
	        return this.myScore.Equals(other.myScore);
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
	        return Equals((CustomMethodInfo) obj);
	    }

	    /// <summary>Serves as a hash function for a particular type. </summary>
	    /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
	    public override int GetHashCode()
	    {
	        return this.myScore.GetHashCode();
	    }

	    /// <summary>Returns a value that indicates whether the values of two <see cref="T:Ciloci.Flee.CustomMethodInfo" /> objects are equal.</summary>
	    /// <param name="left">The first value to compare.</param>
	    /// <param name="right">The second value to compare.</param>
	    /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
	    public static bool operator ==(CustomMethodInfo left, CustomMethodInfo right)
	    {
	        return Equals(left, right);
	    }

	    /// <summary>Returns a value that indicates whether two <see cref="T:Ciloci.Flee.CustomMethodInfo" /> objects have different values.</summary>
	    /// <param name="left">The first value to compare.</param>
	    /// <param name="right">The second value to compare.</param>
	    /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
	    public static bool operator !=(CustomMethodInfo left, CustomMethodInfo right)
	    {
	        return !Equals(left, right);
	    }
	}
}
