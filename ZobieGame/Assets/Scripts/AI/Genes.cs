
// Bundle containing physical parameters and also ID
public class Genes{

    public int level;

    // Unique enemy ID (used by Game Director)
    public int Id;

    // Total (max) HP
    public float G_health;

    // Max speed
    public float G_speed;

    // Strength (influences damage)
    public float G_strength;

    // Melee attack range
    public float G_melee_range;

    public float G_armor;

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

    public int Level()
    {
        return level;
    }

    override public string ToString()
    {
        return "level: " + level + " ID: " + Id + " health: " + G_health + " speed: " + G_speed + " strength: " + G_strength + " melee range: " + G_melee_range + " armor: " + G_armor;
    }
}
