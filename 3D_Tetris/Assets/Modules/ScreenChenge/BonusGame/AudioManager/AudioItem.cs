using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[System.Serializable]
public class AudioContainer{
   // [Serialize]
    public AudioClip Audio;

    public AudioContainer( AudioClip A)
    {
        Audio = A;
    }
}

[ExecuteInEditMode]
public class AudioItem : MonoBehaviour {

    [SerializeField] AudioClip _CurrentAudio;
    [SerializeField] List<AudioContainer> _AudioList;
//    [SerializeField] StructPain My = new StructPain();


    private void Awake()
    {
        // _CurrentAudio.name;

        //   _AudioList.ToArray()
    //    _AudioList = FindObjectOfType<Singltone>().MyAudio.Select<AudioClip, AudioContainer>( (s,p) =>  new AudioContainer(s)).ToList() ;
        foreach (var item in _AudioList)
        {
            Debug.Log(item.Audio.name);
        }
   //     Debug.Log("Hello" + FindObjectOfType<Singltone>().name);
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
