using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(Wave))]
public class WaveEditor : Editor
{
	private ReorderableList list;

	private void OnEnable()
	{
		list = new ReorderableList(serializedObject,
				serializedObject.FindProperty("monsters"),
				true, true, true, true);

		list.drawHeaderCallback = DrawListHeader;
		list.drawElementCallback = DrawListElement;

		list.onSelectCallback = (ReorderableList l) => {
			var prefab = l.serializedProperty.GetArrayElementAtIndex(l.index).FindPropertyRelative("monsterPrefab").objectReferenceValue as GameObject;
			if (prefab)
				EditorGUIUtility.PingObject(prefab.gameObject);
		};

		list.onAddCallback = (ReorderableList l) => {
			var index = l.serializedProperty.arraySize;
			l.serializedProperty.arraySize++;
			l.index = index;
			var element = l.serializedProperty.GetArrayElementAtIndex(index);
			element.FindPropertyRelative("monsterTier").intValue = 1;
			element.FindPropertyRelative("monsterQuantity").intValue = 20;
			element.FindPropertyRelative("monsterPrefab").objectReferenceValue =
					AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Monsters/Monster.prefab",
					typeof(GameObject)) as GameObject;
		};

		list.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
			var menu = new GenericMenu();

			foreach (var command in (CommandType[])Enum.GetValues(typeof(CommandType)))
			{
				if (command == CommandType.Monster)
					continue;
				menu.AddItem(new GUIContent(Enum.GetName(typeof(CommandType), command)),
				false, clickHandler,
				new MonsterCreationParams() { CommandType = command, Path = "" });
			}

			var guids = AssetDatabase.FindAssets("", new[] { "Assets/Prefabs/Monsters" });
			foreach (var guid in guids)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				menu.AddItem(new GUIContent("Monsters/" + Path.GetFileNameWithoutExtension(path)),
				false, clickHandler,
				new MonsterCreationParams() { CommandType = CommandType.Monster, Path = path });
			}
			//guids = AssetDatabase.FindAssets("", new[] { "Assets/Prefabs/Bosses" });
			//foreach (var guid in guids)
			//{
			//	var path = AssetDatabase.GUIDToAssetPath(guid);
			//	menu.AddItem(new GUIContent("Bosses/" + Path.GetFileNameWithoutExtension(path)),
			//	false, clickHandler,
			//	new MonsterCreationParams() { Path = path });
			//}
			menu.ShowAsContext();
		};

	}

	private void clickHandler(object target) {
		var data = (MonsterCreationParams)target;
		var index = list.serializedProperty.arraySize;
		list.serializedProperty.arraySize++;
		list.index = index;
		var element = list.serializedProperty.GetArrayElementAtIndex(index);

		element.FindPropertyRelative("commandType").enumValueIndex = (int)data.CommandType;

		if (data.CommandType == CommandType.RepeatGroup)
		{
			element.FindPropertyRelative("repeatTimes").intValue = 1;
			element.FindPropertyRelative("repeatNextXRows").intValue = 1;
		}
		else
		{
			element.FindPropertyRelative("monsterTier").intValue = 1;
			element.FindPropertyRelative("monsterQuantity").intValue = 10;
			element.FindPropertyRelative("monsterPrefab").objectReferenceValue =
				AssetDatabase.LoadAssetAtPath(data.Path, typeof(GameObject)) as GameObject;
		}
		serializedObject.ApplyModifiedProperties();
	}

	private void DrawListElement(Rect rect, int index, bool isActive, bool isFocused)
    {
		SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);


		rect.y += 2;
		if (element.FindPropertyRelative("commandType").enumValueIndex == (int)CommandType.Monster)
		{
			EditorGUI.PropertyField(
				new Rect(rect.x, rect.y, 140, EditorGUIUtility.singleLineHeight),
				element.FindPropertyRelative("monsterPrefab"), GUIContent.none);
			EditorGUI.LabelField(
				new Rect(rect.x + 150, rect.y, 30, EditorGUIUtility.singleLineHeight),
				"Tier");
			EditorGUI.PropertyField(
				new Rect(rect.x + 180, rect.y, 30, EditorGUIUtility.singleLineHeight),
				element.FindPropertyRelative("monsterTier"), GUIContent.none);
			EditorGUI.LabelField(
				new Rect(rect.x + 220, rect.y, 60, EditorGUIUtility.singleLineHeight),
				"Quantity");
			EditorGUI.PropertyField(
				new Rect(rect.x + 280, rect.y, 30, EditorGUIUtility.singleLineHeight),
				element.FindPropertyRelative("monsterQuantity"), GUIContent.none);
		}
		else if(element.FindPropertyRelative("commandType").enumValueIndex == (int)CommandType.RepeatGroup)
		{
			EditorGUI.LabelField(
					new Rect(rect.x, rect.y, 95, EditorGUIUtility.singleLineHeight),
					"Repeat the next");
			EditorGUI.PropertyField(
				new Rect(rect.x + 95, rect.y, 30, EditorGUIUtility.singleLineHeight),
				element.FindPropertyRelative("repeatNextXRows"), GUIContent.none);
			EditorGUI.LabelField(
					new Rect(rect.x + 130, rect.y, 40, EditorGUIUtility.singleLineHeight),
					"row(s)");
			EditorGUI.LabelField(
				new Rect(rect.x + 180, rect.y, 120, EditorGUIUtility.singleLineHeight),
				"Repeated Times");
			EditorGUI.PropertyField(
				new Rect(rect.x + 280, rect.y, 30, EditorGUIUtility.singleLineHeight),
				element.FindPropertyRelative("repeatTimes"), GUIContent.none);
		}

	}

    private void DrawListHeader(Rect rect)
    {
		EditorGUI.LabelField(rect, "Wave Monsters");
	}

    public override void OnInspectorGUI()
	{
		serializedObject.Update();
		list.DoLayoutList();
		serializedObject.ApplyModifiedProperties();
	}

	private struct MonsterCreationParams
	{
		public CommandType CommandType;
		public string Path;
	}
}


