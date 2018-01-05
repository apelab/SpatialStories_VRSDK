using System;
using UnityEngine;

namespace Gaze
{
    public class Gaze_HandIODetectorFeedback
    {
        public enum FeedbackModes { Default, Colliding }
        public FeedbackModes ActualFeedbackMode = FeedbackModes.Default;

        private Gaze_GrabManager grabManager;
        private Gaze_LaserEventArgs gaze_LaserEventArgs;
        private Gaze_HandIODetectorKernel detectorKernel;
        
        // This is used on the function ShowDistantGrabFeedbacks
        private SpriteRenderer intrctvDstntGrbFdbckSprRndrr;

        public Gaze_HandIODetectorFeedback(Gaze_GrabManager _owner)
        {
            grabManager = _owner;
            gaze_LaserEventArgs = new Gaze_LaserEventArgs();
            gaze_LaserEventArgs.Sender = _owner;
        }

        public void Setup()
        {
            detectorKernel = grabManager.IoDetectorKernel;
            intrctvDstntGrbFdbckSprRndrr = grabManager.InteractableDistantGrabFeedback.GetComponent<SpriteRenderer>();
        }

        public void Update()
        {
            // If it's the detector kernel who draws the line just don't do anything
            if (ActualFeedbackMode != FeedbackModes.Default)
                return;

            ShowDefaultRay();            
        }

        /// <summary>
        /// When the gaze hand detector kernel is not detecting anything the ray will be displayed by this method
        /// </summary>
        private void ShowDefaultRay()
        {
            if (grabManager.laserPointer == null)
                return;
            if(grabManager.InteractableDistantGrabFeedback.activeSelf)
                grabManager.InteractableDistantGrabFeedback.SetActive(false);
            Vector3 _targetPosition = grabManager.distantGrabOrigin.transform.forward;
            Vector3 endPosition = grabManager.transform.position + (grabManager.DefaultDistantGrabRayLength * _targetPosition);
            ShowDistantGrabLaser(grabManager.distantGrabOrigin.transform.position, endPosition, grabManager.distantGrabOrigin.transform.forward, false, false);
        }

        public void ShowDistantGrabFeedbacks(Vector3 _targetPosition, Vector3 _direction, float _length, bool _inRange)
        {
            if (grabManager.closerIO != null)
                _length = grabManager.closerDistance;

            grabManager.intersectsWithInteractiveIO = true;

            if (grabManager.laserPointer == null)
                return;

            // This will check if the raycast intersecs with a valid grabbable object
            if (Math.Abs(_length - float.MaxValue) < 0.01f)
            {
                _length = grabManager.DefaultDistantGrabRayLength;
                grabManager.intersectsWithInteractiveIO = false;
            }

            Vector3 endPosition = _targetPosition + (_length * _direction);

            ShowDistantGrabLaser(_targetPosition, endPosition, _direction, grabManager.intersectsWithInteractiveIO, _inRange);
            ShowDistantGrabPointer(grabManager.intersectsWithInteractiveIO, endPosition, _inRange);
        }

        private void ShowDistantGrabLaser(Vector3 _targetPosition, Vector3 _endPosition, Vector3 _direction, bool _intersectsWithIo, bool _iOInRange)
        {
            if (!grabManager.displayGrabPointer)
                return;

            //laserPointer.enabled = true;
            grabManager.laserPointer.SetPosition(0, _targetPosition);
            grabManager.laserPointer.SetPosition(1, _endPosition);
            Color actualColor;

            if (_iOInRange)
                actualColor = grabManager.InteractableInRangeDistantGrabColor;
            else if (_intersectsWithIo)
                actualColor = grabManager.InteractableDistantGrabColor;
            else
                actualColor = grabManager.NotInteractableDistantGrabColor;

            grabManager.laserPointer.startColor = actualColor;
            grabManager.laserPointer.endColor = actualColor;
            gaze_LaserEventArgs.StartPosition = _targetPosition;
            gaze_LaserEventArgs.EndPosition = _endPosition;
            gaze_LaserEventArgs.LaserHits = detectorKernel.Hits;
            Gaze_EventManager.FireLaserEvent(gaze_LaserEventArgs);
        }

        private void ShowDistantGrabPointer(bool _intersectsWithGrabbableIo, Vector3 _endPosition, bool _ioInRange)
        {
            // Add the object at the end of the ray if needed
            if (grabManager.NotInteractableDistantGrabFeedback != null)
            {
                // Disable or enable the object
                grabManager.NotInteractableDistantGrabFeedback.SetActive(!_intersectsWithGrabbableIo);

                // Move the feedback object to the end of the ray
                if (!_intersectsWithGrabbableIo)
                    grabManager.NotInteractableDistantGrabFeedback.transform.position = _endPosition;
            }

            // Add the object at the end of the ray if needed
            if (grabManager.InteractableDistantGrabFeedback != null)
            {
                // Disable or enable the object
                grabManager.InteractableDistantGrabFeedback.SetActive(_intersectsWithGrabbableIo);

                // Change the pointer color in order to distinguish if the object is in range or not.
                if (intrctvDstntGrbFdbckSprRndrr)
                    intrctvDstntGrbFdbckSprRndrr.color = _ioInRange ? grabManager.InteractableInRangeDistantGrabColor : grabManager.InteractableDistantGrabColor;

                // Move the feedback object to the end of the ray
                if (_intersectsWithGrabbableIo)
                    grabManager.InteractableDistantGrabFeedback.transform.position = _endPosition;
            }
        }
    }
}
