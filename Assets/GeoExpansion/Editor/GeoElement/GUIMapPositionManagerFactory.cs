using System;
using System.Collections.Generic;

namespace uAdventure.Geo
{
    public class GuiMapPositionManagerFactory
    {
        private static GuiMapPositionManagerFactory instance;
        public static GuiMapPositionManagerFactory Instance
        {
            get
            {
                if (instance == null)
                    instance = new GuiMapPositionManagerFactory();
                return instance;
            }
        }
        private readonly List<IGuiMapPositionManager> guiMapPositionManagers;
        private GuiMapPositionManagerFactory()
        {
            guiMapPositionManagers = new List<IGuiMapPositionManager>
            {
                // Add guimap position managers here
                new GeolocationGuiMapPositionManager(),
                new ScreenGuiMapPositionManager(),
                new RadialGuiMapPositionManager()
            };
        }

        public IGuiMapPositionManager CreateInstance(ITransformManagerDescriptor element, TransformManagerDataControl transformManagerDataControl)
        {
            var elem = (IGuiMapPositionManager)Activator.CreateInstance(guiMapPositionManagers.Find(g => g.ForType == element.Type).GetType());
            elem.Configure(transformManagerDataControl);
            return elem;
        }
    }
}
