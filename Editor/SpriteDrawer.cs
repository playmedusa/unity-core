using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Sprite))]
public class SpriteDrawer : PropertyDrawer
{

	private static GUIStyle s_TempStyle = new GUIStyle();

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		var ident = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		Rect spriteRect;

		//create object field for the sprite
		spriteRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
		property.objectReferenceValue = EditorGUI.ObjectField(spriteRect, property.name, property.objectReferenceValue, typeof(Sprite), false);

		//if this is not a repain or the property is null exit now
		if (Event.current.type != EventType.Repaint || property.objectReferenceValue == null)
			return;

		//draw a sprite
		Sprite sp = property.objectReferenceValue as Sprite;
		IEnumerable enumerable = property.objectReferenceValue as IEnumerable;

		spriteRect.y += EditorGUIUtility.singleLineHeight + 4;
		spriteRect.width = 64;
		spriteRect.height = 64;
		if (sp.rect.position.magnitude > 0.1f)
		{
			Texture2D readable = new Texture2D((int)sp.texture.width, (int)sp.texture.height, sp.texture.format, false);
			Graphics.CopyTexture(sp.texture, readable);
			Texture2D tex = new Texture2D((int)sp.rect.width, (int)sp.rect.height);
			tex.SetPixels(readable.GetPixels(
				(int)sp.rect.x, (int)sp.rect.y, (int)sp.rect.width, (int)sp.rect.height, 0
			));
			s_TempStyle.normal.background = tex;
		}
		else
			s_TempStyle.normal.background = sp.texture;
		s_TempStyle.Draw(spriteRect, GUIContent.none, false, false, false, false);

		EditorGUI.indentLevel = ident;
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return base.GetPropertyHeight(property, label) + 70f;
	}
}