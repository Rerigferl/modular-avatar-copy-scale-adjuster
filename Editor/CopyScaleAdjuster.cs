#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using nadena.dev.modular_avatar.core.editor;

using ScaleAdjuster = nadena.dev.modular_avatar.core.ModularAvatarScaleAdjuster;
using MergeArmature = nadena.dev.modular_avatar.core.ModularAvatarMergeArmature;

namespace Numeira
{
    internal static class CopyScaleAdjuster
    {
        private const string MenuItemName = "Copy Scale Adjuster";
        private const string ContextMenuPath = "CONTEXT/ModularAvatarMergeArmature/" + MenuItemName;
        private const string HierarchyContextMenuPath = "GameObject/Modular Avatar/" + MenuItemName;
        private const int HierarchyContextMenuPriority = 956;

        internal const string WithSetupOutfitPath = "GameObject/Modular Avatar/Setup Outfit with Copy Scale Adjuster";
        internal const int WithSetupOutfitPathPriority = -1000;

        #region Context Menu

        [MenuItem(ContextMenuPath, true)]
        public static bool ValidateMergeArmatureContextMenu(MenuCommand command)
        {
            if (command.context is not MergeArmature merge)
            {
                merge = (command.context as GameObject)?.GetComponent<MergeArmature>();
            }
            
            return merge.IsValidMergeArmature();
        }

        [MenuItem(HierarchyContextMenuPath, true, HierarchyContextMenuPriority)]
        public static bool ValidateHierarchyContextMenu()
        {
            return Selection.activeGameObject.GetComponent<MergeArmature>().IsValidMergeArmature();
        }

        [MenuItem(ContextMenuPath, false)]
        [MenuItem(HierarchyContextMenuPath, false, HierarchyContextMenuPriority)]
        public static void OnContextMenu(MenuCommand command)
        {
            if (command.context is not MergeArmature merge)
            {
                merge = (command.context as GameObject)?.GetComponent<MergeArmature>();
            }

            Run(merge);
        }

        private static bool IsValidMergeArmature(this MergeArmature mergeArmature)
            => mergeArmature != null && mergeArmature.mergeTargetObject != null && mergeArmature.mergeTargetObject.GetComponentInChildren<ScaleAdjuster>(true);

        [MenuItem(WithSetupOutfitPath, false, WithSetupOutfitPathPriority)]
        public static void SetupOutfitWithCopyScaleAdjuster(MenuCommand command)
        {
            if (command.context is not GameObject go)
                return;

            RunSetupOutfit(go);
            var merge = go.GetComponentInChildren<MergeArmature>();

            Run(merge);
        }

        private static void RunSetupOutfit(GameObject gameObject)
        {
#if MODULAR_AVATAR_VERSION_1_10
            SetupOutfit.SetupOutfitUI(gameObject);
#else
            try
            {
                var command = new MenuCommand(gameObject);
                typeof(AvatarProcessor).Assembly.GetTypes()
                    .FirstOrDefault(x => x.Name == "EasySetupOutfit")
                    ?.GetMethod("SetupOutfit", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                    ?.Invoke(null, new object[] { command });
            }
            catch 
            {
                Debug.LogError("[Copy Scale Adjuster] Failed to running Setup Outfit");
            }
#endif
        }

#endregion

        public static void Run(MergeArmature merge)
        {
            if (merge == null)
                return;

            var target = merge.mergeTargetObject;
            var sources = target.GetComponentsInChildren<ScaleAdjuster>(true).WithPath(target, merge.prefix, merge.suffix);

            Undo.SetCurrentGroupName("Copy Scale Adjuster");
            var idx = Undo.GetCurrentGroup();
            foreach (var source in sources)
            {
                var o = merge.transform.Find(source.Path);
                if (o != null)
                {
                    if (o.TryGetComponent<ScaleAdjuster>(out var c))
                    {
                        Undo.RecordObject(c, "Copy Serialized");
                    }
                    else
                    {
                        c = Undo.AddComponent<ScaleAdjuster>(o.gameObject);
                    }
                    EditorUtility.CopySerializedIfDifferent(source.Component, c);
                }
            }
            Undo.CollapseUndoOperations(idx);
        }

        private static (string Path, T Component)[] WithPath<T>(this T[] components, GameObject target, ReadOnlySpan<char> prefix = default, ReadOnlySpan<char> suffix = default) where T : Component
        {
            using var builder = new RelativePathBuilder(stackalloc char[256]);
            var result = new (string Path, T Component)[components.Length];
            for(int i  = 0; i < components.Length; i++)
            {
                var component = components[i];
                var path = builder.GetRelativePath(component.gameObject, target, prefix, suffix);
                builder.Reset();
                result[i] = (path, component);
            }
            return result;
        }
    }
}
#endif