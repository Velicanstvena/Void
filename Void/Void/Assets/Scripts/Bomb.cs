using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public bool isPlaced = false;

    private Vector2 overlapBoxSize1 = new Vector2(6f, 1f);
    private Vector2 overlapBoxSize2 = new Vector2(1f, 11f);
    private Collider2D[] colliders;

    private void Update()
    {
        if (isPlaced)
        {
            StartCoroutine(ActivateBomb());
        }
    }

    private void OnDisable()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        isPlaced = false;
    }

    IEnumerator ActivateBomb()
    {
        yield return new WaitForSeconds(2f);
        this.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(2f);
        BombExplosion();
        ObjectPooler.Instance.ReturnToPool(this.gameObject.name.Replace("(Clone)", ""), this.gameObject);
        this.gameObject.SetActive(false);
    }

    public void BombExplosion()
    {
        FindAffectedPlatforms(overlapBoxSize1);
        FindAffectedPlatforms(overlapBoxSize2);
    }

    private void FindAffectedPlatforms(Vector2 boxSize)
    {
        colliders = Physics2D.OverlapBoxAll(this.gameObject.transform.position, boxSize, 0f);

        if (colliders.Length > 0)
        {
            foreach (Collider2D col in colliders)
            {
                if (col.tag == "Player")
                {
                    if (PlayerMovement.LocalPlayerInstance == col.gameObject)
                    {
                        FindObjectOfType<GameController>().Die();
                    }
                }
            }
        }
    }
}
