using IMS.MD.v1p2;
using Malee.List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using uAdventure.Core;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Editor
{
    public class uAdventureWindowMetaData : EditorWindowBase
    {
		private SerializedObject serializedObject;
		private SimpleMetaDataWindow simple;
		private MultiMetaDataWindow multi;
        private Vector2 scroll;
        public SerializedProperty property;

        public static void OpenMetaDataWindow()
		{
			if (!Language.Initialized)
				Language.Initialize();

			var window = ScriptableObject.CreateInstance<uAdventureWindowMetaData>();
			window.ShowUtility();
		}


		protected override void InitWindows()
		{
			titleContent = new GUIContent(TC.Traslate("LOM.Title"));
			WantsMouseMove = true;

			if (serializedObject == null)
			{
				var wrapper = ScriptableObject.CreateInstance<LomWrapper>();
				serializedObject = new SerializedObject(wrapper);
				var adventureMetadata = Controller.Instance.AdventureData.getImsCPMetadata();
				if (adventureMetadata == null)
				{
					adventureMetadata = new lomType();
					Controller.Instance.AdventureData.setImsCPMetadata(adventureMetadata);
				}
				serializedObject.Update();
			}
			var lom = serializedObject.FindProperty("lom");
			var property = lom.FindPropertyRelative("general");
			do
			{
				if (property.isArray)
				{
					AddExtension(multi = new MultiMetaDataWindow(Rect.zero, new GUIContent(TC.get("")), "Window"));
					multi.ButtonContent = new GUIContent(property.displayName);
					multi.property = property.Copy();
					multi.onDraw = DrawProperty;
				}
				else
				{
					AddExtension(simple = new SimpleMetaDataWindow(Rect.zero, new GUIContent(TC.get("")), "Window"));
					simple.ButtonContent = new GUIContent(property.displayName);
					simple.property = property.Copy();
					simple.onDraw = DrawProperty;
				}
			}
			while (property.Next(false));
		}

		protected override void OnGUI()
		{
			EditorGUI.BeginChangeCheck();
			base.OnGUI();
			if (EditorGUI.EndChangeCheck())
			{
			}
			serializedObject.ApplyModifiedProperties();
        }


#if UNITY_2019_3_OR_NEWER
		private const float ELEMENT_EDGE_TOP = 1;
		private const float ELEMENT_EDGE_BOT = 2;
		private const float ELEMENT_EDGE_LEFT = 4;
		private const float ELEMENT_EDGE_RIGHT = 4;
#else
		private const float ELEMENT_EDGE_TOP = 1;
		private const float ELEMENT_EDGE_BOT = 3;
		private const float ELEMENT_EDGE_LEFT = 1;
		private const float ELEMENT_EDGE_RIGHT = 3;
#endif
		private const float ELEMENT_HEIGHT_OFFSET = ELEMENT_EDGE_TOP + ELEMENT_EDGE_BOT;
		private const float ELEMENT_WIDTH_OFFSET = ELEMENT_EDGE_LEFT + ELEMENT_EDGE_RIGHT;

        static class Style
		{

			internal const string PAGE_INFO_FORMAT = "{0} / {1}";

			internal static GUIContent iconToolbarPlus;
			internal static GUIContent iconToolbarPlusMore;
			internal static GUIContent iconToolbarMinus;
			internal static GUIContent iconPagePrev;
			internal static GUIContent iconPageNext;
			internal static GUIContent iconPagePopup;

			internal static GUIStyle paginationText;
			internal static GUIStyle pageSizeTextField;
			internal static GUIStyle draggingHandle;
			internal static GUIStyle headerBackground;
			internal static GUIStyle footerBackground;
			internal static GUIStyle paginationHeader;
			internal static GUIStyle boxBackground;
			internal static GUIStyle preButton;
			internal static GUIStyle preButtonStretch;
			internal static GUIStyle elementBackground;
			internal static GUIStyle verticalLabel;
			internal static GUIContent expandButton;
			internal static GUIContent collapseButton;
			internal static GUIContent sortAscending;
			internal static GUIContent sortDescending;

			internal static GUIContent listIcon;

			static Style()
			{

				iconToolbarPlus = EditorGUIUtility.IconContent("Toolbar Plus", "Add to list");
				iconToolbarPlusMore = EditorGUIUtility.IconContent("Toolbar Plus More", "Choose to add to list");
				iconToolbarMinus = EditorGUIUtility.IconContent("Toolbar Minus", "Remove selection from list");
				iconPagePrev = EditorGUIUtility.IconContent("Animation.PrevKey", "Previous page");
				iconPageNext = EditorGUIUtility.IconContent("Animation.NextKey", "Next page");

#if UNITY_2018_3_OR_NEWER
				iconPagePopup = EditorGUIUtility.IconContent("PopupCurveEditorDropDown", "Select page");
#else
				iconPagePopup = EditorGUIUtility.IconContent("MiniPopupNoBg", "Select page");
#endif
				paginationText = new GUIStyle();
				paginationText.margin = new RectOffset(2, 2, 0, 0);
				paginationText.fontSize = EditorStyles.miniTextField.fontSize;
				paginationText.font = EditorStyles.miniFont;
				paginationText.normal.textColor = EditorStyles.miniTextField.normal.textColor;
				paginationText.alignment = TextAnchor.MiddleLeft;
				paginationText.clipping = TextClipping.Clip;

#if UNITY_2019_3_OR_NEWER
				pageSizeTextField = new GUIStyle("RL Background");
#else
				pageSizeTextField = new GUIStyle("RL Footer");
				pageSizeTextField.overflow = new RectOffset(0, 0, -2, -3);
				pageSizeTextField.contentOffset = new Vector2(0, -1);
#endif
				pageSizeTextField.alignment = TextAnchor.MiddleLeft;
				pageSizeTextField.clipping = TextClipping.Clip;
				pageSizeTextField.fixedHeight = 0;
				pageSizeTextField.padding = new RectOffset(3, 0, 0, 0);
				pageSizeTextField.font = EditorStyles.miniFont;
				pageSizeTextField.fontSize = EditorStyles.miniTextField.fontSize;
				pageSizeTextField.fontStyle = FontStyle.Normal;
				pageSizeTextField.wordWrap = false;

				draggingHandle = new GUIStyle("RL DragHandle");
				headerBackground = new GUIStyle("RL Header");
				footerBackground = new GUIStyle("RL Footer");

#if UNITY_2019_3_OR_NEWER
				paginationHeader = new GUIStyle("TimeRulerBackground");
				paginationHeader.fixedHeight = 0;
#else
				paginationHeader = new GUIStyle("RL Element");
				paginationHeader.border = new RectOffset(2, 3, 2, 3);
#endif
				elementBackground = new GUIStyle("RL Element");
				elementBackground.border = new RectOffset(2, 3, 2, 3);
				verticalLabel = new GUIStyle(EditorStyles.label);
				verticalLabel.alignment = TextAnchor.UpperLeft;
				verticalLabel.contentOffset = new Vector2(10, 3);
				boxBackground = new GUIStyle("RL Background");
				boxBackground.border = new RectOffset(6, 3, 3, 6);

#if UNITY_2019_3_OR_NEWER
				preButton = new GUIStyle("RL FooterButton");
#else
				preButton = new GUIStyle("RL FooterButton");
				preButton.contentOffset = new Vector2(0, -4);
#endif
				preButtonStretch = new GUIStyle("RL FooterButton");
				preButtonStretch.fixedHeight = 0;
				preButtonStretch.stretchHeight = true;

				expandButton = EditorGUIUtility.IconContent("winbtn_win_max");
				expandButton.tooltip = "Expand All Elements";

				collapseButton = EditorGUIUtility.IconContent("winbtn_win_min");
				collapseButton.tooltip = "Collapse All Elements";

				sortAscending = EditorGUIUtility.IconContent("align_vertically_bottom");
				sortAscending.tooltip = "Sort Ascending";

				sortDescending = EditorGUIUtility.IconContent("align_vertically_top");
				sortDescending.tooltip = "Sort Descending";

				listIcon = EditorGUIUtility.IconContent("align_horizontally_right");
			}
		}

		public void DrawProperty(int id, SerializedProperty property)
		{
			if (!Controller.Instance.Loaded)
			{
				EditorGUILayout.HelpBox("Adventure is not loaded.", MessageType.Error);
				return;
			}

			using (var scope = new GUILayout.ScrollViewScope(scroll))
			{
				scroll = scope.scrollPosition;
				Rect rect = EditorGUILayout.GetControlRect(false, GetFullHeight(property, property), EditorStyles.largeLabel);
				DrawProperty(rect, property, property);
			}
		}

		private void DrawProperty(Rect rect, SerializedProperty parent, SerializedProperty element)
		{
			if (HasCustomPropertyDrawer(element) || element.propertyType == SerializedPropertyType.String)
			{
				DoElement(rect, parent, element);
			}
			else
			{
				var iter = element.Copy();
				iter.NextVisible(true);
				var depth = iter.depth;
				rect.height = 0;
				do
				{
					rect.y += rect.height;
					rect.height = GetHeight(element, iter);
					DoElement(rect, element, iter);
				} while (iter.NextVisible(false) && iter.depth == depth);
			}
		}

		private float GetFullHeight(SerializedProperty parent, SerializedProperty element)
		{
			var height = 0f;
			if (HasCustomPropertyDrawer(element) || element.propertyType == SerializedPropertyType.String)
			{
				height += GetHeight(parent, element);
			}
			else
			{
				var iter = element.Copy();
				iter.NextVisible(true);
				var depth = iter.depth;
				do
				{
					height += GetHeight(element, iter);
				} while (iter.NextVisible(false) && iter.depth == depth);
			}
			return height;
		}

		private float GetHeightOld(SerializedProperty parent, SerializedProperty property)
		{
			if (property.isArray && property.propertyType != SerializedPropertyType.String)
			{
				var list = ReorderableDrawer.GetList(parent, new ReorderableAttribute { labels = false }, property.name.GetHashCode(), property.name);
				if (!listWithCallback.ContainsKey(list))
				{
					list.drawElementCallback += (r, e, l, s, f) => List_drawElementCallback(r, property, e, l, s, f);
					list.getElementHeightCallback += (e) => List_getElementHeightCallback(property, e);
					listWithCallback.Add(list, true);
				}
				return list.GetHeight() + ELEMENT_HEIGHT_OFFSET * 2;
			}
			else if (property.hasVisibleChildren)
			{
				var height = ELEMENT_HEIGHT_OFFSET * 2;

				if (property.isExpanded)
				{
					height += EditorGUI.GetPropertyHeight(property, GUIContent.none, true);
					if (HasCustomPropertyDrawer(property) && level == 0)
					{
						height += 20;
					}
				}
				else
				{
					height += 20;
				}

				return height;
			}
			else
			{
				var height = 0f;
				if (level == 0)
				{
					height += 20 + ELEMENT_HEIGHT_OFFSET * 2;
				}
				if (property.isExpanded)
				{
					height += EditorGUI.GetPropertyHeight(property, true);
				}

				return height;
			}
		}

		private void DoElement(Rect rect, SerializedProperty parent, SerializedProperty property)
		{
			if (property.isArray && property.propertyType != SerializedPropertyType.String)
			{
				DoListProperty(rect, parent, property);
			}
			else if (level == 0)
			{
				DoPrettyProperty(rect, parent, property);
			}
			else
			{
				EditorGUI.PropertyField(rect, property, GUIContent.none, true);
			}
		}

		private float GetHeight(SerializedProperty parent, SerializedProperty property)
		{
			float height = 0f;
			if (property.isArray && property.propertyType != SerializedPropertyType.String)
			{
				height = GetListPropertyHeight(parent, property);
			}
			else if (level == 0)
			{
				height = GetPrettyPropertyHeight(parent, property);
			}
			else
			{
				height = EditorGUI.GetPropertyHeight(property, GUIContent.none, true);
			}
			return height;
		}

		private void DoPrettyProperty(Rect rect, SerializedProperty parent, SerializedProperty property)
		{
			var headerRect = new Rect(rect.x, rect.y, rect.width, 20);
			rect.y += 20 + ELEMENT_EDGE_TOP;
			rect.height -= (20 + ELEMENT_EDGE_BOT);

			DrawHeader(headerRect, property, new GUIContent(property.displayName));
			if (property.isExpanded)
			{
				DrawElement(rect, Event.current, parent, property);
			}

			GUILayout.Space(ELEMENT_EDGE_BOT);
		}

		private float GetPrettyPropertyHeight(SerializedProperty parent, SerializedProperty property)
		{
			var height = 20f + ELEMENT_EDGE_TOP + ELEMENT_EDGE_BOT; // Header Height
			if (property.isExpanded)
			{
				height += GetElementHeight(parent, property);
			}
			return height;
		}

		private Dictionary<ReorderableList, bool> listWithCallback = new Dictionary<ReorderableList, bool>();

		private void DoListProperty(Rect rect, SerializedProperty parent, SerializedProperty property)
		{
			var list = ReorderableDrawer.GetList(parent, new ReorderableAttribute { labels = false }, property.name.GetHashCode(), property.name);
			if (list == null)
			{
				Debug.Log(parent.propertyPath + " " + property.propertyPath);
				return;
			}
			list.elementLabels = false;
			if (!listWithCallback.ContainsKey(list))
			{
				var originalP = property.Copy();
				list.drawElementCallback += (r, e, l, s, f) => List_drawElementCallback(r, property, e, l, s, f);
				list.getElementHeightCallback += (e) => List_getElementHeightCallback(property, e);
				listWithCallback.Add(list, true);
			}
			list.DoList(rect, new GUIContent(property.displayName));
		}

		private float GetListPropertyHeight(SerializedProperty parent, SerializedProperty property)
		{
			var list = ReorderableDrawer.GetList(parent, new ReorderableAttribute { labels = false }, property.name.GetHashCode(), property.name);
			if (list == null)
			{
				Debug.Log(parent.propertyPath + " " + property.propertyPath);
				return 0f;
			}
			list.elementLabels = false;
			if (!listWithCallback.ContainsKey(list))
			{
				var originalP = property.Copy();
				list.drawElementCallback += (r, e, l, s, f) => List_drawElementCallback(r, originalP, e, l, s, f);
				list.getElementHeightCallback += (e) => List_getElementHeightCallback(originalP, e);
				listWithCallback.Add(list, true);
			}
			return list.GetHeight();
		}

		private float List_getElementHeightCallback(SerializedProperty parent, SerializedProperty element)
		{
			level++;
			var height = GetFullHeight(parent, element);
			level--;
			return height;
		}

		private void List_drawElementCallback(Rect rect, SerializedProperty parent, SerializedProperty element, GUIContent label, bool selected, bool focused)
		{
			level++;
			DrawProperty(rect, parent, element);
			level--;
		}


		private void DrawHeader(Rect rect, SerializedProperty property, GUIContent label)
		{
			if (Event.current.type == EventType.Repaint)
			{

				Style.headerBackground.Draw(rect, false, false, false, false);
			}

			Rect titleRect = rect;
			titleRect.xMin += 6f;
			titleRect.xMax -= 95f;

			label = EditorGUI.BeginProperty(titleRect, label, property);

			titleRect.xMin += 10;

			EditorGUI.BeginChangeCheck();

			bool isExpanded = EditorGUI.Foldout(titleRect, property.isExpanded, label, true);

			if (EditorGUI.EndChangeCheck())
			{

				property.isExpanded = isExpanded;
			}

			EditorGUI.EndProperty();
			//draw sorting options
		}

		private int level = 0;

		private void DrawElement(Rect rect, Event evt, SerializedProperty parent, SerializedProperty element)
		{
			if (evt.type == EventType.Repaint)
			{
				Style.boxBackground.Draw(rect, false, false, false, false);
			}
			rect = AdjustRectHeight(rect);
			rect = AdjustRectWidth(rect);

			EditorGUI.BeginProperty(rect, GUIContent.none, element);

			if (element.hasVisibleChildren && !HasCustomPropertyDrawer(element))
			{
				level++;
				DrawProperty(rect, parent, element);
				level--;
			}
			else
			{
				EditorGUI.PropertyField(rect, element, GUIContent.none, true);
			}
			EditorGUI.EndProperty();
		}

		private float GetElementHeight(SerializedProperty parent, SerializedProperty element)
		{
			if (element.hasVisibleChildren && !HasCustomPropertyDrawer(element))
			{
				level++;
				var height = GetFullHeight(parent, element);
				level--;
				return height;
			}
			else
			{
				return EditorGUI.GetPropertyHeight(element, GUIContent.none, true);
			}

		}

		private static Rect AdjustRectHeight(Rect rect)
		{
			rect.height -= ELEMENT_HEIGHT_OFFSET;
			rect.y += ELEMENT_EDGE_TOP;
			return rect;
		}
		private static Rect AdjustRectWidth(Rect rect)
		{
			rect.width -= ELEMENT_WIDTH_OFFSET;
			rect.x += ELEMENT_EDGE_LEFT;
			return rect;
		}

		static MethodInfo drawerSearcher;

		public static bool HasCustomPropertyDrawer(SerializedProperty property)
		{
			if (drawerSearcher == null)
			{
				var scriptAttributeUtility = AppDomain.CurrentDomain.GetAssemblies()
				   .Select(a =>
				   {
					   try
					   {
						   return a.GetType("UnityEditor.ScriptAttributeUtility");
					   }
					   catch { }
					   return null;
				   }).Where(t => t != null).First();

				drawerSearcher = scriptAttributeUtility.GetMethod("GetDrawerTypeForType", BindingFlags.Static | BindingFlags.NonPublic);
			}

			var type = property.GetPropertyFieldType();
			var drawerType = drawerSearcher.Invoke(null, new object[] { type });

			return drawerType != null;
		}


        private static string CleanXMLGarbage(string finalText)
        {
            var lomTypeRegex = @"[\s\n\r]*<imsmd:[a-z]*[A-Z]*>[\s\n\r]*<imsmd:source>[\s\n\r]*<imsmd:langstring xml:lang=\x22\x22>[\s\n\r]*<\/imsmd:langstring>[\s\n\r]*<\/imsmd:source>[\s\n\r]*<imsmd:value>[\s\n\r]*<imsmd:langstring xml:lang=\x22\x22>[\s\n\r]*<\/imsmd:langstring>[\s\n\r]*<\/imsmd:value>[\s\n\r]*<\/imsmd:[a-z]*[A-Z]*>";
            var emptyLangStringRegex = @"[\s\n\r]*<imsmd:langstring xml:lang=\x22[a-zA-Z]*\x22>[\s\n\r]*<\/imsmd:langstring>";
            var emptyFieldRegex = @"[\s\n\r]*<imsmd:[a-zA-Z]* \/>";
            var emptyFieldOpenRegex = @"[\s\n\r]*<imsmd:[a-zA-Z]*>[\s\n\r]*<\/imsmd:[a-zA-Z]*>";

            foreach (Match match in Regex.Matches(finalText, lomTypeRegex))
            {
                finalText = finalText.Replace(match.Value, "");
            }
            foreach (Match match in Regex.Matches(finalText, emptyLangStringRegex))
            {
                finalText = finalText.Replace(match.Value, "");
            }
            foreach (Match match in Regex.Matches(finalText, emptyFieldRegex))
            {
                finalText = finalText.Replace(match.Value, "");
            }
            string beforeStart;
            do
            {
                beforeStart = finalText;
                foreach (Match match in Regex.Matches(finalText, emptyFieldOpenRegex))
                {
                    finalText = finalText.Replace(match.Value, "");
                }
            } while (beforeStart != finalText);
            return finalText;
        }
	}
}
