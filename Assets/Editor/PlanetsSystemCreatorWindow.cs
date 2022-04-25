using System.Collections.Generic;
using Mechanics;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public sealed class PlanetsSystemCreatorWindow : EditorWindow
    {
        private List<PlanetCharacteristics> _planets;
        private PlanetOrbit[] _solarSystemPlanets;
        private PlanetCharacteristics _newPlanet;
        private PlanetOrbit _planet;
        private float _positionX;
        private float _positionZ;
        private float _speed;
        private bool _removeButton;

        public void OnEnable()
        {
            UpdatePlanetsList();
        }

        private void UpdatePlanetsList()
        {
            _planets = new List<PlanetCharacteristics>();
            var _solarSystemPlanets = FindObjectsOfType<PlanetOrbit>();
            for (int i = 0; i < _solarSystemPlanets.Length; i++)
            {
                var newPlanet = new PlanetCharacteristics
                {
                    Planet = _solarSystemPlanets[i],
                    Position = _solarSystemPlanets[i].gameObject.transform.position,
                    Speed = _solarSystemPlanets[i].CircleInSecond
                };

                _planets.Add(newPlanet);
            }
        }

        public void OnGUI()
        {
            GUILayout.Label("Existing planets", EditorStyles.boldLabel);

            for (int i = 0; i < _planets.Count; i++)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginHorizontal();
                _planets[i].Planet.name = EditorGUILayout.TextField(_planets[i].Planet.name, GUILayout.Width(95f));
                EditorGUIUtility.labelWidth = 14.0f;
                _planets[i].Position.x = EditorGUILayout.FloatField("X", _planets[i].Position.x);
                _planets[i].Position.z = EditorGUILayout.FloatField("Z", _planets[i].Position.z);
                EditorGUIUtility.labelWidth = 40.0f;
                _planets[i].Speed = EditorGUILayout.FloatField("Speed", _planets[i].Speed);
                _removeButton = GUILayout.Button("-");
                EditorGUILayout.EndHorizontal();

                if (_removeButton)
                {
                    DestroyImmediate(_planets[i].Planet.gameObject);
                    _planets.Remove(_planets[i]);
                    UpdatePlanetsList();
                }
            }

            GUILayout.Label("New planet", EditorStyles.boldLabel);
            _planet =
                EditorGUILayout.ObjectField("Planet", _planet, typeof(PlanetOrbit), false) as PlanetOrbit;
            _positionX = EditorGUILayout.Slider("X", _positionX, -1500f, 1500f);
            _positionZ = EditorGUILayout.Slider("Z", _positionZ, -1500f, 1500f);
            _speed = EditorGUILayout.Slider("Speed", _speed, -0.0f, 1.0f);
            var createButton = GUILayout.Button("Create new planet");

            if (createButton)
            {
                Instantiate(_planet);
                _planet.transform.position = new Vector3(_positionX, 0.0f, _positionZ);
                _planet.CircleInSecond = _speed;
                var creatingPlanet = new PlanetCharacteristics
                {
                    Planet = _planet,
                    Position = _planet.transform.position,
                    Speed = _planet.CircleInSecond
                };
                _planets.Add(creatingPlanet);
                UpdatePlanetsList();
            }
            
        }
    }
}
