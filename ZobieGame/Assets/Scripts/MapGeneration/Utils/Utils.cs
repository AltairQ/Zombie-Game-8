

public class Utils
{
    public static void Swap<T>(ref T e1, ref T e2)
    {
        T tmp = e1;
        e1 = e2;
        e2 = tmp;
    }
}