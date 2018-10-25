using Gaze;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialStories
{
    /// <summary>
    /// Stores all the necessary information about an interaction such as
    /// name, dependencies, conditions and actions.
    /// </summary>
    public class S_InteractionDefinition : S_AbstractDefinition
    {
        public List<Gaze_AbstractCondition> ConditionsAndDependencies;
        public List<Gaze_AbstractBehaviour> Actions;
        public float Delay = 0;

        // As unity doesn't allow us to create new monobehaivous we need an intermediate object
        // to store all the components for the creation
        public GameObject ComponentHolder = null;

        public S_InteractionDefinition(string _name) : base(_name)
        {
            ConditionsAndDependencies = new List<Gaze_AbstractCondition>();
            Actions = new List<Gaze_AbstractBehaviour>();
        }
        
        /// <summary>
        /// Creates a new condition or dependency object for the interaction, then the user needs to
        /// populate it with the correct data.
        /// </summary>
        /// <typeparam name="T">An objects that inherits from Gaze_AbstractCondition and has an empty
        /// constructor (0 arguments)</typeparam>
        /// <returns> The specified condition / dependency ready to be configured </returns>
        public T CreateCondition<T>() where T : Gaze_AbstractCondition, new()
        {
            T cond = new T();
            ConditionsAndDependencies.Add(cond);
            return cond;
        }
        
        /// <summary>
        /// Creates a new condition or dependency object for the interaction and adds some initial data to it
        /// </summary>
        /// <typeparam name="T">An objects that inherits from Gaze_AbstractCondition and has an empty
        /// constructor (0 arguments)</typeparam>
        /// <param name="_data">Initial data for the condition / dependency </param>
        /// <returns> The specified condition / dependency ready to be configured </returns>
        public T CreateCondition<T>(params object[] _data) where T : Gaze_AbstractCondition, new()
        {
            T cond = new T();
            ConditionsAndDependencies.Add(cond);
            cond.AddCreationData(_data);
            return cond;
        }

        /// <summary>
        /// Creates an action ready with its initial data
        /// </summary>
        /// <typeparam name="T">A Gaze_AbstractBehaivour</typeparam>
        /// <param name="_data">Initial data for the action</param>
        /// <returns>An action ready to use</returns>
        public T CreateAction<T>(params object[] _data) where T : Gaze_AbstractBehaviour, new()
        {
            // If we don't have any component holder create one
            if (ComponentHolder == null)
                ComponentHolder = new GameObject("Holder");
            
            // Add a the action to the component holder
            T action = (T)ComponentHolder.AddComponent(typeof(T));
            Actions.Add(action);
            action.AddCreationData(_data);
            return action;
        }

        /// <summary>
        /// Creates an action ready to setup
        /// </summary>
        /// <typeparam name="T">A Gaze_AbstractBehaivour</typeparam>
        /// <returns>An action ready to setup</returns>
        public T CreateAction<T>() where T : Gaze_AbstractBehaviour, new()
        {
            // If we don't have any component holder create one
            if (ComponentHolder == null)
                ComponentHolder = new GameObject("Holder");

            // Add a the action to the component holder
            T action = (T)ComponentHolder.AddComponent(typeof(T));
            Actions.Add(action);
            return action;
        }

        /// <summary>
        /// Crates a new dependency for the interaction
        /// </summary>
        /// <param name="_dependentObjectGUID">The dependency interaction GUID</param>
        /// <returns>A fully configured Gaze_Dependency</returns>
        public Gaze_Dependency CreateDependency(string _dependentObjectGUID)
        {
            return CreateCondition<Gaze_Dependency>(_dependentObjectGUID);
        }

        /// <summary>
        /// Crates a new dependency for the interaction, ready to be configured
        /// </summary>
        /// <returns>A dependency to be configured</returns>
        public Gaze_Dependency CreateDependency()
        {
            return CreateCondition<Gaze_Dependency>();
        }

        /// <summary>
        /// Destroy the game object used to hold monobehaivours
        /// </summary>
        public void DestroyComponentHolder()
        {
            if (ComponentHolder != null)
                GameObject.DestroyImmediate(ComponentHolder);
        }

    }
}