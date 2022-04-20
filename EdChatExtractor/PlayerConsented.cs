namespace EdChatExtractor
{
    internal class PlayerConsented
    {
        public PlayerConsented(string cmdr)
        {
            Cmdr = cmdr;
        }

        public PlayerConsented(string cmdr, bool consent) : this(cmdr)
        {
            Consent = consent;
        }

        public string Cmdr { get; }
        public bool Consent { get; private set; }

        public override string ToString()
        {
            return $"{Cmdr} : {Consent}";
        }

        internal void HaveConsented()
        {
            Consent = true;
        }
    }
}