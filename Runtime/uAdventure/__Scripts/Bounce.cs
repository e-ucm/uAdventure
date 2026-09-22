using UnityEngine;
using System.Collections;

public class Bounce : MonoBehaviour {

    public RectTransform into;
    private bool backwards = false;
    public float speed = 1f;

    private RectTransform m_transform;

    void Start()
    {
        m_transform = transform as RectTransform;
    }
	
	// Update is called once per frame
	void Update () {

        m_transform.anchoredPosition = m_transform.anchoredPosition + new Vector2(0, into.rect.height * speed * Time.deltaTime * (backwards ? 1 : -1));
        if(Mathf.Abs(m_transform.anchoredPosition.y) > into.rect.height / 2f)
        {
            m_transform.anchoredPosition = new Vector2(m_transform.anchoredPosition.x, Mathf.Clamp(m_transform.anchoredPosition.y, -into.rect.height / 2, into.rect.height / 2f));
            backwards = !backwards;
        }
	}
}
