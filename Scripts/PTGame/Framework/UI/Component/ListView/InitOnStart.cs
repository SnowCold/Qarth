using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PTGame.Framework.LoopScrollRect))]
[DisallowMultipleComponent]
public class InitOnStart : MonoBehaviour {
	void Start () {
        GetComponent<PTGame.Framework.LoopScrollRect>().RefillCells();
	}
}
