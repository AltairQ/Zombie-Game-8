using System.Collections;
using System.Collections.Generic;


public class GameDirector{

    // Bundle of info to be stored in the GD's database
    struct ActorInfo
    {
        public Genes genes;
        public Memes memes;
    }

    // Internal storage
    private Dictionary<int, ActorInfo> _database;

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

    // To be executed every X frames
    public void WorldTick<T>(T warudo)
        where T : IAIEnvActions, IAIEnvState
    {
        _tick_count++;

        if (_tick_count % 10000 == 0)
        {
            Genes newgenes = new Genes();

            newgenes.Id = ++_lastId;
            newgenes.G_health = 100.0f;
            newgenes.G_melee_range = 2.0f;
            newgenes.G_speed = 1.0f;
            newgenes.G_strength = 10.0f;

            warudo.SpawnEnemy(newgenes);
        }
    }

}
