/**
/// HashTag.Core Library
/// Copyright © 2010-2014
///
/// This module is Copyright © 2010-2014 Steve Powell
/// All rights reserved.
///
/// This library is free software; you can redistribute it and/or
/// modify it under the terms of the Microsoft Public License (Ms-PL)
/// 
/// This library is distributed in the hope that it will be
/// useful, but WITHOUT ANY WARRANTY; without even the implied
/// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
/// PURPOSE.  See theMicrosoft Public License (Ms-PL) License for more
/// details.
///
/// You should have received a copy of the Microsoft Public License (Ms-PL)
/// License along with this library; if not you may 
/// find it here: http://www.opensource.org/licenses/ms-pl.html
///
/// Steve Powell, hashtagdonet@gmail.com
**/
// This collection of non-binary tree data structures created by Dan Vanderboom.
// Critical Development blog: http://dvanderboom.wordpress.com
// Original Tree<T> blog article: http://dvanderboom.wordpress.com/2008/03/15/treet-implementing-a-non-binary-tree-in-c/
// Linked-in: http://www.linkedin.com/profile?viewProfile=&key=13009616&trk=tab_pro

using System;
using System.Text;

namespace System.Collections.Generic
{
    /// <summary>
    /// Determines how traversal actions will take place
    /// </summary>
    public enum TreeTraversalType
    {
        /// <summary>
        /// Go to the end of each ancestor chain first
        /// </summary>
        DepthFirst,

        /// <summary>
        /// Traverse each child node first then each ancestor children
        /// </summary>
        BreadthFirst
    }

    /// <summary>
    /// Determines how traversal actions will take place
    /// </summary>
    public enum TreeTraversalDirection
    {
        /// <summary>
        /// Traverse from parent down
        /// </summary>
        TopDown,

        /// <summary>
        /// Traverse from ancestor to parent
        /// </summary>
        BottomUp
    }
}