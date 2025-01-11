namespace RenDisco {
    public class InputContext {
        public int? Choice {get; set;}
        public bool Proceed {get; set;}

        public InputContext (int? choice = null) {
            Choice = choice;
            Proceed = true;
        }
    }
}