using System;

namespace Physics2DxSystem.Utilities
{
    /// <summary>Conversion options for converting PolygonCollider2D to MeshCollider.</summary>
    [Flags]
    public enum PolygonCollider2DConversionOptions
    {
        /// <summary>The old shared mesh will be destroyed.</summary>
        DestroySharedMesh = 1,
        /// <summary>A new mesh will be created.</summary>
        CreateMesh = 2,
        /// <summary>Create backfaces for the mesh.</summary>
        CreateBackfaces = 4,
    }
}

