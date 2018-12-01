using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Player : MonoBehaviourPunCallbacks, IPunObservable {
	[Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
	public static GameObject localPlayer;
	public GameObject myBall;

	public void Awake() {
		// used in GameManager.cs: we keep track of the localPlayer instance to prevent instanciation when levels are synchronized
		if (photonView.IsMine) {
			localPlayer = gameObject;
		}

		// we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
		DontDestroyOnLoad(gameObject);
	}

	void Update() {
		if (photonView.IsMine) {
			float speed = 10.0f;
			if (Input.GetKey(KeyCode.UpArrow)) {
				// check if stricker reached the top.
				if (transform.position.y < 3.0f) {
					transform.position += Vector3.up * speed * Time.deltaTime;
				} else {
					// restrict the stricker at top.
					transform.position = new Vector3(transform.position.x, 3.0f, 0.0f);
				}
			}
			if (Input.GetKey(KeyCode.DownArrow)) {
				transform.position += Vector3.up * -speed * Time.deltaTime;
				// check if stricker reached the bottom.
				if (transform.position.y > -3.0f) {
					transform.position += Vector3.up * -speed * Time.deltaTime;
				} else {
					// restrict the stricker at bottom.
					transform.position = new Vector3(transform.position.x, -3.0f, 0.0f);
				}
			}
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		// Not implemented.
	}

	void OnCollisionEnter2D(Collision2D collision) {
        // if local ball is colliding with stricker (mine) then apply an impulse.
        if (collision.gameObject.tag == "ball" && (photonView.IsMine || Ball.experimentStateSync)) {
			if (collision.contacts.Length > 0) {
				Vector2 collisionNormal = Vector2.zero;
				collisionNormal = -collision.contacts[0].normal;
				collision.rigidbody.AddForce(collisionNormal * 6.0f, ForceMode2D.Impulse);
			}
		}
	}
}
