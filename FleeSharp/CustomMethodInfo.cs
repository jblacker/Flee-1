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
// ' Copyright � 2007 Eugene Ciloci
// ' Updated to .net 4.6 Copyright 2017 Steven Hoff

namespace FleeSharp
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    [Obsolete]
    internal class CustomMethodInfo : IComparable<CustomMethodInfo>, IEquatable<CustomMethodInfo>
    {
        /// <summary>
        /// Method we are wrapping
        /// </summary>
        private readonly MethodInfo myTarget;

        public bool isParamArray;
        public Type[] myFixedArgTypes;
        public Type[] myParamArrayArgTypes;

        /// <summary>
        /// The rating of how close the method matches the given arguments (0 is best)
        /// </summary>
        private float myScore;

        public Type paramArrayElementType;

        public CustomMethodInfo(MethodInfo target)
        {
            this.myTarget = target;
        }

        public int CompareTo(CustomMethodInfo other)
        {
            return this.myScore.CompareTo(other.myScore);
        }

        bool IEquatable<CustomMethodInfo>.Equals(CustomMethodInfo other)
        {
            return this.Equals1(other);
        }

        public void ComputeScore(Type[] argTypes)
        {
            var @params = this.myTarget.GetParameters();

            if (@params.Length == 0)
            {
                this.myScore = 0.0F;
            }
            else if (this.isParamArray)
            {
                this.myScore = this.ComputeScoreForParamArray(@params, argTypes);
            }
            else
            {
                this.myScore = this.ComputeScoreInternal(@params, argTypes);
            }
        }

        /// <summary>
        /// Compute a score showing how close our method matches the given argument types
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="argTypes"></param>
        /// <returns></returns>
        private float ComputeScoreInternal(ParameterInfo[] parameters, Type[] argTypes)
        {
            // Our score is the average of the scores of each parameter.  The lower the score, the better the match.
            var sum = ComputeSum(parameters, argTypes);

            return sum / argTypes.Length;
        }

        private static int ComputeSum(ParameterInfo[] parameters, Type[] argTypes)
        {
            Debug.Assert(parameters.Length == argTypes.Length);
            var sum = 0;

            for (var i = 0; i <= parameters.Length - 1; i++)
            {
                sum += ImplicitConverter.GetImplicitConvertScore(argTypes[i], parameters[i].ParameterType);
            }

            return sum;
        }

        private float ComputeScoreForParamArray(ParameterInfo[] parameters, Type[] argTypes)
        {
            var paramArrayParameter = parameters[parameters.Length - 1];
            var fixedParameterCount = paramArrayParameter.Position;

            var fixedParameters = new ParameterInfo[fixedParameterCount];

            Array.Copy(parameters, fixedParameters, fixedParameterCount);

            var fixedSum = ComputeSum(fixedParameters, this.myFixedArgTypes);

            var paramArrayElementType = paramArrayParameter.ParameterType.GetElementType();

            var paramArraySum = 0;

            foreach (var argType in this.myParamArrayArgTypes)
            {
                paramArraySum += ImplicitConverter.GetImplicitConvertScore(argType, paramArrayElementType);
            }

            float score = 0;

            if (argTypes.Length > 0)
            {
                score = (fixedSum + paramArraySum) / argTypes.Length;
            }
            else
            {
                score = 0;
            }

            // The param array score gets a slight penalty so that it scores worse than direct matches
            return score + 1;
        }

        public bool IsAccessible(MemberElement owner)
        {
            return owner.IsMemberAccessible(this.myTarget);
        }

        /// <summary>
        /// Is the given MethodInfo usable as an overload?
        /// </summary>
        /// <param name="argTypes"></param>
        /// <returns></returns>
        public bool IsMatch(Type[] argTypes)
        {
            var parameters = this.myTarget.GetParameters();

            // If there are no parameters and no arguments were passed, then we are a match.
            if ((parameters.Length == 0) & (argTypes.Length == 0))
            {
                return true;
            }

            // If there are no parameters but there are arguments, we cannot be a match
            if ((parameters.Length == 0) & (argTypes.Length > 0))
            {
                return false;
            }

            // Is the last parameter a paramArray?
            var lastParam = parameters[parameters.Length - 1];

            if (lastParam.IsDefined(typeof(ParamArrayAttribute), false) == false)
            {
                if (parameters.Length != argTypes.Length)
                {
                    // Not a paramArray and parameter and argument counts don't match
                    return false;
                }
                // Regular method call, do the test
                return AreValidArgumentsForParameters(argTypes, parameters);
            }

            // At this point, we are dealing with a paramArray call

            // If the parameter and argument counts are equal and there is an implicit conversion from one to the other, we are a match.
            if (parameters.Length == argTypes.Length && AreValidArgumentsForParameters(argTypes, parameters))
            {
                return true;
            }
            if (this.IsParamArrayMatch(argTypes, parameters, lastParam))
            {
                this.isParamArray = true;
                return true;
            }
            return false;
        }

        private bool IsParamArrayMatch(Type[] argTypes, ParameterInfo[] parameters, ParameterInfo paramArrayParameter)
        {
            // Get the count of arguments before the paramArray parameter
            var fixedParameterCount = paramArrayParameter.Position;
            var fixedArgTypes = new Type[fixedParameterCount];
            var fixedParameters = new ParameterInfo[fixedParameterCount];

            // Get the argument types and parameters before the paramArray
            Array.Copy(argTypes, fixedArgTypes, fixedParameterCount);
            Array.Copy(parameters, fixedParameters, fixedParameterCount);

            // If the fixed arguments don't match, we are not a match
            if (AreValidArgumentsForParameters(fixedArgTypes, fixedParameters) == false)
            {
                return false;
            }

            // Get the type of the paramArray
            this.paramArrayElementType = paramArrayParameter.ParameterType.GetElementType();

            // Get the types of the arguments passed to the paramArray
            var paramArrayArgTypes = new Type[argTypes.Length - fixedParameterCount];
            Array.Copy(argTypes, fixedParameterCount, paramArrayArgTypes, 0, paramArrayArgTypes.Length);

            // Check each argument
            foreach (var argType in paramArrayArgTypes)
            {
                if (ImplicitConverter.EmitImplicitConvert(argType, this.paramArrayElementType, null) == false)
                {
                    return false;
                }
            }

            this.myFixedArgTypes = fixedArgTypes;
            this.myParamArrayArgTypes = paramArrayArgTypes;

            // They all match, so we are a match
            return true;
        }

        private static bool AreValidArgumentsForParameters(Type[] argTypes, ParameterInfo[] parameters)
        {
            Debug.Assert(argTypes.Length == parameters.Length);
            // Match if every given argument is implicitly convertible to the method's corresponding parameter
            for (var i = 0; i <= argTypes.Length - 1; i++)
            {
                if (ImplicitConverter.EmitImplicitConvert(argTypes[i], parameters[i].ParameterType, null) == false)
                {
                    return false;
                }
            }

            return true;
        }

        private bool Equals1(CustomMethodInfo other)
        {
            return this.myScore == other.myScore;
        }

        public MethodInfo Target => this.myTarget;
    }
}