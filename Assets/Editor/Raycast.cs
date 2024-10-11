using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class Raycast : EditorWindow
{
    static bool activo;
    public List<GameObject> prefabsParaInstanciar = new List<GameObject>();
    public GameObject prefabSeleccionado;
    public GameObject objetoPadre;

    // Variables para la escala mínima y máxima
    public float escalaMin = 0.5f;
    public float escalaMax = 2.0f;

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
        if (!activo) return;

        Ray rayo = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;

        if (Event.current.type == EventType.MouseDown && prefabSeleccionado != null && Physics.Raycast(rayo, out hit))
        {
            Debug.Log("Impacto: " + hit.collider.gameObject.name);

            var objeto = (GameObject)PrefabUtility.InstantiatePrefab(prefabSeleccionado);

            // Establece la posición y escala aleatoria del objeto
            objeto.transform.position = hit.point;
            float escalaAleatoria = Random.Range(escalaMin, escalaMax);
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

        GUILayout.Label("Arrastra y Suelta Prefabs Abajo:");
        Event evt = Event.current;
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Arrastra los Prefabs Aquí");

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(evt.mousePosition)) break;

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
                break;
        }

        GUILayout.Space(20);

        GUILayout.Label("Seleccionar Prefab:");
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

        GUILayout.Space(20);
        GUILayout.Label("Escala aleatoria de instanciación:");
        escalaMin = EditorGUILayout.FloatField("Escala mínima", escalaMin);
        escalaMax = EditorGUILayout.FloatField("Escala máxima", escalaMax);

        // Restricción para evitar que el mínimo sea mayor que el máximo
        escalaMin = Mathf.Min(escalaMin, escalaMax);
    }
}
