
public interface ICharacterInteractable
{
    public void ApplyRoll(int roll, PlayerCharacter playerCharacter);

    public string GetTaskName();
    public string GetTaskDescription();
}