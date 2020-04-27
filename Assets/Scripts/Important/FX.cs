using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PoolTypes;

public class FX : MonoBehaviour {

    public bool isCallFromPool;
    FxType fxType;
    ObjectPooler objectPooler;
    Transform parent;
    public bool IsCallFromPool { get => isCallFromPool; set => isCallFromPool = value; }
    public FxType FXType { get => fxType; set => fxType = value; }
    public Transform Parent { get => parent; set => parent = value; }

    ParticleSystem ps;
    AudioSource m_audio;
    void Awake(){
		ps = GetComponent<ParticleSystem>();		// On récupère le composant ParticleSystem et on le "met" dans une variable (ps) de type ParticleSystem 
		m_audio = GetComponent<AudioSource>();		// De même pour l'AudioSource
        objectPooler = ObjectPooler.Instance;

        if (!IsCallFromPool)
        {
		    if( (m_audio != null) && (ps != null) ){					// Si le FX possède du son et un effet de particule alors : 

			    if(m_audio.clip.length >= ps.main.duration){			// Si la duré du son est plus longue ou égale à la duré de l'effet de particule alors :
				    Destroy(this.gameObject, m_audio.clip.length);	// On détruit le FX lorsque l'audio est fini

			    }else if(ps.main.duration >= m_audio.clip.length){	// Sinon si la duré de l'effet de particule est plus longue ou égale à la duré du son alors :
				    if(!ps.main.loop){
					    Destroy(this.gameObject, ps.main.duration);		// On détruit le FX lorsque l'effet de particule se termine
				    }
			    }

		    }else{													// S'il manque ou en effet de particule ou du son alors :

			    if(m_audio != null){									// Si le FX possède un "AudioSource" alors :
				    Destroy(this.gameObject, m_audio.clip.length);	// On détruit le FX à la fin du son 
			    }
			    if(ps != null){	
				    if(!ps.main.loop){									// Si le FX possède un "ParticleSystem" alors :
					    Destroy(this.gameObject, ps.main.duration);		// On détruit le gameObject à la fin de l'effet de particule
				    }
			    }
		    }
        }
        
	}
    private void OnEnable()
    {
        if(IsCallFromPool)
        {
            if ((m_audio != null) && (ps != null) && m_audio.clip != null)
            {
                if (m_audio.clip.length >= ps.main.duration)
                {
                    StartCoroutine(ReturnToPoolAfterATime(m_audio.clip.length));
                }
                else if (ps.main.duration >= m_audio.clip.length)
                {
                    if (!ps.main.loop)
                    {
                        StartCoroutine(ReturnToPoolAfterATime(ps.main.duration));

                    }
                }
            }
            else
            {
                if (m_audio != null && m_audio.clip != null)
                {
                    if(m_audio.clip != null)
                    {
                        StartCoroutine(ReturnToPoolAfterATime(m_audio.clip.length));
                    }
                }
                if (ps != null)
                {
                    if (!ps.main.loop)
                    {
                        StartCoroutine(ReturnToPoolAfterATime(ps.main.duration));

                    }
                }
            }
        }
    }
    private void Update()
    {
        if (IsCallFromPool && parent != null)
        {
            transform.position = parent.position;
        }
    }

    IEnumerator ReturnToPoolAfterATime(float time)
    {
        yield return new WaitForSeconds(time);
        objectPooler.ReturnFXToPool(FXType, this.gameObject);
    }
}
