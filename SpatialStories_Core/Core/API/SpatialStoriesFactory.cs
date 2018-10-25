using Gaze;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("SpatialStoriesAPI")]
namespace SpatialStories
{
    /// <summary>
    /// Creates and adds Unity Game Objects to the current scene using
    /// S_IODefinition data.
    /// </summary>
    public static class SpatialStoriesFactory
    {
        /// <summary>
        /// List of all interactions / Definitions wich dependencies hasn't been wire up
        /// </summary>
        private static List<KeyValuePair<GameObject, S_InteractionDefinition>> depenenciesToWire = new List<KeyValuePair<GameObject, S_InteractionDefinition>>();

        /// <summary>
        /// Creates a full interactive object including interactions, conditions... By using
        /// S_IODefinition as an argument
        /// NOTE :This method can be only be called from the SpatialStories API.
        /// </summary>
        /// <param name="_definition">An interactive object definition</param>
        /// <param name="_wireDependencies"> Do the dependencies will be wired now ? (set that to false if you have circular dependencies that need
        /// to be post processed when all the ios and interactions have been created) </param>
        /// <returns>An interactive object ready to use</returns>
        internal static Gaze_InteractiveObject CreateInteractiveObject(S_IODefinition _definition, bool _wireDependencies)
        {
            // Create the interactive object from the prefab
            GameObject ioGameObject = GameObject.Instantiate(Resources.Load("Interactive Object") as GameObject);

            // Add the GUID to the io in order to be able to identify it between platforms and executions
            ioGameObject.AddComponent<S_Guid>().SetGUID(_definition.GUID);

            // Begin the crafting process
            ioGameObject.name = _definition.Name + " (IO)";
            ioGameObject.transform.position = _definition.Position;
            ioGameObject.transform.rotation = _definition.Rotation;
            ioGameObject.transform.localScale = _definition.Scale;

            // Create the interactions once the IO is ready
            Gaze_InteractiveObject io = ioGameObject.GetComponent<Gaze_InteractiveObject>();

            // If some visuals where specified add them to the new game object.
            if(_definition.Visuals != null)
            {
                AddNestedVisuals(_definition.Visuals, io);
            }

            CreateInteractionsForGameObject(_definition, io, _wireDependencies);

            return io;
        }

        /// <summary>
        /// Connects all the io's dependencies that were pending and clears the list
        /// </summary>
        internal static void WirePendingDependencies()
        {
            foreach (KeyValuePair<GameObject, S_InteractionDefinition> dep in depenenciesToWire)
            {
                CreateDependenciesForInteraction(dep.Value, dep.Key);
            }
            depenenciesToWire.Clear();
        }

        /// <summary>
        /// Creates all the interactions specified in the IO definition and binds them into the 
        /// newly created Interactive Object
        /// </summary>
        /// <param name="_ioDef"> The definition of the IO in process of creation </param>
        /// <param name="_io"> The instance of the interactive object being created </param>
        /// <param name="_wireDependencies"> Do the dependencies will be wired now ? (set that to false if you have circular dependencies that need
        /// to be post processed when all the ios and interactions have been created) </param>
        private static void CreateInteractionsForGameObject(S_IODefinition _ioDef, Gaze_InteractiveObject _io, bool _wireDependencies)
        {
            // Get the base interaction in order to replicate it as many times as needed
            Gaze_Interaction modelInteraction = _io.GetComponentInChildren<Gaze_Interaction>();

            // Create all the interactions specified on the io definition
            int numInteractions = _ioDef.Interactions.Count;
            for (int i = 0; i < numInteractions; i++)
            {
                // Create a new copy of the model interation and parent it to the interactions of the io 
                GameObject interaction = GameObject.Instantiate(modelInteraction.gameObject, modelInteraction.transform.parent);

                // Get the interaction definition
                S_InteractionDefinition interDef = _ioDef.Interactions[i];

                // Add a GUID in order to identify in a unique way across all the apps and runtimes
                interaction.AddComponent<S_Guid>().SetGUID(interDef.GUID);

                // Set the interaction name
                interaction.name = interDef.Name;
                
                // Create the interaction conditions
                CreateConditionsForInteraction(interDef, interaction);

                // Create the interaction Actions
                CreateActionsForInteraction(interDef, interaction);
                
                // Add the Delay to the interaction
                if (interDef.Delay > 0)
                {
                    Gaze_Conditions cond = interaction.GetComponent<Gaze_Conditions>();
                    cond.delayed = true;
                    cond.delayDuration = interDef.Delay;
                }

                // Destroy the game object used to hold the monobehaivours before
                // the creation of the interaction
                interDef.DestroyComponentHolder();


                // Create the interaction dependencies if necessary
                if (_wireDependencies)
                {
                    CreateDependenciesForInteraction(interDef, interaction);
                }
                else
                {
                    depenenciesToWire.Add(new KeyValuePair<GameObject, S_InteractionDefinition>(interaction, interDef));
                }
            }
        }
        
