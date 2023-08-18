using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public AudioClip StartSE;
    public AudioClip ClearSE;
    public GameObject HeartEnemy;
    private GameObject heartObject;

    private bool isClearSEPlayed = false; // 再生フラグ
    void Start()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(StartSE);

        string heartObjectName = "Heart_enemy03";
        Transform heartTransform = HeartEnemy.transform.Find(heartObjectName);
        if (heartTransform != null)
        {
            heartObject = heartTransform.gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (heartObject != null && !heartObject.activeSelf && !isClearSEPlayed)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.PlayOneShot(ClearSE);
            isClearSEPlayed = true; // 再生フラグを立てる

        }
    }
}