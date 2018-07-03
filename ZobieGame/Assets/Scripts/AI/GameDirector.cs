using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;


// Class coordinating populations and the interface between Unity and AI "engine"
public class GameDirector{

    // Internal storage
    public Dictionary<int, ActorInfo> _database = new Dictionary<int, ActorInfo>();

    // Map from Ids to populations
    private Dictionary<string, Population> _population_db = new Dictionary<string, Population>();

    private List<Population> _populations = new List<Population>();

    // private int _max_candidates = 10;

    // private List<int> _candidates = new List<int>();

    private HashSet<int> _population = new HashSet<int>();

    // Next ID to be used
    int _nextId = 0;

    // Game tick count
    private int _tick_count = 0;

    // Pseudorandomness source
    // Let's hardcode the seed, because why not
    private Random _rnd = new Random(unchecked ((int)0xdeadbeef));


    private float killRadius = 50.0f;

    // amount to be added to each populations' account every tick
    private float _subsidy = 20.0f;

    // Manual "debug mode"
    // set to true to enable logging evolution history
    private bool _log_enabled = false; 
    private string _log_path = "ev_history.txt";

    public GameDirector()
    {
        string pid = "a";

        Genes g = new Genes
        {
            G_health = UMChoice(70, 70),
            G_speed = UMChoice(2, 2),
            G_strength = UMChoice(20, 20),
            G_melee_range = UMChoice(2, 2),
            G_armor = UMChoice(1, 1),            
        };

        Memes m = new Memes
        {
            M_courage = 1.0F
        };

        var tmpg = new Genotype(g, m)
        {
            Id = _nextId++,
            species = pid
        };
        _database.Add(tmpg.Id, new ActorInfo(g, m));

        Population ptmp = new Population(this, pid, tmpg.Id);
        _population_db.Add(ptmp.Id, ptmp);
        _populations.Add(ptmp);
    }


    // TODO ADD VALIDATION

    public ActorInfo InfoFromActor(IAIState actor)
    {
        return _database[actor.GetID()];
    }

    public ActorInfo InfoFromId(int i)
    {
        return _database[i];
    }

    // Compute actions for an individual enemy
    public void Animate<T>(T actor)
        where T : MonoBehaviour, IAIActions, IAIState
    {
        if (!actor.IsAlive())
            return;

        if (actor.EuclidDistanceToPlayer() >= killRadius)
        {
            actor.Suicide();
            return;
        }

        if (actor.PlayerInMeleeRange())
            actor.AttackMeleePlayer();

        if (actor.CurrentStimuli() != null)
            actor.GoToStimuli();

        if ((actor.CurrentStimuli() != null) && (Vector3.Distance(actor.transform.position, actor.CurrentStimuli().position) < 1))
            actor.ChangeStimuli(null);
    }

    public void ApplyStimuli<T>(T actor, Stimuli st)
        where T : IAIActions, IAIState
    {
        if((actor.CurrentStimuli() == null) || (st.intensity >= actor.CurrentStimuli().intensity))
            actor.ChangeStimuli(st);

        return;
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
        // Throwing exceptions is for the weak
        if (x > y)
        {
            float c = y;
            y = x;
            x = c;
        }

        return x + (y - x) * this.UniformRandomFloat();
    }

    public int UniformInt(int a, int b)
    {
        return this._rnd.Next(a, b);
    }

    // Mutating float so that x differs by +- degree times
    public float Mutate(float x, float degree)
    {
        return x * (1 + degree*(this.UniformRandomFloat() - 0.5F));
    }

    public float UMChoice(float x, float y, float degree = 0.7F)
    {
        return Mutate(UniformChoice(x, y), degree);
    }

    

    public Genotype NewEnemy(string pop_id)
    {
        var cpop = _population_db[pop_id];

        Genotype newdna = cpop.Evolve();

        cpop.Add(AddEnemy(newdna).Id);
        return newdna;
    }

    // TODO remove and add proper population choice
    public Genotype NewEnemy()
    {
        return NewEnemy("a");
    }

    /// <summary>
    /// Add an enemy to database and assign an id. DOES NOT CREATE A UNITY OBJECT!
    /// </summary>
    /// <param name="ing">Genotype on which the Enemy will be based</param>
    /// <returns>The edited Genotype</returns>
    public Genotype AddEnemy(Genotype ing)
    {
        ing.Id = _nextId++;
        
        _database.Add(ing.Id, new ActorInfo(ing));

        return ing;
    }

    // QUICK AND DIRTY
    // To be called with:
    // - enemy ID (from the genes)
    // - score representing a sum(?) of damage done to the player
    // - score representing proximity to the player
    public void EnemyDead(int eid, float att_score, float dist_score)
    {
        _database[eid].att_score = att_score;
        _database[eid].dist_score = 30.0f/(dist_score+2.0f) - 1.0f;

        Genotype tmp = InfoFromId(eid).DNA;

        _population_db[tmp.species].Kill(eid);
    }

