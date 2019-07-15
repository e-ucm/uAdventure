using uAdventure.Core;
using System.Collections.Generic;
using System;

namespace uAdventure.QR
{
    public class QRScannerEffect : AbstractEffect, HasExtraIds
    {
        public enum ListType { BlackList, WhiteList }

        public QRScannerEffect()
        {
            ValidIds = new List<string>();
        }

        public override EffectType getType()
        {
            return EffectType.CUSTOM_EFFECT;
        }

        public string[] getIds()
        {
            return ValidIds.ToArray();
        }

        public void setIds(string[] ids)
        {
            ValidIds.Clear();
            ValidIds.AddRange(ids);
        }

        public string ScannerMessage { get; set; }
        public List<string> ValidIds { get; set; }
        public ListType SelectionType { get; set; }
    }
}