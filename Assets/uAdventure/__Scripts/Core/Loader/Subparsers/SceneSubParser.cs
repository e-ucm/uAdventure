using UnityEngine;
using System.Xml;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;

namespace uAdventure.Core
{
    [DOMParser("scene")]
	[DOMParser(typeof(Scene))]
	public class SceneSubParser : IDOMParser
    {
        /**
         * Stores the element being parsed
         */
        private Scene scene;

        /**
         * Stores the current exit being used
         */
        private Exit currentExit;

        /**
         * Stores the current exit look being used
         */
        private ExitLook currentExitLook;

        /**
         * Stores the current next-scene being used
         */
        private NextScene currentNextScene;

        private Vector2 currentPoint;

        /**
         * Stores the current element reference being used
         */
        private ElementReference currentElementReference;


		public object DOMParse(XmlElement element, params object[] parameters)
        {
			var chapter = parameters [0] as Chapter;

			string sceneId = element.GetAttribute("id");
			bool initialScene = ExString.EqualsDefault(element.GetAttribute("start"), "yes", false);
            bool hideInventory = ExString.EqualsDefault(element.GetAttribute("hideInventory"), "yes", false);
            bool allowsZoom = ExString.EqualsDefault(element.GetAttribute("allowsZoom"), "yes", false);
            bool allowsSavingGame = ExString.EqualsDefault(element.GetAttribute("allowsSavingGame"), "yes", true);
            int playerLayer = ExParsers.ParseDefault (element.GetAttribute("playerLayer"), -1);
			float playerScale = ExParsers.ParseDefault (element.GetAttribute("playerScale"), CultureInfo.InvariantCulture, 1.0f);
            playerScale = Mathf.Max(0, playerScale);

            scene = new Scene(sceneId)
            {
                HideInventory = hideInventory,
                AllowsZoom = allowsZoom
            };
            scene.setPlayerLayer(playerLayer);
            scene.setPlayerScale(playerScale);
            scene.setAllowsSavingGame(allowsSavingGame);

            if (initialScene)
            {
                chapter.setTargetId(sceneId);
            }

			var name = element.SelectSingleNode ("name");
			if (name != null)
            {
                scene.setName(name.InnerText);
            }

			var documentation = element.SelectSingleNode ("documentation");
			if (documentation != null)
            {
                scene.setDocumentation(documentation.InnerText);
            }

			//XAPI ELEMENTS
			scene.setXApiClass(ExString.Default(element.GetAttribute("class"), "accesible"));
            scene.setXApiType(ExString.Default(element.GetAttribute("type"), "area"));
            //END OF XAPI

			foreach(var res in DOMParserUtility.DOMParse <ResourcesUni> (element.SelectNodes("resources"), parameters))
				scene.addResources (res);

			var defaultsinitialsposition = element.SelectSingleNode ("default-initial-position") as XmlElement;
            if (defaultsinitialsposition != null)
            {
				int x = ExParsers.ParseDefault (defaultsinitialsposition.GetAttribute("x"), int.MinValue), 
					y = ExParsers.ParseDefault (defaultsinitialsposition.GetAttribute("y"), int.MinValue);

                scene.setDefaultPosition(x, y);
            }

