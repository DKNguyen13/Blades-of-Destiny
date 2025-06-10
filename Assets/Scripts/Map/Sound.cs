using UnityEngine;

public class Sound : MonoBehaviour
{
    [SerializeField] private AudioClip[] m_Clip;
    private AudioSource m_Source;
    // Start is called before the first frame update
    void Start()
    {
        m_Source = GetComponent<AudioSource>();
        m_Source.clip = m_Clip[Random.Range(0, m_Clip.Length-1)];
        m_Source.Play();
    }

    public void SoundWin()
    {
        m_Source.loop = false;
        m_Source.ignoreListenerPause = true;
        m_Source.clip = m_Clip[m_Clip.Length-1];
        m_Source.Play();

    }
}
