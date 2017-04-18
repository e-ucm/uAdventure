using UnityEngine;
using System.Collections;

using uAdventure.Core;
using uAdventure.Editor;
using UnityEditor;
using System.Collections.Generic;

namespace uAdventure.Geo
{
    public class TriggerZonedSceneEffectEditor : TriggerSceneEffectEditor
    {
        private List<string> zones;

        public TriggerZonedSceneEffectEditor() : base()
        {
            this.zones = Controller.Instance.SelectedChapterDataControl.getObjects<GeoElement>().ConvertAll(g => g.getId());
            this.effect = subeffect = new TriggerZonedSceneEffect(effect.getTargetId(), "", effect.getX(), effect.getY());
        }
        protected TriggerZonedSceneEffect subeffect;
        public override void draw()
        {
            base.draw();

            subeffect.ZoneId = zones[EditorGUILayout.Popup(this.zones.IndexOf(subeffect.ZoneId), zones.ToArray())];
        }

        public override string EffectName
        {
            get
            {
                return "Trigger zoned scene";
            }
        }

        public override EffectEditor clone()
        {
            return new TriggerZonedSceneEffectEditor();
        }

        public override bool Usable
        {
            get
            {
                return zones.Count > 0;
            }
        }


    }
}