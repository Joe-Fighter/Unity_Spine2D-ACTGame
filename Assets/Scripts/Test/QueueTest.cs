using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class QueueTest : MonoBehaviour {
	
	public Text text;
	public PlayerController m_player;

	void Start() {	
	}
	public void onConfirmClicked() {
		int[] queue = new int[6];
		for(int i=0; i<6; i++) {
			queue[i] = int.Parse(text.text.Substring(i,1));
		}
		m_player.setComboQueue(queue);
	}
	
}
