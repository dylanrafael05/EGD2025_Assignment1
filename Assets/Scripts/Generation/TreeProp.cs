using UnityEngine;

public class TreeProp : SceneProp
{
    public GameObject deadTreeSpriteContainer;
    public GameObject coniferTreeSpriteContainer;

    private SpriteRenderer[] deadTreeSprites;
    private SpriteRenderer[] coniferTreeSprites;

    new void Awake()
    {
        base.Awake();

        deadTreeSprites = deadTreeSpriteContainer.GetComponentsInChildren<SpriteRenderer>();
        coniferTreeSprites = coniferTreeSpriteContainer.GetComponentsInChildren<SpriteRenderer>();
    }

    public override void OnPlace()
    {
        base.OnPlace();
        var kind = GeneratorManager.Instance.CalcForestKind(Chunk.ID, transform.position.tofloat3().xz);

        SpriteRenderer[] active, inactive;

        // (kind)% chance of being a conifer
        if (Random.value < kind)
        {
            active = coniferTreeSprites;
            inactive = deadTreeSprites;
        }
        else
        {
            active = deadTreeSprites;
            inactive = coniferTreeSprites;
        }

        var idx = Random.Range(0, active.Length);

        for (int i = 0; i < active.Length; i++)
            active[i].gameObject.SetActive(i == idx);

        for (int i = 0; i < inactive.Length; i++)
            inactive[i].gameObject.SetActive(false);
    }

    public void ChopDown()
    {
        gameObject.SetActive(false);
        GeneratorManager.Instance.KillForestAt(transform.position.tofloat3().xz);
    }
}
