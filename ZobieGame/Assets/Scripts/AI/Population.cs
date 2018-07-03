using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;

// Collection of actors - just a simple wrapper
public class Population {

    // credits to spawn zombies
    public float score = 55.0f;

    // cost of previously spawned zombie
    // TODO
    public float last_cost = 10.0f;

    // population identifier
    public string Id;

    // list of alive entities
    private HashSet<int> _population = new HashSet<int>();

    // candidates are dead actors from which we derive the descendants
    private List<int> _candidates = new List<int>();

    // hard limit for the size of _candidates
    private int _max_candidates = 30;

    // reference to the parent GD for data passing
    private GameDirector _GD;

    public Stats PopulationStats;

    public Population(GameDirector parentgd, string pid, int progenitor)
    {
        // saving the reference
        _GD = parentgd;

        Id = pid;

        // seed the population
        _population.Add(progenitor);
        // and the candidates
        _candidates.Add(progenitor);
    }


    /// <summary>
    /// Add (alive) id to the population list
    /// </summary>
    /// <param name="id">Global GD Id of the actor</param>
    public void Add(int id)
    {
        _population.Add(id);
    }

    /// <summary>
    /// Remove (kill) actor with given Id. Adds them to the candidates list.
    /// </summary>
    /// <param name="id">Global GD Id of the actor</param>
    public void Remove(int id)
    {
        if(_population.Remove(id))
            _candidates.Add(id);

        this.score += (_GD._database[id].att_score + _GD._database[id].dist_score);
    }

    public void Kill(int id)
    {
        Remove(id);
    }

    private void DarwinInAction()
    {
        // Sorting every time is not ideal, but meh for now
        _candidates.Sort(
            (a, b) => {
                float s_a = _GD._database[a].att_score * 10 + _GD._database[a].dist_score;
                float s_b = _GD._database[b].att_score * 10 + _GD._database[b].dist_score;

                return s_b.CompareTo(s_a);
            });


        int n = _candidates.Count;

        // because why bother
        if (n < 3)
            return;


        HashSet<int> kill_list = new HashSet<int>();

        // A simple way to do non-uniform distribution
        // Every candidate has a different sized window based on its positon
        // k-th candidate is killed if we draw from [n^2, (n+1)^2)
        // the "max" parameter of Random.Next is exclusive.

        // Kill off the population until we are left with 10 representatives
        while (n - kill_list.Count > _max_candidates)
            kill_list.Add(_GD.NonuniformRandomHigh(n));


        List<int> newlist = new List<int>();

        for (int i = 0; i < n; i++)
        {
            if (kill_list.Contains(i))
                continue;

            newlist.Add(_candidates[i]);
        }

        _candidates = newlist;

    }

    private Genotype MateAndMutate(int a, int b)
    {
        Genotype d1 = _GD.InfoFromId(a).DNA;
        Genotype d2 = _GD.InfoFromId(b).DNA;

        Memes m = d1.memes;

        // Very ugly block of code       
        Genes g = new Genes
        {
            G_health = _GD.UMChoice(d1.genes.G_health, d2.genes.G_health),
            G_speed = _GD.UMChoice(d1.genes.G_speed, d2.genes.G_speed),
            G_strength = _GD.UMChoice(d1.genes.G_strength, d2.genes.G_strength),
            // this was so strong I had to nerf it
            G_melee_range = 2, //  UMChoice(d1.genes.G_melee_range, d2.genes.G_melee_range),
            G_armor = _GD.UMChoice(d1.genes.G_armor, d2.genes.G_armor)
        };

        return new Genotype(g, m, this.Id);
    }

    private Genotype SelectAndBreed()
    {
        int mom = _candidates[_GD.NonuniformRandomLow(_candidates.Count)];
        int dad = _candidates[_GD.NonuniformRandomLow(_candidates.Count)];

        Genotype son = MateAndMutate(mom, dad);

        last_cost = son.GetValue();
        score -= last_cost;

        return son;
    }

    public Genotype Evolve()
    {
        this.DarwinInAction();
        return this.SelectAndBreed();
    }

    public bool CanSpawn()
    {
        return score > last_cost;
    }


}
