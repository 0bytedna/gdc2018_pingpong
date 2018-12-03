using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Ball : MonoBehaviourPunCallbacks, IPunObservable {
	[Tooltip("The local ball instance. Use this to know if the local ball is represented in the Scene")]
	public static GameObject localBall;

	Vector3 initPos = Vector3.zero;

    public static bool experimentStateSync = false;

	public void Awake() {
		// used in GameManager.cs: we keep track of the localPlayer instance to prevent instanciation when levels are synchronized
		if (photonView.IsMine) {
			localBall = gameObject;
		} else {
            if (!experimentStateSync) {
                // disable the physics simulation for the remote player.  Positions are synched for remote player.
                GetComponent<Rigidbody2D>().simulated = false;
                GetComponent<CircleCollider2D>().isTrigger = true;
            }
		}

		if (PhotonNetwork.IsMasterClient) {
			if (photonView.IsMine) {
				GetComponent<SpriteRenderer>().color = Color.red;
			} else {
				GetComponent<SpriteRenderer>().color = Color.green;
			}
		} else {
			if (!photonView.IsMine) {
				GetComponent<SpriteRenderer>().color = Color.green;
			} else {
				GetComponent<SpriteRenderer>().color = Color.red;
			}
		}

		// we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
		DontDestroyOnLoad(gameObject);
	}

	public void Start() {
		initPos = transform.position;
		if (photonView.IsMine || experimentStateSync) {
			Reset();
		}
	}

	public void Update() {
		Rigidbody2D rb = GetComponent<Rigidbody2D>();
		if (photonView.IsMine || experimentStateSync) {
            // reset the ball position if the velocity is very less.
			if (rb.velocity.magnitude <= 0.001f) {
				Reset();
			}
		}
	}

	void Reset() {
		if (photonView.IsMine || experimentStateSync) {
			transform.SetPositionAndRotation(initPos, Quaternion.identity);
			GetComponent<Rigidbody2D>().AddForce(Vector2.up * -2.0f + Vector2.left * 10.0f, ForceMode2D.Impulse);
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        // Not implemented.
	}
}
