# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [0.1.3] - 2020-10-30
### Added
- TransformSplitter now has a property to change the directional focus of the Transform2D.
- Added icon for MeshConverter.
- Added Editors for the ColliderConverters to push Colliders to the transformSplitters gameobjects. A ColliderConverter does not allow colliders on the same Object anymore.

### Changed
- Pragma warning disabling in TransformSplitter removed and instead initialized the variable in question with default.
- Converters will now Convert for the first time in Start and only after that in OnEnable.

### Removed
- RigidbodyConverter had a script execution order of -1, which has been reset to 0.
- ColliderConverter no longer caches Colliders that are on the same Object as the ColliderConverter (this in tandem with the added Editors which won't allow Colliders to reside on the same Object).

### Fixed
- ContourTracer would miss a point if a certain condition was met.
- CameraConverter wouldn't lerp the projection matrix from 2D to 3D because orthographic was true.
- RigidbodyConverter creates copies for the Rigidbody and Rigidbody2D to reference. These copies would be new instances, not having the same values as any Rigidbodies that were already on the RigidbodyConverter. Now, these values are set on Awake.

## [0.1.2] - 2020-10-24
### Added
- Added in the old xml documentation. Needs reordering.
- Outliner icon added.

### Removed
- ColliderConverters no longer have an array to ignore certain colliders. Instead, add colliders that should not be converted to a child object.
- beta removed from the versioning as it is redundent (version already starts with 0).

## [0.1.1-beta] - 2020-10-16
### Bugfixes
- TransformSplitter would throw away all its components on play mode start and throw an error on play mode exit.
- TransformSplitter would throw away all its components on play mode start and throw an error on play mode exit.
