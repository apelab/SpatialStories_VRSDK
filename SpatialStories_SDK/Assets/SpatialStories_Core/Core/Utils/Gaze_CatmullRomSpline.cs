//-----------------------------------------------------------------------
// <copyright file="LevitationEventArgs.cs" company="apelab sàrl">
// © apelab. All Rights Reserved.
//
// This source is subject to the apelab license.
// All other rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// <author>Michaël Martin</author>
// <email>dev@apelab.ch</email>
// <web>https://twitter.com/apelab_ch</web>
// <web>http://www.apelab.ch</web>
// <date>2016-01-25</date>
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

namespace Gaze
{
    public static class Gaze_CatmullRomSpline
    {
        // distance between each point created between control points
        private static float stepsDistance = .1f;

        public static float StepDistance { set { stepsDistance = value; } }

        // has to be at least 4 control points
        private static List<Vector3> controlPoints = new List<Vector3>();

        // all the points for the spline curve
        private static Vector3[] splinePoints;

        public static Vector3[] SplinePoints
        {
            set
            {
                splinePoints = new Vector3[value.Length];
                splinePoints = value;
            }
        }

        private static float splinePointsIndex = 0;

        public static Vector3[] getSplinePoints(Vector3[] _controlPoints)
        {
            controlPoints.Clear();
            controlPoints.AddRange(_controlPoints);

            //Draw the Catmull-Rom lines between the points
            //Cant draw between the endpoints
            //Neither do we need to draw from the second to the last endpoint
            for (int i = 1; i < controlPoints.Count - 2; i++)
            {
                createSplineAtPosition(i);
            }

            return splinePoints;
        }

        //Display a spline between 2 points derived with the Catmull-Rom spline algorithm
        private static void createSplineAtPosition(int pos)
        {
            //Clamp to allow looping
            Vector3 p0 = controlPoints[ClampListPos(pos - 1)];
            Vector3 p1 = controlPoints[pos];
            Vector3 p2 = controlPoints[ClampListPos(pos + 1)];
            Vector3 p3 = controlPoints[ClampListPos(pos + 2)];


            //Just assign a tmp value to this
            Vector3 lastPos = Vector3.zero;

            //t is always between 0 and 1 and determines the resolution of the spline
            //0 is always at p1
            for (float t = 0f; t < 1f; t += stepsDistance)
            {
                //Find the coordinates between the control points with a Catmull-Rom spline
                Vector3 newPos = ReturnCatmullRom(t, p0, p1, p2, p3);

                // add the new position to the list of spline points
                splinePointsIndex = ((pos - 1) * 10f) + (t * 10f);
                splinePoints[(int)splinePointsIndex] = newPos;

                lastPos = newPos;
            }
        }

        //Returns a position between 4 Vector3 with Catmull-Rom Spline algorithm
        //http://www.iquilezles.org/www/articles/minispline/minispline.htm
        private static Vector3 ReturnCatmullRom(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            Vector3 a = 0.5f * (2f * p1);
            Vector3 b = 0.5f * (p2 - p0);
            Vector3 c = 0.5f * (2f * p0 - 5f * p1 + 4f * p2 - p3);
            Vector3 d = 0.5f * (-p0 + 3f * p1 - 3f * p2 + p3);

            Vector3 pos = a + (b * t) + (c * t * t) + (d * t * t * t);

            return pos;
        }

        //Clamp the list positions to allow looping
        //start over again when reaching the end or beginning
        private static int ClampListPos(int pos)
        {
            if (pos < 0)
            {
                pos = controlPoints.Count - 1;
            }

            if (pos > controlPoints.Count)
            {
                pos = 1;
            }
            else if (pos > controlPoints.Count - 1)
            {
                pos = 0;
            }

            return pos;
        }
    }
}