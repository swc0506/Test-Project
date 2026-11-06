using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GeUIEffectPlayer : MonoBehaviour
{
    GeUIEffectParticle[] m_UIParticles = null;

    public GeUIEffectParticle[] UIParticles
    {
        get { return m_UIParticles; }
    }

    // Use this for initialization
    void Start ()
    {
        m_UIParticles = gameObject.GetComponentsInChildren<GeUIEffectParticle>();
    }
	
	// Update is called once per frame
	void Update ()
    {
	}

    void OnEnable()
    {
        m_UIParticles = gameObject.GetComponentsInChildren<GeUIEffectParticle>();
    }

    public void Reinit()
    {
        m_UIParticles = gameObject.GetComponentsInChildren<GeUIEffectParticle>();
    }

}
