// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.
#pragma warning disable IDE0055

/*
Licensed to the .NET Foundation under one or more agreements.
 The .NET Foundation licenses this file to you under the MIT license.
 See the LICENSE file in the project root for more information.
*/

#if NETSTANDARD2_0 || NET6 || NET461 || NET462 || NET47 || NET471 || NET472 || NET48
namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Reserved to be used by the compiler for tracking metadata.
    /// This class should not be used by developers in source code.
    /// </summary>
    //[EditorBrowsable(EditorBrowsableState.Never)]
    internal static class IsExternalInit
    {
    }
}

#endif