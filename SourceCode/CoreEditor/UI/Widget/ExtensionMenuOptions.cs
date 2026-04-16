using Core.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CoreEditor.UI
{
    internal static class ExtensionMenuOptions
    {
        private const string UILayerName = "UI";
        private static Vector2 ImageSize = new Vector2(100f, 100f);


        private static RectTransform CreateUIElementRoot(string name, MenuCommand menuCommand, Vector2 size)
        {
            GameObject parent = menuCommand.context as GameObject;
            if (parent == null || parent.GetComponentInParent<Canvas>() == null)
            {
                parent = GetOrCreateCanvasGameObject();
            }
            GameObject go = new GameObject(name);

            Undo.RegisterCreatedObjectUndo(go, "Create " + name);
            Undo.SetTransformParent(go.transform, parent.transform, "Parent " + go.name);
            GameObjectUtility.SetParentAndAlign(go, parent);

            RectTransform rectTransform = go.AddComponent<RectTransform>();
            rectTransform.sizeDelta = size;
            if (parent != menuCommand.context) // not a context click, so center in sceneview
            {
                SetPositionVisibleInSceneView(parent.GetComponent<RectTransform>(), rectTransform);
            }
            Selection.activeGameObject = go;
            return rectTransform;
        }


        private static GameObject GetOrCreateCanvasGameObject()
        {
            GameObject selectedGo = Selection.activeGameObject;

            Canvas canvas = (selectedGo != null) ? selectedGo.GetComponentInParent<Canvas>() : null;
            if (canvas != null && canvas.gameObject.activeInHierarchy)
                return canvas.gameObject;

            canvas = Object.FindObjectOfType(typeof(Canvas)) as Canvas;
            if (canvas != null && canvas.gameObject.activeInHierarchy)
                return canvas.gameObject;

            return CreateNewUIRoot();
        }

        private static GameObject CreateNewUIRoot()
        {
            GameObject root = new GameObject("Canvas");
            root.layer = LayerMask.NameToLayer(UILayerName);
            Canvas canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            root.AddComponent<CanvasScaler>();
            root.AddComponent<GraphicRaycaster>();
            Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);

            CreateEventSystem(false, null);
            return root;
        }

        private static void CreateEventSystem(bool select, GameObject parent)
        {
            EventSystem esys = Object.FindObjectOfType<EventSystem>();
            if (esys == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                GameObjectUtility.SetParentAndAlign(eventSystem, parent);
                esys = eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();

                Undo.RegisterCreatedObjectUndo(eventSystem, "Create " + eventSystem.name);
            }

            if (select && esys != null)
            {
                Selection.activeGameObject = esys.gameObject;
            }
        }

        private static void SetPositionVisibleInSceneView(RectTransform canvasRectTrans, RectTransform itemRectTrans)
        {
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView == null && SceneView.sceneViews.Count > 0)
                sceneView = SceneView.sceneViews[0] as SceneView;

            // Couldn't find a SceneView. Don't set position.
            if (sceneView == null || sceneView.camera == null)
                return;

            // Create world space Plane from canvas position.
            Vector2 localPlanePosition;
            Camera camera = sceneView.camera;
            Vector3 position = Vector3.zero;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTrans, new Vector2(camera.pixelWidth / 2, camera.pixelHeight / 2), camera, out localPlanePosition))
            {
                // Adjust for canvas pivot
                localPlanePosition.x = localPlanePosition.x + canvasRectTrans.sizeDelta.x * canvasRectTrans.pivot.x;
                localPlanePosition.y = localPlanePosition.y + canvasRectTrans.sizeDelta.y * canvasRectTrans.pivot.y;

                localPlanePosition.x = Mathf.Clamp(localPlanePosition.x, 0, canvasRectTrans.sizeDelta.x);
                localPlanePosition.y = Mathf.Clamp(localPlanePosition.y, 0, canvasRectTrans.sizeDelta.y);

                // Adjust for anchoring
                position.x = localPlanePosition.x - canvasRectTrans.sizeDelta.x * itemRectTrans.anchorMin.x;
                position.y = localPlanePosition.y - canvasRectTrans.sizeDelta.y * itemRectTrans.anchorMin.y;

                Vector3 minLocalPosition;
                minLocalPosition.x = canvasRectTrans.sizeDelta.x * (0 - canvasRectTrans.pivot.x) + itemRectTrans.sizeDelta.x * itemRectTrans.pivot.x;
                minLocalPosition.y = canvasRectTrans.sizeDelta.y * (0 - canvasRectTrans.pivot.y) + itemRectTrans.sizeDelta.y * itemRectTrans.pivot.y;

                Vector3 maxLocalPosition;
                maxLocalPosition.x = canvasRectTrans.sizeDelta.x * (1 - canvasRectTrans.pivot.x) - itemRectTrans.sizeDelta.x * itemRectTrans.pivot.x;
                maxLocalPosition.y = canvasRectTrans.sizeDelta.y * (1 - canvasRectTrans.pivot.y) - itemRectTrans.sizeDelta.y * itemRectTrans.pivot.y;

                position.x = Mathf.Clamp(position.x, minLocalPosition.x, maxLocalPosition.x);
                position.y = Mathf.Clamp(position.y, minLocalPosition.y, maxLocalPosition.y);
            }

            itemRectTrans.anchoredPosition = position;
            itemRectTrans.localRotation = Quaternion.identity;
            itemRectTrans.localScale = Vector3.one;
        }



        [MenuItem("GameObject/UI/Extensions/CircleImage", false)]
        public static void AddCircleImage(MenuCommand menuCommand)
        {
            RectTransform rectTrans = CreateUIElementRoot("CircleImage", menuCommand, ImageSize);
            rectTrans.gameObject.AddComponent<CircleImage>();
            Selection.activeGameObject = rectTrans.gameObject;
        }

        [MenuItem("GameObject/UI/Extensions/CombineImage", false)]
        public static void AddCombineImage(MenuCommand menuCommand)
        {
            RectTransform rectTrans = CreateUIElementRoot("CombineImage", menuCommand, ImageSize);
            rectTrans.gameObject.AddComponent<CombineImage>();
            Selection.activeGameObject = rectTrans.gameObject;
        }
    }

}
