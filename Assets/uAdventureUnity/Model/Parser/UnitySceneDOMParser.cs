using System.Xml;
using uAdventure.Core;

namespace uAdventure.Unity
{
    [DOMParser("unity-scene")]
    public class UnitySceneDOMParser : IDOMParser
    {
        public object DOMParse(XmlElement element, params object[] parameters)
        {
            var id = element.GetAttribute("id");
            var scene = element.GetAttribute("scene");
            var unityScene = new UnityScene
            {
                Id = id,
                Scene = scene
            };
            return unityScene;

        }
    }
}
