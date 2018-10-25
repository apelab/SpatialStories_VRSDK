using System.Collections.Generic;
using UnityEngine;

namespace SpatialStories
{
    /// <summary>
    /// Stores all data necesary for the creation of an Interactive Object
    /// </summary>
    public class S_IODefinition : S_AbstractDefinition
    {
        public Vector3 Position { get; private set; }
        public Quaternion Rotation { get; private set; }
        public Vector3 Scale { get; private set; }
        public GameObject Visuals { get; private set; }

        public List<S_InteractionDefinition> Interactions { get; private set; }

        /// <summary>
        /// Creates an io definition using a game object as basis, the _visuals GameObject
        /// will be nested under the visuals game object after the io creation
        /// </summary>
        /// <param name="_visuals"> Object that will be used as visuals of the new Interactive Object </param>
        /// <param name="_usePositionAndRotation"> If true the created interactive object will use the position
        /// of the _visuals object as the starting position</param>
        public S_IODefinition(GameObject _visuals, bool _usePositionAndRotation) : base(_visuals.name)
        {
            SetVisuals(_visuals, _usePositionAndRotation);
            Interactions = new List<S_InteractionDefinition>();
        }

        /// <summary>
        /// Create an Interactive Object definition object.
        /// </summary>
        /// <param name="_name"> The new object's name </param>
        public S_IODefinition(string _name) : base(_name)
        {
            Interactions = new List<S_InteractionDefinition>();
        }

        /// <summary>
        /// Defines where the new interactive object will be place
        /// </summary>
        /// <param name="_newPosition">Desired position of the game object in world coordinates</param>
        /// <returns>A reference to the same io definition. (usefull for concatenate method calls) </returns>
        public S_IODefinition SetPosition(Vector3 _newPosition)
        {
            Position = _newPosition;
            return this;
        }

        /// <summary>
        /// Defines the initial rotation of the new interactive object
        /// </summary>
        /// <param name="_rotation"> Desired Rotation </param>
        /// <returns>A reference to the same io definition. (usefull for concatenate method calls) </returns>
        public S_IODefinition SetRotation(Vector3 _rotation)
        {
            Rotation = Quaternion.Euler(_rotation);
            return this;
        }

        /// <summary>
        /// Defines the initial rotation of the new interactive object
        /// </summary>
        /// <param name="_rotation"> Desired Rotation </param>
        /// <returns>A reference to the same io definition. (usefull for concatenate method calls) </returns>
        public S_IODefinition SetRotation(Quaternion _rotation)
        {
            Rotation = _rotation;
            return this;
        }

        /// <summary>
        /// Defines the initial scale of the new interactive object
        /// </summary>
        /// <param name="_rotation"> Desired Scale </param>
        /// <returns>A reference to the same io definition. (usefull for concatenate method calls) </returns>
        public S_IODefinition SetScale(Vector3 _newScale)
        {
            Scale = _newScale;
            return this;
        }

        /// <summary>
        /// Sets the game object that will represent the visuals of the new interactive 
        /// object.
        /// </summary>
        /// <param name="_visuals">The visuals game object</param>
        /// <param name="_usePositionAndRotation"> If true the created interactive object will use the position
        /// of the _visuals object as the starting position </param>
        /// <returns>A reference to the same io definition. (usefull for concatenate method calls) </returns>
        public S_IODefinition SetVisuals(GameObject _visuals, bool _usePositionAndRotation = true)
        {
            Visuals = _visuals;
            if (_usePositionAndRotation)
            {
                Position = _visuals.transform.position;
                Rotation = _visuals.transform.rotation;
            }
            return this;
        }

        /// <summary>
        /// Create a new interaction definition for the game object, used to add new interactions
        /// </summary>
        /// <param name="_name"> Name for the new interaction </param>
        /// <returns>An interaction definition, that can be modified in order to create dependencies,
        /// conditions and action. </returns>
        public S_InteractionDefinition CreateInteractionDefinition(string _name)
        {
            S_InteractionDefinition definition = new S_InteractionDefinition(_name);
            Interactions.Add(definition);
            return definition;
        }
    }
}
