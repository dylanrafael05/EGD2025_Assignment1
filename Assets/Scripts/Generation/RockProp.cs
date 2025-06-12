using UnityEngine;

public class RandomSpriteProp : SceneProp
{
    private SpriteRenderer[] sprites;

    new void Awake()
    {
        base.Awake();
        sprites = GetComponentsInChildren<SpriteRenderer>();
    }

    public override void OnPlace()
    {
        base.OnPlace();

        var idx = Random.Range(0, sprites.Length);

        for (int i = 0; i < sprites.Length; i++)
            sprites[i].gameObject.SetActive(i == idx);
    }
}
