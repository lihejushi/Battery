using System;

namespace XT.MVC.Framework.EmbeddedViews
{
    [Serializable]
    public class EmbeddedViewMetadata
    {
        public string Name { get; set; }
        public string AssemblyFullName { get; set; }
    }
}