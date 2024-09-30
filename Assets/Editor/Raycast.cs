using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class Raycast : EditorWindow
{
    static bool activo;
    public List<GameObject> prefabsParaInstanciar = new List<GameObject>();
    public GameObject prefabSeleccionado;
    public GameObject objetoPadre; // Objeto donde se instanciarán los prefabs si se selecciona

    // Abrir desde el menú Morion/Creador de Arda
    [MenuItem("Morion/Creador de Arda")]
    static void Init()
    {
        var ventana = (Raycast)EditorWindow.GetWindow(typeof(Raycast));
        ventana.Show();
    }

    // Escuchar eventos de la escena
    void OnEnable() => SceneView.duringSceneGui += OnSceneGUI;
    void OnDisable() => SceneView.duringSceneGui -= OnSceneGUI;

    // Recibir eventos de la escena
    void OnSceneGUI(SceneView vista)
    {
        if (!activo)
        {
            return;
        }

        Ray rayo = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;

        // Instanciar el prefab seleccionado al hacer clic
        if (Event.current.type == EventType.MouseDown && prefabSeleccionado != null && Physics.Raycast(rayo, out hit))
        {
            Debug.Log("Impacto: " + hit.collider.gameObject.name);

            // Instanciar el prefab
            var objeto = (GameObject)PrefabUtility.InstantiatePrefab(prefabSeleccionado);

            // Establecer la posición en el punto de impacto
            objeto.transform.position = hit.point;

            // Si se ha seleccionado un objeto en la jerarquía, establecerlo como padre
            if (objetoPadre != null)
            {
                objeto.transform.SetParent(objetoPadre.transform);
            }
        }

        Event.current.Use();
        SceneView.RepaintAll();
    }

    // Crear ventana del editor con botones para controlar el raycasting
    // y seleccionar prefabs
    void OnGUI()
    {
        // Botón para activar/desactivar el raycasting
        if (GUILayout.Button(activo ? "Desactivar" : "Activar"))
        {
            activo = !activo;
        }

        // Cambiar color de label dependiendo de si está activo o no
        GUIStyle estiloEstado = new GUIStyle(GUI.skin.label);
        estiloEstado.normal.textColor = activo ? Color.green : Color.red;

        GUILayout.Label("Activo: " + activo, estiloEstado);

        // Mostrar vista previa del prefab seleccionado centrada
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

        // Campo para seleccionar un GameObject de la jerarquía como padre
        GUILayout.Label("Seleccionar Objeto Padre (Opcional):");
        objetoPadre = (GameObject)EditorGUILayout.ObjectField(objetoPadre, typeof(GameObject), true);

        // Área para arrastrar múltiples prefabs a la vez
        GUILayout.Label("Arrastra y Suelta Prefabs Abajo:");
        Event evt = Event.current;
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Arrastra los Prefabs Aquí");

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(evt.mousePosition))
                    break;

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

        // Mostrar la lista de prefabs como botones organizados en cuadrícula con vista previa
        GUILayout.Label("Seleccionar Prefab:");

        float windowWidth = position.width;  // Ancho de la ventana del editor
        int columnas = Mathf.Max(1, Mathf.FloorToInt(windowWidth / 70));  // Calcula columnas en función del ancho disponible
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
    }
}
