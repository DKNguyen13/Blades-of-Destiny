using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [Header("Parallax")]
    [SerializeField] private float animationSpeed = 0.1f;
    private MeshRenderer m_Renderer;

    private void Awake()
    {
        m_Renderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        m_Renderer.material.mainTextureOffset += new Vector2(animationSpeed * Time.deltaTime, 0);
    }
}