    public void InitGraphs()
    {
        var graphs = GameSystem.Get().GraphsManager;
        graphs.CreateGraph("physSize", UnityEngine.Color.green);
        graphs.CreateGraph("physSpeed", UnityEngine.Color.yellow);
        graphs.CreateGraph("physDamage", UnityEngine.Color.red);
        graphs.CreateGraph("armor", UnityEngine.Color.grey);
    }

    public void ShowPopulationStats()
    {
        var graphs = GameSystem.Get().GraphsManager;

        float physSizeValue = _population.Average(x => InfoFromId(x).DNA.genes.GetPhysSize());
        float physSpeedValue = _population.Average(x => InfoFromId(x).DNA.genes.GetPhysSpeed());
        float physDamageValue = _population.Average(x => InfoFromId(x).DNA.genes.GetPhysDamage());
        float armorValue = _population.Average(x => InfoFromId(x).DNA.genes.GetArmor());

        graphs.AddValue("physSize", physSizeValue);
        graphs.AddValue("physSpeed", physSpeedValue);
        graphs.AddValue("physDamage", physDamageValue);
        graphs.AddValue("armor", armorValue);

        //  DebugConsole.Clear();

        //  string buf =
        //string.Format("MIN HP:{0} SP:{1} STR:{2} AP:{3}",
        //_population.Min(x => InfoFromId(x).DNA.genes.GetPhysSize()),
        //_population.Min(x => InfoFromId(x).DNA.genes.GetPhysSpeed()),
        //_population.Min(x => InfoFromId(x).DNA.genes.GetPhysDamage()),
        //_population.Min(x => InfoFromId(x).DNA.genes.GetArmor())
        //);

        //  DebugConsole.Log(buf);

        //  buf =
        //  string.Format("AVG HP:{0} SP:{1} STR:{2} AP:{3}",
        //  _population.Average(x => InfoFromId(x).DNA.genes.GetPhysSize()),
        //  _population.Average(x => InfoFromId(x).DNA.genes.GetPhysSpeed()),
        //  _population.Average(x => InfoFromId(x).DNA.genes.GetPhysDamage()),
        //  _population.Average(x => InfoFromId(x).DNA.genes.GetArmor())
        //  );

        //  DebugConsole.Log(buf);

        //  buf =
        //  string.Format("MAX HP:{0} SP:{1} STR:{2} AP:{3}",
        //  _population.Max(x => InfoFromId(x).DNA.genes.GetPhysSize()),
        //  _population.Max(x => InfoFromId(x).DNA.genes.GetPhysSpeed()),
        //  _population.Max(x => InfoFromId(x).DNA.genes.GetPhysDamage()),
        //  _population.Max(x => InfoFromId(x).DNA.genes.GetArmor())
        //  );

        //  DebugConsole.Log(buf);

        if (!_log_enabled)
            return;


        if (!System.IO.File.Exists(_log_path))
            System.IO.File.CreateText(_log_path);

        using (System.IO.StreamWriter sw = System.IO.File.AppendText(_log_path))
        {
              

            sw.WriteLine(
                string.Format(
                    "{0}\t {1}\t {2}\t {3}\t {4}\t {5}\t {6}\t {7}\t {8}\t {9}\t {10}\t {11}\t {12}\t"
                    ,
                    _nextId-1,
                    _population.Min(x => InfoFromId(x).DNA.genes.GetPhysSize()),
                    _population.Average(x => InfoFromId(x).DNA.genes.GetPhysSize()),
                    _population.Max(x => InfoFromId(x).DNA.genes.GetPhysSize()),

                    _population.Min(x => InfoFromId(x).DNA.genes.GetPhysSpeed()),
                    _population.Average(x => InfoFromId(x).DNA.genes.GetPhysSpeed()),
                    _population.Max(x => InfoFromId(x).DNA.genes.GetPhysSpeed()),

                    _population.Min(x => InfoFromId(x).DNA.genes.GetPhysDamage()),
                    _population.Average(x => InfoFromId(x).DNA.genes.GetPhysDamage()),
                    _population.Max(x => InfoFromId(x).DNA.genes.GetPhysDamage()),

                    _population.Min(x => InfoFromId(x).DNA.genes.GetArmor()),
                    _population.Average(x => InfoFromId(x).DNA.genes.GetArmor()),
                    _population.Max(x => InfoFromId(x).DNA.genes.GetArmor())

                ));
        }

    }

    // To be executed every now and then
    public void WorldTick<T>(T warudo)
        where T : IAIEnvActions, IAIEnvState
    {
        _tick_count++;

        foreach (var pop in _populations)
        {
            pop.score += _subsidy;

            if (pop.CanSpawn())
                warudo.SpawnEnemy(this.NewEnemy(pop.Id));
        }
    }

}
