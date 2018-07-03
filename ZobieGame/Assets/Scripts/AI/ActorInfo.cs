
// Bundle of info to be stored in the GD's database
public class ActorInfo
{
    public Genotype DNA;

    public float att_score;
    public float dist_score;

    public ActorInfo(Genes g, Memes m)
    {
        this.DNA = new Genotype(g, m);
    }

    public ActorInfo(Genotype d)
    {
        this.DNA = d;
    }

}