        /// <summary>
        /// Creates all the specified conditions for the interaction being created
        /// </summary>
        /// <param name="_interDef">The interaction definition file</param>
        /// <param name="_interaction"> The interaction in process of creation </param>
        private static void CreateConditionsForInteraction(S_InteractionDefinition _interDef, GameObject _interaction)
        {
            int numConditions = _interDef.ConditionsAndDependencies.Count;
            for (int i = 0; i < numConditions; i++)
            {
                Gaze_AbstractCondition cond = _interDef.ConditionsAndDependencies[i];
                // Create all the conditions letting the dependencies being created in next stages
                if (!(cond is Gaze_Dependency))
                {
                    cond.SetupUsingApi(_interaction);
                }
            }
        }

        /// <summary>
        /// Creates all the dependencies for the interaction being created, we should consider
        /// exposing this method in order to allow the creation of circular dependencies, like that
        /// the user will be able to first, create all the ios, conditions, actions and then wire up
        /// all the dependencies without having to care about the order of creation of the ios.
        /// </summary>
        /// <param name="_interDef"> The definition of the interaciton being created </param>
        /// <param name="_interaction"> The interaction in process of creation </param>
        private static void CreateDependenciesForInteraction(S_InteractionDefinition _interDef, GameObject _interaction)
        {
            int numConditions = _interDef.ConditionsAndDependencies.Count;
            for (int i = 0; i < numConditions; i++)
            {
                Gaze_AbstractCondition cond = _interDef.ConditionsAndDependencies[i];
                // We need to make sure that the object is a dependency (a dependency is a condition as well)
                if (cond is Gaze_Dependency)
                {
                    cond.SetupUsingApi(_interaction);
                }
            }
        }

        /// <summary>
        /// Creates all the actions defined on the S_Interaction definitions
        /// </summary>
        /// <param name="_interDef"> Interaction definition object </param>
        /// <param name="_interaction"> Interaction </param>
        private static void CreateActionsForInteraction(S_InteractionDefinition _interDef, GameObject _interaction)
        {
            int numActions = _interDef.Actions.Count;
            for (int i = 0; i < numActions; i++)
            {
                // Get the temporary action
                Gaze_AbstractBehaviour tempAction = _interDef.Actions[i];

                // Create the final action using the temporary aciton type
                Component comp = _interaction.AddComponent(tempAction.GetType());

                // Cast to get the Gaze_AbstractBehaivour of the action
                Gaze_AbstractBehaviour finalAction = ((Gaze_AbstractBehaviour)comp);

                // Copy the temp action data to the final data
                finalAction.creationData = new List<object>(tempAction.creationData);

                // Setupt the final action
                finalAction.SetupUsingApi(_interaction);
            }
        }

        /// <summary>
        /// Sets the object given as a parameter as visuals of the io in process of creation and copies its transform
        /// if necessary.
        /// </summary>
        /// <param name="_visuals"> The object that will represent the visuals of the io </param>
        /// <param name="_io"> The io in process of creation</param>
        private static void AddNestedVisuals(GameObject _visuals, Gaze_InteractiveObject _io)
        {
            Gaze_InteractiveObjectVisuals visualsParent = _io.GetComponentInChildren<Gaze_InteractiveObjectVisuals>();
            _visuals.transform.SetParent(visualsParent.transform);
            _visuals.transform.localPosition = Vector3.zero;
            _visuals.transform.localRotation = Quaternion.identity;
        }


    }
}