			foreach (XmlElement el in  element.SelectNodes("exits/exit"))
			{
				int x 				= ExParsers.ParseDefault (el.GetAttribute("x"), 0), 
					y 				= ExParsers.ParseDefault (el.GetAttribute("y"), 0), 
					width 			= ExParsers.ParseDefault (el.GetAttribute("width"), 0), 
					height 			= ExParsers.ParseDefault (el.GetAttribute("height"), 0);

				bool rectangular 	= ExString.EqualsDefault(el.GetAttribute("rectangular"), "yes", true);
				bool hasInfluence 	= ExString.EqualsDefault(el.GetAttribute("hasInfluenceArea"), "yes", false);

				int influenceX 		= ExParsers.ParseDefault (el.GetAttribute("influenceX"), 0), 
					influenceY 		= ExParsers.ParseDefault (el.GetAttribute("influenceY"), 0), 
					influenceWidth 	= ExParsers.ParseDefault (el.GetAttribute("influenceWidth"), 0), 
					influenceHeight = ExParsers.ParseDefault (el.GetAttribute("influenceHeight"), 0);

				string idTarget 	= el.GetAttribute("idTarget");
				int destinyX 		= ExParsers.ParseDefault (el.GetAttribute ("destinyX"), int.MinValue), 
					destinyY 		= ExParsers.ParseDefault (el.GetAttribute ("destinyY"), int.MinValue);

                float destinyScale  = ExParsers.ParseDefault (el.GetAttribute ("destinyScale"), CultureInfo.InvariantCulture, float.MinValue);

				int transitionType 	= ExParsers.ParseDefault(el.GetAttribute("transitionType"), 0),
					transitionTime 	= ExParsers.ParseDefault(el.GetAttribute("transitionTime"), 0);
				bool notEffects     = ExString.EqualsDefault(el.GetAttribute("not-effects"), "yes", false);


                currentExit = new Exit(rectangular, x, y, width, height);
                currentExit.setNextSceneId(idTarget);
                currentExit.setDestinyX(destinyX);
                currentExit.setDestinyY(destinyY);
                currentExit.setDestinyScale(destinyScale);
                currentExit.setTransitionTime(transitionTime);
                currentExit.setTransitionType(transitionType);
                currentExit.setHasNotEffects(notEffects);

                if (hasInfluence)
                {
                    InfluenceArea influenceArea = new InfluenceArea(influenceX, influenceY, influenceWidth, influenceHeight);
                    currentExit.setInfluenceArea(influenceArea);
                }

				foreach (XmlElement ell in el.SelectNodes("exit-look"))
                {
                    currentExitLook = new ExitLook();
					string text = ell.GetAttribute("text");
					string cursorPath = ell.GetAttribute("cursor-path");
					string soundPath = ell.GetAttribute("sound-path");

                    currentExitLook.setCursorPath(cursorPath);
                    currentExitLook.setExitText(text);
					currentExitLook.setSoundPath(soundPath);
                    currentExit.setDefaultExitLook(currentExitLook);
                }

                if (el.SelectSingleNode("documentation") != null)
                {
                    currentExit.setDocumentation(el.SelectSingleNode("documentation").InnerText);
                }

				foreach (XmlElement ell in el.SelectNodes("point"))
				{
					currentPoint = new Vector2(
						ExParsers.ParseDefault (ell.GetAttribute("x"), 0), 
						ExParsers.ParseDefault (ell.GetAttribute("y"), 0));
                    currentExit.addPoint(currentPoint);
                }

				currentExit.setConditions(DOMParserUtility.DOMParse (el.SelectSingleNode("condition"), parameters) 	as Conditions ?? new Conditions());
				currentExit.setEffects(DOMParserUtility.DOMParse (el.SelectSingleNode("effect"), parameters) 		as Effects ?? new Effects());
				currentExit.setNotEffects(DOMParserUtility.DOMParse (el.SelectSingleNode("not-effect"), parameters) as Effects ?? new Effects());
				currentExit.setPostEffects(DOMParserUtility.DOMParse (el.SelectSingleNode("post-effect"), parameters) as Effects ?? new Effects());

                if (currentExit.getNextScenes().Count > 0)
                {
                    foreach (NextScene nextScene in currentExit.getNextScenes())
                    {
                        Exit exit = (Exit)currentExit;
                        exit.setNextScenes(new List<NextScene>());
                        exit.setDestinyX(nextScene.getPositionX());
                        exit.setDestinyY(nextScene.getPositionY());
                        exit.setEffects(nextScene.getEffects());
                        exit.setPostEffects(nextScene.getPostEffects());
                        if (exit.getDefaultExitLook() == null)
                        {
                            exit.setDefaultExitLook(nextScene.getExitLook());
                        }
                        else
                        {
                            if (nextScene.getExitLook() != null)
                            {
                                if (nextScene.getExitLook().getExitText() != null &&
                                    !nextScene.getExitLook().getExitText().Equals(""))
                                {
                                    exit.getDefaultExitLook().setExitText(nextScene.getExitLook().getExitText());
                                }
                                if (nextScene.getExitLook().getCursorPath() != null &&
                                    !nextScene.getExitLook().getCursorPath().Equals(""))
                                {
                                    exit.getDefaultExitLook().setCursorPath(nextScene.getExitLook().getCursorPath());
                                }
                            }
                        }
                        exit.setHasNotEffects(false);
                        exit.setConditions(nextScene.getConditions());
                        exit.setNextSceneId(nextScene.getTargetId());
                        scene.addExit(exit);
                    }
                }
                else
                {
                    scene.addExit(currentExit);
                }
            }


