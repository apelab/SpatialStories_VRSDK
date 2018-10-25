using System;

namespace SpatialStories
{
    /// <summary>
    /// Holds a guid and a name for objects that will be created
    /// with the spatial stories API such as Interactions and 
    /// Interactive Objects.
    /// </summary>
    public abstract class S_AbstractDefinition
    {
        public string GUID { get; private set; }
        public string Name { get; private set; }

        public S_AbstractDefinition(string _name)
        {
            GenerateGUID();
            Name = _name;
        }

        /// <summary>
        /// Creates a new GUID for the object
        /// </summary>
        internal void GenerateGUID()
        {
            GUID = Guid.NewGuid().ToString();
        }

        public void SetName(string _newName)
        {
            Name = _newName;
        }
    }
}