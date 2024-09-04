namespace RenDisco {
    public class StepContext {
        public int? Choice {get; set;}
        public bool Proceed {get; set;}

        public StepContext (int? choice = null) {
            Choice = choice;
            Proceed = true;
        }
    }
}