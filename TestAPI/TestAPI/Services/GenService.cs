using TestAPI.Service;

public interface IGenService<T>
{
    void Set(int k, T val);
    T Get(int k);
}

public class GenService<T> : IGenService<T>
{
    public Dictionary<int, T> Dict { get; set; }

    public GenService()
    {
        Dict = new Dictionary<int, T>();
    }
    
    public void Set(int k, T val){
        if(!Dict.Where(x=>x.Key == k).Any()){
            Dict.Add(k,val);
        } 
    }

    public T Get(int k)
    {
        return Dict.FirstOrDefault(x=>x.Key == k).Value;
    }
}