
// Bundle containing physical parameters and also ID
public class Genes
{

    // Unique enemy ID (used by Game Director)
    public int Id;

    public int Lvl;

    // Total (max) HP
    public float G_health;

    // Max speed
    public float G_speed;

    // Strength (influences damage)
    public float G_strength;

    // Melee attack range
    public float G_melee_range;

    // Armour (blocks a fraction of damage)
    public float G_armor;

    public int GetLvl()
    {
        return Lvl;
    }

    public float GetPhysArmor()
    {
        return G_armor;
    }

    public float GetPhysSize()
    {
        return G_health;
    }

    public float GetPhysDamage()
    {
        return G_strength;
    }

    public float GetPhysSpeed()
    {
        return G_speed;
    }

    public float GetPhysMeleeRange()
    {
        return G_melee_range;
    }

    public float GetArmor()
    {
        return G_armor;
    }

    public override string ToString()
    {
        return string.Format("ID:{0}, HP:{1}, SP:{2}, STR:{3}, RNG:{4}", Id, G_health, G_speed, G_strength, G_melee_range);
    }
}
