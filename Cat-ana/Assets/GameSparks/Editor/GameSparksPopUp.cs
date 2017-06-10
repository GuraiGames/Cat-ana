using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GameSparksPopUp : EditorWindow {
	private Texture2D logo;

	void OnEnable()
	{
		logo = Resources.Load("GameSparksLogo", typeof(Texture2D)) as Texture2D;

		GUIContent content = new GUIContent ("GameSparksSDK");

		titleContent = content;
	}

	void OnGUI()
	{
		GUIStyle style = new GUIStyle(EditorStyles.wordWrappedLabel);

		style.alignment = TextAnchor.MiddleCenter;

		GUILayout.Label(logo);

		EditorGUILayout.LabelField("Welcome to GameSparks!", style);

		GUILayout.Space(20);

		EditorGUILayout.LabelField("To take advantage of this plugin, you must first register", style);
		EditorGUILayout.LabelField("on the GameSparks platform. Here you will be able to build", style);
		EditorGUILayout.LabelField("and configure your game.", style);

		GUILayout.Space(30);

		if (GUILayout.Button("Register Now")) {
			Close();

			Application.OpenURL("https://auth.gamesparks.net/register.htm?utm_source=Unity%20SDK&utm_medium=Unity%20Editor&utm_campaign=Unity%20Asset%20Store");
		}

		GUILayout.Space(10);
	}
}
