using UnityEngine;
using System.Collections.Generic;

namespace Gaze
{
    public delegate void SpatialStoriesEventHandler(string _nameEvent, params object[] _list);

    /******************************************
	 * 
	 * SpatialStoriesEventController
	 * 
	 * Class used to dispatch events through all the system
	 * 
	 * @author Esteban Gallardo
	 */
    public class SpatialStoriesEventController : MonoBehaviour
    {
        public event SpatialStoriesEventHandler SpatialStoriesEvent;

        // ----------------------------------------------
        // EVENTS
        // ----------------------------------------------	

        // ----------------------------------------------
        // SINGLETON
        // ----------------------------------------------	
        private static SpatialStoriesEventController _instance;

        public static SpatialStoriesEventController Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = GameObject.FindObjectOfType(typeof(SpatialStoriesEventController)) as SpatialStoriesEventController;
                    if (!_instance)
                    {
                        GameObject container = new GameObject();
                        DontDestroyOnLoad(container);
                        container.name = "SpatialStoriesEventController";
                        _instance = container.AddComponent(typeof(SpatialStoriesEventController)) as SpatialStoriesEventController;
                    }
                }
                return _instance;
            }
        }

        // ----------------------------------------------
        // PRIVATE MEMBERS
        // ----------------------------------------------
        private List<TimedSpatialStoriesEventData> listEvents = new List<TimedSpatialStoriesEventData>();

        // -------------------------------------------
        /* 
		 * Constructor
		 */
        private SpatialStoriesEventController()
        {
        }

        // -------------------------------------------
        /* 
		 * OnDestroy
		 */
        void OnDestroy()
        {
            Destroy();
        }

        // -------------------------------------------
        /* 
		 * Destroy
		 */
        public void Destroy()
        {
            if (_instance != null)
            {
                Destroy(_instance.gameObject);
                _instance = null;
            }
        }

        // -------------------------------------------
        /* 
		 * Will dispatch a basic system event
		 */
        public void DispatchBasicSystemEvent(string _nameEvent, params object[] _list)
        {
            if (SpatialStoriesEvent != null) SpatialStoriesEvent(_nameEvent, _list);
        }

        // -------------------------------------------
        /* 
		 * Will add a new delayed local event to the queue
		 */
        public void DelayBasicSystemEvent(string _nameEvent, float _time, params object[] _list)
        {
            listEvents.Add(new TimedSpatialStoriesEventData(_nameEvent, _time, _list));
        }

        // -------------------------------------------
        /* 
		 * Will dispatch a delayed basic system events
		 */
        public void ClearBasicSystemEvents(string _nameEvent = "")
        {
            if (_nameEvent.Length == 0)
            {
                for (int i = 0; i < listEvents.Count; i++)
                {
                    listEvents[i].Time = -1000;
                }
            }
            else
            {
                for (int i = 0; i < listEvents.Count; i++)
                {
                    TimedSpatialStoriesEventData eventData = listEvents[i];
                    if (eventData.NameEvent == _nameEvent)
                    {
                        eventData.Time = -1000;
                    }
                }
            }
        }

        // -------------------------------------------
        /* 
		 * Will process the queue of delayed events 
		 */
        void Update()
        {
            // DELAYED EVENTS
            for (int i = 0; i < listEvents.Count; i++)
            {
                TimedSpatialStoriesEventData eventData = listEvents[i];
                if (eventData.Time == -1000)
                {
                    eventData.Destroy();
                    listEvents.RemoveAt(i);
                    break;
                }
                else
                {
                    eventData.Time -= Time.deltaTime;
                    if (eventData.Time <= 0)
                    {
                        if ((eventData != null) && (SpatialStoriesEvent != null))
                        {
                            SpatialStoriesEvent(eventData.NameEvent, eventData.List);
                            eventData.Destroy();
                        }
                        listEvents.RemoveAt(i);
                        break;
                    }
                }
            }
        }
    }
}