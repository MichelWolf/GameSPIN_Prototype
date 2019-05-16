using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UiScript : MonoBehaviour
{
	public GameObject hitmarkerO;
	public GameObject damageEffect;
	private bool hitmarkerOn=false;
	private bool damageEffectOn=false;
	
    // Start is called before the first frame update
    void Start()
    {

		 hitmarkerO.GetComponent<Image>().color = new Color(1,1,1,0);
		 damageEffect.GetComponent<Image>().color = new Color(1,1,1,0);
		float worldScreenHeight = Camera.main.orthographicSize * 2.0f;
		float worldScreenWidth = (worldScreenHeight / Screen.height) * Screen.width;
	//	damageEffect.GetComponent<RectTransform>().sizeDelta= new Vector2(1920, 1080);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	 IEnumerator FadeTo(float aValue, float aTime, GameObject obj)
	{
		float alpha = obj.GetComponent<Image>().color.a;
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
		{
			Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha,aValue,t));
			obj.GetComponent<Image>().color = newColor;
			yield return null;
		}
	}

	public void activateHitmarker(){
		 hitmarkerOn = true;
		 StartCoroutine(FadeTo(1f,.1f,hitmarkerO));
		 StartCoroutine(waitTurnDown(.2f, .1f, hitmarkerO));
		 hitmarkerOn = false;
	}
	public void activateDamageEffect(){
		 damageEffectOn = true;
		 StartCoroutine(FadeTo(1f,.4f,damageEffect));
	     StartCoroutine(waitTurnDown(.5f, 1f, damageEffect));
		 damageEffectOn = false;
		 //StartCoroutine(FadeTo(0f,5f,damageEffect));
	}
	
	IEnumerator waitTurnDown(float sec, float fadespeed, GameObject obj)
    {
        yield return new WaitForSeconds(sec);
		StartCoroutine(FadeTo(0f,fadespeed,obj));	
    }
}