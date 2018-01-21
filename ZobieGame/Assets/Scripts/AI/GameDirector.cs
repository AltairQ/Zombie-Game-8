using System.Collections;
using System.Collections.Generic;


public class GameDirector{

    // Bundle of info to be stored in the GD's database
    class ActorInfo
    {
        public Genes genes;
        public Memes memes;
        public float att_score;
        public float dist_score;

        public ActorInfo(Genes g, Memes m)
        {
            this.genes = g;
            this.memes = m;
        }

    }

    // Internal storage
    private Dictionary<int, ActorInfo> _database = new Dictionary<int, ActorInfo>();

    private int _max_candidates;
    private List<int> _candidates = new List<int>();

    // Last assigned Id
    private int _lastId = 0;

    // Game tick count
    private int _tick_count = 0;

    private ActorInfo InfoFromActor(IAIState actor)
    {
        return _database[actor.GetID()];
    }

    // Compute actions for an individual enemy
    public void Animate<T>(T actor)
        where T : IAIActions, IAIState
    {
        if (!actor.IsAlive())
            return;

        // if (actor.EuclidDistanceToPlayer() < InfoFromActor(actor).genes.G_melee_range)
        if (actor.EuclidDistanceToPlayer() < 2.0f)
            actor.AttackMeleePlayer();

        actor.GoToPlayer();
    }


    public Genes NewEnemy()
    {
        _candidates.Sort(
            (a, b) => {
                float s_a = _database[a].att_score * 10 + _database[a].dist_score;
                float s_b = _database[b].att_score * 10 + _database[b].dist_score;

                return s_b.CompareTo(s_a);
            });

        _lastId++;

        Genes newgenes = new Genes
        {
            Id = _lastId,
            G_health = 100.0f,
            G_melee_range = 2.0f,
            G_speed = 1.0f,
            G_strength = 10.0f,
            G_armor = 0.0f
        };

        Memes newmemes = new Memes
        {
            M_courage = 1.0f
        };

        _database.Add(newgenes.Id, new ActorInfo(newgenes, newmemes));

        return newgenes;
    }

    // QUICK AND DIRTY
    // To be called with:
    // - enemy ID (from the genes)
    // - score representing a sum(?) of damage done to the player
    // - score representing proximity to the player
    public void EnemyDead(int eid, float att_score, float dist_score)
    {
        _database[eid].att_score = att_score;
        _database[eid].dist_score = dist_score;
        _candidates.Add(eid);
    }

    // To be executed every now and then
    public void WorldTick<T>(T warudo)
        where T : IAIEnvActions, IAIEnvState
    {
        _tick_count++;

        warudo.SpawnEnemy(this.NewEnemy());
    }

}
