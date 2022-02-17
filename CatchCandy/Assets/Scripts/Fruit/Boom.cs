using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boom : MonoBehaviour
{
    public ParticleSystem BoomParticle;

    private AudioSource audioSource;
    public AudioClip FireSound;
    public AudioClip HitGroundSound;
    public AudioClip HitCharacterSound;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource.loop = true;
        audioSource.clip = FireSound;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void PlayAudio(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Fruit")
        {
            audioSource.Pause();
            audioSource.loop = false;
            var boom = Instantiate(BoomParticle, transform.position, Quaternion.identity);

            if (collision.gameObject.tag == "Player")
            {
                PlayAudio(HitCharacterSound);
            }

            PlayAudio(HitGroundSound);


            GameObject.Destroy(GetComponent<Rigidbody>());
            GameObject.Destroy(GetComponent<MeshRenderer>());
            GameObject.Destroy(GetComponent<BoxCollider>());


            GameObject.Destroy(gameObject, 2.0f);
        }
    }

    IEnumerator StartBoom()
    {
        var boom = Instantiate(BoomParticle, transform.position, Quaternion.identity);
        this.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        GameObject.Destroy(boom);
        GameObject.Destroy(gameObject);
        yield return null;
    }
}
