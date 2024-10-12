using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class Raycast : EditorWindow
{
    static bool activo;
    public List<GameObject> prefabsParaInstanciar = new List<GameObject>();
    public GameObject prefabSeleccionado;
    public GameObject objetoPadre;

    private float escalaSlider = 0.5f;
    private Vector2 scrollPos; // Posición del scroll para la lista de prefabs

    [MenuItem("Morion/Creador de Arda")]
    static void Init()
    {
        var ventana = (Raycast)EditorWindow.GetWindow(typeof(Raycast));
        ventana.Show();
    }

    void OnEnable() => SceneView.duringSceneGui += OnSceneGUI;
    void OnDisable() => SceneView.duringSceneGui -= OnSceneGUI;

    void OnSceneGUI(SceneView vista)
    {
        if (!activo || Event.current.button != 0 || prefabSeleccionado == null)
        {
            return;
        }

        Ray rayo = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;

        if (Event.current.type == EventType.MouseDown && Physics.Raycast(rayo, out hit))
        {
            Debug.Log("Impacto: " + hit.collider.gameObject.name);

            // Instancia el prefab y añade undo
            var objeto = (GameObject)PrefabUtility.InstantiatePrefab(prefabSeleccionado);
            Undo.RegisterCreatedObjectUndo(objeto, "Crear Prefab");

            // Posiciona y aplica escala en base a 1 + (valor aleatorio entre -escalaSlider y +escalaSlider)
            objeto.transform.position = hit.point;
            float escalaAleatoria = 1 + Random.Range(-escalaSlider, escalaSlider);
            objeto.transform.localScale = Vector3.one * escalaAleatoria;

            if (objetoPadre != null)
            {
                objeto.transform.SetParent(objetoPadre.transform);
            }
        }

        Event.current.Use();
        SceneView.RepaintAll();
    }

    void OnGUI()
    {
        // Verificar si se presionó la tecla espacio y alternar "activo"
        Event e = Event.current;
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Space)
        {
            activo = !activo;
            Repaint(); // Refrescar la ventana para reflejar el cambio
        }

        if (GUILayout.Button(activo ? "Desactivar" : "Activar"))
        {
            activo = !activo;
        }

        GUIStyle estiloEstado = new GUIStyle(GUI.skin.label);
        estiloEstado.normal.textColor = activo ? Color.green : Color.red;
        GUILayout.Label("Activo: " + activo, estiloEstado);

        if (prefabSeleccionado != null)
        {
            GUILayout.Label("Prefab Seleccionado:");
            Texture2D vistaPrevia = AssetPreview.GetAssetPreview(prefabSeleccionado);
            if (vistaPrevia != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(vistaPrevia, GUILayout.Width(100), GUILayout.Height(100));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }
        else
        {
            GUILayout.Label("No hay prefab seleccionado.");
        }

        GUILayout.Label("Seleccionar Objeto Padre (Opcional):");
        objetoPadre = (GameObject)EditorGUILayout.ObjectField(objetoPadre, typeof(GameObject), true);

        GUILayout.Label("Escala Aleatoria");
        escalaSlider = EditorGUILayout.Slider(escalaSlider, 0f, 1f);

        GUILayout.Label("Arrastra y Suelta Prefabs Abajo:");
        Event evt = Event.current;
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Arrastra los Prefabs Aquí");

        if (evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform)
        {
            if (!dropArea.Contains(evt.mousePosition)) return;

            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            if (evt.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                foreach (var draggedObject in DragAndDrop.objectReferences)
                {
                    if (draggedObject is GameObject go && PrefabUtility.IsPartOfPrefabAsset(go))
                    {
                        prefabsParaInstanciar.Add(go);
                    }
                }
            }
            Event.current.Use();
        }

        GUILayout.Space(20);
        GUILayout.Label("Seleccionar Prefab:");

        // Activar scroll si la lista es más grande que el espacio disponible
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        float windowWidth = position.width;
        int columnas = Mathf.Max(1, Mathf.FloorToInt(windowWidth / 70));
        int filas = Mathf.CeilToInt((float)prefabsParaInstanciar.Count / columnas);

        for (int fila = 0; fila < filas; fila++)
        {
            GUILayout.BeginHorizontal();
            for (int col = 0; col < columnas; col++)
            {
                int index = fila * columnas + col;
                if (index < prefabsParaInstanciar.Count && prefabsParaInstanciar[index] != null)
                {
                    if (GUILayout.Button(AssetPreview.GetAssetPreview(prefabsParaInstanciar[index]), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        prefabSeleccionado = prefabsParaInstanciar[index];
                        Debug.Log("Prefab seleccionado: " + prefabSeleccionado.name);
                    }
                }
            }
            GUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }
}
