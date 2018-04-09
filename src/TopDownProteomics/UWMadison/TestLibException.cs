// Copyright 2012, 2013, 2014 Derek J. Bailey
// Modified work copyright 2016, 2017 Stefan Solntsev
//
// This file (ChemicalFormula.cs) is part of Chemistry Library.
//
// Chemistry Library is free software: you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Chemistry Library is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public
// License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with Chemistry Library. If not, see <http://www.gnu.org/licenses/>.

using System;

namespace UWMadison.Chemistry
{
    /// <summary>
    /// Base exception for the library.
    /// </summary>
    /// <seealso cref="Exception" />
    [Serializable]
    public class TestLibException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestLibException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public TestLibException(string message) : base(message)
        {
        }
    }
}