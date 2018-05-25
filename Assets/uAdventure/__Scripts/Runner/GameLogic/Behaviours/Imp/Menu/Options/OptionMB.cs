using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Runner
{
    public class OptionMB : MonoBehaviour
    {
        Transform button, line;

        private Vector2 finalPosition;

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void setAction(Action action, IActionReceiver actionReceiver = null)
        {
            var buttonMB = this.button.GetComponent<ButtonMB>();
            buttonMB.Action = action;
            buttonMB.Receiver = actionReceiver;
        }


        public void collapse()
        {

        }

        void OnMouseEnter()
        {
        }

        void OnMouseExit()
        {
        }

        bool highlight = false;
        public bool Highlight
        {
            set { this.highlight = value; }
            get { return this.highlight; }
        }

        //######################################################################
        //############################## MOVEMENT ##############################
        //######################################################################

        public float easing = 0.05f;
        public bool _____________________________;
        // fields set dynamically
        public float camZ; // The desired Z pos of the camera
        void Awake()
        {
            this.button = transform.Find("Button");
            this.line = transform.Find("Line");
            camZ = this.transform.position.z;
        }
        
        void FixedUpdate()
        {
            // If there is no poi, return to P:[0,0,0]
            Vector3 destination = finalPosition;
            Vector3 linedestination = finalPosition / 2f;
            
            destination = Vector3.Lerp(button.localPosition, destination, easing);
            destination.z = camZ;

            linedestination = Vector3.Lerp(line.localPosition, linedestination, easing);
            linedestination.z = camZ;

            if (destination.y > 0)
                line.eulerAngles = new Vector3(0f, 0f, 90 + Mathf.Acos(destination.normalized.x) * Mathf.Rad2Deg);
            else
                line.eulerAngles = new Vector3(0f, 0f, 90 - Mathf.Acos(destination.normalized.x) * Mathf.Rad2Deg);

            line.localScale = new Vector3(0.05f, destination.magnitude, 1);


            if (highlight)
                destination.z = camZ - 2;
            else
                destination.z = camZ - 1;

            button.localPosition = destination;
            line.localPosition = linedestination;
        }



        public void moveTo(Vector2 position)
        {
            this.finalPosition = position;

        }
    }
}