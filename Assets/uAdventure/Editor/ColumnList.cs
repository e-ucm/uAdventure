using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Editor
{
    public class ColumnList : ScrollableList
    {
        private float[] columnPercent;
        private static GUILayoutOption[] defaultColumnBehaviour = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };

        public List<Column> Columns { get; set; }

        public delegate void DrawColumn(Rect rect, Column column);
        public DrawColumn drawColumn;
        public delegate void DrawCell(Rect rect, int row, int column, bool isActive, bool isFocused);
        public DrawCell drawCell;
        private Rect headerRect, columnRect;
        public BaseWindow.VoidMethodDelegate RequestRepaint;

        protected void Repaint()
        {
            if(RequestRepaint != null)
            {
                RequestRepaint();
            }
        }

        public class Column
        {
            public bool Callback { get; set; }
            public string Text { get; set; }
            public GUILayoutOption[] SizeOptions { get; set; }
        }

        public ColumnList(SerializedObject serializedObject, SerializedProperty elements) : base(serializedObject, elements)
        {
            Init();
        }

        public ColumnList(IList elements, Type elementType) : base(elements, elementType)
        {
            Init();
        }

        public ColumnList(SerializedObject serializedObject, SerializedProperty elements, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton) : base(serializedObject, elements, draggable, displayHeader, displayAddButton, displayRemoveButton)
        {
            Init();
        }

        public ColumnList(IList elements, Type elementType, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton) : base(elements, elementType, draggable, displayHeader, displayAddButton, displayRemoveButton)
        {
            Init();
        }

        private void drawColumns(Rect rowRect, Action<Rect, int> callback)
        {
            columnRect = new Rect(rowRect)
            {
                width = 0
            };
            for (int i = 0; i < Columns.Count; i++)
            {
                columnRect.x += columnRect.width;
                columnRect.width = rowRect.width * columnPercent[i];
                callback(columnRect, i);
            }
        }

        private void Init()
        {
            Columns = new List<Column>();
            headerHeight = 20;

            drawHeaderCallback = (rect) =>
            {
                if (Event.current.type == EventType.Repaint)
                {
                    headerRect = rect;
                }
            };

            drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                drawColumns(rect, (columnRect, column) =>
                {
                    drawCell(columnRect, index, column, isActive, isFocused);
                });
            };
        }

        public override void DoList(float height)
        {
            if (columnPercent == null || columnPercent.Length != Columns.Count)
            {
                columnPercent = new float[Columns.Count];
            }
            base.DoList(height);

            DrawHeader(headerRect);
        }

        private void DrawHeader(Rect rect)
        {
            GUILayout.BeginArea(rect);
            EditorGUILayout.BeginHorizontal(GUILayout.Width(rect.width));
            int i = 0;
            foreach (var c in Columns)
            {
                if (drawColumn != null && c.Callback)
                {
                    columnRect = GUILayoutUtility.GetRect(0, rect.height, c.SizeOptions ?? defaultColumnBehaviour);
                }
                else
                {
                    columnRect = GUILayoutUtility.GetRect(new GUIContent(c.Text), GUI.skin.label, c.SizeOptions ?? defaultColumnBehaviour);
                }

                if (Event.current.type == EventType.Repaint)
                {
                    columnPercent[i] = columnRect.width / rect.width;
                }
                i++;
            }

            if (Event.current.type == EventType.Repaint)
            {
                var total = columnPercent.Sum();
                columnPercent = columnPercent.ToList().ConvertAll(c => c / total).ToArray();
                Repaint();
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();

            drawColumns(rect, (columnRect, column) =>
            {
                if (drawColumn != null && Columns[column].Callback)
                {
                    drawColumn(columnRect, Columns[column]);
                }
                else
                {
                    EditorGUI.LabelField(columnRect, Columns[column].Text);
                }
            });
        }
    }
}