			foreach (XmlElement el in element.SelectNodes("next-scene"))
            {
				string idTarget = el.GetAttribute("idTarget");
				int x 				= ExParsers.ParseDefault (el.GetAttribute ("x"), int.MinValue), 
					y 				= ExParsers.ParseDefault (el.GetAttribute ("y"), int.MinValue),
					transitionType 	= ExParsers.ParseDefault (el.GetAttribute ("transitionType"), 0), 
					transitionTime 	= ExParsers.ParseDefault (el.GetAttribute ("transitionTime"), 0);

                currentNextScene = new NextScene(idTarget, x, y);
                currentNextScene.setTransitionType((TransitionType)transitionType);
                currentNextScene.setTransitionTime(transitionTime);

                currentNextScene.setExitLook(currentExitLook);

				currentNextScene.setConditions(DOMParserUtility.DOMParse (el.SelectSingleNode("condition"), parameters) as Conditions ?? new Conditions());
				currentNextScene.setEffects(DOMParserUtility.DOMParse (el.SelectSingleNode("effect"), parameters) 		as Effects ?? new Effects());
				currentNextScene.setPostEffects(DOMParserUtility.DOMParse (el.SelectSingleNode("post-effect"), parameters) as Effects ?? new Effects());
            }

			foreach (XmlElement el in element.SelectNodes("objects/object-ref"))
			{
				currentElementReference = parseElementReference (el, parameters);
                scene.addItemReference(currentElementReference);
            }

			foreach (XmlElement el in element.SelectNodes("characters/character-ref"))
            {
				currentElementReference = parseElementReference (el, parameters);
                scene.addCharacterReference(currentElementReference);
            }

			foreach (XmlElement el in element.SelectNodes("atrezzo/atrezzo-ref"))
			{
				currentElementReference = parseElementReference (el, parameters);
                scene.addAtrezzoReference(currentElementReference);
            }

			foreach(var activeArea in DOMParserUtility.DOMParse<ActiveArea>(element.SelectNodes("active-areas/active-area"), parameters).ToList())
            {
                scene.addActiveArea(activeArea);
            }
				
			foreach(var barrier in DOMParserUtility.DOMParse<Barrier>(element.SelectNodes("barriers/barrier"), parameters).ToList())
            {
                scene.addBarrier(barrier);
            }

			foreach(var trajectory in DOMParserUtility.DOMParse<Trajectory>(element.SelectNodes("trajectory"), parameters).ToList())
            {
                scene.setTrajectory(trajectory);
            }


            if (scene != null)
            {
                TrajectoryFixer.fixTrajectory(scene);
            }

			return scene;
        }

		private ElementReference parseElementReference(XmlElement element, params object[] parameters){
			string idTarget = element.GetAttribute("idTarget");	

			int x = ExParsers.ParseDefault (element.GetAttribute ("x"), 0), 
				y = ExParsers.ParseDefault (element.GetAttribute ("y"), 0);

            Orientation orientation = (Orientation) ExParsers.ParseDefault(element.GetAttribute("orientation"), 2);

            float scale = ExParsers.ParseDefault (element.GetAttribute("scale"), CultureInfo.InvariantCulture, 0f);
			int layer = ExParsers.ParseDefault (element.GetAttribute("layer"), -1);
            bool glow = ExString.Default(element.GetAttribute("glow"), "yes").Equals("yes", System.StringComparison.InvariantCultureIgnoreCase);

			int influenceX 		= ExParsers.ParseDefault (element.GetAttribute("influenceX"), 0), 
				influenceY 		= ExParsers.ParseDefault (element.GetAttribute("influenceY"), 0), 
				influenceWidth 	= ExParsers.ParseDefault (element.GetAttribute("influenceWidth"), 0), 
				influenceHeight = ExParsers.ParseDefault (element.GetAttribute("influenceHeight"), 0);

			bool hasInfluence = ExString.EqualsDefault(element.GetAttribute("hasInfluenceArea"), "yes", false);

			var currentElementReference = new ElementReference(idTarget, x, y, layer);
            currentElementReference.Orientation = orientation;
            currentElementReference.Glow = glow;
            if (hasInfluence)
			{
				InfluenceArea influenceArea = new InfluenceArea(influenceX, influenceY, influenceWidth, influenceHeight);
				currentElementReference.setInfluenceArea(influenceArea);
			}
			if (scale > 0.001 || scale < -0.001)
            {
                currentElementReference.Scale = scale;
            }

			if (element.SelectSingleNode("documentation") != null)
            {
                currentElementReference.setDocumentation(element.SelectSingleNode("documentation").InnerText);
            }

			currentElementReference.Conditions = DOMParserUtility.DOMParse (element.SelectSingleNode("condition"), parameters) as Conditions ?? new Conditions();

			return currentElementReference;
		}
    }
}