namespace EcoMonitor.Core.Models
{
    public class BinType
    {
        public Guid Id { get; private set; }
        public string Code { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;
        public ICollection<BinPhotoBinType> BinPhotoBinTypes { get; private set; } = new List<BinPhotoBinType>();

        private BinType(
            string code, 
            string name)
        {
            Id = Guid.NewGuid();
            Code = code;
            Name = name;
            Validate();
        }

        private void Validate()
        {
            if(string.IsNullOrWhiteSpace(Code)) throw new ArgumentException("Code required");
            if (string.IsNullOrWhiteSpace(Name)) throw new ArgumentException("Name required");
        }

        public static BinType Create(
            string code,
            string name)
        {
            return new BinType(
                code, 
                name);
        }

        
    }
}
