using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using uAdventure.Editor;

namespace uAdventure.Test
{
    public class AdventureCreationTests
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var created = Controller.Instance.NewAdventure(Controller.FILE_ADVENTURE_1STPERSON_PLAYER);
            Assert.True(Controller.Instance.Initialized);
            Assert.True(Controller.Instance.Loaded);
            Assert.True(created);
        }


        [Test]
        public void CreateFirstPersonAdventure()
        {
            var created = Controller.Instance.NewAdventure(Controller.FILE_ADVENTURE_1STPERSON_PLAYER);
            Assert.True(created);

            var adventureData = Controller.Instance.AdventureData;
            Assert.IsNotNull(adventureData);
            Assert.IsNotNull(adventureData.getChapters());
            Assert.That(adventureData.getChapters(), Has.Count.EqualTo(1));

            var chapterDataControl = Controller.Instance.SelectedChapterDataControl;

            // Should have one scene
            Assert.IsNotNull(chapterDataControl);
            Assert.IsNotNull(chapterDataControl.getScenesList());
            Assert.IsNotNull(chapterDataControl.getScenesList().getScenes());
            Assert.That(chapterDataControl.getScenesList().getContent(), Has.Count.EqualTo(1));
            Assert.That(chapterDataControl.getScenesList().getScenes(), Has.Count.EqualTo(1));
            // Should have no cutscenes
            Assert.IsNotNull(chapterDataControl);
            Assert.IsNotNull(chapterDataControl.getCutscenesList());
            Assert.IsNotNull(chapterDataControl.getCutscenesList().getCutscenes());
            Assert.That(chapterDataControl.getCutscenesList().getContent(), Has.Count.EqualTo(0));
            Assert.That(chapterDataControl.getCutscenesList().getCutscenes(), Has.Count.EqualTo(0));
            // Should have no atrezzos
            Assert.IsNotNull(chapterDataControl);
            Assert.IsNotNull(chapterDataControl.getAtrezzoList());
            Assert.IsNotNull(chapterDataControl.getAtrezzoList().getAtrezzoList());
            Assert.That(chapterDataControl.getAtrezzoList().getContent(), Has.Count.EqualTo(0));
            Assert.That(chapterDataControl.getAtrezzoList().getAtrezzoList(), Has.Count.EqualTo(0));
            // Should have no characters
            Assert.IsNotNull(chapterDataControl);
            Assert.IsNotNull(chapterDataControl.getNPCsList());
            Assert.IsNotNull(chapterDataControl.getNPCsList().getNPCs());
            Assert.That(chapterDataControl.getNPCsList().getContent(), Has.Count.EqualTo(0));
            Assert.That(chapterDataControl.getNPCsList().getNPCs(), Has.Count.EqualTo(0));
            // Should have no items
            Assert.IsNotNull(chapterDataControl);
            Assert.IsNotNull(chapterDataControl.getItemsList());
            Assert.IsNotNull(chapterDataControl.getItemsList().getItems());
            Assert.That(chapterDataControl.getItemsList().getContent(), Has.Count.EqualTo(0));
            Assert.That(chapterDataControl.getItemsList().getItems(), Has.Count.EqualTo(0));

            var sceneDataControl = chapterDataControl.getScenesList().getScenes()[0];
            // Resources
            Assert.IsNotNull(sceneDataControl);
            Assert.IsNotNull(sceneDataControl.getResources());
            Assert.That(sceneDataControl.getResources(), Has.Count.EqualTo(1));
            // References
            Assert.IsNotNull(sceneDataControl);
            Assert.IsNotNull(sceneDataControl.getReferencesList());
            Assert.IsNotNull(sceneDataControl.getReferencesList().getRefferences());
            Assert.That(sceneDataControl.getReferencesList().getRefferences(), Has.Count.EqualTo(0));
            // Active areas
            Assert.IsNotNull(sceneDataControl);
            Assert.IsNotNull(sceneDataControl.getActiveAreasList());
            Assert.IsNotNull(sceneDataControl.getActiveAreasList().getActiveAreas());
            Assert.That(sceneDataControl.getActiveAreasList().getActiveAreas(), Has.Count.EqualTo(0));
            // Exits
            Assert.IsNotNull(sceneDataControl);
            Assert.IsNotNull(sceneDataControl.getExitsList());
            Assert.IsNotNull(sceneDataControl.getExitsList().getExits());
            Assert.That(sceneDataControl.getExitsList().getExits(), Has.Count.EqualTo(0));
        }

        [Test]
        public void LoadAdventure()
        {
            var loaded = Controller.Instance.LoadFile();
            Assert.True(loaded);

            var adventureData = Controller.Instance.AdventureData;
            Assert.IsNotNull(adventureData);

            var chapterDataControl = Controller.Instance.SelectedChapterDataControl;
            Assert.IsNotNull(chapterDataControl);
        }


        [Test]
        public void CreateScene()
        {
            var loaded = Controller.Instance.LoadFile();
            Assert.AreEqual(true, loaded);

            var chapterDataControl = Controller.Instance.SelectedChapterDataControl;
            var scenesList = chapterDataControl.getScenesList();

            var added = scenesList.addElement(Controller.SCENE, "scene");
            Assert.True(added);

            var sceneAddedIndex = scenesList.getSceneIndexByID("scene");
            Assert.AreEqual(1, sceneAddedIndex);

            var scene = scenesList.getScenes()[sceneAddedIndex];
            Assert.IsNotNull(scene);

            Controller.Instance.Save();

            loaded = Controller.Instance.LoadFile();
            Assert.AreEqual(true, loaded);

            var reloadedScene = Controller.Instance.SelectedChapterDataControl.getScenesList().getScenes()[sceneAddedIndex];
            Assert.AreEqual(scene.getId(), reloadedScene.getId());
        }


    }

}
