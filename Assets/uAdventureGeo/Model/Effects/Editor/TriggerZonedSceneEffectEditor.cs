using UnityEngine;
using uAdventure.Editor;
using UnityEditor;
using System;
using uAdventure.Core;

namespace uAdventure.Geo
{
    public class TriggerZonedSceneEffectEditor : TriggerSceneEffectEditor
    {
        protected TriggerZonedSceneEffect triggerZonedSceneEffect;

        public TriggerZonedSceneEffectEditor() : base()
        {
            this.effect = triggerZonedSceneEffect = new TriggerZonedSceneEffect(effect.getTargetId(), "", effect.getX(), effect.getY());
        }

        public override void draw()
        {
            base.draw();
            var geoElementsIds = Controller.Instance.IdentifierSummary.getIds<GeoElement>();
            var selectedZoneIndex = Mathf.Max(0, Array.IndexOf(geoElementsIds, triggerZonedSceneEffect.ZoneId));

            if(Usable)
            {
                EditorGUI.BeginChangeCheck();
                var newZoneId = geoElementsIds[EditorGUILayout.Popup(selectedZoneIndex, geoElementsIds)];
                if (EditorGUI.EndChangeCheck())
                {
                    Controller.Instance.AddTool(new ChangeValueTool<TriggerZonedSceneEffect, string>(triggerZonedSceneEffect, 
                        newZoneId, "ZoneId"));
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Geo.TriggetZonedSceneEffectEditor.NoZones".Traslate(), MessageType.Error);
            }
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
                var geoElementsIds = Controller.Instance.IdentifierSummary.getIds<GeoElement>();
                return geoElementsIds != null && geoElementsIds.Length > 0;
            }
        }


    }
}