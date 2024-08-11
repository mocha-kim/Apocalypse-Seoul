namespace Core.Interface
{
    public interface IMovable
    {
        public float MoveSpeed { get; }
        
        public void Move();
        public void SetDestination()
        {}
    }
}