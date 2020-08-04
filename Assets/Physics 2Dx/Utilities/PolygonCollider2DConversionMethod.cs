namespace Physics2DxSystem.Utilities
{
    /// <summary>Conversion options that are available with PolygonCollider2D.</summary>
    public enum PolygonCollider2DConversionMethod
    {
        /// <summary>The mesh will remain as it is.</summary>
        None,
        /// <summary>A new mesh will be created.</summary>
        CreateMesh,
        /// <summary>A new mesh will be created and the shared mesh will be destroyed.</summary>
        CreateMeshAndDestroySharedMesh,
    }
}

