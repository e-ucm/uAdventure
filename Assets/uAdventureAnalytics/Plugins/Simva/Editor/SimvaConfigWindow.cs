using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using uAdventure.Editor;
using UnityEngine;

namespace uAdventure.Simva
{
    public class SimvaConfigWindow : LayoutWindow
    {

        protected TabsManager tabsManager;

        protected SimvaConfigWindow(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
        {
        }

        public override void Draw(int aID)
        {
        }

        protected void AddTab(string name, Enum identifier, LayoutWindow window)
        {
        }
    }
}
