using System.Collections.Generic;
using Crystals;
using Mechanics;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public sealed class PlanetsSystemCreatorWindow : EditorWindow
    {
        private List<PlanetCharacteristics> _planets;
        private Crystal[] _crystals;
        private PlanetOrbit[] _solarSystemPlanets;
        private PlanetOrbit _planet;
        private Crystal _crystal;
        private float _positionX;
        private float _positionZ;
        private float _speed;
        private float _zonePositionX;
        private float _zonePositionZ;
        private float _zoneRadius;

        public void OnEnable()
        {
            _planets = new List<PlanetCharacteristics>();
            UpdatePlanetsList();
            UpdateCrystalList();
        }

        private void UpdatePlanetsList()
        {
            _planets.Clear();

            _solarSystemPlanets = FindObjectsOfType<PlanetOrbit>();
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

        private void UpdateCrystalList()
        {
            _crystals = FindObjectsOfType<Crystal>();
        }

        public void OnGUI()
        {
            ShowPlanets();
            ShowPlanetsCreator();
            ShowCrystals();
            ShowCrystalCreator();
            SaveAllChanges();
        }

        private void ShowPlanets()
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
                var removeButton = GUILayout.Button("-");
                EditorGUILayout.EndHorizontal();

                if (removeButton)
                {
                    DestroyImmediate(_planets[i].Planet.gameObject);
                    _planets.Remove(_planets[i]);
                    UpdatePlanetsList();
                }
            }
        }

        private void ShowPlanetsCreator()
        {
            GUILayout.Label("New planet", EditorStyles.boldLabel);
            _planet =
                EditorGUILayout.ObjectField("Planet", _planet, typeof(PlanetOrbit), false) as PlanetOrbit;
            _positionX = EditorGUILayout.Slider("X", _positionX, -1500f, 1500f);
            _positionZ = EditorGUILayout.Slider("Z", _positionZ, -1500f, 1500f);
            _speed = EditorGUILayout.Slider("Speed", _speed, 0.0f, 0.1f);
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

        private void ShowCrystals()
        {
            GUILayout.Label("Existing crystals", EditorStyles.boldLabel);

            for (int i = 0; i < _crystals.Length; i++)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginHorizontal();
                _crystals[i].name = EditorGUILayout.TextField(_crystals[i].name, GUILayout.Width(120.0f));
                var removeCrystalButton = GUILayout.Button("-", GUILayout.Width(45.0f));
                EditorGUILayout.EndHorizontal();

                if (removeCrystalButton)
                {
                    DestroyImmediate(_crystals[i].gameObject);
                    UpdateCrystalList();
                }
            }
        }

        private void ShowCrystalCreator()
        {
            GUILayout.Label("New crystal zone", EditorStyles.boldLabel);
            _crystal =
                EditorGUILayout.ObjectField("Crystal", _crystal, typeof(Crystal), false) as Crystal;
            _zonePositionX = EditorGUILayout.Slider("X", _zonePositionX, -1500f, 1500f);
            _zonePositionZ = EditorGUILayout.Slider("Z", _zonePositionZ, -1500f, 1500f);
            _zoneRadius = EditorGUILayout.Slider("Radius", _zoneRadius, 5.0f, 100.0f);

            var createCrystalButton = GUILayout.Button("Create crystals");

            if (createCrystalButton)
            {
                int crystalsNumber;
                if (_zoneRadius <= 30)
                    crystalsNumber = 1;
                else
                    crystalsNumber = 3;
                for (int i = 0; i < crystalsNumber; i++)
                {
                    Instantiate(_crystal);
                    var posX = _zonePositionX + Random.Range(-_zoneRadius, _zoneRadius);
                    var posZ = _zonePositionZ + Random.Range(-_zoneRadius, _zoneRadius);

                    _crystal.transform.position = new Vector3(posX, 0.0f, posZ);
                    UpdateCrystalList();
                }
            }
        }

        private void SaveAllChanges()
        {
            var saveChangesButton = GUILayout.Button("Save changes");
            if (saveChangesButton)
            {
                for (int i = 0; i < _planets.Count; i++)
                {
                    _planets[i].Planet.transform.position = _planets[i].Position;
                    _planets[i].Planet.CircleInSecond = _planets[i].Speed;
                }
            }
        }
    }
}
