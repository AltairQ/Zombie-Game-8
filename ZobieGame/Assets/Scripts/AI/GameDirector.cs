using System;
using System.Collections;
using System.Collections.Generic;


public class GameDirector{

    // Bundle of info to be stored in the GD's database
    class ActorInfo
    {
        public Genotype DNA;
        // public Genes genes;
        // public Memes memes;
        public float att_score;
        public float dist_score;

        public ActorInfo(Genes g, Memes m)
        {
            this.DNA  = new Genotype(g, m);            
        }

        public ActorInfo(Genotype d)
        {
            this.DNA = d;
        }

    }

    // Internal storage
    private Dictionary<int, ActorInfo> _database = new Dictionary<int, ActorInfo>();

    private int _max_candidates;
    private List<int> _candidates = new List<int>();

    // Next ID to be used
    // TODO maybe rename..?
    private int _lastId = 0;

    // Game tick count
    private int _tick_count = 0;

    // Pseudorandomness source
    // Let's hardcode the seed, because why not
    private Random _rnd = new Random(12112014);

    public GameDirector()
    {
        Genes g = new Genes
        {
            Id = _lastId++,
            G_health = UMChoice(100, 100),
            G_speed = UMChoice(1, 1),
            G_strength = UMChoice(20, 20),
            G_melee_range = UMChoice(2, 2),
            G_armor = UMChoice(1, 1)
        };

        Memes m = new Memes
        {
            M_courage = 1.0F
        };

        _database.Add(0, new ActorInfo(g, m));
        _candidates.Add(0);
    }


    // TODO ADD VALIDATION

    private ActorInfo InfoFromActor(IAIState actor)
    {
        return _database[actor.GetID()];
    }

    private ActorInfo InfoFromId(int i)
    {
        return _database[i];
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

    // Draws an int from range [0; n)
    // Higher numbers have a (lot) higher chance of being picked
    public int NonuniformRandomHigh(int n)
    {
        if (n == 0)
            return n;

        // Simple quadratic weight distribution
        return (int)Math.Floor(Math.Sqrt(_rnd.Next(0, n * n)) );
    }

    // The same as above, but with the distibution mirrored
    public int NonuniformRandomLow(int n)
    {
        // The simplest way
        return n - 1 - this.NonuniformRandomHigh(n);
    }

    public float UniformRandomFloat()
    {
        return (float)this._rnd.Next(0, 10000) / 10000.0F;
    }

    public float UniformChoice(float x, float y)
    {
        // Throwing exceptions is for weak
        if (x > y)
        {
            float c = y;
            y = x;
            x = c;
        }

        return x + (y - x) * this.UniformRandomFloat();
    }

    // Mutating float so that x differs by +- degree times
    public float Mutate(float x, float degree = 0.1F )
    {
        return x * (1 + degree*(this.UniformRandomFloat() - 0.5F));
    }

    public float UMChoice(float x, float y, float degree = 0.7F)
    {
        return Mutate(UniformChoice(x, y), degree);
    }

    // Let's reduce the population
    // Kills off half of the population using the NonuniformRandom distribution

    private void DarwinInAction()
    {
        // Sorting every time is not ideal, but meh for now
        _candidates.Sort(
            (a, b) => {
                float s_a = _database[a].att_score * 10 + _database[a].dist_score;
                float s_b = _database[b].att_score * 10 + _database[b].dist_score;

                return s_b.CompareTo(s_a);
            });


        int n = _candidates.Count;

        // because why bother
        if(n < 3)
            return;
        

        HashSet<int> kill_list = new HashSet<int>();

        // A simple way to do non-uniform distribution
        // Every candidate has a different sized window based on its positon
        // k-th candidate is killed if we draw from [n^2, (n+1)^2)
        // the "max" parameter of Random.Next is exclusive.

        // Kill off the population until we reach half of the original size
        while( kill_list.Count < n/2 )
            kill_list.Add( this.NonuniformRandomHigh(n) );
        

        List<int> newlist = new List<int>();

        for(int i = 0; i < n; i++)
        {
            if (kill_list.Contains(i))
                continue;

            newlist.Add(_candidates[i]);
        }

        _candidates = newlist;       
        
    }

    private Genotype MateAndMutate(int a, int b)
    {
        Genotype d1 = this.InfoFromId(a).DNA;
        Genotype d2 = this.InfoFromId(b).DNA;

        Memes m = d1.memes;

        // Very ugly block of code       
        Genes g = new Genes
        {
            G_health      = UMChoice(d1.genes.G_health,      d2.genes.G_health     ),
            G_speed       = UMChoice(d1.genes.G_speed,       d2.genes.G_speed      ),
            G_strength    = UMChoice(d1.genes.G_strength,    d2.genes.G_strength   ),
            G_melee_range = UMChoice(d1.genes.G_melee_range, d2.genes.G_melee_range),
            G_armor       = UMChoice(d1.genes.G_armor,       d2.genes.G_armor      )
        };

        return new Genotype(g, m);
    }

    private Genotype SelectAndBreed()
    {
        int mom = _candidates[this.NonuniformRandomLow(_candidates.Count)];
        int dad = _candidates[this.NonuniformRandomLow(_candidates.Count)];

        return MateAndMutate(mom, dad);
    }
    


    public Genes NewEnemy()
    {
        this.DarwinInAction();       

        Genotype newdna = this.SelectAndBreed();

        newdna.genes.Id = _lastId++;
        _database.Add(newdna.genes.Id, new ActorInfo(newdna));

        return newdna.genes;
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
