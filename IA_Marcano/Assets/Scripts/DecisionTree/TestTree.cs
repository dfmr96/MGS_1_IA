using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTree : MonoBehaviour
{
    public int bullets;
    public bool enemySpotted;
    public int life;
    ITreeNode _root;
    void Start()
    {
        InitializedTree();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            _root.Execute();
        }
    }
    void InitializedTree()
    {
        //Actions
        var shoot = new ActionTree(Shoot);
        var reload = new ActionTree(() =>
        {
            print("Reload");
            bullets++;
        });
        var patrol = new ActionTree(() => print("Patrol"));
        var dead = new ActionTree(() => print("Dead"));

        //Question
        //var qHasBullet = new QuestionTree(() => bullets > 0, )
        var qHasBullet = new QuestionTree(HasABullet, shoot, reload);
        var qAmmonInClip = new QuestionTree(HasABullet, patrol, reload);
        var qEnemyInView = new QuestionTree(() => enemySpotted, qHasBullet, qAmmonInClip);
        var qHasLife = new QuestionTree(() => life > 0, qEnemyInView, dead);

        _root = qHasLife;
    }
    // ()=>()
    void Shoot()
    {
        print("Shoot");
        bullets--;
    }
    bool HasABullet()
    {
        return bullets > 0;
    }
}
