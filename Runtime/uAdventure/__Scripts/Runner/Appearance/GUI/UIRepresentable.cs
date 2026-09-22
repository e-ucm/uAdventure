using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using uAdventure.Core;
using UnityEngine;
using UnityEngine.UI;

namespace uAdventure.Runner
{
    public class UIRepresentable : MonoBehaviour
    {
        private static readonly string[] emptyTextures = { "EmptyImage", "EmptyIcon", "EmptyAnimation" };

        private ResourcesUni resource;
        private Image image;
        protected Sprite sprite;

        private Element element;
        public Element Element {
            get
            {
                return element;
            }
            set
            {
                element = value;
                if(element != null)
                {
                    this.gameObject.name = element.getId();
                }
            }
        }

        protected void CheckResources()
        {
            if (Element == null)
                return;

            resource = Element.getResources().Find(res => ConditionChecker.check(res.getConditions()));
        }

        protected bool SetTexture(string uri)
        {
            sprite = Game.Instance.ResourceManager.getSprite(resource.getAssetPath(uri));
            image.sprite = sprite;
            return sprite != null && !emptyTextures.Where(t => t == sprite.texture.name).Any();
        }


        protected virtual void Start()
        {
            if (!image)
            {
                image = GetComponent<Image>() ?? GetComponentInChildren<Image>();
            }
            CheckResources();
        }
    }
}