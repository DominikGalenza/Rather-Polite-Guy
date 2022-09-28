using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        Dialogue selectedDialogue = null;
        GUIStyle nodeStyle;
        bool isDragging = false;

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if (dialogue != null)
            {
                ShowEditorWindow();
                return true;
            }
            return false;
        }

        private void OnEnable() 
        {
            Selection.selectionChanged += OnSelectionChanged;

            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;

            nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            nodeStyle.border = new RectOffset(10, 10, 10, 10);
        }

        private void OnSelectionChanged()
        {
            Dialogue newDialogue = Selection.activeObject as Dialogue;

            if (newDialogue != null)
            {
                selectedDialogue = newDialogue;
                Repaint();
            }
        }

        private void OnGUI() 
        {
            if (selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No dialogue selected");
            }
            else
            {
                ProcessEvents();

                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    OnGUINode(node);
                }
            }
        }

        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && !isDragging)
            {
                isDragging = true;
            }
            else if (Event.current.type == EventType.MouseDrag && isDragging)
            {
                Undo.RecordObject(this, "Move Dialogue Node");
                selectedDialogue.GetRootNode().rect.position = Event.current.mousePosition;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && isDragging)
            {
                isDragging = false;
            }
        }

        private void OnGUINode(DialogueNode node)
        {
            GUILayout.BeginArea(node.rect, nodeStyle);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Node:", EditorStyles.whiteLabel);

            string newText = EditorGUILayout.TextField(node.text);
            string newUniqueID = EditorGUILayout.TextField(node.uniqueID);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(selectedDialogue, "Update dialogue text");
                node.text = newText;
                node.uniqueID = newUniqueID;
                EditorUtility.SetDirty(this);
            }

            GUILayout.EndArea();
        }
    }
}
