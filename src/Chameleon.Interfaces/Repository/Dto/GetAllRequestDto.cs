namespace Chameleon.Interfaces.Repository
{
    public class GetAllRequestDto
    {
        public int SkipCount { get; set; }
        public int MaxResultCount { get; set; } = int.MaxValue;
        public string Sorting { get; set; }
    }
}
