using System.Collections;
using System.Collections.Generic;


public class GameDirector{

    System.Random rnd = new System.Random();

    float health_base = 50;
    float health_step = 5;
    float speed_base = 2;
    float speed_step = 0.1f;
    float strength_base = 10;
    float strength_step = 2;
    float melee_range_base = 1.0f;
    float melee_range_step = 0.1f;
    float armor_base = 0;
    float armor_step = 0.5f;

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

        int[] stats = new int[5];
        int[] stats_best = new int[5];

        int best_level = 0;

        if (_candidates.Count > 0)
        {
            stats_best[0] = (int)((_database[_candidates[0]].genes.G_health - health_base) / health_step);
            stats_best[1] = (int)((_database[_candidates[0]].genes.G_melee_range - melee_range_base) / melee_range_step);
            stats_best[2] = (int)((_database[_candidates[0]].genes.G_speed - speed_base) / speed_step);
            stats_best[3] = (int)((_database[_candidates[0]].genes.G_strength - strength_base) / strength_step);
            stats_best[4] = (int)((_database[_candidates[0]].genes.G_armor - armor_base) / armor_step);

            best_level = _database[_candidates[0]].genes.level;
        }

        for (int i = 0; i < _tick_count - best_level; i++)
            stats[rnd.Next(0, 4)] += 1;

        Genes newgenes = new Genes
        {
            level = _tick_count,
            Id = _lastId,
            G_health = health_base + (stats[0] + stats_best[0]) * health_step,
            G_melee_range = melee_range_base + (stats[1] + stats_best[1]) * melee_range_step,
            G_speed = speed_base + (stats[2] + stats_best[2]) * speed_step,
            G_strength = strength_base + (stats[3] + stats_best[3]) * strength_step,
            G_armor = armor_base + (stats[4] + stats_best[4]) * armor_step
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
