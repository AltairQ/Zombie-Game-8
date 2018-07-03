using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Wrapper class for generic statistics
// Assuming 'float' as the numeric type

public class Stats {

    // lecimy z public a co
    public float Max
    {
        get;
        set;
    }

    public float Mean
    {
        get
        {
            return Mean;
        }

        // we want the Mean value to be automatic
        private set
        {
            Mean = value;
        }
    }

    public float Min
    {
        get;
        set;
    }

    // Init to NaN
    public Stats()
    {
        Max = float.NaN;
        Mean = float.NaN;
        Min = float.NaN;
    }

    // Pass IEnumerable<float> to calculate stats
    // Would have used 'in' keyword, but our C# version is too old
    public void UpdateFromContainer<T>(ref T in_list) where T : IEnumerable<float>
    {
        this.Max  = in_list.Max();
        this.Mean = in_list.Average();
        this.Min  = in_list.Min();
    }

    public override string ToString()
    {
        return string.Format("({0}; {1}; {2})",
            Min,
            Mean,
            Max
            );
    }
}
