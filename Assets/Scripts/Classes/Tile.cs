using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Animator animator;

    private SpriteRenderer sr;
    // ========================================================== GET/ SET ==========================================================
    public Vector3 GetPosition() { return transform.position; }
    public SpriteRenderer GetSpriteRenderer() { return sr; }

    // ========================================================== UNITY FUNC ==========================================================
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}

    // ========================================================== PUBLIC FUNC ==========================================================
    public void PlayHighlightAnim(Ball.Type _type)
    {
        string animKey = "hl_" + _type.ToString().ToLower();
        animator.Play(animKey);
    }

    public void PlayMovingAnim(Ball.Type _type)
    {
        string animKey = "move_" + _type.ToString().ToLower();
        animator.Play(animKey);
    }
}
