using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHideCube : MonoBehaviour {

    [SerializeField] ParticleSystem _Boom;
    [SerializeField] GameObject[] _Cube;

    [SerializeField] float _Time = 2;

    Color StartColor;
    Color FinishColor;
    Material _myMaterial;

    // Use this for initialization
    void Start () {

      //  _myMeshRenderer = this.GetComponent<MeshRenderer>();

      //  StartColor = _Cube.GetComponent<MeshRenderer>().material.color;
     //   FinishColor = new Color(StartColor.r, StartColor.g, StartColor.b, 0.5f);

      //  _myMaterial = _Cube.GetComponent<MeshRenderer>().material;
       
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartAnimationDestroy()
    {
        //_myMeshRenderer.
        //StartColor;



        StartCoroutine(AnimationDestroy());

    }

    IEnumerator AnimationDestroy()
    {
        foreach (var item in _Cube)
        {
            item.SetActive(true);
        }
        yield return new WaitForSeconds(1f);

        _Boom.Play();
        yield return new WaitForSeconds(0.2f);

         //   _myMaterial.color = StartColor;


        //  _Boom.Play();
        //while (timer < _Time)
        //{

        //    _myMaterial.color = Color.Lerp(StartColor, FinishColor, timer / _Time);

        //    //_myMeshRenderer.mat
        //    timer += Time.deltaTime;
        //    yield return null;

        //}

        foreach (var item in _Cube)
        {
            item.SetActive(false);
        }


        yield return new WaitForSeconds(1f);

        //for (int i = 0; i < _myMaterial.Length; i++)
        //{
        //    _myMaterial[i].color = new Color(FinishColor.r, FinishColor.g, FinishColor.b, 0);
        //}


        yield break;
    }
}
