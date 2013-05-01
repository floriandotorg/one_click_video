// --------------------------------------------------------------------------------------------------------------------
// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.
// --------------------------------------------------------------------------------------------------------------------

namespace WP7Contrib.View.Controls.Extensions
{
    using System;

    /// <summary>
    /// Possible modes for creating a transform
    /// </summary>
    [Flags]
    public enum TransformCreationMode
    {
        /// <summary>
        /// Don't try and create a transform if it doesn't already exist
        /// </summary>
        None = 0,

        /// <summary>
        /// Create a transform if none exists
        /// </summary>
        Create = 1,

        /// <summary>
        /// Create and add to an existing group
        /// </summary>
        AddToGroup = 2,

        /// <summary>
        /// Create a group and combine with existing transform; may break existing animations
        /// </summary>
        CombineIntoGroup = 4,

        /// <summary>
        /// Treat identity matrix as if it wasn't there; may break existing animations
        /// </summary>
        IgnoreIdentityMatrix = 8,

        /// <summary>
        /// Create a new transform or add to group
        /// </summary>
        CreateOrAddAndIgnoreMatrix = Create | AddToGroup | IgnoreIdentityMatrix,

        /// <summary>
        /// Default behaviour, equivalent to CreateOrAddAndIgnoreMatrix
        /// </summary>
        Default = CreateOrAddAndIgnoreMatrix,
    }
}