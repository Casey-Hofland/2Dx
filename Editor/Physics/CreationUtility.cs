#nullable enable
using UnityEditor;
using UnityEngine;

namespace Unity2Dx.Physics.Editor
{
    public static class CreationUtility
    {
        private static GameObject CreatePrimitive(PrimitiveType type, GameObject? parent)
        {
            var gameObject = GameObject.CreatePrimitive(type);
            Undo.RegisterCreatedObjectUndo(gameObject, nameof(CreatePrimitive));

            GameObjectUtility.SetParentAndAlign(gameObject, parent);
            GameObjectUtility.EnsureUniqueNameForSibling(gameObject);

            Unity2Dx.Editor.CreationUtility.PlaceCreatedObject(gameObject);
            Selection.activeObject = gameObject;

            return gameObject;
        }

        public static void CreatePrimitive(PrimitiveType type)
        {
            switch (type)
            {
                case PrimitiveType.Sphere:
                    CreateSphere(default);
                    break;
                case PrimitiveType.Capsule:
                    CreateCapsule(default);
                    break;
                case PrimitiveType.Cylinder:
                    CreateCylinder(default);
                    break;
                case PrimitiveType.Cube:
                    CreateCube(default);
                    break;
                case PrimitiveType.Plane:
                    CreatePlane(default);
                    break;
                case PrimitiveType.Quad:
                    CreateQuad(default);
                    break;
            }
        }

        [MenuItem("GameObject/2Dx Object/Cube", false, 1)]
        public static void CreateCube(MenuCommand? menuCommand)
        {
            var cube = CreatePrimitive(PrimitiveType.Cube, menuCommand?.context as GameObject);
            Undo.AddComponent<Collider2Dx>(cube);
        }

        [MenuItem("GameObject/2Dx Object/Sphere", false, 1)]
        public static void CreateSphere(MenuCommand? menuCommand)
        {
            var sphere = CreatePrimitive(PrimitiveType.Sphere, menuCommand?.context as GameObject);
            Undo.AddComponent<Collider2Dx>(sphere);
        }

        [MenuItem("GameObject/2Dx Object/Capsule", false, 1)]
        public static void CreateCapsule(MenuCommand? menuCommand)
        {
            var capsule = CreatePrimitive(PrimitiveType.Capsule, menuCommand?.context as GameObject);
            Undo.AddComponent<Collider2Dx>(capsule);
        }

        [MenuItem("GameObject/2Dx Object/Cylinder", false, 1)]
        public static void CreateCylinder(MenuCommand? menuCommand)
        {
            var cylinder = CreatePrimitive(PrimitiveType.Cylinder, menuCommand?.context as GameObject);
            Undo.AddComponent<Collider2Dx>(cylinder);
        }

        [MenuItem("GameObject/2Dx Object/Plane", false, 1)]
        public static void CreatePlane(MenuCommand? menuCommand)
        {
            var plane = CreatePrimitive(PrimitiveType.Plane, menuCommand?.context as GameObject);
            Undo.AddComponent<Collider2Dx>(plane);
        }

        [MenuItem("GameObject/2Dx Object/Quad", false, 1)]
        public static void CreateQuad(MenuCommand? menuCommand)
        {
            var quad = CreatePrimitive(PrimitiveType.Quad, menuCommand?.context as GameObject);
            Undo.AddComponent<Collider2Dx>(quad);
        }
    }
}
