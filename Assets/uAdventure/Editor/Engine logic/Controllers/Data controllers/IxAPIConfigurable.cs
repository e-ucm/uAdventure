using System.Collections.Generic;

namespace uAdventure.Editor
{
    public interface IxAPIConfigurable
    {
        List<string> getxAPIValidTypes(string @class);
        List<string> getxAPIValidClasses();
        string getxAPIType();
        string getxAPIClass();
        void setxAPIType(string type);
        void setxAPIClass(string value);

    }
}