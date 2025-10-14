namespace EcoMonitor.Core.Models
{
    public class BinType
    {
        public Guid Id { get; private set; }
        public string Code { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;
        public ICollection<BinPhotoBinType> BinPhotoBinTypes { get; private set; } = new List<BinPhotoBinType>();

        private BinType() {}

        private void Validate()
        {
            if(string.IsNullOrWhiteSpace(Code)) throw new ArgumentException("Code required");
            if (string.IsNullOrWhiteSpace(Name)) throw new ArgumentException("Name required");
        }

        public static BinType Create(
            string code,
            string name)
        {
            var binType = new BinType
            {
                Id = Guid.NewGuid(),
                Code = code,
                Name = name
            };
            binType.Validate();
            return binType;
        }

        public void Rename(
            string name,
            string code)
        {
            if (!string.IsNullOrWhiteSpace(name) 
                && !string.IsNullOrWhiteSpace(code))
            {
                Name = name;
                Code = code;
            } 
        }
    }
}
