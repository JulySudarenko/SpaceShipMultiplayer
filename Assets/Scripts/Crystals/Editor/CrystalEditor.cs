using UnityEditor;
using UnityEngine;

namespace Crystals.Editor
{
    [CustomEditor(typeof(Crystal)), CanEditMultipleObjects]
    public class CrystalEditor : UnityEditor.Editor
    {
        private SerializedProperty _center;
        private SerializedProperty _points;
        private SerializedProperty _frequency;
        
        private Vector3 _pointSnap = new Vector3(0.05f, 0.05f, 0.05f);
        private bool _showList;
        private void OnEnable()
        {
            _center = serializedObject.FindProperty("_center");
            _points = serializedObject.FindProperty("_points");
            _frequency = serializedObject.FindProperty("_frequency");
        }
        // public override void OnInspectorGUI()
        // {
        //     serializedObject.Update();
        //     EditorGUILayout.PropertyField(_center);
        //     EditorGUILayout.PropertyField(_points);
        //     EditorGUILayout.IntSlider(_frequency, 1, 20);
        //     var totalPoints = _frequency.intValue * _points.arraySize;
        //     if (totalPoints < 3)
        //     {
        //         EditorGUILayout.HelpBox("At least three points are needed.",
        //             MessageType.Warning);
        //     }
        //     else
        //     {
        //         EditorGUILayout.HelpBox(totalPoints + " points in total.",
        //             MessageType.Info);
        //     }
        //     serializedObject.ApplyModifiedProperties();
        // }
        
         public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(_center);
            _showList = EditorGUILayout.Foldout(_showList, _points.displayName);

            if (_showList)
            {
                for (int i = 0; i < _points.arraySize; i++)
                {
                    var point = _points.GetArrayElementAtIndex(i);
                    var horizontal = EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(point);
                    if (GUILayout.Button("↓", GUILayout.Width(20)))
                    {
                        MoveDown(i);
                    }
                    if (GUILayout.Button("+", GUILayout.Width(20)))
                    {
                        DuplicateItem(i);
                    }
                    if (GUILayout.Button("-", GUILayout.Width(20)))
                    {
                        RemoveItem(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.IntSlider(_frequency, 1, 20);

            var totalPoints = _frequency.intValue * _points.arraySize;

            if (totalPoints < 3)
            {
                EditorGUILayout.HelpBox("At least three points are needed.", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.HelpBox(totalPoints + " points in total.", MessageType.Info);
            }
            
            if (!serializedObject.ApplyModifiedProperties() &&
                (Event.current.type != EventType.ExecuteCommand ||
                 Event.current.commandName != "UndoRedoPerformed"))
            {
                return;
            }
            
            foreach (var obj in targets)
            {
                if (obj is Crystal crustal)
                {
                    crustal.UpdateMesh();
                }
            }
        }

        private void MoveDown(int index)
        {
            if (_points.arraySize > index + 1)
                _points.MoveArrayElement(index, index + 1);
        }

        private void DuplicateItem(int index)
        {
            _points.InsertArrayElementAtIndex(index);
        }

        private void RemoveItem(int index)
        {
            _points.DeleteArrayElementAtIndex(index);
        }
        private void OnSceneGUI()
        {
            if (!(target is Crystal crystal))
            {
                return;
            }
            var starTransform = crystal.transform;
            var angle = -360f / (crystal.Frequency * crystal.Points.Length);
            for (var i = 0; i < crystal.Points.Length; i++)
            {
                var rotation = Quaternion.Euler(0f, 0f, angle * i);
                var oldPoint = starTransform.TransformPoint(rotation *
                                                            crystal.Points[i].Position);
                var newPoint = Handles.FreeMoveHandle(oldPoint, Quaternion.identity,
                    0.02f, _pointSnap, Handles.DotHandleCap);
                if (oldPoint == newPoint)
                {
                    continue;
                }
                crystal.Points[i].Position = Quaternion.Inverse(rotation) *
                                          starTransform.InverseTransformPoint(newPoint);
                crystal.UpdateMesh();
            }
        }

    }
}
