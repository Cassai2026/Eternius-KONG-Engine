public class GenerationalTrust {
    public float creationEquity;
    public bool isUnlocked = false;

    public void CheckMilestone(int age) {
        if (age >= 18) isUnlocked = true;
    }
}
