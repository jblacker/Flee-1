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
    internal class GeneralErrorResourceKeys
    {
        public const string TypeNotAccessibleToExpression = "TypeNotAccessibleToExpression";

        public const string VariableWithNameAlreadyDefined = "VariableWithNameAlreadyDefined";

        public const string UndefinedVariable = "UndefinedVariable";

        public const string InvalidVariableName = "InvalidVariableName";

        public const string CannotDetermineNewVariableType = "CannotDetermineNewVariableType";

        public const string VariableValueNotAssignableToType = "VariableValueNotAssignableToType";

        public const string CouldNotFindPublicStaticMethodOnType = "CouldNotFindPublicStaticMethodOnType";

        public const string OnlyPublicStaticMethodsCanBeImported = "OnlyPublicStaticMethodsCanBeImported";

        public const string InvalidNamespaceName = "InvalidNamespaceName";

        public const string NewOwnerTypeNotAssignableToCurrentOwner = "NewOwnerTypeNotAssignableToCurrentOwner";

        private GeneralErrorResourceKeys()
        {
        }
    }
}