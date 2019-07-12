using uAdventure.Core;
using UnityEngine;

namespace uAdventure.Runner
{
    public class ScenePositioner : MonoBehaviour, Movable
    {
        public static readonly Vector2 SceneElementPivot = new Vector2(0.5f, 0f);

        private float z = 0;
        private Renderer rend;
        private Representable representable;

        public ElementReference Context { get; set; }

        public Representable Representable
        {
            get { return representable; }
            set
            {
                if (representable != value)
                {
                    if (representable != null)
                    {
                        representable.RepresentableChanged -= Positionate;
                    }

                    representable = value;
                    representable.RepresentableChanged += Positionate;
                }
            }
        }

        public Vector2 Position
        {
            get
            {
                return new Vector2(Context.getX(), Context.getY());
            }

            set
            {
                if (Context == null)
                {
                    Debug.Log("No context was set!", this);
                    return;
                }

                Context.setPosition((int)value.x, (int)value.y);
                Positionate();
            }
        }

        protected virtual void Start()
        {
            rend = this.GetComponent<Renderer>() ?? this.GetComponentInChildren<Renderer>();
        }

        public SceneMB Scene { get; set; }

        public void Positionate()
        {
            if (!rend)
            {
                rend = this.GetComponent<Renderer>() ?? this.GetComponentInChildren<Renderer>();
            }

            if (!rend)
            {
                Debug.Log("No renderer was found!", this);
                return;
            }

            if (Scene == null)
            {
                Debug.Log("No scene was set!", this);
                return;
            }

            if (Representable == null)
            {
                Debug.Log("No representable was set!", this);
                return;
            }

            if (Context == null)
            {
                Debug.Log("No context was set!", this);
                return;
            }

            transform.localPosition = Scene.ToWorldPosition(Position, Representable.Size, SceneElementPivot, z);
        }

        public float Z
        {
            get { return z; }
            set
            {
                if (value != z)
                {
                    z = value;
                    this.Positionate();
                }
            }
        }

    }
}
