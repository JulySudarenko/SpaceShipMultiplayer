using System;
using Mechanics;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    
    public sealed class PlanetsSystemCreatorWindow : EditorWindow
    {
        private PlanetOrbit[] _planets = Array.Empty<PlanetOrbit>();
        private int[] _positionsZ = Array.Empty<int>();
        
        private GameObject _worldPrefab;
        private GameObject _loadedPrefab;
        private string _prefabPath;

        private void OnGUI()
        {
            var oldPrefab = _worldPrefab;
            _worldPrefab = (GameObject)EditorGUILayout.ObjectField("World prefab", _worldPrefab,
                typeof(GameObject), false);

            if (_worldPrefab != oldPrefab && _worldPrefab != null)
            {
                LoadPrefab();
            }

            for (var i = 0; i < _planets.Length; i++)
            {
                var planet = _planets[i];
                var transform = planet.transform;

                _positionsZ[i] = EditorGUILayout.IntSlider($"{planet.name} distance", _positionsZ[i],
                    -3000, 3000);
                
                var newPosition = transform.position;
                newPosition.z = _positionsZ[i];
                
                transform.position = newPosition;
            }

            if (GUILayout.Button("Save"))
            {
                PrefabUtility.SaveAsPrefabAsset(_loadedPrefab, _prefabPath);
            }
        }

        private void LoadPrefab()
        {
            if (_loadedPrefab != null)
            {
                PrefabUtility.SaveAsPrefabAsset(_loadedPrefab, _prefabPath);
                PrefabUtility.UnloadPrefabContents(_loadedPrefab);
            }

            _prefabPath = AssetDatabase.GetAssetPath(_worldPrefab);
            _loadedPrefab = PrefabUtility.LoadPrefabContents(_prefabPath);

            _planets = _loadedPrefab.GetComponentsInChildren<PlanetOrbit>();
            _positionsZ = new int[_planets.Length];

            for (int i = 0; i < _positionsZ.Length; i++)
            {
                _positionsZ[i] = (int)_planets[i].transform.position.z;
            }
        }

        private void OnDestroy()
        {
            if (_loadedPrefab != null)
            {
                PrefabUtility.SaveAsPrefabAsset(_loadedPrefab, _prefabPath);
                PrefabUtility.UnloadPrefabContents(_loadedPrefab);
                _planets = Array.Empty<PlanetOrbit>();
                _positionsZ = Array.Empty<int>();
            }
        }
    }
}
