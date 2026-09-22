
using System.Collections.Generic;
using uAdventure.Runner;
using UnityFx.Async;

namespace uAdventure.Core
{
    public abstract class FileHandler<T>
    {
        protected T content;
        protected ResourceManager resourceManager;
        protected List<Incidence> incidences;
        protected XmlUpgrader.Upgrader upgrader;

        protected FileHandler(ResourceManager resourceManager, List<Incidence> incidences) : this(default(T), resourceManager, incidences)
        {
        }

        protected FileHandler(T content, ResourceManager resourceManager, List<Incidence> incidences)
        {
            this.content = object.Equals(content, default(T)) ? CreateObject() : content;
            this.resourceManager = resourceManager;
            this.incidences = incidences;
            this.upgrader = new XmlUpgrader.Upgrader(resourceManager, incidences);
        }


        protected abstract T CreateObject();

        public abstract T Parse(string path);
        public abstract IAsyncOperation<T> ParseAsync(string path);
    }
}
