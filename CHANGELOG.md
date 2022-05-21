# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [0.3.0-exp] - 2022-05-21
# New Workflow!
The new workflow is finally component-based!!! Classes can now inherit from `Convertible` to turn into convertibles with their own is2DNot3D switch! The component is effectively 2D when this is true, and 3D when this is false.

Some components however depend on another Convertible. Don't fret: `Converter<T>` has got you covered! This allows a component to turn into a converter that listens to a convertibles is2DNot3D switch. If the convertible switches, its converters switch with it!

Even then, you're not there yet. Sometimes components need to convert into components that are incompatible to be on the same object (such as Rigidbody and Rigidbody2D). Even in these cases, there's a simple solution: `CopyConvertible` and `CopyConverter<T>` make sure to convert between those annoying incompatibles and are *guaranteed to work!* They are more complex to hook into with outside code though, so `Convertible` and `Converter<T>` are preferred.

And finally: 2Dx now offers a dead-easy way to write your own converters! Simply inherit from `Convertible<Component, Component2D>` or `Converter<T, Component, Component2D>`, write the conversion code *only* and you can immediately test it out in the inspector, no playmode required! Works with the `CopyConvertible` and similar variants as well.

There's still a lot improvements left to make in 2Dx. However, the component-based workflow should make it easier for changes to affect separate parts of the codebase now, instead of the whole thing.

### Added
- Reliance on the UnityExtras package. This package holds common Unity Components that may be reused in other projects.

### Changed
- Minimum Unity version changed to 2021.1.
- nullable reference types enabled in all scripts.
- Renamed PhysicsConverter to PhysicsConvert and made it a partial class for better organization.
- Renamed PhysicsCopier to PhysicsCopy and made it a partial class for better organization.
- Too much to count, if the library wasn't in experimental this would simply release as a whole new version.

## [0.2.1-pre.3] - 2021-07-17
### Added
- RequireRigidbodyConverter<> abstract Component added. This is a specialized Converter that takes care of converting components that rely on Rigidbody and Rigidbody2D to work. This requires some special care, since these components need to be destroyed before the Rigidbody, but added after the Rigidbody is created, and this converter handles it nicely.

### Changed
- Rigidbody2Dx Ignore Overlap feature now uses for loops instead of foreach loops as to not create unnecessary garbage.
- ConstantForce2Dx inherits from RequireRigidbodyConverter<ConstantForce, ConstantForce2D>. This change has no bearing on any existing code. In the background it works completely the same.

### Fixed
- Rigidbody2Dx Ignore Overlap now respects Physics2D contact offset. Previously this would make colliders with a very small distance, such as 0, return as overlapping.
- If ConstantForce2D was converted to ConstantForce with a torque of 0 it would return NAN values.

## [0.2.1-pre.2] - 2021-07-16
### Fixed
- Conversion Order wasn't saving.

## [0.2.1-pre] - 2021-07-15
### Added
- AudioTransform2Dx Component added. This creates a child transform that will follow the Audio Listeners depth in 2D. If e.g. an Audio Source is added to this transform, in 2D its distance from the Audio Listener will never include depth, effectively creating a 2D Audio Source while keeping all of its settings customizable.
- CreationUtility added. This is an editor script that adds MenuItems to instantly create 2Dx objects and should hopefully give creators a jumping off point when learning how to use 2Dx.

### Removed
- BoxCollider2Dx untested warning.

## [0.2.0-pre] - 2021-04-22
### Added
- ConstantForce2Dx Component added.
- Utility methods to Convert and Copy between ConstantForces and ConstantForce2Ds added.
- Added Dark mode Icons.

### Changed
- Completely changed the package name, from Dimension Converter to the much more streamlined name 2Dx. The namespace is called Unity2Dx since namespaces can't start with a number.
- The Dimension class has been moved from the UnityEngine namespace into the main namespace, now Unity2Dx.
- Core functionality and Physics functionality has been split into different namespaces. Unity2Dx is the core, Unity2Dx.Physics is the Physics namespace.
- Changed the names of 2Dx Components. Instead of "RigidbodyConverter", it is now "Rigidbody2Dx". The Collider Components have been renamed to "SphereCollider2Dx" and otherwise, and "TransformSplitter" is now also "Transform2Dx". It is believed that this naming gives a stronger identity to the 2Dx package and provides an easy naming convention for extensions to follow.
- Utility namespaces have been removed since these were redundant.
- Transform2Dx now has a bool "convert" to replace the Component "SplitterConverter".
- Transform2Dx can now be given an UpdateMode to specify how often the transforms need to be updated.
- Transform2Dx now has a context menu "Update Transforms" that can be used in editor for when the UpdateMode is Custom.
- You can now specify the time scale when converting in the Dimension Converter Settings. This is important when converting over a period of time.
- Utility methods ToVector2D and ToVector have been made public. These can be used to transpose a Vector3 to a Vector2 on a game object with different rotation and the other way around.

### Removed
- DimensionOverrider Component removed.
- TimeStopper Component removed.
- SplitterConverter Component removed.
- BoolEvent Component removed.

## [0.1.3-1] - 2020-12-20
### Changed
- RigidbodyConverter will throw events for when Rigidbodies are assigned instead of when they are converted. If this behaviour is required, Dimension.isConverting can be checked when the event is thrown.

### Fixed
- Persistent will set itself enabled in OnDisable if it may not be disabled.
- ColliderConverter wouldn't cache colliders on the component itself. Although colliders on a collider converter should be avoided, it should still check and cache them.
- CamereConverterEditor had SerializedProperties with wrong names.

## [0.1.3] - 2020-10-30
### Added
- TransformSplitter now has a property to change the directional focus of the Transform2D.
- Added icon for MeshConverter.
- Added Editors for the ColliderConverters to push Colliders to the transformSplitters gameobjects. A ColliderConverter does not allow colliders on the same Object anymore.
- RigidbodyEvent and Rigidbody2DEvent of type UnityEvent<T>.
- RigidbodyConverter now invokes a RigidbodyEvent after it has converted to use a Rigidbody and a Rigidbody2DEvent after it has converted to use a Rigidbody2D. These can be used to set a Rigidbody or Rigidbody2D property dynamically via the Editor.
- All fields have been made private and gotten a public property variant to stay inline with Unitys naming conventions.

### Changed
- Pragma warning disabling in TransformSplitter removed and instead initialized the variable in question with default.
- Converters will now Convert for the first time in Start and only after that in OnEnable.
- ConvertEvent renamed to the more general BoolEvent.

### Removed
- RigidbodyConverter had a script execution order of -1, which has been reset to 0.
- ColliderConverter no longer caches Colliders that are on the same Object as the ColliderConverter (this in tandem with the added Editors which won't allow Colliders to reside on the same Object).
- OnAfterConvert Component removed.
- OnBeforeConvert component removed.

### Fixed
- ContourTracer would miss a point if a certain condition was met.
- CameraConverter wouldn't lerp the projection matrix from 2D to 3D because orthographic was true.
- RigidbodyConverter creates copies for the Rigidbody and Rigidbody2D to reference. These copies would be new instances, not having the same values as any Rigidbodies that were already on the RigidbodyConverter. Now, these values are set on Awake.
- onBeforeConvert and onAfterConvert are events now (they were properties).
- TransformSplitter no longer throws any errors in its lifecycle. This was a result of methods being called by ExecuteAlways and not handling destruction of serialized properties cleanly.

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
