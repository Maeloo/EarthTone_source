using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {

	public Camera		CameraUI;
	public GameObject	Anchor;
	public GameObject	Head;
	public GameObject	Body;
	public float		ScaleFactor = 1f;

	private GameObject	mTargetedCard;
	public	ACreature	targetedCard {
		get { return mTargetedCard ? mTargetedCard.GetComponentInParent<ACreature> ( ) : null; }
	}

	void OnEnable ( ) {
		updateArrow ( );
		
		Screen.showCursor = false;
	}

	void OnDisable ( ) {
		Screen.showCursor = true;
	}

	// On enter frame
	void Update () {
		updateArrow ( );
	}

	void OnTriggerStay ( Collider collider ) {
		Debug.Log ( collider.transform.tag  );
		if ( collider.transform.tag == "Card" || collider.transform.tag == "Heros" ) {
			if ( mTargetedCard == null ) {
				mTargetedCard = collider.gameObject;
			}
		}
	}

	void OnCollisionStay ( Collision collision ) {
		Debug.Log ( collision.collider.transform.tag );
	}

	void OnTriggerExit ( Collider collider ) {
		if ( collider.gameObject == mTargetedCard ) {
			mTargetedCard = null;
		}			
	}

	void updateArrow ( ) {
		Vector3 mousePointer = CameraUI.ScreenToWorldPoint ( Input.mousePosition );

		Anchor.transform.LookAt ( mousePointer );

		mousePointer.z = 0;
		transform.position = mousePointer;

		Vector3 bodyScale = Body.transform.localScale;
		bodyScale.y = Vector3.Distance ( Anchor.transform.position, mousePointer ) * ScaleFactor;

		Body.transform.localScale = bodyScale;
	}

	public void clearTarget ( ) {
		mTargetedCard = null;
	}
}
